namespace BlueWP.ATProto.Lexicons.Chat.BSky.Actor
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/chat/bsky/actor/defs.json"/>
  public class Defs
  {
    public class ProfileViewBasic
    {
      public string did;
      public string handle;
      public string displayName;
      public string avatar;
      public App.BSky.Actor.Defs.ProfileAssociated associated;
      public object viewer;
      public object labels;
      public bool chatDisabled;

      public string DisplayName
      {
        get
        {
          return displayName ?? handle ?? "[ERROR]";
        }
      }
      public string Handle
      {
        get
        {
          return $"@{handle}";
        }
      }
    }
  }
}
