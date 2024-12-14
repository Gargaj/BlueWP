using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/searchPosts.json"/>
  public class SearchPosts : ILexiconRequest
  {
    public string EndpointID => "app.bsky.feed.searchPosts";

    public string q;
    public string sort;
    public string since;
    public string until;
    public string mentions;
    public string author;
    public string lang;
    public string domain;
    public string url;
    public string tag;
    public uint? limit;
    public string cursor;

    public class Response : ILexiconResponse
    {
      public string cursor;
      public uint hitsTotal;
      public List<Defs.PostView> posts;
    }
  }
}
