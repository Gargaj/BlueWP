using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/record.json"/>
  class Record
  {
    public class View
    {
      public object record = null;
    }

    public class ViewRecord
    {
      public string uri = string.Empty;
      public string cid = string.Empty;
      public object value = null;
      public List<COM.AtProto.Label.Defs.Label> labels = null;
      public List<object> embeds = null;
    }

    public class ViewNotFound
    {
      public string uri = string.Empty;
      public bool notFound = false;
    }

    public class ViewBlocked
    {
      public string uri = string.Empty;
      public bool blocked = false;
      public Feed.Defs.BlockedAuthor author = null;
    }
  }
}
