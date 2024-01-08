using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/external.json"/>
  public class External
  {
    public class View
    {
      public ViewExternal external;

      public string URL => external?.uri ?? "[ERROR]";
      public bool HasThumb => !string.IsNullOrEmpty(external?.thumb);
      public string ThumbURL => external?.thumb ?? "[ERROR]";
      public string Hostname => new Uri(external?.uri ?? string.Empty).Host;
      public string Title => external?.title ?? "[ERROR]";
      public string Description => external?.description ?? "[ERROR]";
    }

    public class ViewExternal
    {
      public string uri;
      public string title;
      public string description;
      public string thumb;
    }
  }
}
