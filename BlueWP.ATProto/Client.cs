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
    public Settings.Credential CurrentCredential => Settings.CurrentCredential;
    private string CurrentHost { get { return _hostOverride ?? CurrentCredential?.ServiceHost; } }

    public async Task<bool> Authenticate()
    {
      return await _settings.ReadCredentials();
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
      Lexicons.COM.ATProto.Server.CreateSessionResponse response = null;
      try
      {
        response = await PostAsync<Lexicons.COM.ATProto.Server.CreateSessionResponse>(new Lexicons.COM.ATProto.Server.CreateSession()
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

      var credentials = new Settings.Credential()
      {
        ServiceHost = host,
        DID = response.did,
        Handle = response.handle,
        AccessToken = response.accessJwt,
        RefreshToken = response.refreshJwt,
      };

      _settings.SelectedDID = credentials.DID;
      _settings.Credentials.Add(credentials);
      return await _settings.WriteCredentials();
    }

    public bool IsAuthenticated { get { return CurrentCredential?.AccessToken != null; } }
    public string Handle { get { return CurrentCredential?.Handle; } }
    public string DID { get { return CurrentCredential?.DID; } }

    public async Task<T> GetAsync<T>(ILexicon input) where T : ILexicon
    {
      return await RequestAsync<T>("GET", input);
    }

    public async Task<T> PostAsync<T>(ILexicon input) where T : ILexicon
    {
      return await RequestAsync<T>("POST", input);
    }

    protected async Task<T> RequestAsync<T>(string method, ILexicon input) where T : ILexicon
    {
      if (string.IsNullOrEmpty(CurrentHost))
      {
        throw new ArgumentException();
      }

      var http = new HTTP();

      var rawPost = input as IRawPost;
      var headers = new NameValueCollection();
      headers["Content-Type"] = "application/json";
      if (rawPost != null)
      {
        headers["Content-Type"] = rawPost.MimeType;
      }
      if (input as ICustomAuthorizationHeaderProvider != null)
      {
        string header = (input as ICustomAuthorizationHeaderProvider).GetAuthorizationHeader(_settings.CurrentCredential);
        if (!string.IsNullOrEmpty(header))
        {
          headers["Authorization"] = header;
        }
      }
      else
      {
        headers["Authorization"] = $"Bearer {CurrentCredential.AccessToken}";
      }
      var url = $"https://{CurrentHost}/xrpc/{input.EndpointID}";
      string responseJson = null;
      string bodyJson = string.Empty;
      try
      {
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
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        var error = ex.Response != null ? await new StreamReader(ex.Response.GetResponseStream()).ReadToEndAsync() : ex.ToString();
        if (ex?.Response?.Headers != null && ex.Response.Headers["content-type"].ToLower().StartsWith("application/json"))
        {
          var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(error) as Newtonsoft.Json.Linq.JObject;
          if (jsonObj != null && jsonObj.GetValue("error") != null && jsonObj.GetValue("error").ToString() == "ExpiredToken")
          {
            if (await RefreshCredentials())
            {
              headers["Authorization"] = $"Bearer {CurrentCredential.AccessToken}";
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
            else
            {
              throw ex;
            }
          }
          else
          {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[HTTP ERROR {webResponse.StatusCode}] {error}");
#endif
            throw new WebException(error, ex, ex.Status, ex.Response);
          }
        }
        else
        {
          throw new WebException(error, ex, ex.Status, ex.Response);
        }
      }

      return responseJson != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseJson, _deserializerSettings) : default(T);
    }

    private async Task<bool> RefreshCredentials()
    {
      var body = new Lexicons.COM.ATProto.Server.RefreshSession();
      var response = await PostAsync<Lexicons.COM.ATProto.Server.RefreshSessionResponse>(body);
      if (response != null && !string.IsNullOrEmpty(response.accessJwt))
      {
        CurrentCredential.DID = response.did;
        CurrentCredential.Handle = response.handle;
        CurrentCredential.AccessToken = response.accessJwt;
        CurrentCredential.RefreshToken = response.refreshJwt;
        await _settings.WriteCredentials();
      }

      return true;
    }

    private string SerializeInputToQueryString(ILexicon input)
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
              { "blob",                                    typeof(Blob) },
              { "app.bsky.actor.defs#adultContentPref",    typeof(Lexicons.App.BSky.Actor.Defs.AdultContentPref) },
              { "app.bsky.actor.defs#contentLabelPref",    typeof(Lexicons.App.BSky.Actor.Defs.ContentLabelPref) },
              { "app.bsky.actor.defs#savedFeedsPref",      typeof(Lexicons.App.BSky.Actor.Defs.SavedFeedsPref) },
              { "app.bsky.actor.defs#personalDetailsPref", typeof(Lexicons.App.BSky.Actor.Defs.PersonalDetailsPref) },
              { "app.bsky.actor.defs#feedViewPref",        typeof(Lexicons.App.BSky.Actor.Defs.FeedViewPref) },
              { "app.bsky.actor.defs#threadViewPref",      typeof(Lexicons.App.BSky.Actor.Defs.ThreadViewPref) },
              { "app.bsky.actor.searchActors",             typeof(Lexicons.App.BSky.Actor.SearchActors) },
              { "app.bsky.actor.searchActorsTypeahead",    typeof(Lexicons.App.BSky.Actor.SearchActorsTypeahead) },
              { "app.bsky.embed.external",                 typeof(Lexicons.App.BSky.Embed.External) },
              { "app.bsky.embed.external#view",            typeof(Lexicons.App.BSky.Embed.External.View) },
              { "app.bsky.embed.images",                   typeof(Lexicons.App.BSky.Embed.Images) },
              { "app.bsky.embed.images#image",             typeof(Lexicons.App.BSky.Embed.Images.Image) },
              { "app.bsky.embed.images#view",              typeof(Lexicons.App.BSky.Embed.Images.View) },
              { "app.bsky.embed.record",                   typeof(Lexicons.App.BSky.Embed.Record) },
              { "app.bsky.embed.record#view",              typeof(Lexicons.App.BSky.Embed.Record.View) },
              { "app.bsky.embed.record#viewRecord",        typeof(Lexicons.App.BSky.Embed.Record.ViewRecord) },
              { "app.bsky.embed.record#viewNotFound",      typeof(Lexicons.App.BSky.Embed.Record.ViewNotFound) },
              { "app.bsky.embed.record#viewBlocked",       typeof(Lexicons.App.BSky.Embed.Record.ViewBlocked) },
              { "app.bsky.embed.recordWithMedia",          typeof(Lexicons.App.BSky.Embed.RecordWithMedia) },
              { "app.bsky.embed.recordWithMedia#view",     typeof(Lexicons.App.BSky.Embed.RecordWithMedia.View) },
              { "app.bsky.feed.defs#generatorView",        typeof(Lexicons.App.BSky.Feed.Defs.GeneratorView) },
              { "app.bsky.feed.defs#postView",             typeof(Lexicons.App.BSky.Feed.Defs.PostView) },
              { "app.bsky.feed.defs#reasonRepost",         typeof(Lexicons.App.BSky.Feed.Defs.ReasonRepost) },
              { "app.bsky.feed.defs#threadViewPost",       typeof(Lexicons.App.BSky.Feed.Defs.ThreadViewPost) },
              { "app.bsky.feed.like",                      typeof(Lexicons.App.BSky.Feed.Like) },
              { "app.bsky.feed.post",                      typeof(Lexicons.App.BSky.Feed.Post) },
              { "app.bsky.feed.post#replyRef",             typeof(Lexicons.App.BSky.Feed.Post.ReplyRef) },
              { "app.bsky.feed.repost",                    typeof(Lexicons.App.BSky.Feed.Repost) },
              { "app.bsky.feed.searchPosts",               typeof(Lexicons.App.BSky.Feed.SearchPosts) },
              { "app.bsky.feed.threadgate",                typeof(Lexicons.App.BSky.Feed.Threadgate) },
              { "app.bsky.graph.follow",                   typeof(Lexicons.App.BSky.Graph.Follow) },
              { "app.bsky.richtext.facet",                 typeof(Lexicons.App.BSky.RichText.Facet) },
              { "app.bsky.richtext.facet#byteSlice",       typeof(Lexicons.App.BSky.RichText.Facet.ByteSlice) },
              { "app.bsky.richtext.facet#link",            typeof(Lexicons.App.BSky.RichText.Facet.Link) },
              { "app.bsky.richtext.facet#mention",         typeof(Lexicons.App.BSky.RichText.Facet.Mention) },
              { "app.bsky.richtext.facet#tag",             typeof(Lexicons.App.BSky.RichText.Facet.Tag) },
              { "com.atproto.label.defs#selfLabels",       typeof(Lexicons.COM.ATProto.Label.Defs.SelfLabels) },
              { "com.atproto.repo.createRecord",           typeof(Lexicons.COM.ATProto.Repo.CreateRecord) },
              { "com.atproto.repo.deleteRecord",           typeof(Lexicons.COM.ATProto.Repo.DeleteRecord) },
              { "com.atproto.repo.strongRef",              typeof(Lexicons.COM.ATProto.Repo.StrongRef) },
              { "com.atproto.repo.uploadBlob",             typeof(Lexicons.COM.ATProto.Repo.UploadBlob) },
              { "com.atproto.server.createSession",        typeof(Lexicons.COM.ATProto.Server.CreateSession) },
              { "com.atproto.server.refreshSession",       typeof(Lexicons.COM.ATProto.Server.RefreshSession) },
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
