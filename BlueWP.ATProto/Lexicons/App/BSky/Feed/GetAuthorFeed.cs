using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getAuthorFeed.json"/>
  public class GetAuthorFeed : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getAuthorFeed";

    public string actor;
    public uint? limit;
    public string cursor;
    public string filter;
  }
  public class GetAuthorFeedResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getAuthorFeed";

    public string cursor;
    public List<Defs.FeedViewPost> feed;
  }
}
