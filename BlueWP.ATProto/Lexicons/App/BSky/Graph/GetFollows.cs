using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Graph
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/graph/getFollows.json"/>
  public class GetFollows : ILexiconRequest
  {
    public string EndpointID => "app.bsky.graph.getFollows";

    public string actor;
    public uint? limit;
    public string cursor;
  }
  public class Response : ILexiconResponse
  {
    public string EndpointID => "app.bsky.graph.getFollows";

    public Actor.Defs.ProfileView subject;
    public string cursor;
    public List<Actor.Defs.ProfileView> follows;
  }
}
