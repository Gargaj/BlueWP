using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/sendMessage.json"/>
  public class SendMessage : ILexiconRequest, ICustomHeaderProvider
  {
    public string EndpointID => "chat.bsky.convo.sendMessage";

    public void SetCustomHeaders(NameValueCollection headers, Settings.AccountSettingsData accountSettings)
    {
      headers["atproto-proxy"] = $"did:web:api.bsky.chat#bsky_chat";
    }

    public string convoId;
    public Defs.MessageInput message;

    public class Response : Defs.MessageView, ILexiconResponse
    {
    }
  }
}
