using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/record.json"/>
  public class Record
  {
    public COM.ATProto.Repo.StrongRef record = null;

    public class View
    {
      public object record = null;
    }

    public class ViewRecord : IPost
    {
      public string uri = string.Empty;
      public string cid = string.Empty;
      public Actor.Defs.ProfileViewBasic author = null;
      public object value = null;
      public List<COM.ATProto.Label.Defs.Label> labels = null;
      public List<object> embeds = null;

      // To be clear, these MAY be true in actuality, but we don't know from just a view
      public bool IsRepost => false;
      public bool IsReply => false;
      public bool HasQuotedPost => false;
      public bool HasEmbedExternal => false;

      public string PostAuthorAvatarURL => author?.avatar ?? "[ERROR]";
      public string PostAuthorDisplayName => author?.DisplayName ?? "[ERROR]";
      public string PostAuthorHandle => author?.Handle ?? "[ERROR]";
      public string PostElapsedTime => Helpers.ToElapsedTime((value as Feed.Post).createdAt);
      public string PostText => (value as Feed.Post) == null ? "[ERROR]" : (value as Feed.Post).text;
   }

    public class ViewNotFound
    {
      public string uri = string.Empty;
      public bool notFound = false;
    }

    public class ViewBlocked
    {
      public string uri = string.Empty;
      public bool blocked = false;
      public Feed.Defs.BlockedAuthor author = null;
    }
  }
}
