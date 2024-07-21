using System;
using System.Collections.Generic;

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
      public ProfileAssociated associated;
      public object viewer;
      public object labels;

      public string DisplayName
      {
        get
        {
          if (!string.IsNullOrEmpty(displayName))
          {
            return displayName;
          }
          if (!string.IsNullOrEmpty(handle))
          {
            return handle;
          }
          return "[ERROR]";
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

    public class ProfileView
    {
      public string did;
      public string handle;
      public string displayName;
      public string description;
      public string avatar;
      public ProfileAssociated associated;
      public DateTime indexedAt;
      public object viewer;
      public object labels;

      public string DisplayName
      {
        get
        {
          if (!string.IsNullOrEmpty(displayName))
          {
            return displayName;
          }
          if (!string.IsNullOrEmpty(handle))
          {
            return handle;
          }
          return "[ERROR]";
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

    public class ProfileViewDetailed
    {
      public string did;
      public string handle;
      public string displayName;
      public string description;
      public string avatar;
      public ProfileAssociated associated;
      public string banner;
      public uint followersCount;
      public uint followsCount;
      public uint postsCount;
      public DateTime indexedAt;
      public ViewerState viewer;
      public List<COM.ATProto.Label.Defs.Label> labels;

      public string DisplayName
      {
        get
        {
          if (!string.IsNullOrEmpty(displayName))
          {
            return displayName;
          }
          if (!string.IsNullOrEmpty(handle))
          {
            return handle;
          }
          return "[ERROR]";
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

    public class ProfileAssociated
    {
      public int lists;
      public int feedgens;
      public int startPacks;
      public bool labeler;
      public ProfileAssociatedChat chat;
    }

    public class ProfileAssociatedChat
    {
      public string allowIncoming; // ["all", "none", "following"]
    }

    public class ViewerState
    {
      public bool muted;
      public object mutedByList;
      public bool blockedBy;
      public string blocking;
      public object blockingByList;
      public string following;
      public string followedBy;
    }

    public class AdultContentPref
    {
      public bool enabled;
    }

    public class ContentLabelPref
    {
      public string label;
      public string visibility;
    }

    public class SavedFeed
    {
      public string id;
      public string type;
      public string value;
      public bool pinned;
    }

    public class SavedFeedsPref
    {
      public List<string> pinned;
      public List<string> saved;
      public int timelineIndex;
    }

    public class SavedFeedsPrefV2
    {
      public List<Defs.SavedFeed> items;
    }

    public class PersonalDetailsPref
    {
      public DateTime? birthDate;
    }

    public class FeedViewPref
    {
      public string feed;
      public bool hideReplies;
      public bool hideRepliesByUnfollowed;
      public bool hideRepliesByLikeCount;
      public bool hideReposts;
      public bool hideQuotePosts;
    }

    public class ThreadViewPref
    {
      public string sort;
      public bool prioritizeFollowedUsers;
    }
  }
}
