using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/updateRead.json"/>
  public class UpdateRead : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.updateRead";

    public string convoId;
    public string messageId;
  }
  public class UpdateReadResponse : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.updateRead";

    public Defs.ConvoView convo;
  }
}
