﻿using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Feed
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/feed/getFeedGenerator.json"/>
  public class GetFeedGenerator : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getFeedGenerator";

    public string feed;
  }
  public class GetFeedGeneratorResponse : LexiconBase
  {
    public override string EndpointID => "app.bsky.feed.getFeedGenerator";

    public Defs.GeneratorView view;
    public bool isOnline;
    public bool isValid;
  }
}
