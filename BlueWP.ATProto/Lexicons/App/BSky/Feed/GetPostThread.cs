using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getPostThread.json"/>
  public class GetPostThread : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getPostThread";

    public string uri;
    public uint? depth;
    public uint? parentHeight;
  }
  public class GetPostThreadResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getPostThread";

    public object thread;
  }
}
