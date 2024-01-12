using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Notification
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/notification/getUnreadCount.json"/>
  public class GetUnreadCount : ILexicon
  {
    public string EndpointID => "app.bsky.notification.getUnreadCount";

    public DateTime? seenAt;
  }
  public class GetUnreadCountResponse : ILexicon
  {
    public string EndpointID => "app.bsky.notification.getUnreadCount";

    public uint count;
  }
}
