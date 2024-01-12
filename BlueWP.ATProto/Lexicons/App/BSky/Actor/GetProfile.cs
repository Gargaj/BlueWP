using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
  /// <see cref="  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getAuthorFeed.json"/>
  public class GetProfile : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getAuthorFeed";

    public string actor;
  }
  public class GetProfileResponse : Defs.ProfileViewDetailed
  {
  }
}
