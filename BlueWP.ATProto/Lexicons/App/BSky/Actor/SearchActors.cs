using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/actor/searchActors.json"/>
  public class SearchActors : ILexicon
  {
    public string EndpointID => "app.bsky.actor.searchActors";

    //public string term; // DEPRECATED
    public string q;
    public uint? limit;
    public string cursor;
  }
  public class SearchActorsResponse : ILexicon
  {
    public string EndpointID => "app.bsky.actor.searchActors";

    public string cursor;
    public List<Defs.ProfileView> actors;
  }
}
