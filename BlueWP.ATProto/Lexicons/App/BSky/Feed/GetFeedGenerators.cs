using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getFeedGenerators.json"/>
  public class GetFeedGenerators : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getFeedGenerators";

    public List<string> feeds;
  }
  public class GetFeedGeneratorsResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getFeedGenerators";

    public List<Defs.GeneratorView> feeds;
  }
}
