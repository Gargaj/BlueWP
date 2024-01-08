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
      public List<COM.ATProto.Label.Defs.Label> labels;
      public ThreadgateView threadgate;

      public string AuthorDisplayName
      {
        get
        {
          return author?.DisplayName ?? "[ERROR]";
        }
      }
      public string AuthorHandle
      {
        get
        {
          return author?.Handle ?? "[ERROR]";
        }
      }
    }

    public class ViewerState
    {
      public string repost;
      public string like;
      public bool replyDisabled;
    }

    public class FeedViewPost : IPost
    {
      public PostView post;
      public ReplyRef reply;
      public ReasonRepost reason;

      public string PostAuthorAvatarURL => post?.author?.avatar ?? null;
      public string PostAuthorDisplayName => post?.AuthorDisplayName ?? "[ERROR]";
      public string PostAuthorHandle => post?.AuthorHandle ?? "[ERROR]";
      public string PostElapsedTime => Helpers.ToElapsedTime(post.indexedAt);

      public bool IsRepost => reason != null;
      public string PostReason => reason == null ? string.Empty : $"Reposted by {reason?.by?.DisplayName}";

      public bool IsReply => (reply?.parent as PostView) != null;
      public string PostReplyTo
      {
        get
        {
          var replyParentPostView = reply?.parent as PostView;
          if (replyParentPostView == null)
          {
            return string.Empty;
          }
          return $"Reply to {replyParentPostView.AuthorDisplayName}";
        }
      }

      public bool HasQuotedPost => (post?.embed as Embed.Record.View) != null || (post?.embed as Embed.RecordWithMedia.View) != null;
      public Embed.Record.ViewRecord QuotedPost
      {
        get
        {
          var recordView = post?.embed as Embed.Record.View;
          if (recordView != null)
          {
            return recordView.record as Embed.Record.ViewRecord;
          }
          var recordWithMediaView = post?.embed as Embed.RecordWithMedia.View;
          if (recordWithMediaView != null)
          {
            return recordWithMediaView.record.record as Embed.Record.ViewRecord;
          }
          return null;
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
      public IEnumerable<Embed.Images.ViewImage> PostImages
      {
        get
        {
          var imagesView = post?.embed as Embed.Images.View;
          if (imagesView != null)
          {
            return imagesView.images;
          }
          var rwmView = post?.embed as Embed.RecordWithMedia.View;
          if (rwmView != null)
          {
            imagesView = rwmView?.media as Embed.Images.View;
            if (imagesView != null)
            {
              return imagesView.images;
            }
          }
          return null;
        }
      }
      public bool HasEmbedExternal => (post?.embed as Embed.External.View) != null;
      public Embed.External.View PostEmbedExternal => post?.embed as Embed.External.View;
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

    public class GeneratorView
    {
      public string uri;
      public string cid;
      public string did;
      public Actor.Defs.ProfileView creator;
      public string displayName;
      public string description;
      public List<object> descriptionFacets; // TODO
      public string avatar;
      public uint likeCount;
      public GeneratorViewerState viewer;
      public DateTime indexedAt;
    }

    public class GeneratorViewerState
    {
      public string like;
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
