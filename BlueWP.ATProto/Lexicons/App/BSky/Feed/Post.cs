using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/post.json"/>
  public class Post : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.post";

    public string text;
    public List<Entity> entities;
    public List<RichText.Facet> facets;
    public ReplyRef reply;
    public object embed;
    public List<string> langs;
    public object labels;
    public List<string> tags;
    public DateTime createdAt;

    public class ReplyRef
    {
      public object root; // union
      public object parent; // union
    }

    public class Entity
    {
      public TextSlice index;
      public string type;
      public string value;
    }

    public class TextSlice
    {
      public uint start;
      public uint end;
    }
  }
}
