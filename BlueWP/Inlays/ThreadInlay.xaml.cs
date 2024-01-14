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
    public ATProto.IPost PostData { get; set; }

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetPostThreadResponse>(new ATProto.Lexicons.App.BSky.Feed.GetPostThread()
      {
        uri = PostURI
      });
      var thread = response.thread as ATProto.Lexicons.App.BSky.Feed.Defs.ThreadViewPost;

      PostData = thread.post;
      OnPropertyChanged(nameof(PostData));

      _mainPage?.EndLoading();
    }

    public void Flush()
    {
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
