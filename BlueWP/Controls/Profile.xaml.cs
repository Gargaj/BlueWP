using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public partial class Profile : UserControl, INotifyPropertyChanged
  {
    private App _app;
    protected Pages.MainPage _mainPage;

    public Profile()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += ProfileList_Loaded;
      LayoutRoot.DataContext = this;
    }

    private void ProfileList_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public string ActorDID => ProfileData?.DID;
    public string DisplayName => ProfileData?.DisplayName;
    public string Handle => ProfileData?.Handle;
    public string AvatarURL => ProfileData?.AvatarURL;
    public string Description => ProfileData?.Description;

    public bool IsFollowing => !string.IsNullOrEmpty(ProfileData?.viewer?.following);
    public bool IsBeingFollowedBy => !string.IsNullOrEmpty(ProfileData?.viewer?.followedBy);
    public string FollowButtonText => IsFollowing ? "Unfollow" : (IsBeingFollowedBy ? "Follow Back" : "Follow");

    private void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      _mainPage.SwitchToProfileInlay(ActorDID);
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

    public ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView ProfileData
    {
      get { return GetValue(ProfileDataProperty) as ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView; }
      set { SetValue(ProfileDataProperty, value); }
    }
    public static readonly DependencyProperty ProfileDataProperty = DependencyProperty.Register("ProfileData", typeof(ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView), typeof(Profile), new PropertyMetadata(null, OnPostDataChanged));

    private static void OnPostDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var profile = d as Profile;
      if (profile != null)
      {
        profile.OnPropertyChanged(nameof(DisplayName));
        profile.OnPropertyChanged(nameof(Handle));
        profile.OnPropertyChanged(nameof(AvatarURL));
        profile.OnPropertyChanged(nameof(Description));
        profile.OnPropertyChanged(nameof(FollowButtonText));
      }
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
