using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getFeedGenerator.json"/>
  public class GetFeedGenerator : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.getFeedGenerator";

    public string feed;

    public class Response : ILexiconResponse
    {
      public Defs.GeneratorView view;
      public bool isOnline;
      public bool isValid;
    }
  }
}
