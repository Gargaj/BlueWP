using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
    /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/post.json"/>
    public class Post : LexiconBase
    {
        public override string EndpointID => "app.bsky.feed.post";

        public string text;
        // entities
        // facets
        // reply
        // embed
        // langs
        // labels
        // tags
        public DateTime createdAt;
    }
}
