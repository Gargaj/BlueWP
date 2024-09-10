using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/video.json"/>
  public class Video
  {
    public Blob video = null;
    public List<Caption> captions;
    public string alt;
    public Defs.AspectRatio aspectRatio;

    public class Caption
    {
      public string lang;
      public Blob file = null;
    }

    public class View
    {
      public string cid;
      public string playlist;
      public string thumbnail;
      public string alt;
      public Defs.AspectRatio aspectRatio;
    }
  }
}
