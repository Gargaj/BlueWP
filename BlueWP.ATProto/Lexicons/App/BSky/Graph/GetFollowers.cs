using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Graph
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/graph/getFollowers.json"/>
  public class GetFollowers : ILexicon
  {
    public string EndpointID => "app.bsky.graph.getFollowers";

    public string actor;
    public uint? limit;
    public string cursor;
  }
  public class GetFollowersResponse : ILexicon
  {
    public string EndpointID => "app.bsky.graph.getFollowers";

    public Actor.Defs.ProfileView subject;
    public string cursor;
    public List<Actor.Defs.ProfileView> followers;
  }
}
