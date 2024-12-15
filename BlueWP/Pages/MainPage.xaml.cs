﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Pages
{
  public partial class MainPage : Page, INotifyPropertyChanged
  {
    private App _app;
    private uint _isLoading = 0;
    private bool _hasError = false;
    private string _errorText = string.Empty;
    private int _unreadCount = 0;
    private int _unreadConvoCount = 0;
    private List<Feed> _feeds = null;
    private List<object> _preferences;
    private string _profileActorDID;
    private string _threadPostURI;
    private string _convoID;
    private ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPrefV2 _savedFeedsPrefV2;
    private DispatcherTimer _notificationsTimer = new DispatcherTimer();
    private string _zoomedImageURL;
    private string _followersActorDID;
    private string _followingActorDID;

    public MainPage()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      DataContext = this;

      _notificationsTimer.Tick += RefreshNotifications_Timer;
      _notificationsTimer.Interval = TimeSpan.FromSeconds(60);
    }

    public void StartLoading() { _isLoading++; OnPropertyChanged(nameof(IsLoading)); }
    public void EndLoading() { _isLoading--; OnPropertyChanged(nameof(IsLoading)); }
    public bool IsLoading { get { return _isLoading > 0; } }

    public bool HasError { get { return _hasError; } set { _hasError = value; OnPropertyChanged(nameof(HasError)); } }
    public string ErrorText { get { return _errorText; } set { _errorText = value; OnPropertyChanged(nameof(ErrorText)); } }
    public int UnreadNotificationCount { get { return _unreadCount; } set { _unreadCount = value; OnPropertyChanged(nameof(UnreadNotificationCount)); OnPropertyChanged(nameof(UnreadCountVisibility)); } }
    public Visibility UnreadCountVisibility { get { return _unreadCount > 0 ? Visibility.Visible : Visibility.Collapsed; } }
    public int UnreadConvoNotificationCount { get { return _unreadConvoCount; } set { _unreadConvoCount = value; OnPropertyChanged(nameof(UnreadConvoNotificationCount)); OnPropertyChanged(nameof(UnreadConvoNotificationCountVisibility)); } }
    public Visibility UnreadConvoNotificationCountVisibility { get { return _unreadConvoCount > 0 ? Visibility.Visible : Visibility.Collapsed; } }
    public List<Feed> Feeds { get { return _feeds; } }

    public string ZoomedImageURL
    {
      get => _zoomedImageURL;
      set
      {
        _zoomedImageURL = value;
        OnPropertyChanged(nameof(ZoomedImageURL));
        OnPropertyChanged(nameof(IsZoomedImageValid));
      }
    }
    public bool IsZoomedImageValid => !string.IsNullOrEmpty(ZoomedImageURL);

    protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
    {
      StartLoading();

      await LoadPreferences();

      EndLoading();

      _notificationsTimer.Start();
      await RefreshNotificationCounters(); // Start() doesn't trigger immediately

      if (e.Parameter != null && e.Parameter is string)
      {
        await SwitchToThreadViewInlayFromHTTPURL(e.Parameter as string);
      }
    }

    protected async Task LoadPreferences()
    {
      var preferences = await Get<ATProto.Lexicons.App.BSky.Actor.GetPreferences.Response>(new ATProto.Lexicons.App.BSky.Actor.GetPreferences());
      if (preferences?.preferences != null)
      {
        _preferences = preferences.preferences;

        await GenerateFeeds();
      }
    }

    protected async Task GenerateFeeds()
    {
      _savedFeedsPrefV2 = _preferences?.FirstOrDefault(s => s is ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPrefV2) as ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPrefV2;

      var getFeedReq = new ATProto.Lexicons.App.BSky.Feed.GetFeedGenerators();
      getFeedReq.feeds = new List<string>();

      _feeds = new List<Feed>();
      foreach (var feedInfo in _savedFeedsPrefV2.items.Where(s => s.pinned))
      {
        switch (feedInfo.type)
        {
          case "timeline":
            {
              _feeds.Add(new Feed() { Name = feedInfo.value.Substring(0, 1).ToUpper() + feedInfo.value.Substring(1) });
            }
            break;
          case "feed":
            {
              var feedURI = feedInfo.value;
              var name = feedURI.Contains("/") ? feedURI.Substring(feedURI.LastIndexOf("/") + 1) : feedURI; // temp name
              getFeedReq.feeds.Add(feedURI);

              _feeds.Add(new Feed() { Name = name, URI = feedURI });
            }
            break;
        }
      }
      OnPropertyChanged(nameof(Feeds));

      var feedInfoResponse = await Get<ATProto.Lexicons.App.BSky.Feed.GetFeedGenerators.Response>(getFeedReq);
      if (feedInfoResponse?.feeds != null)
      {
        foreach (var feedInfo in feedInfoResponse.feeds)
        {
          var feed = _feeds.FirstOrDefault(s => s.URI == feedInfo.uri);
          if (feed == null)
          {
            continue;
          }
          feed.FeedInfo = feedInfo;
          feed.Name = feedInfo.displayName;
          feed.OnPropertyChanged("Name");
        }
      }
    }

    protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
    {
      _notificationsTimer.Stop();
    }

    private async void RefreshNotifications_Timer(object sender, object o)
    {
      await RefreshNotificationCounters();
    }

    public async Task RefreshNotificationCounters()
    {
      var unreadCountResponse = await Get<ATProto.Lexicons.App.BSky.Notification.GetUnreadCount.Response>(new ATProto.Lexicons.App.BSky.Notification.GetUnreadCount());
      if (unreadCountResponse != null)
      {
        UnreadNotificationCount = (int)unreadCountResponse.count;
      }

      var convoUnreadCountResponse = await Get<ATProto.Lexicons.Chat.BSky.Convo.ListConvos.Response>(new ATProto.Lexicons.Chat.BSky.Convo.ListConvos() {
        limit = 1
      });
      if (convoUnreadCountResponse != null)
      {
        UnreadConvoNotificationCount = convoUnreadCountResponse.convos.Sum(s => s.unreadCount);
      }
    }

    private async void Main_PivotItemLoading(Pivot sender, PivotItemEventArgs args)
    {
      var notificationsInlay = args.Item.ContentTemplateRoot as Inlays.NotificationsInlay;
      if (notificationsInlay != null)
      {
        await notificationsInlay.Refresh();
      }

      var profileInlay = args.Item.ContentTemplateRoot as Inlays.ProfileInlay;
      if (profileInlay != null)
      {
        profileInlay.ActorDID = _profileActorDID;
        await profileInlay.Refresh();
      }

      var threadInlay = args.Item.ContentTemplateRoot as Inlays.ThreadInlay;
      if (threadInlay != null)
      {
        threadInlay.PostURI = _threadPostURI;
        await threadInlay.Refresh();
      }

      var convoListInlay = args.Item.ContentTemplateRoot as Inlays.ConvoListInlay;
      if (convoListInlay != null)
      {
        await convoListInlay.Refresh();
      }

      var convoInlay = args.Item.ContentTemplateRoot as Inlays.ConvoInlay;
      if (convoInlay != null)
      {
        convoInlay.ID = _convoID;
        await convoInlay.Refresh();
      }

      var followersInlay = args.Item.ContentTemplateRoot as Controls.ProfileList.ProfileListFollowers;
      if (followersInlay != null)
      {
        followersInlay.ActorDID = _followersActorDID;
        await followersInlay.Refresh();
      }

      var followingInlay = args.Item.ContentTemplateRoot as Controls.ProfileList.ProfileListFollowing;
      if (followingInlay != null)
      {
        followingInlay.ActorDID = _followingActorDID;
        await followingInlay.Refresh();
      }
    }

    private void Main_PivotItemUnloading(Pivot sender, PivotItemEventArgs args)
    {
      var profileInlay = args.Item.ContentTemplateRoot as Inlays.ProfileInlay;
      if (profileInlay != null)
      {
        profileInlay.Flush();
      }
      var threadInlay = args.Item.ContentTemplateRoot as Inlays.ThreadInlay;
      if (threadInlay != null)
      {
        threadInlay.Flush();
      }
      var convoListInlay = args.Item.ContentTemplateRoot as Inlays.ConvoListInlay;
      if (convoListInlay != null)
      {
        convoListInlay.Flush();
      }
      var convoInlay = args.Item.ContentTemplateRoot as Inlays.ConvoInlay;
      if (convoInlay != null)
      {
        convoInlay.Flush();
      }
    }

    private async void Home_PivotItemLoading(Pivot sender, PivotItemEventArgs args)
    {
      var feed = args.Item.DataContext as Feed;
      if (feed == null)
      {
        return;
      }
      var feedInlay = args.Item.ContentTemplateRoot as Controls.PostList.PostListFeed;
      if (feedInlay == null)
      {
        return;
      }
      feedInlay.FollowedOnly = true;
      feedInlay.FeedURI = feed.URI; // Preempt the binding cos that seems to happen later
      await feedInlay.Refresh();
    }

    public void TriggerError(string error)
    {
      HasError = true;
      ErrorText = error;
    }

    private void CloseErrorPopup_Click(object sender, RoutedEventArgs e)
    {
      HasError = false;
      ErrorText = string.Empty;
    }

    public async void SwitchToProfileInlay(string actorDID)
    {
      if (string.IsNullOrEmpty(actorDID))
      {
        return;
      }
      _profileActorDID = actorDID;
      if (MainMenu.SelectedItem == ProfilePivotItem)
      {
        var profileInlay = ProfilePivotItem.ContentTemplateRoot as Inlays.ProfileInlay;
        if (profileInlay != null)
        {
          profileInlay.Flush();
          profileInlay.ActorDID = actorDID;
          await profileInlay.Refresh();
        }
      }
      else
      {
        MainMenu.SelectedItem = ProfilePivotItem;
      }
    }

    public async Task SwitchToThreadViewInlay(string postAtURI)
    {
      if (string.IsNullOrEmpty(postAtURI))
      {
        return;
      }
      _threadPostURI = postAtURI;
      if (MainMenu.SelectedItem == ThreadPivotItem)
      {
        var threadInlay = ThreadPivotItem.ContentTemplateRoot as Inlays.ThreadInlay;
        if (threadInlay != null)
        {
          threadInlay.Flush();
          threadInlay.PostURI = postAtURI;
          await threadInlay.Refresh();
        }
      }
      else
      {
        MainMenu.SelectedItem = ThreadPivotItem;
      }
    }

    public async Task SwitchToConvoInlay(string convoID)
    {
      if (string.IsNullOrEmpty(convoID))
      {
        return;
      }
      _convoID = convoID;
      if (MainMenu.SelectedItem == ConvoPivotItem)
      {
        var convoInlay = ConvoPivotItem.ContentTemplateRoot as Inlays.ConvoInlay;
        if (convoInlay != null)
        {
          convoInlay.Flush();
          convoInlay.ID = convoID;
          await convoInlay.Refresh();
        }
      }
      else
      {
        MainMenu.SelectedItem = ConvoPivotItem;
      }
    }

    public async Task SwitchToFollowersInlay(string actorDID)
    {
      if (string.IsNullOrEmpty(actorDID))
      {
        return;
      }
      _followersActorDID = actorDID;
      if (MainMenu.SelectedItem == FollowersPivotItem)
      {
        var followersInlay = FollowersPivotItem.ContentTemplateRoot as Controls.ProfileList.ProfileListFollowers;
        if (followersInlay != null)
        {
          followersInlay.Flush();
          followersInlay.ActorDID = actorDID;
          await followersInlay.Refresh();
        }
      }
      else
      {
        MainMenu.SelectedItem = FollowersPivotItem;
      }
    }

    public async Task SwitchToFollowingInlay(string actorDID)
    {
      if (string.IsNullOrEmpty(actorDID))
      {
        return;
      }
      _followingActorDID = actorDID;
      if (MainMenu.SelectedItem == FollowingPivotItem)
      {
        var followingInlay = FollowingPivotItem.ContentTemplateRoot as Controls.ProfileList.ProfileListFollowing;
        if (followingInlay != null)
        {
          followingInlay.Flush();
          followingInlay.ActorDID = actorDID;
          await followingInlay.Refresh();
        }
      }
      else
      {
        MainMenu.SelectedItem = FollowingPivotItem;
      }
    }

    public async Task SwitchToThreadViewInlayFromHTTPURL(string postHttpURI)
    {
      string atUri = await ATProto.Helpers.HTTPToATURI(_app.Client, postHttpURI);
      if (atUri != null)
      {
        await SwitchToThreadViewInlay(atUri);
      }
    }

    public void Reply(ATProto.Lexicons.App.BSky.Feed.Defs.PostView post)
    {
      var postInlay = PostInlay as Inlays.NewPostInlay;
      if (postInlay != null)
      {
        postInlay.IsReplying = true;
        postInlay.OnPropertyChanged(nameof(postInlay.IsReplying));
        postInlay.RepliedPost = post;
        postInlay.OnPropertyChanged(nameof(postInlay.RepliedPost));

        postInlay.IsQuoting = false;
        postInlay.OnPropertyChanged(nameof(postInlay.IsQuoting));
        postInlay.QuotedPost = null;
        postInlay.OnPropertyChanged(nameof(postInlay.QuotedPost));
      }
      MainMenu.SelectedItem = PostPivotItem;
    }

    public void Quote(ATProto.Lexicons.App.BSky.Feed.Defs.PostView post)
    {
      var postInlay = PostInlay as Inlays.NewPostInlay;
      if (postInlay != null)
      {
        postInlay.IsReplying = false;
        postInlay.OnPropertyChanged(nameof(postInlay.IsReplying));
        postInlay.RepliedPost = null;
        postInlay.OnPropertyChanged(nameof(postInlay.RepliedPost));

        postInlay.IsQuoting = true;
        postInlay.OnPropertyChanged(nameof(postInlay.IsQuoting));
        postInlay.QuotedPost = post;
        postInlay.OnPropertyChanged(nameof(postInlay.QuotedPost));
      }
      MainMenu.SelectedItem = PostPivotItem;
    }

    private void CloseZoomedImage_Click(object sender, RoutedEventArgs e)
    {
      ZoomedImageURL = null;
    }

    public async Task<T> Get<T>(ATProto.ILexiconRequest input) where T : class, ATProto.ILexiconResponse
    {
      try
      {
        return await _app.Client.GetAsync<T>(input);
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        if (webResponse != null)
        {
          TriggerError($"HTTP ERROR {(int)webResponse.StatusCode}\n\n{ex.Message}");
        }
        else
        {
          TriggerError($"ERROR\n\n{ex?.InnerException?.Message ?? ex?.Message}");
        }
      }
      catch (Exception ex)
      {
        TriggerError($"ERROR\n{ex?.InnerException?.Message ?? ex?.Message}");
      }
      return null;
    }

    public async Task<T> Post<T>(ATProto.ILexiconRequest input) where T : class, ATProto.ILexiconResponse
    {
      try
      {
        return await _app.Client.PostAsync<T>(input);
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        if (webResponse != null)
        {
          TriggerError($"HTTP ERROR {(int)webResponse.StatusCode}\n\n{ex.Message}");
        }
        else
        {
          TriggerError($"ERROR\n\n{ex?.InnerException?.Message ?? ex?.Message}");
        }
      }
      catch (Exception ex)
      {
        TriggerError($"ERROR\n{ex?.InnerException?.Message ?? ex?.Message}");
      }
      return null;
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

    public class Feed : INotifyPropertyChanged
    {
      public string Name { get; set; }
      public string URI { get; set; } = null;
      public ATProto.Lexicons.App.BSky.Feed.Defs.GeneratorView FeedInfo { get; set; } = null;

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
}
