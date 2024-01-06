using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Actor
{
    /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/actor/defs.json"/>
    public class Defs
    {
        public class ProfileViewBasic
        {
            public string did;
            public string handle;
            public string displayName;
            public string avatar;
            public object viewer;
            public object labels;
        }
        public class ProfileView
        {
            public string did;
            public string handle;
            public string displayName;
            public string description;
            public string avatar;
            public DateTime indexedAt;
            public object viewer;
            public object labels;
        }
    }
}
