using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getPostThread.json"/>
  public class GetPostThread : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.getPostThread";

    public string uri;
    public uint? depth;
    public uint? parentHeight;

    public class Response : ILexiconResponse
    {
      public object thread;
    }
  }
}
