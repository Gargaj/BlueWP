using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getPosts.json"/>
  public class GetPosts : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getPosts";

    public List<string> uris;
  }
  public class GetPostsResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getPosts";

    public List<Defs.PostView> posts;
  }
}
