﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Pages
{
  public partial class FeedPage : Page, INotifyPropertyChanged
  {
    private App _app;
    private bool _isLoading = false;
    private bool _hasError = false;
    private string _errorText = string.Empty;
    private int _unreadCount = 0;
    private List<Feed> _feeds = null;
    private List<object> _preferences;
    private ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref _savedFeedsPref;

    public FeedPage()
    {
      InitializeComponent();
      _app = (App)Windows.UI.Xaml.Application.Current;
      DataContext = this;
    }

    public bool IsLoading { get { return _isLoading; } set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }
    public bool HasError { get { return _hasError; } set { _hasError = value; OnPropertyChanged(nameof(HasError)); } }
    public string ErrorText { get { return _errorText; } set { _errorText = value; OnPropertyChanged(nameof(ErrorText)); } }
    public int UnreadNotificationCount { get { return _unreadCount; } }
    public List<Feed> Feeds { get { return _feeds; } }

    protected async Task RefreshFeed( string feedDID )
    {
      IsLoading = true;

      var unreadCountResponse = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Notification.GetUnreadCountResponse>(new ATProto.Lexicons.App.BSky.Notification.GetUnreadCount());
      if (unreadCountResponse != null)
      {
        _unreadCount = (int)unreadCountResponse.count;
        OnPropertyChanged(nameof(UnreadNotificationCount));
      }

      IsLoading = false;
    }

    protected async void Refresh_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      await RefreshFeed(null);
    }

    protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
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

    private async void Home_PivotItemLoading(Pivot sender, PivotItemEventArgs args)
    {
      var feed = args.Item.DataContext as Feed;
      if (feed == null)
      {
        return;
      }
      if (feed.FeedItems == null)
      {
        try
        {
          if (string.IsNullOrEmpty(feed.URI))
          {
            var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetTimelineResponse>(new ATProto.Lexicons.App.BSky.Feed.GetTimeline()
            {
              limit = 60
            });
            feed.FeedItems = response?.feed;
          }
          else
          {
            if (feed.FeedInfo == null)
            {
              var feedInfoResponse = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetFeedGeneratorResponse>(new ATProto.Lexicons.App.BSky.Feed.GetFeedGenerator()
              {
                feed = feed.URI
              });
              feed.FeedInfo = feedInfoResponse.view;
              feed.Name = feed.FeedInfo.displayName;
              feed.OnPropertyChanged("Name");
            }
            var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetFeedResponse>(new ATProto.Lexicons.App.BSky.Feed.GetFeed()
            {
              limit = 60,
              feed = feed.URI
            });
            feed.FeedItems = response?.feed;
          }
          OnPropertyChanged(nameof(Feeds));
          feed.OnPropertyChanged("FeedItems");
        }
        catch (WebException ex)
        {
          HasError = true;
          var webResponse = ex.Response as HttpWebResponse;
          var error = ex.Response != null ? new StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : ex.ToString();
          ErrorText = $"HTTP ERROR {(int)webResponse.StatusCode}\n\n{error}";
        }
      }
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
      public List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> FeedItems { get; set; }

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
