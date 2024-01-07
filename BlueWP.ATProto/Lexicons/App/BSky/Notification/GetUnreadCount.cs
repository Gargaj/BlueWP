using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Notification
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/notification/getUnreadCount.json"/>
  public class GetUnreadCount : LexiconBase
  {
    public override string EndpointID => "app.bsky.notification.getUnreadCount";

    public DateTime? seenAt;
  }
  public class GetUnreadCountResponse : LexiconBase
  {
    public override string EndpointID => "app.bsky.notification.getUnreadCount";

    public uint count;
  }
}
