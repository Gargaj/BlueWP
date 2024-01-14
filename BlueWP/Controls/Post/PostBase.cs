using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BlueWP.Controls.Post
{
  public class PostBase : UserControl, INotifyPropertyChanged
  {
    protected App _app;
    protected Pages.MainPage _mainPage;
    public PostBase()
    {
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += PostBase_Loaded;
    }

    protected void PostBase_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public bool IsRepost
    {
      get { return PostData?.IsRepost ?? false; }
    }

    public bool IsReply
    {
      get { return PostData?.IsReply ?? false; }
    }

    public bool HasQuotedPost
    {
      get { return PostData?.HasQuotedPost ?? false; }
    }

    public bool HasEmbedExternal
    {
      get { return PostData?.HasEmbedExternal ?? false; }
    }

    public ATProto.IPost PostData
    {
      get { return GetValue(PostDataProperty) as ATProto.IPost; }
      set { SetValue(PostDataProperty, value); }
    }
    public static readonly DependencyProperty PostDataProperty = DependencyProperty.Register("PostData", typeof(ATProto.IPost), typeof(PostBase), new PropertyMetadata(null, OnPostDataChanged));

    protected static void OnPostDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var post = d as PostBase;
      if (post != null)
      {
        post.OnPropertyChanged(nameof(IsRepost));
        post.OnPropertyChanged(nameof(IsReply));
        post.OnPropertyChanged(nameof(HasQuotedPost));
        post.OnPropertyChanged(nameof(HasEmbedExternal));
      }
    }

    protected void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      var postData = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      _mainPage.SwitchToProfileInlay(postData?.post?.author?.did);
    }

    protected void ViewThread_Click(object sender, RoutedEventArgs e)
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

    protected void Reply_Click(object sender, RoutedEventArgs e)
    {
      var postData = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      _mainPage.Reply(postData?.post);
    }

    protected void RepostMenu_Click(object sender, RoutedEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element != null)
      {
        FlyoutBase.ShowAttachedFlyout(element);
      }
    }

    protected void Repost_Click(object sender, RoutedEventArgs e)
    {
      // TODO
    }

    protected void Like_Click(object sender, RoutedEventArgs e)
    {
      // TODO
    }

    protected void Quote_Click(object sender, RoutedEventArgs e)
    {
      var feedViewPost = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost;
      if (feedViewPost != null)
      {
        _mainPage.Quote(feedViewPost?.post);
        return;
      }

      var postView = PostData as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
      if (postView != null)
      {
        _mainPage.Quote(postView);
        return;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
