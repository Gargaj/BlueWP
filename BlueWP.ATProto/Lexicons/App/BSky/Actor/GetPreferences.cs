using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
    /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getPreferences.json"/>
    public class GetPreferences : LexiconBase
    {
        public override string EndpointID => "app.bsky.actor.getPreferences";
    }
    public class GetPreferencesResponse : LexiconBase
    {
        public override string EndpointID => "app.bsky.actor.getPreferences";

        public List<object> preferences;
    }
}
