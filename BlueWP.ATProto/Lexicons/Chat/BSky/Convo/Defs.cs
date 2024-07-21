using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.Chat.BSky.Convo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/convo/defs.json"/>
  public class Defs
  {
    public class MessageRef
    {
      public string did;
      public string convoId;
      public string messageId;
    }

    public class MessageInput
    {
      public string text;
      public List<App.BSky.RichText.Facet> facets;
      public App.BSky.Embed.Record.View embed;
    }

    public class MessageView
    {
      public string id;
      public string rev;
      public string text;
      public List<App.BSky.RichText.Facet> facets;
      public App.BSky.Embed.Record.View embed;
      public MessageViewSender sender;
      public DateTime sentAt;
    }

    public class MessageViewSender
    {
      public string did;
    }

    public class ConvoView
    {
      public string id;
      public string rev;
      public List<Actor.Defs.ProfileViewBasic> members;
      public object lastMessage;
      public bool muted;
      public int unreadCount;
    }
  }
}
