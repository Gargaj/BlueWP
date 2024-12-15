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
    public string Description { get; set; }
    public uint FollowerCount { get; set; }
    public uint FollowCount { get; set; }

    private void ProfileInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public async Task Refresh()
    {
      if (string.IsNullOrEmpty(ActorDID))
      {
        return;
      }

      _mainPage?.StartLoading();
      var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Actor.GetProfile.Response>(new ATProto.Lexicons.App.BSky.Actor.GetProfile()
      {
        actor = ActorDID
      });
      if (response != null)
      {
        CoverImageURL = response.banner;
        AvatarURL = response.avatar;
        DisplayName = response.displayName;
        Handle = $"@{response.handle}";
        Description = response.description;
        FollowerCount = response.followersCount;
        FollowCount = response.followsCount;
        OnPropertyChanged(nameof(CoverImageURL));
        OnPropertyChanged(nameof(AvatarURL));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(Handle));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(FollowerCount));
        OnPropertyChanged(nameof(FollowCount));
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
      Description = string.Empty;
      FollowerCount = 0;
      FollowCount = 0;
      OnPropertyChanged(nameof(CoverImageURL));
      OnPropertyChanged(nameof(AvatarURL));
      OnPropertyChanged(nameof(DisplayName));
      OnPropertyChanged(nameof(Handle));
      OnPropertyChanged(nameof(Description));
      OnPropertyChanged(nameof(FollowerCount));
      OnPropertyChanged(nameof(FollowCount));

      feed.Flush();
    }

    private async void Followers_Click(object sender, RoutedEventArgs e)
    {
      await _mainPage.SwitchToFollowersInlay(ActorDID);
    }

    private async void Following_Click(object sender, RoutedEventArgs e)
    {
      await _mainPage.SwitchToFollowingInlay(ActorDID);
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
