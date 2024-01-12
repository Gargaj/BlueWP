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
    private const string _credentialsFilename = "credentials.dat";
    private string _appPassword;
    private Credentials _credentials;
    private Newtonsoft.Json.JsonSerializerSettings _deserializerSettings;
    public Client()
    {
      _deserializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
      {
        MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.ReadAhead,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
        SerializationBinder = new TypesBinder(),
        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
      };
    }

    public async Task<bool> Authenticate()
    {
      return await ReadCredentials();
    }

    public async Task<bool> AuthenticateWithPassword(string host, string handle, string appPassword)
    {
      if (string.IsNullOrEmpty(host)
          || string.IsNullOrEmpty(handle)
          || string.IsNullOrEmpty(appPassword))
      {
        throw new ArgumentException();
      }

      _credentials.serviceHost = host;
      _credentials.handle = handle;
      _appPassword = appPassword;

      var body = new Lexicons.COM.ATProto.Server.CreateSession()
      {
        identifier = _credentials.handle,
        password = _appPassword,
      };
      var response = await PostAsync<Lexicons.COM.ATProto.Server.CreateSessionResponse>(body);
      if (response != null && !string.IsNullOrEmpty(response.accessJwt))
      {
        _credentials.did = response.did;
        _credentials.handle = response.handle;
        _credentials.accessToken = response.accessJwt;
        _credentials.refreshToken = response.refreshJwt;
        await WriteCredentials();
      }

      return true;
    }

    public bool IsAuthenticated { get { return _credentials.accessToken != null; } }
    public string Handle { get { return _credentials.handle; } }
    public string DID { get { return _credentials.did; } }

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
      if (string.IsNullOrEmpty(_credentials.serviceHost))
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
        string header = (input as ICustomAuthorizationHeaderProvider).GetAuthorizationHeader(_credentials);
        if (!string.IsNullOrEmpty(header))
        {
          headers["Authorization"] = header;
        }
      }
      else
      {
        headers["Authorization"] = $"Bearer {_credentials.accessToken}";
      }
      var url = $"https://{_credentials.serviceHost}/xrpc/{input.EndpointID}";
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
              headers["Authorization"] = $"Bearer {_credentials.accessToken}";
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
        _credentials.did = response.did;
        _credentials.handle = response.handle;
        _credentials.accessToken = response.accessJwt;
        _credentials.refreshToken = response.refreshJwt;
        await WriteCredentials();
      }

      return true;
    }

    private async Task<bool> ReadCredentials()
    {
      try
      {
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        var provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider();

        var file = await localFolder.GetFileAsync(_credentialsFilename);
        var buffProtected = await Windows.Storage.FileIO.ReadBufferAsync(file);

        var buffUnprotected = await provider.UnprotectAsync(buffProtected);
        var strClearText = Windows.Security.Cryptography.CryptographicBuffer.ConvertBinaryToString(Windows.Security.Cryptography.BinaryStringEncoding.Utf8, buffUnprotected);

        _credentials = Newtonsoft.Json.JsonConvert.DeserializeObject<Credentials>(strClearText);

        return IsAuthenticated;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private async Task<bool> WriteCredentials()
    {
      try
      {
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(_credentials);

        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        var provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider("LOCAL=user");

        var buffMsg = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(str, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
        var buffProtected = await provider.ProtectAsync(buffMsg);

        var file = await localFolder.CreateFileAsync(_credentialsFilename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
        await Windows.Storage.FileIO.WriteBufferAsync(file, buffProtected);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> DeleteCredentials()
    {
      try
      {
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        var file = await localFolder.GetFileAsync(_credentialsFilename);
        await file.DeleteAsync();

        _credentials.serviceHost = string.Empty;
        _credentials.did = string.Empty;
        _credentials.handle = string.Empty;
        _credentials.accessToken = string.Empty;
        _credentials.refreshToken = string.Empty;

        return true;
      }
      catch (Exception)
      {
        return false;
      }
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

    public struct Credentials
    {
      public string serviceHost;
      public string did;
      public string handle;
      public string accessToken;
      public string refreshToken;
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
              { "app.bsky.feed.defs#reasonRepost",         typeof(Lexicons.App.BSky.Feed.Defs.ReasonRepost) },
              { "app.bsky.feed.defs#postView",             typeof(Lexicons.App.BSky.Feed.Defs.PostView) },
              { "app.bsky.feed.like",                      typeof(Lexicons.App.BSky.Feed.Like) },
              { "app.bsky.feed.post",                      typeof(Lexicons.App.BSky.Feed.Post) },
              { "app.bsky.feed.threadgate",                typeof(Lexicons.App.BSky.Feed.Threadgate) },
              { "app.bsky.graph.follow",                   typeof(Lexicons.App.BSky.Graph.Follow) },
              { "app.bsky.richtext.facet",                 typeof(Lexicons.App.BSky.RichText.Facet) },
              { "app.bsky.richtext.facet#byteSlice",       typeof(Lexicons.App.BSky.RichText.Facet.ByteSlice) },
              { "app.bsky.richtext.facet#link",            typeof(Lexicons.App.BSky.RichText.Facet.Link) },
              { "app.bsky.richtext.facet#mention",         typeof(Lexicons.App.BSky.RichText.Facet.Mention) },
              { "app.bsky.richtext.facet#tag",             typeof(Lexicons.App.BSky.RichText.Facet.Tag) },
              { "com.atproto.label.defs#selfLabels",       typeof(Lexicons.COM.ATProto.Label.Defs.SelfLabels) },
              { "com.atproto.repo.createRecord",           typeof(Lexicons.COM.ATProto.Repo.CreateRecord) },
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
