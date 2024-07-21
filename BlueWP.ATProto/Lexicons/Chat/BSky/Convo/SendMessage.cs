using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/sendMessage.json"/>
  public class SendMessage : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.sendMessage";

    public string convoId;
    public Defs.MessageInput message;
  }
  public class SendMessageResponse : Defs.MessageView, ILexicon
  {
    public string EndpointID => "chat.bsky.convo.sendMessage";
  }
}
