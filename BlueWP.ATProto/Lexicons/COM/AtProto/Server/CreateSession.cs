namespace BlueWP.ATProto.Lexicons.COM.ATProto.Server
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/server/createSession.json"/>
  public class CreateSession : ILexicon, ICustomAuthorizationHeaderProvider
  {
    public string EndpointID => "com.atproto.server.createSession";
    public string GetAuthorizationHeader(Settings.AccountSettingsData accountSettings) { return string.Empty; }

    public string identifier;
    public string password;

  }
  public class CreateSessionResponse : ILexicon, ICustomAuthorizationHeaderProvider
  {
    public string EndpointID => "com.atproto.server.createSession";
    public string GetAuthorizationHeader(Settings.AccountSettingsData accountSettings) { return string.Empty; }

    public string accessJwt;
    public string refreshJwt;
    public string handle;
    public string did;
    public object didDoc;
    public string email;
    public bool emailConfirmed;
  }
}
