﻿namespace BlueWP.ATProto.Lexicons.COM.ATProto.Repo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/repo/getRecord.json"/>
  public class GetRecord : ILexiconRequest
  {
    public string EndpointID => "com.atproto.repo.getRecord";

    public string repo;
    public string collection;
    public string rkey;
    public string cid;

    public class Response : ILexiconResponse
    {
      public string uri;
      public string cid;
      public object value;
    }
  }
}
