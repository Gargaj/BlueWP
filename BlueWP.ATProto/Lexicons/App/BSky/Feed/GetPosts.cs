using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getPosts.json"/>
  public class GetPosts : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.getPosts";

    public List<string> uris;

    public class Response : ILexiconResponse
    {
      public List<Defs.PostView> posts;
    }
  }
}
