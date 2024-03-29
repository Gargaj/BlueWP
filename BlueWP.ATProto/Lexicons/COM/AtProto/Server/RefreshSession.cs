﻿namespace BlueWP.ATProto.Lexicons.COM.ATProto.Server
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/server/refreshSession.json"/>
  public class RefreshSession : ILexicon, ICustomAuthorizationHeaderProvider
  {
    public string EndpointID => "com.atproto.server.refreshSession";

    public string GetAuthorizationHeader(Settings.Credential credentials)
    {
      return $"Bearer {credentials.RefreshToken}";
    }
  }
  public class RefreshSessionResponse : ILexicon, ICustomAuthorizationHeaderProvider
  {
    public string EndpointID => "com.atproto.server.refreshSession";

    public string GetAuthorizationHeader(Settings.Credential credentials)
    {
      return $"Bearer {credentials.RefreshToken}";
    }

    public string accessJwt;
    public string refreshJwt;
    public string handle;
    public string did;
    public object didDoc;
  }
}
