using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/post.json"/>
  public class Post : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.post";

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
      public COM.ATProto.Repo.StrongRef root;
      public COM.ATProto.Repo.StrongRef parent;
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
