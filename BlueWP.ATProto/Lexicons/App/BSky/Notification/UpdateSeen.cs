using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Notification
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/notification/updateSeen.json"/>
  public class UpdateSeen : ILexiconRequest
  {
    public string EndpointID => "app.bsky.notification.updateSeen";

    public DateTime? seenAt;

    public class Response : ILexiconResponse
    {
    }
  }
}
