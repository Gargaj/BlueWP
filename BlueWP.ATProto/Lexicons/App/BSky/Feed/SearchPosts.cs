using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/searchPosts.json"/>
  public class SearchPosts : ILexicon
  {
    public string EndpointID => "app.bsky.feed.searchPosts";

    public string q;
    public uint? limit;
    public string cursor;
  }
  public class SearchPostsResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.searchPosts";

    public string cursor;
    public uint hitsTotal;
    public List<Defs.PostView> posts;
  }
}
