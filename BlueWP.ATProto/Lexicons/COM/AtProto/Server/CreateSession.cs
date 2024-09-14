using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlueWP.ATProto.Lexicons.COM.ATProto.Server
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/server/createSession.json"/>
  public class CreateSession : ILexiconRequest, ICustomHeaderProvider
  {
    public string EndpointID => "com.atproto.server.createSession";
    public void SetCustomHeaders(NameValueCollection headers, Settings.AccountSettingsData accountSettings)
    {
      headers.Remove("Authorization");
    }

    public string identifier;
    public string password;

    public class Response : ILexiconResponse
    {
      public string accessJwt;
      public string refreshJwt;
      public string handle;
      public string did;
      public object didDoc;
      public string email;
      public bool emailConfirmed;
      public bool emailAuthFactor;
      public bool active;
      public string status;
    }
  }

  public class DIDDoc
  {
    public class VerificationMethod
    {
      public string id;
      public string type;
      public string controller;
      public string publicKeyMultibase;
    }
    public class Service
    {
      public string id;
      public string type;
      public string serviceEndpoint;
    }
    public string id;
    public List<string> alsoKnownAs;
    public List<VerificationMethod> verificationMethod;
    public List<Service> service;
  }
}
