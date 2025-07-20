using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BlueWP.ATProto
{
  public class Client
  {
    private Settings _settings = new Settings();
    private Newtonsoft.Json.JsonSerializerSettings _deserializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
      {
        MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.ReadAhead,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
        SerializationBinder = new TypesBinder(),
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
      };
    private string _hostOverride = null;

    public Client()
    {
    }

    public Settings Settings => _settings;
    public Settings.AccountSettingsData CurrentAccountSettings => Settings.CurrentAccountSettings;
    private string CurrentEndpoint { get { return !string.IsNullOrEmpty(_hostOverride) ? $"https://{_hostOverride}" : CurrentAccountSettings?.Credentials.Endpoint; } }

    public async Task<bool> Authenticate()
    {
      return await _settings.ReadSettings();
    }

    public async Task<bool> AuthenticateWithPassword(string host, string handle, string appPassword)
    {
      if (string.IsNullOrEmpty(host)
          || string.IsNullOrEmpty(handle)
          || string.IsNullOrEmpty(appPassword))
      {
        throw new ArgumentException();
      }

      _hostOverride = host;
      Lexicons.COM.ATProto.Server.CreateSession.Response response = null;
      try
      {
        response = await PostAsync<Lexicons.COM.ATProto.Server.CreateSession.Response>(new Lexicons.COM.ATProto.Server.CreateSession()
        {
          identifier = handle,
          password = appPassword,
        });
      }
      catch (WebException)
      {
        return false;
      }
      if (response == null || string.IsNullOrEmpty(response.accessJwt))
      {
        return false;
      }
      _hostOverride = null;

      var didDoc = (response?.didDoc as Newtonsoft.Json.Linq.JObject).ToObject<Lexicons.COM.ATProto.Server.DIDDoc>();

      // TODO: check if account already exists in the db
      var credentials = new Settings.AccountSettingsData()
      {
        Credentials = new Settings.CredentialsData()
        {
          DID = response.did,
          Handle = response.handle,
          AccessToken = response.accessJwt,
          RefreshToken = response.refreshJwt,
          Endpoint = didDoc?.service?.FirstOrDefault()?.serviceEndpoint ?? $"https://{host}",
        }
      };

      _settings.SelectedDID = credentials?.Credentials.DID;
      _settings.AccountSettings.Add(credentials);
      return await _settings.WriteSettings();
    }

    public bool IsAuthenticated { get { return CurrentAccountSettings?.Credentials.AccessToken != null; } }
    public string Handle { get { return CurrentAccountSettings?.Credentials.Handle; } }
    public string DID { get { return CurrentAccountSettings?.Credentials.DID; } }

    public async Task<T> GetAsync<T>(ILexiconRequest input) where T : ILexiconResponse
    {
      return await RequestAsync<T>("GET", input);
    }

    public async Task<T> PostAsync<T>(ILexiconRequest input) where T : ILexiconResponse
    {
      return await RequestAsync<T>("POST", input);
    }

    protected async Task<T> RequestAsync<T>(string method, ILexiconRequest input) where T : ILexiconResponse
    {
      if (string.IsNullOrEmpty(CurrentEndpoint))
      {
        throw new ArgumentException();
      }

      var http = new HTTP();

      var headers = new NameValueCollection();
      headers["Content-Type"] = "application/json";

      var rawPost = input as IRawPost;
      if (rawPost != null)
      {
        headers["Content-Type"] = rawPost.MimeType;
      }

      if (CurrentAccountSettings != null)
      {
        headers["Authorization"] = $"Bearer {CurrentAccountSettings.Credentials.AccessToken}";
        (input as ICustomHeaderProvider)?.SetCustomHeaders(headers, CurrentAccountSettings);
      }

      var url = $"{CurrentEndpoint}/xrpc/{input.EndpointID}";
      string responseJson = null;
      string bodyJson = string.Empty;
      switch (method)
      {
        case "GET":
          {
            url += SerializeInputToQueryString(input);
            responseJson = await http.DoGETRequestAsync(url, null, headers);
          }
          break;
        case "POST":
          {
            if (rawPost != null)
            {
              responseJson = await http.DoPOSTRequestAsync(url, rawPost.PostData, headers);
            }
            else
            {
              var inputType = input.GetType();
              var fields = inputType.GetFields();
              bodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(input, _deserializerSettings);
              if (bodyJson == "{}" || fields.Length == 0)
              {
                bodyJson = string.Empty;
              }
              responseJson = await http.DoPOSTRequestAsync(url, bodyJson, headers);
            }
          }
          break;
      }
      if (http.Response == null || !http.Response.IsSuccessStatusCode)
      {
        var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson) as Newtonsoft.Json.Linq.JObject;
        if (jsonObj != null && jsonObj.GetValue("error") != null && jsonObj.GetValue("error").ToString() == "ExpiredToken")
        {
          if (await RefreshCredentials())
          {
            headers["Authorization"] = $"Bearer {CurrentAccountSettings.Credentials.AccessToken}";
            switch (method)
            {
              case "GET":
                {
                  responseJson = await http.DoGETRequestAsync(url, null, headers);
                }
                break;
              case "POST":
                {
                  if (rawPost != null)
                  {
                    responseJson = await http.DoPOSTRequestAsync(url, rawPost.PostData, headers);
                  }
                  else
                  {
                    responseJson = await http.DoPOSTRequestAsync(url, bodyJson, headers);
                  }
                }
                break;
            }
          }
        }
        else
        {
#if DEBUG
          System.Diagnostics.Debug.WriteLine($"[HTTP ERROR {http.Response.StatusCode}] {responseJson}");
#endif
        }
      }

      return responseJson != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseJson, _deserializerSettings) : default(T);
    }

    private async Task<bool> RefreshCredentials()
    {
      var body = new Lexicons.COM.ATProto.Server.RefreshSession();
      var response = await PostAsync<Lexicons.COM.ATProto.Server.RefreshSession.Response>(body);
      if (response != null && !string.IsNullOrEmpty(response.accessJwt))
      {
        CurrentAccountSettings.Credentials.DID = response.did;
        CurrentAccountSettings.Credentials.Handle = response.handle;
        CurrentAccountSettings.Credentials.AccessToken = response.accessJwt;
        CurrentAccountSettings.Credentials.RefreshToken = response.refreshJwt;
        await _settings.WriteSettings();
      }

      return true;
    }

    private string SerializeInputToQueryString(ILexiconRequest input)
    {
      var inputType = input.GetType();
      bool first = true;
      var queryString = string.Empty;
      foreach (var field in inputType.GetFields())
      {
        if (field.GetCustomAttribute(typeof(Newtonsoft.Json.JsonIgnoreAttribute)) != null)
        {
          continue;
        }
        var value = inputType.GetField(field.Name).GetValue(input);
        if (value != null)
        {
          if (value is IEnumerable<object>)
          {
            var a = value as IEnumerable<object>;
            foreach (var i in a)
            {
              queryString += first ? "?" : "&";
              queryString += $"{field.Name}[]={WebUtility.UrlEncode(i.ToString())}";
              first = false;
            }
            continue;
          }
          queryString += first ? "?" : "&";
          queryString += $"{field.Name}={WebUtility.UrlEncode(value.ToString())}";
          first = false;
        }
      }
      return queryString;
    }
    
    private class TypesBinder : Newtonsoft.Json.Serialization.ISerializationBinder
    {
      private Dictionary<string, Type> _atprotoTypes;

      public TypesBinder()
      {
        _atprotoTypes = new Dictionary<string, Type>()
          {
              { "blob",                                     typeof(Blob) },
              { "app.bsky.actor.defs#adultContentPref",     typeof(Lexicons.App.BSky.Actor.Defs.AdultContentPref) },
              { "app.bsky.actor.defs#contentLabelPref",     typeof(Lexicons.App.BSky.Actor.Defs.ContentLabelPref) },
              { "app.bsky.actor.defs#savedFeed",            typeof(Lexicons.App.BSky.Actor.Defs.SavedFeed) },
              { "app.bsky.actor.defs#savedFeedsPref",       typeof(Lexicons.App.BSky.Actor.Defs.SavedFeedsPref) },
              { "app.bsky.actor.defs#savedFeedsPrefV2",     typeof(Lexicons.App.BSky.Actor.Defs.SavedFeedsPrefV2) },
              { "app.bsky.actor.defs#personalDetailsPref",  typeof(Lexicons.App.BSky.Actor.Defs.PersonalDetailsPref) },
              { "app.bsky.actor.defs#feedViewPref",         typeof(Lexicons.App.BSky.Actor.Defs.FeedViewPref) },
              { "app.bsky.actor.defs#threadViewPref",       typeof(Lexicons.App.BSky.Actor.Defs.ThreadViewPref) },
              { "app.bsky.actor.defs#interestsPref",        typeof(Lexicons.App.BSky.Actor.Defs.InterestsPref) },
              { "app.bsky.actor.defs#mutedWordsPref",       typeof(Lexicons.App.BSky.Actor.Defs.MutedWordsPref) },
              { "app.bsky.actor.defs#hiddenPostsPref",      typeof(Lexicons.App.BSky.Actor.Defs.HiddenPostsPref) },
              { "app.bsky.actor.defs#labelersPref",         typeof(Lexicons.App.BSky.Actor.Defs.LabelersPref) },
              { "app.bsky.actor.defs#bskyAppStatePref",     typeof(Lexicons.App.BSky.Actor.Defs.BskyAppStatePref) },
              { "app.bsky.actor.searchActors",              typeof(Lexicons.App.BSky.Actor.SearchActors) },
              { "app.bsky.actor.searchActorsTypeahead",     typeof(Lexicons.App.BSky.Actor.SearchActorsTypeahead) },
              { "app.bsky.embed.defs#aspectRatio",          typeof(Lexicons.App.BSky.Embed.Defs.AspectRatio) },
              { "app.bsky.embed.external",                  typeof(Lexicons.App.BSky.Embed.External) },
              { "app.bsky.embed.external#view",             typeof(Lexicons.App.BSky.Embed.External.View) },
              { "app.bsky.embed.images",                    typeof(Lexicons.App.BSky.Embed.Images) },
              { "app.bsky.embed.images#image",              typeof(Lexicons.App.BSky.Embed.Images.Image) },
              { "app.bsky.embed.images#view",               typeof(Lexicons.App.BSky.Embed.Images.View) },
              { "app.bsky.embed.record",                    typeof(Lexicons.App.BSky.Embed.Record) },
              { "app.bsky.embed.record#view",               typeof(Lexicons.App.BSky.Embed.Record.View) },
              { "app.bsky.embed.record#viewRecord",         typeof(Lexicons.App.BSky.Embed.Record.ViewRecord) },
              { "app.bsky.embed.record#viewNotFound",       typeof(Lexicons.App.BSky.Embed.Record.ViewNotFound) },
              { "app.bsky.embed.record#viewBlocked",        typeof(Lexicons.App.BSky.Embed.Record.ViewBlocked) },
              { "app.bsky.embed.recordWithMedia",           typeof(Lexicons.App.BSky.Embed.RecordWithMedia) },
              { "app.bsky.embed.recordWithMedia#view",      typeof(Lexicons.App.BSky.Embed.RecordWithMedia.View) },
              { "app.bsky.embed.video",                     typeof(Lexicons.App.BSky.Embed.Video) },
              { "app.bsky.embed.video#view",                typeof(Lexicons.App.BSky.Embed.Video.View) },
              { "app.bsky.embed.video#caption",             typeof(Lexicons.App.BSky.Embed.Video.Caption) },
              { "app.bsky.feed.defs#generatorView",         typeof(Lexicons.App.BSky.Feed.Defs.GeneratorView) },
              { "app.bsky.feed.defs#postView",              typeof(Lexicons.App.BSky.Feed.Defs.PostView) },
              { "app.bsky.feed.defs#notFoundPost",          typeof(Lexicons.App.BSky.Feed.Defs.NotFoundPost) },
              { "app.bsky.feed.defs#blockedPost",           typeof(Lexicons.App.BSky.Feed.Defs.BlockedPost) },
              { "app.bsky.feed.defs#reasonRepost",          typeof(Lexicons.App.BSky.Feed.Defs.ReasonRepost) },
              { "app.bsky.feed.defs#threadViewPost",        typeof(Lexicons.App.BSky.Feed.Defs.ThreadViewPost) },
              { "app.bsky.feed.like",                       typeof(Lexicons.App.BSky.Feed.Like) },
              { "app.bsky.feed.post",                       typeof(Lexicons.App.BSky.Feed.Post) },
              { "app.bsky.feed.post#replyRef",              typeof(Lexicons.App.BSky.Feed.Post.ReplyRef) },
              { "app.bsky.feed.repost",                     typeof(Lexicons.App.BSky.Feed.Repost) },
              { "app.bsky.feed.searchPosts",                typeof(Lexicons.App.BSky.Feed.SearchPosts) },
              { "app.bsky.feed.threadgate",                 typeof(Lexicons.App.BSky.Feed.Threadgate) },
              { "app.bsky.feed.threadgate#mentionRule",     typeof(Lexicons.App.BSky.Feed.Threadgate.MentionRule) },
              { "app.bsky.feed.threadgate#followerRule",    typeof(Lexicons.App.BSky.Feed.Threadgate.FollowerRule) },
              { "app.bsky.feed.threadgate#followingRule",   typeof(Lexicons.App.BSky.Feed.Threadgate.FollowingRule) },
              { "app.bsky.graph.defs#starterPackViewBasic", typeof(Lexicons.App.BSky.Graph.Defs.StarterPackViewBasic) },
              { "app.bsky.graph.starterpack",               typeof(Lexicons.App.BSky.Graph.StarterPack) },
              { "app.bsky.graph.follow",                    typeof(Lexicons.App.BSky.Graph.Follow) },
              { "app.bsky.notification.updateSeen",         typeof(Lexicons.App.BSky.Notification.UpdateSeen) },
              { "app.bsky.richtext.facet",                  typeof(Lexicons.App.BSky.RichText.Facet) },
              { "app.bsky.richtext.facet#byteSlice",        typeof(Lexicons.App.BSky.RichText.Facet.ByteSlice) },
              { "app.bsky.richtext.facet#link",             typeof(Lexicons.App.BSky.RichText.Facet.Link) },
              { "app.bsky.richtext.facet#mention",          typeof(Lexicons.App.BSky.RichText.Facet.Mention) },
              { "app.bsky.richtext.facet#tag",              typeof(Lexicons.App.BSky.RichText.Facet.Tag) },
              { "com.atproto.label.defs#selfLabels",        typeof(Lexicons.COM.ATProto.Label.Defs.SelfLabels) },
              { "com.atproto.repo.createRecord",            typeof(Lexicons.COM.ATProto.Repo.CreateRecord) },
              { "com.atproto.repo.deleteRecord",            typeof(Lexicons.COM.ATProto.Repo.DeleteRecord) },
              { "com.atproto.repo.strongRef",               typeof(Lexicons.COM.ATProto.Repo.StrongRef) },
              { "com.atproto.repo.uploadBlob",              typeof(Lexicons.COM.ATProto.Repo.UploadBlob) },
              { "com.atproto.server.createSession",         typeof(Lexicons.COM.ATProto.Server.CreateSession) },
              { "com.atproto.server.refreshSession",        typeof(Lexicons.COM.ATProto.Server.RefreshSession) },
              { "chat.bsky.convo.defs#messageRef",          typeof(Lexicons.Chat.BSky.Convo.Defs.MessageRef) },
              { "chat.bsky.convo.defs#messageInput",        typeof(Lexicons.Chat.BSky.Convo.Defs.MessageInput) },
              { "chat.bsky.convo.defs#messageView",         typeof(Lexicons.Chat.BSky.Convo.Defs.MessageView) },
              { "chat.bsky.convo.defs#messageViewSender",   typeof(Lexicons.Chat.BSky.Convo.Defs.MessageViewSender) },
              { "chat.bsky.convo.defs#convoView",           typeof(Lexicons.Chat.BSky.Convo.Defs.ConvoView) },
              { "chat.bsky.convo.getConvo",                 typeof(Lexicons.Chat.BSky.Convo.GetConvo) },
              { "chat.bsky.convo.getMessages",              typeof(Lexicons.Chat.BSky.Convo.GetMessages) },
              { "chat.bsky.convo.listConvos",               typeof(Lexicons.Chat.BSky.Convo.ListConvos) },
              { "chat.bsky.convo.sendMessage",              typeof(Lexicons.Chat.BSky.Convo.SendMessage) },
              { "chat.bsky.convo.updateRead",               typeof(Lexicons.Chat.BSky.Convo.UpdateRead) },
          };
      }

      public Type BindToType(string assemblyName, string typeName)
      {
        if (!_atprotoTypes.ContainsKey(typeName))
        {
#if TRUE
          System.Diagnostics.Debug.WriteLine($"ATProto type '{typeName}' not found");
          _atprotoTypes[typeName] = typeof(object);
          return typeof(object);
#else
                    throw new ArgumentException($"ATProto type '{typeName}' not found");
#endif
        }
        return _atprotoTypes[typeName];
      }

      public void BindToName(Type serializedType, out string assemblyName, out string typeName)
      {
        assemblyName = null;

        var kvp = _atprotoTypes.FirstOrDefault(s => s.Value == serializedType);
        if (kvp.Value == null)
        {
#if TRUE
          System.Diagnostics.Debug.WriteLine($"ATProto name '{serializedType.ToString()}' not found");
          typeName = serializedType.Name;
          return;
#else
          throw new ArgumentException($"ATProto name '{serializedType.ToString()}' not found");
#endif
        }
        typeName = kvp.Key;
      }
    }
  }
}
