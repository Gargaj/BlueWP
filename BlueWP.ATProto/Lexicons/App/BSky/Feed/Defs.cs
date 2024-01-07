using System;
using System.Collections.Generic;

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
            public object record; // unknown
            public object embed; // union
            public uint? replyCount;
            public uint? repostCount;
            public uint? likeCount;
            public DateTime indexedAt;
            public ViewerState viewer;
            public List<COM.AtProto.Label.Defs.Label> labels;
            public ThreadgateView threadgate;
        }

        public class ViewerState
        {
            public string repost;
            public string like;
            public bool replyDisabled;
        }

        public class FeedViewPost
        {
            public PostView post;
            public ReplyRef reply;
            public ReasonRepost reason;

            public string PostAuthorDisplayName
            {
                get
                {
                    return post?.author?.displayName ?? post?.author?.handle ?? "[ERROR]";
                }
            }
            public string PostAuthorHandle
            {
                get
                {
                    return $"@{post?.author?.handle}";
                }
            }
            public string PostElapsedTime
            {
                get
                {
                    var timespan = DateTime.Now - post.indexedAt;
                    if (timespan.TotalSeconds < 60)
                    {
                        return timespan.ToString("%s") + "s";
                    }
                    if (timespan.TotalSeconds < 60 * 60)
                    {
                        return timespan.ToString("%m") + "m";
                    }
                    if (timespan.TotalSeconds < 60 * 60 * 24)
                    {
                        return timespan.ToString("%h") + "h";
                    }
                    if (timespan.TotalSeconds < 60 * 60 * 24 * 7)
                    {
                        return timespan.ToString("%d") + "d";
                    }
                    if (post.indexedAt.Year != DateTime.Now.Year)
                    {
                        return post.indexedAt.ToString("MMM d");
                    }
                    return post.indexedAt.ToString("'yy MMM d");
                }
            }
            public string PostReplyTo
            {
                get
                {
                    var replyParentPostView = reply?.parent as PostView;
                    if (replyParentPostView == null)
                    {
                        return string.Empty;
                    }
                    return $"Reply to {replyParentPostView.author.displayName}";
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
                    var typedPost = post?.record as Post;
                    if (typedPost != null)
                    {
                        return typedPost.text;
                    }
                    return "[UNKNOWN]";
                }
            }
            public string PostImage
            {
                get
                {
                    var imagesView = post?.embed as Embed.Images.View;
                    if (imagesView != null)
                    {
                        if (imagesView.images == null || imagesView.images.Count < 1)
                        {
                            return null;
                        }
                        return imagesView.images[0].thumb;
                    }
                    return null;
                }
            }
        }

        public class ReplyRef
        {
            public object root; // union
            public object parent; // union
        }

        public class ReasonRepost
        {
            public Actor.Defs.ProfileViewBasic by;
            public DateTime indexedAt;
        }

        public class NotFoundPost
        {
            public string uri;
            public bool notFound;
        }

        public class BlockedPost
        {
            public string uri;
            public bool blocked;
            public BlockedAuthor author;
        }

        public class BlockedAuthor
        {
            public string did;
            public Actor.Defs.ViewerState viewer;
        }

        public class ThreadgateView
        {
            public string uri;
            public string cid;
            public object record;
            public List<object> lists;
        }
    }
}
