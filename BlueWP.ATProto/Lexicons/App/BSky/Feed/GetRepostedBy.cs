using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getRepostedBy.json"/>
  public class GetRepostedBy : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getRepostedBy";

    public string uri;
    public string cid;
    public uint? limit;
    public string cursor;
  }
  public class GetRepostedByResponse : ILexicon
  {
    public string EndpointID => "app.bsky.feed.getRepostedBy";

    public string uri;
    public string cid;
    public string cursor;
    public List<Actor.Defs.ProfileView> repostedBy;
  }
}
