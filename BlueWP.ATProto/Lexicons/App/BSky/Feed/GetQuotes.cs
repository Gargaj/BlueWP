using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getQuotes.json"/>
  public class GetQuotes : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.getQuotes";

    public string uri;
    public string cid;
    public uint? limit;
    public string cursor;

    public class Response : ILexiconResponse
    {
      public string uri;
      public string cid;
      public string cursor;
      public List<Defs.PostView> posts;
    }
  }
}
