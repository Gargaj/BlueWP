using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/actor/getProfiles.json"/>
  public class GetProfiles : ILexicon
  {
    public string EndpointID => "app.bsky.actor.getProfiles";

    public List<string> actors;
  }
  public class GetProfilesResponse : ILexicon
  {
    public string EndpointID => "app.bsky.actor.getProfiles";

    public List<Defs.ProfileViewDetailed> profiles;
  }
}
