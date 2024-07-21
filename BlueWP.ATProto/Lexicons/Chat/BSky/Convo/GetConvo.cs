using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/getConvo.json"/>
  public class GetConvo : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.getConvo";

    public string convoId;
  }
  public class GetConvoResponse : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.getConvo";

    public Defs.ConvoView convo;
  }
}
