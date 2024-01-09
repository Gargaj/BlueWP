using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getFeedGenerators.json"/>
  public class GetFeedGenerators : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getFeedGenerators";

    public List<string> feeds;
  }
  public class GetFeedGeneratorsResponse : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getFeedGenerators";

    public List<Defs.GeneratorView> feeds;
  }
}
