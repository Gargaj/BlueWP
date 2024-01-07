namespace BlueWP.ATProto.Lexicons.COM.AtProto.Server
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/server/createSession.json"/>
  public class CreateSession : LexiconBase
  {
    public override string EndpointID => "com.atproto.server.createSession";
    public override bool RequiresAuthorization => false;

    public string identifier;
    public string password;
  }
  public class CreateSessionResponse : LexiconBase
  {
    public override string EndpointID => "com.atproto.server.createSession";
    public override bool RequiresAuthorization => false;

    public string accessJwt;
    public string refreshJwt;
    public string handle;
    public string did;
    public object didDoc;
    public string email;
    public bool emailConfirmed;
  }
}
