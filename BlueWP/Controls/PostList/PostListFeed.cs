using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BlueWP.Controls.PostList
{
  public class PostListFeed : PostListBase
  {
    private bool _followedOnly = false;

    public bool FollowedOnly { get => _followedOnly; set => _followedOnly = value; }

    public string FeedURI
    {
      get { return (string)GetValue(FeedURIProperty); }
      set { SetValue(FeedURIProperty, value); }
    }
    public static readonly DependencyProperty FeedURIProperty = DependencyProperty.Register("FeedURI", typeof(string), typeof(PostListFeed), new PropertyMetadata(string.Empty));

    public async override Task<List<ATProto.IPost>> GetListItems()
    {
      if (string.IsNullOrEmpty(FeedURI))
      {
        var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.GetTimeline.Response>(new ATProto.Lexicons.App.BSky.Feed.GetTimeline()
        {
          limit = 60
        });

        var feedItems = response?.feed;
        if (_followedOnly)
        {
          feedItems = feedItems.Where((s) =>
          {
            if (s?.reply?.parent == null)
            {
              return true;
            }
            var post = s.reply.parent as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
            if (post == null)
            {
              return true;
            }
            return !string.IsNullOrEmpty(post.author.viewer.following);
          }).ToList();
        }

        return feedItems.ToList<ATProto.IPost>();
      }
      else
      {
        var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.GetFeed.Response>(new ATProto.Lexicons.App.BSky.Feed.GetFeed()
        {
          limit = 60,
          feed = FeedURI
        });
        return response?.feed.ToList<ATProto.IPost>();
      }
    }
  }
}
