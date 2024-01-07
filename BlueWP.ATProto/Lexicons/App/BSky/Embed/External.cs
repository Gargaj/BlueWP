using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/external.json"/>
  public class External
  {
    public class View
    {
      public ViewExternal external;
    }

    public class ViewExternal
    {
      public string uri;
      public string title;
      public string description;
      public string thumb;
    }
  }
}
