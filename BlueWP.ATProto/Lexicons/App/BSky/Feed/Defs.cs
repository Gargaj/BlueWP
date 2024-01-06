using System;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
    /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/defs.json"/>
    public class Defs
    {
        public class PostView
        {
            public string uri;
            public string cid;
            public Actor.Defs.ProfileViewBasic author;
            public object record; // marked as unknown
            public object embed; // TODO
            public uint? replyCount;
            public uint? repostCount;
            public uint? likeCount;
            public DateTime indexedAt;
            public object viewer; // TODO
            public object labels; // TODO
            public object threadgate; // TODO
        }

        public class FeedViewPost
        {
            public PostView post;
            public ReplyRef reply;
            public ReasonRepost reason;

            public string PostAuthor
            {
                get
                {
                    return post?.author?.displayName ?? "[ERROR]";
                }
            }
            public string PostReason
            {
                get
                {
                    return reason == null ? string.Empty : $"Reposted by {reason?.by?.displayName}";
                }
            }
            public string PostAvatar
            {
                get
                {
                    return post?.author?.avatar ?? string.Empty;
                }
            }
            public string PostText
            {
                get
                {
                    var jobjPost = post?.record as Newtonsoft.Json.Linq.JObject;
                    if (jobjPost == null)
                    {
                        return "[ERROR]";
                    }
                    var type = jobjPost.GetValue("$type")?.ToString() ?? string.Empty;
                    if (type == "app.bsky.feed.post")
                    {
                        var typedPost = jobjPost.ToObject<Post>();
                        if (typedPost == null)
                        {
                            return "[ERROR]";
                        }
                        return typedPost.text;
                    }
                    return "[UNKNOWN]";
                }
            }
        }

        public class ReplyRef
        {
            public object root; // TODO
            public object parent; // TODO
        }

        public class ReasonRepost
        {
            public Actor.Defs.ProfileViewBasic by;
            public DateTime indexedAt;
        }
    }
}
