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
      public object viewer;
      public object labels;

      public string DisplayName
      {
        get
        {
          return displayName ?? handle ?? "[ERROR]";
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
      public DateTime indexedAt;
      public object viewer;
      public object labels;

      public string DisplayName
      {
        get
        {
          return displayName ?? handle ?? "[ERROR]";
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
          return displayName ?? handle ?? "[ERROR]";
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

    public class SavedFeedsPref
    {
      public List<string> pinned;
      public List<string> saved;
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
