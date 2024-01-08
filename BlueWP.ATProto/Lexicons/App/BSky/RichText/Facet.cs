using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.RichText
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/richtext/facet.json"/>
  public class Facet
  {
    public ByteSlice index;
    public List<object> features;

    public class Mention
    {
      public string did;
    }

    public class Link
    {
      public string uri;
    }

    public class Tag
    {
      public string tag;
    }

    public class ByteSlice
    {
      public uint byteStart;
      public uint byteEnd;
    }
  }
}
