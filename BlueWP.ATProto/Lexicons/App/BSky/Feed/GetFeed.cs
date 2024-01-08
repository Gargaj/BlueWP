using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getFeed.json"/>
  public class GetFeed : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getFeed";

    public string feed;
    public uint? limit;
    public string cursor;
  }
  public class GetFeedResponse : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getFeed";

    public string cursor;
    public List<Defs.FeedViewPost> feed;
  }
}
