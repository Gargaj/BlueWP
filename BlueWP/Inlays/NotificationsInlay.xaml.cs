using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class NotificationsInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;

    public NotificationsInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      Loaded += NotificationsInlay_Loaded;
      DataContext = this;
    }

    private void NotificationsInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public List<NotificationGroup> NotificationGroups { get; set; }
    public Dictionary<string,ATProto.Lexicons.App.BSky.Feed.Defs.PostView> PostCache { get; set; } = new Dictionary<string,ATProto.Lexicons.App.BSky.Feed.Defs.PostView>();

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      try
      {
        var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Notification.ListNotificationsResponse>(new ATProto.Lexicons.App.BSky.Notification.ListNotifications()
        {
          limit = 60
        });
        NotificationGroups = response?.notifications.GroupBy(
          s => {
            if (s.reason == "like" || s.reason == "repost" || s.reason == "follow")
            {
              return $"{s.reason}|{s.reasonSubject}"; // only group these three types
            }
            return $"{s.reason}|{s.cid}";
          },
          s => s,
          (k, v) => new NotificationGroup(this, k, v)
        ).ToList();

        var subjectUris = response?.notifications.Select(s => s.reasonSubject).Where(s => !string.IsNullOrEmpty(s)).Distinct().Take(25);
        var responsePosts = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetPostsResponse>(new ATProto.Lexicons.App.BSky.Feed.GetPosts()
        {
          uris = subjectUris.ToList()
        });
        responsePosts?.posts.ForEach(s => { if (!PostCache.ContainsKey(s.uri)) { PostCache.Add(s.uri, s); } });
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        _mainPage?.TriggerError($"HTTP ERROR {(int)webResponse.StatusCode}\n\n{ex.Message}");
      }

      _mainPage?.EndLoading();

      OnPropertyChanged(nameof(NotificationGroups));

      if (_mainPage.UnreadNotificationCount > 0)
      {
        // Reset notification counter
        await _app.Client.PostAsync<ATProto.Lexicons.App.BSky.Notification.UpdateSeen>(new ATProto.Lexicons.App.BSky.Notification.UpdateSeen()
        {
          seenAt = System.DateTime.Now
        });
        _mainPage.UnreadNotificationCount = 0;
      }
    }

    private async void Post_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      var post = sender as Controls.Post.PostBase;
      if (post != null)
      {
        await _mainPage.SwitchToThreadViewInlay(post.PostData.PostURI);
      }
    }

    private async void LikedPost_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      var post = button.DataContext as NotificationGroup;
      if (post?.FirstPost != null)
      {
        await _mainPage.SwitchToThreadViewInlay(post?.FirstPost.reasonSubject);
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

    public class NotificationGroup
    {
      NotificationsInlay _parent;
      public NotificationGroup(NotificationsInlay parent, string type, IEnumerable<ATProto.Lexicons.App.BSky.Notification.Notification> notifications)
      {
        _parent = parent;
        var s = type.Split('|');
        Type = s[0];
        UID = s[1];
        Notifications = notifications;
      }
      public string Type { get; set; }
      public string UID { get; set; }
      public IEnumerable<ATProto.Lexicons.App.BSky.Notification.Notification> Notifications { get; set; }
      public IEnumerable<string> Avatars { get { return Notifications.Select(s => s.PostAuthorAvatarURL); } }
      public string Verb
      {
        get
        {
          switch (Type)
          {
            case "like": return "liked your post";
            case "repost": return "reposted your post";
            case "follow": return "followed you";
            case "mention": return "mentioned you";
            case "reply": return "replied to your post";
            case "quote": return "quoted your post";
            default: return null;
          }
        }
      }
      public string Icon
      {
        get
        {
          switch (Type)
          {
            case "like": return "\xEB51";
            case "repost": return "\xE8EB";
            case "follow": return "\xE8FA";
            default: return null;
          }
        }
      }
      public string FirstName => Notifications.First().PostAuthorDisplayName;
      public string PostElapsedTime => Notifications.First().PostElapsedTime;
      public string AdditionalNames => Notifications.Count() > 1 ? $"and {Notifications.Count() - 1} others" : string.Empty;
      public string SubjectPostText => FirstPost.reasonSubject != null && _parent.PostCache.ContainsKey(FirstPost.reasonSubject) ? _parent?.PostCache[FirstPost.reasonSubject]?.PostText : null;
      public ATProto.Lexicons.App.BSky.Notification.Notification FirstPost => Notifications.First();
    }
  }
}
