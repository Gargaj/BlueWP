using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/getMessages.json"/>
  public class GetMessages : ILexiconRequest, ICustomHeaderProvider
  {
    public string EndpointID => "chat.bsky.convo.getMessages";

    public void SetCustomHeaders(NameValueCollection headers, Settings.AccountSettingsData accountSettings)
    {
      headers["atproto-proxy"] = $"did:web:api.bsky.chat#bsky_chat";
    }

    public string convoId;
    public uint? limit;
    public string cursor;

    public class Response : ILexiconResponse
    {
      public string cursor;
      public List<object> messages;
    }
  }
}
