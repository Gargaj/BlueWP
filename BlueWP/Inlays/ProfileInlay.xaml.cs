using System;
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

    public ATProto.Lexicons.App.BSky.Actor.Defs.ProfileViewDetailed ProfileData;

    public string ActorDID { get; set; }
    public string CoverImageURL => ProfileData?.banner;
    public string AvatarURL => ProfileData?.AvatarURL;
    public string DisplayName => ProfileData?.DisplayName;
    public string Handle => ProfileData?.Handle;
    public string Description => ProfileData?.Description;
    public uint? FollowerCount => ProfileData?.followersCount;
    public uint? FollowCount => ProfileData?.followsCount;

    public bool IsFollowing => !string.IsNullOrEmpty(ProfileData?.viewer?.following);
    public bool IsBeingFollowedBy => !string.IsNullOrEmpty(ProfileData?.viewer?.followedBy);
    public string FollowButtonText => IsFollowing ? "Unfollow" : (IsBeingFollowedBy ? "Follow Back" : "Follow");

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
        ProfileData = response;

        OnPropertyChanged(nameof(CoverImageURL));
        OnPropertyChanged(nameof(AvatarURL));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(Handle));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(FollowerCount));
        OnPropertyChanged(nameof(FollowCount));
        OnPropertyChanged(nameof(FollowButtonText));
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
      ProfileData = null;

      OnPropertyChanged(nameof(CoverImageURL));
      OnPropertyChanged(nameof(AvatarURL));
      OnPropertyChanged(nameof(DisplayName));
      OnPropertyChanged(nameof(Handle));
      OnPropertyChanged(nameof(Description));
      OnPropertyChanged(nameof(FollowerCount));
      OnPropertyChanged(nameof(FollowCount));
      OnPropertyChanged(nameof(FollowButtonText));

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

    private async void Follow_Click(object sender, RoutedEventArgs e)
    {
      if (IsFollowing)
      {
        var followURL = ProfileData?.viewer?.following;
        var repo = string.Empty;
        var collection = string.Empty;
        var rkey = string.Empty;
        if (ATProto.Helpers.ParseATURI(followURL, ref repo, ref collection, ref rkey))
        {
          var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord.Response>(new ATProto.Lexicons.COM.ATProto.Repo.DeleteRecord()
          {
            repo = repo,
            collection = collection,
            rkey = rkey,
          });
        }
        if (ProfileData?.viewer != null)
        {
          ProfileData.viewer.following = null;
        }
      }
      else
      {
        var response = await _mainPage.Post<ATProto.Lexicons.COM.ATProto.Repo.CreateRecord.Response>(new ATProto.Lexicons.COM.ATProto.Repo.CreateRecord()
        {
          repo = _app.Client.DID,
          collection = "app.bsky.graph.follow",
          record = new ATProto.Lexicons.App.BSky.Graph.Follow()
          {
            createdAt = DateTime.Now,
            subject = ActorDID
          }
        });
        if (ProfileData?.viewer != null)
        {
          ProfileData.viewer.following = response.uri;
        }
      }
      OnPropertyChanged(nameof(FollowButtonText));
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
