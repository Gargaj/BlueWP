using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueWP.ATProto.Lexicons.App.BSky.Notification
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/notification/listNotifications.json"/>
  public class ListNotifications : LexiconBase
  {
    public override string EndpointID => "app.bsky.notification.listNotifications";

    public uint limit;
    public string cursor;
    public DateTime? seenAt;
  }
  public class ListNotificationsResponse : LexiconBase
  {
    public override string EndpointID => "app.bsky.notification.listNotifications";

    public string cursor;
    public List<Notification> notifications;
    public DateTime? seenAt;
  }
  public class Notification
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

    public string TempString => 
      $"Reason: {reason}\n"
      + $"Author: {author?.DisplayName}\n"
      + $"Subject: {reasonSubject}";
  }
}
