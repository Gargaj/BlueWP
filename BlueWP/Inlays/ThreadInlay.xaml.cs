using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class ThreadInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;
    public ThreadInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      Loaded += ThreadInlay_Loaded;
      DataContext = this;
    }

    private void ThreadInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public string PostURI { get; set; }
    public ObservableCollection<ATProto.Lexicons.App.BSky.Feed.Defs.PostView> Posts { get; set; }

    public async Task Refresh()
    {
      if (string.IsNullOrEmpty(PostURI))
      {
        return;
      }

      var selector = Resources["selector"] as Controls.ThreadPostTemplateSelector;
      selector.SelectedPostURI = PostURI;

      _mainPage?.StartLoading();

      var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetPostThreadResponse>(new ATProto.Lexicons.App.BSky.Feed.GetPostThread()
      {
        uri = PostURI
      });
      var thread = response.thread as ATProto.Lexicons.App.BSky.Feed.Defs.ThreadViewPost;

      // main post
      Posts = new ObservableCollection<ATProto.Lexicons.App.BSky.Feed.Defs.PostView>();
      Posts.Add(thread.post);

      // previous posts
      var post = thread.parent as ATProto.Lexicons.App.BSky.Feed.Defs.ThreadViewPost;
      while (post != null)
      {
        Posts.Insert(0, post.post);
        post = post?.parent as ATProto.Lexicons.App.BSky.Feed.Defs.ThreadViewPost;
      }

      // replies
      foreach (var reply in thread.replies)
      {
        var replyPost = reply as ATProto.Lexicons.App.BSky.Feed.Defs.ThreadViewPost;
        Posts.Add(replyPost?.post);
      }

      OnPropertyChanged(nameof(Posts));

      _mainPage?.EndLoading();

      listView.ScrollIntoView(thread.post);
    }

    public void Flush()
    {
      Posts?.Clear();
      OnPropertyChanged(nameof(Posts));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    public virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Post_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      var post = sender as Controls.Post.PostBase;
      if (post != null)
      {
        _mainPage.SwitchToThreadViewInlay(post.PostView.uri);
      }
    }
  }
}
