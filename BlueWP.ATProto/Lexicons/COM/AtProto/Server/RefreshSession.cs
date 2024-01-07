namespace BlueWP.ATProto.Lexicons.COM.AtProto.Server
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/server/refreshSession.json"/>
  public class RefreshSession : LexiconBase
  {
    public override string EndpointID => "com.atproto.server.refreshSession";
    public override bool RequiresAuthorization => true;
  }
  public class RefreshSessionResponse : LexiconBase
  {
    public override string EndpointID => "com.atproto.server.refreshSession";
    public override bool RequiresAuthorization => true;

    public string accessJwt;
    public string refreshJwt;
    public string handle;
    public string did;
    public object didDoc;
  }
}
