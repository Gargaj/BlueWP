using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public partial class Post : UserControl, INotifyPropertyChanged
  {
    public Post()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public bool IsRepost
    {
      get { return !string.IsNullOrEmpty(PostData?.PostReason); }
    }

    public bool IsReply
    {
      get { return !string.IsNullOrEmpty(PostData?.PostReplyTo); }
    }

    public ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost PostData
    {
      get { return GetValue(PostDataProperty) as ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost; }
      set { SetValue(PostDataProperty, value); }
    }
    internal static readonly DependencyProperty PostDataProperty = DependencyProperty.Register("PostData", typeof(ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost), typeof(Post), new PropertyMetadata(null, OnPostDataChanged));

    private static void OnPostDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var post = d as Post;
      if (post != null)
      {
        post.OnPropertyChanged(nameof(IsRepost));
        post.OnPropertyChanged(nameof(IsReply));
      }
    }

    private async void OpenExternalURL_Click(object sender, RoutedEventArgs e)
    {
      var button = e.OriginalSource as Button;
      if (button == null)
      {
        return;
      }
      var dataContext = button.DataContext as Post;
      if (dataContext == null)
      {
        return;
      }
      if (!string.IsNullOrEmpty(dataContext.PostData.PostEmbedExternalURL))
      {
        await Windows.System.Launcher.LaunchUriAsync(new Uri(dataContext.PostData.PostEmbedExternalURL));
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
