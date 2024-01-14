using System;
using System.Collections.Generic;
using BlueWP.ATProto.Lexicons.App.BSky.Embed;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/defs.json"/>
  public class Defs
  {
    public class PostView : IPost
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

      public bool IsRepost => false;
      public bool IsReply => false;
      public bool HasQuotedPost => QuotedPost != null;
      public bool HasEmbedExternal => PostEmbedExternal != null;

      public string PostAuthorAvatarURL => author?.avatar;
      public string PostAuthorDisplayName => author?.DisplayName ?? "[ERROR]";
      public string PostAuthorHandle => author?.Handle ?? "[ERROR]";
      public string PostElapsedTime => Helpers.ToElapsedTime(indexedAt);
      public string PostText => (record as Post)?.text ?? "[ERROR]";
      public string PostDateTime => indexedAt.ToString("MMM d, yyyy") + " at " + indexedAt.ToString("HH:mm");

      public uint ReplyCount => replyCount ?? 0;
      public uint RepostCount => repostCount ?? 0;
      public uint LikeCount => likeCount ?? 0;

      public bool PostReposted => !string.IsNullOrEmpty(viewer?.repost);
      public bool PostLiked => !string.IsNullOrEmpty(viewer?.like);

      public External.View PostEmbedExternal => embed as External.View;

      public Record.ViewRecord QuotedPost
      {
        get
        {
          var recordView = embed as Record.View;
          if (recordView != null)
          {
            return recordView.record as Record.ViewRecord;
          }
          var recordWithMediaView = embed as RecordWithMedia.View;
          if (recordWithMediaView != null)
          {
            return recordWithMediaView.record.record as Record.ViewRecord;
          }
          return null;
        }
      }

      public IEnumerable<Images.ViewImage> PostImages
      {
        get
        {
          var imagesView = embed as Images.View;
          if (imagesView != null)
          {
            return imagesView.images;
          }
          var rwmView = embed as RecordWithMedia.View;
          if (rwmView != null)
          {
            imagesView = rwmView?.media as Images.View;
            if (imagesView != null)
            {
              return imagesView.images;
            }
          }
          return null;
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

      public string PostAuthorAvatarURL => post?.PostAuthorAvatarURL;
      public string PostAuthorDisplayName => post?.PostAuthorDisplayName ?? "[ERROR]";
      public string PostAuthorHandle => post?.PostAuthorHandle ?? "[ERROR]";
      public string PostElapsedTime => post?.PostElapsedTime;
      public string PostDateTime => post?.PostDateTime;

      public bool IsRepost => reason != null;
      public string PostReason => reason == null ? string.Empty : $"Reposted by {reason?.by?.DisplayName}";

      public uint ReplyCount => post?.ReplyCount ?? 0;
      public uint RepostCount => post?.RepostCount ?? 0;
      public uint LikeCount => post?.LikeCount ?? 0;

      public bool PostReposted => post?.PostReposted ?? false;
      public bool PostLiked => post?.PostLiked ?? false;

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
          return $"Reply to {replyParentPostView.PostAuthorDisplayName}";
        }
      }

      public bool HasQuotedPost => post?.HasQuotedPost ?? false;
      public Record.ViewRecord QuotedPost => post?.QuotedPost;
      public string PostText => post.PostText;
      public IEnumerable<Images.ViewImage> PostImages => post.PostImages;

      public bool HasEmbedExternal => post?.HasEmbedExternal ?? false;
      public External.View PostEmbedExternal => post?.PostEmbedExternal;
    }

    public class ReplyRef
    {
      public object root; // union
      public object parent; // union
    }

    public class ThreadViewPost
    {
      public PostView post;
      public object parent;
      public List<object> replies;
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
