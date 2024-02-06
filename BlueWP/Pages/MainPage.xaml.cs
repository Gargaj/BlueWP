using System;
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
    private List<Feed> _feeds = null;
    private List<object> _preferences;
    private string _profileActorDID;
    private string _threadPostURI;
    private ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref _savedFeedsPref;
    private DispatcherTimer _notificationsTimer = new DispatcherTimer();
    private string _zoomedImageURL;

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

      try
      {
        var preferences = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Actor.GetPreferencesResponse>(new ATProto.Lexicons.App.BSky.Actor.GetPreferences());
        if (preferences != null)
        {
          _preferences = preferences.preferences;
          _savedFeedsPref = _preferences?.FirstOrDefault(s => s is ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref) as ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref;

          var req = new ATProto.Lexicons.App.BSky.Feed.GetFeedGenerators();
          req.feeds = new List<string>();

          _feeds = new List<Feed>();
          _feeds.Add(new Feed() { Name = "Following" });
          foreach (var feed in _savedFeedsPref.pinned)
          {
            var name = feed.Substring(feed.LastIndexOf("/") + 1); // temp
            _feeds.Add(new Feed() { Name = name, URI = feed });
            req.feeds.Add(feed);
          }
          OnPropertyChanged(nameof(Feeds));

          var feedInfoResponse = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetFeedGeneratorsResponse>(req);
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

      EndLoading();

      _notificationsTimer.Start();
      await RefreshNotificationCounter(); // Start() doesn't trigger immediately
    }

    protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
    {
      _notificationsTimer.Stop();
    }

    private async void RefreshNotifications_Timer(object sender, object o)
    {
      await RefreshNotificationCounter();
    }

    private async Task RefreshNotificationCounter()
    {
      try
      {
        var unreadCountResponse = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Notification.GetUnreadCountResponse>(new ATProto.Lexicons.App.BSky.Notification.GetUnreadCount());
        if (unreadCountResponse != null)
        {
          UnreadNotificationCount = (int)unreadCountResponse.count;
        }
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
    }

    private async void Home_PivotItemLoading(Pivot sender, PivotItemEventArgs args)
    {
      var feed = args.Item.DataContext as Feed;
      if (feed == null)
      {
        return;
      }
      var feedInlay = args.Item.ContentTemplateRoot as Inlays.FeedInlay;
      if (feedInlay == null)
      {
        return;
      }
      feedInlay.FeedURI = feed.URI; // Preempt the binding cos that seems to happen later
      await feedInlay.Refresh();
    }

    public void TriggerError(string error)
    {
      HasError = true;
      ErrorText = error;
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

    public async void SwitchToThreadViewInlay(string postURI)
    {
      if (string.IsNullOrEmpty(postURI))
      {
        return;
      }
      _threadPostURI = postURI;
      if (MainMenu.SelectedItem == ThreadPivotItem)
      {
        var threadInlay = ThreadPivotItem.ContentTemplateRoot as Inlays.ThreadInlay;
        if (threadInlay != null)
        {
          threadInlay.Flush();
          threadInlay.PostURI = postURI;
          await threadInlay.Refresh();
        }
      }
      else
      {
        MainMenu.SelectedItem = ThreadPivotItem;
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
      ZoomedImageURL = string.Empty;
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
