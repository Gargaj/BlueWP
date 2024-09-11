using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getLikes.json"/>
  public class GetLikes : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getLikes";

    public string uri;
    public string cid;
    public uint? limit;
    public string cursor;
  }
  public class GetLikesResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getLikes";

    public string uri;
    public string cid;
    public string cursor;
    public List<Like> posts;

    public class Like
    {
      public DateTime? indexedAt;
      public DateTime? createdAt;
      public Actor.Defs.ProfileView actor;
    }
  }
}
