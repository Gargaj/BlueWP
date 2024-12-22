using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/images.json"/>
  public class Images
  {
    public List<Image> images;

    public class Image
    {
      public Blob image;
      public string alt;
      public Defs.AspectRatio aspectRatio;
    }

    public class View
    {
      public List<ViewImage> images;
    }

    public class ViewImage
    {
      public string thumb;
      public string fullsize;
      public string alt;
      public Defs.AspectRatio aspectRatio;

      public string ThumbURL
      {
        get
        {
          return thumb;
        }
      }
    }
  }
}
