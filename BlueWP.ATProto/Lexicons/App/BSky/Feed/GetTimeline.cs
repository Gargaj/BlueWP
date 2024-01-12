using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getTimeline.json"/>
  public class GetTimeline : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getTimeline";

    public string algorithm;
    public uint? limit;
    public string cursor;
  }
  public class GetTimelineResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getTimeline";

    public string cursor;
    public List<Defs.FeedViewPost> feed;
  }
}
