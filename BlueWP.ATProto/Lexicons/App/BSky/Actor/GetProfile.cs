using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/actor/getProfile.json"/>
  public class GetProfile : ILexicon
  {
    public string EndpointID => "app.bsky.actor.getProfile";

    public string actor;
  }
  public class GetProfileResponse : Defs.ProfileViewDetailed, ILexicon
  {
    public string EndpointID => "app.bsky.actor.getProfile";
  }
}
