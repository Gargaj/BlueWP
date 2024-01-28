using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Graph
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/graph/getFollows.json"/>
  public class GetFollows : ILexicon
  {
    public string EndpointID => "app.bsky.graph.getFollows";

    public string actor;
    public uint? limit;
    public string cursor;
  }
  public class GetFollowsResponse : ILexicon
  {
    public string EndpointID => "app.bsky.graph.getFollows";

    public Actor.Defs.ProfileView subject;
    public string cursor;
    public List<Actor.Defs.ProfileView> follows;
  }
}
