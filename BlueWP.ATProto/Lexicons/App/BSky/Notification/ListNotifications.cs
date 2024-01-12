using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueWP.ATProto.Lexicons.App.BSky.Notification
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/notification/listNotifications.json"/>
  public class ListNotifications : ILexicon
  {
    public string EndpointID => "app.bsky.notification.listNotifications";

    public uint limit;
    public string cursor;
    public DateTime? seenAt;
  }
  public class ListNotificationsResponse : ILexicon
  {
    public string EndpointID => "app.bsky.notification.listNotifications";

    public string cursor;
    public List<Notification> notifications;
    public DateTime? seenAt;
  }
  public class Notification : IPost
  {
    public string uri;
    public string cid;
    public Actor.Defs.ProfileView author;
    public string reason;
    public string reasonSubject;
    public object record; // type: unknown
    public bool isRead;
    public DateTime? indexedAt;
    public List<COM.ATProto.Label.Defs.Label> labels;

    // To be clear, these MAY be true in actuality, but we don't know from just a view
    public bool IsRepost => false;
    public bool IsReply => false;
    public bool HasQuotedPost => false;
    public bool HasEmbedExternal => false;

    public string PostAuthorAvatarURL => author?.avatar;
    public string PostAuthorDisplayName => author?.DisplayName ?? "[ERROR]";
    public string PostAuthorHandle => author?.Handle ?? "[ERROR]";
    public string PostElapsedTime => indexedAt != null ? Helpers.ToElapsedTime(indexedAt.GetValueOrDefault()) : string.Empty;
    public string PostText => (record as Feed.Post) == null ? "[ERROR]" : (record as Feed.Post).text;
    public IEnumerable<Embed.Images.ViewImage> PostImages => null; // TODO

  }
}
