using System;

namespace BlueWP.ATProto.Lexicons.COM.ATProto.Label
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/label/defs.json"/>
  public class Defs
  {
    public class Label
    {
      public string src;
      public string uri;
      public string cid;
      public string val;
      public bool neg;
      public DateTime? cts;
    }
  }
}
