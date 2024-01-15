using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/actor/searchActorsTypeahead.json"/>
  public class SearchActorsTypeahead : ILexicon
  {
    public string EndpointID => "app.bsky.actor.searchActorsTypeahead";

    //public string term; // DEPRECATED
    public string q;
    public uint? limit;
  }
  public class SearchActorsTypeaheadResponse : ILexicon
  {
    public string EndpointID => "app.bsky.actor.searchActorsTypeahead";

    public string cursor;
    public List<Defs.ProfileViewBasic> actors;
  }
}
