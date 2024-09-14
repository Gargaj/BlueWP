using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getPreferences.json"/>
  public class GetPreferences : ILexiconRequest
  {
    public string EndpointID => "app.bsky.actor.getPreferences";

    public class Response : ILexiconResponse
    {
      public List<object> preferences;
    }
  }
}
