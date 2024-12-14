using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls.PostList
{
  public partial class PostListBase : UserControl, INotifyPropertyChanged
  {
    private App _app;
    protected Pages.MainPage _mainPage;

    public PostListBase()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += FeedInlay_Loaded;
      DataContext = this;
    }

    private void FeedInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public async virtual Task<List<ATProto.IPost>> GetListItems()
    {
      return null;
    }

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      var feedItems = await GetListItems();
      FeedItems = feedItems == null ? new ObservableCollection<ATProto.IPost>() : new ObservableCollection<ATProto.IPost>(feedItems);

      _mainPage?.EndLoading();

      OnPropertyChanged(nameof(FeedItems));
    }

    public void Flush()
    {
      FeedItems?.Clear();
      OnPropertyChanged(nameof(FeedItems));
    }

    public ObservableCollection<ATProto.IPost> FeedItems { get; set; }

    private async void Post_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      if (e.OriginalSource as Image != null)
      {
        return;
      }
      var post = sender as Controls.Post.PostBase;
      if (post != null)
      {
        await _mainPage.SwitchToThreadViewInlay(post.PostView.uri);
      }
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
      await Refresh();
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
  }
}
