using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Graph
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/graph/defs.json"/>
  public class Defs
  {
    public class ListViewBasic
    {
      public string uri;
      public string cid;
      public string name;
      public string purpose;
      public string avatar;
      public uint listItemCount;
      public List<COM.ATProto.Label.Defs.Label> labels;
      public ListViewerState viewer;
      public DateTime indexedAt;
    }
    public class StarterPackView
    {
      public string uri;
      public string cid;
      public object record;
      public Actor.Defs.ProfileViewBasic creator;
      public ListViewBasic list;
      public List<ListViewBasic> listItemsSample;
      public List<Feed.Defs.GeneratorView> feeds;
      public uint joinedWeekCount;
      public uint joinedAllTimeCount;
      public List<COM.ATProto.Label.Defs.Label> labels;
      public DateTime indexedAt;
    }
    public class StarterPackViewBasic
    {
      public string uri;
      public string cid;
      public object record;
      public Actor.Defs.ProfileViewBasic creator;
      public uint listItemCount;
      public uint joinedWeekCount;
      public uint joinedAllTimeCount;
      public List<COM.ATProto.Label.Defs.Label> labels;
      public DateTime indexedAt;
    }
    public class ListViewerState
    {
      public bool muted;
      public string blocked;
    }
  }
}
