using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class ProfileInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;
    public ProfileInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      Loaded += ProfileInlay_Loaded;
      DataContext = this;
    }

    public string ActorDID { get; set; }
    public string CoverImageURL { get; set; }
    public string AvatarURL { get; set; }
    public string DisplayName { get; set; }
    public string Handle { get; set; }

    private void ProfileInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public async Task Refresh()
    {
      _mainPage?.StartLoading();
      var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Actor.GetProfileResponse>(new ATProto.Lexicons.App.BSky.Actor.GetProfile()
      {
        actor = ActorDID
      });
      if (response != null)
      {
        CoverImageURL = response.banner;
        AvatarURL = response.avatar;
        DisplayName = response.displayName;
        Handle = $"@{response.handle}";
        OnPropertyChanged(nameof(CoverImageURL));
        OnPropertyChanged(nameof(AvatarURL));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(Handle));
      }
      
      if (feed != null)
      {
        feed.ActorDID = ActorDID;
        await feed.Refresh();
      }
      _mainPage?.EndLoading();
    }

    public void Flush()
    {
      CoverImageURL = null;
      AvatarURL = null;
      DisplayName = string.Empty;
      Handle = string.Empty;
      OnPropertyChanged(nameof(CoverImageURL));
      OnPropertyChanged(nameof(AvatarURL));
      OnPropertyChanged(nameof(DisplayName));
      OnPropertyChanged(nameof(Handle));

      feed.Flush();
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
