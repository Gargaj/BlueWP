using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Graph
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/graph/starterpack.json"/>
  public class StarterPack
  {
    public string name;
    public string description;
    public List<RichText.Facet> descriptionFacets;
    public string list;
    public List<FeedItem> feeds;
    public DateTime? createdAt;
  }
  public class FeedItem
  {
    public string uri;
  }
}
  