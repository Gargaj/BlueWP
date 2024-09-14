namespace BlueWP.ATProto.Lexicons.COM.ATProto.Identity
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/identity/resolveHandle.json"/>
  public class ResolveHandle : ILexiconRequest
  {
    public string EndpointID => "com.atproto.identity.resolveHandle";

    public string handle;

    public class Response : ILexiconResponse
    {
      public string EndpointID => "com.atproto.identity.resolveHandle";

      public string did;
    }
  }
}
