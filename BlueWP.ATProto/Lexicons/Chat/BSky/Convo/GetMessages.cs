using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/getMessages.json"/>
  public class GetMessages : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.getMessages";

    public string convoId;
    public uint? limit;
    public string cursor;
  }
  public class GetMessagesResponse : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.getMessages";

    public string cursor;
    public List<object> messages;
  }
}
