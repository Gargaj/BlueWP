using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/listConvos.json"/>
  public class ListConvos : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.listConvos";

    public uint? limit;
    public string cursor;
  }
  public class ListConvosResponse : ILexicon
  {
    public string EndpointID => "chat.bsky.convo.listConvos";

    public string cursor;
    public List<Defs.ConvoView> convos;
  }
}
