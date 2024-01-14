using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace BlueWP.Controls.Post
{
  public partial class Post : PostBase
  {
    public Post() : base()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    private void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      var postData = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      _mainPage.SwitchToProfileInlay(postData?.post?.author?.did);
    }

    private void ViewThread_Click(object sender, RoutedEventArgs e)
    {
      var feedViewPost = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      if (feedViewPost != null)
      {
        _mainPage.SwitchToThreadViewInlay(feedViewPost?.post?.uri);
        return;
      }
      var postView = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
      if (postView != null)
      {
        _mainPage.SwitchToThreadViewInlay(postView?.uri);
        return;
      }
    }

    private void Reply_Click(object sender, RoutedEventArgs e)
    {
      var postData = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      _mainPage.Reply(postData?.post);
    }

    private void RepostMenu_Click(object sender, RoutedEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element != null)
      {
        FlyoutBase.ShowAttachedFlyout(element);
      }
    }

    private void Repost_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Quote_Click(object sender, RoutedEventArgs e)
    {
      var postData = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      _mainPage.Quote(postData?.post);
    }
  }
}
