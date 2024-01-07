using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getTimeline.json"/>
  public class GetTimeline : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getTimeline";

    public string algorithm;
    public uint? limit;
    public string cursor;
  }
  public class GetTimelineResponse : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getTimeline";

    public string cursor;
    public List<Defs.FeedViewPost> feed;
  }
}
