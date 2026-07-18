using System.Collections.Specialized;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/updateRead.json"/>
  public class UpdateRead : ILexiconRequest, ICustomHeaderProvider
  {
    public string EndpointID => "chat.bsky.convo.updateRead";

    public void SetCustomHeaders(NameValueCollection headers, Settings.AccountSettingsData accountSettings)
    {
      headers["atproto-proxy"] = $"did:web:api.bsky.chat#bsky_chat";
    }

    public string convoId;
    public string messageId;

    public class Response : ILexiconResponse
    {
      public Defs.ConvoView convo;
    }
  }
}
