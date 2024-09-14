using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getFeedGenerators.json"/>
  public class GetFeedGenerators : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.getFeedGenerators";

    public List<string> feeds;

    public class Response : ILexiconResponse
    {
      public List<Defs.GeneratorView> feeds;
    }
  }
}
