using System.Collections.Specialized;

namespace BlueWP.ATProto.Lexicons.COM.ATProto.Server
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/server/refreshSession.json"/>
  public class RefreshSession : ILexiconRequest, ICustomHeaderProvider
  {
    public string EndpointID => "com.atproto.server.refreshSession";

    public void SetCustomHeaders(NameValueCollection headers, Settings.AccountSettingsData accountSettings)
    {
      headers["Authorization"] = $"Bearer {accountSettings.Credentials.RefreshToken}";
    }

    public class Response : ILexiconResponse
    {
      public string accessJwt;
      public string refreshJwt;
      public string handle;
      public string did;
      public object didDoc;
    }
  }
}
