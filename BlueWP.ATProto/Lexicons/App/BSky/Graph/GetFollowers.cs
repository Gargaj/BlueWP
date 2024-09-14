using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Graph
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/graph/getFollowers.json"/>
  public class GetFollowers : ILexiconRequest
  {
    public string EndpointID => "app.bsky.graph.getFollowers";

    public string actor;
    public uint? limit;
    public string cursor;

    public class Response : ILexiconResponse
    {
      public Actor.Defs.ProfileView subject;
      public string cursor;
      public List<Actor.Defs.ProfileView> followers;
    }
  }
}
