using System;
using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  public class Threadgate
  {
    public string post = string.Empty;
    public List<object> allow = null;
    public DateTime createdAt = DateTime.Now;

    public class MentionRule
    {
    }

    public class FollowingRule
    {
    }

    public class ListRule
    {
      public string list;
    }
  }
}
