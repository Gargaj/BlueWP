﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class FeedInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;
    private bool _followedOnly = false;

    public FeedInlay()
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

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> feedItems = null;
      if (!string.IsNullOrEmpty(FeedURI))
      {
        var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.GetFeed.Response>(new ATProto.Lexicons.App.BSky.Feed.GetFeed()
        {
          limit = 60,
          feed = FeedURI
        });
        feedItems = response?.feed;
      }
      else if (!string.IsNullOrEmpty(ActorDID))
      {
        var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.GetAuthorFeed.Response>(new ATProto.Lexicons.App.BSky.Feed.GetAuthorFeed()
        {
          limit = 60,
          actor = ActorDID
        });
        feedItems = response?.feed;
      }
      else
      {
        var response = await _mainPage.Get<ATProto.Lexicons.App.BSky.Feed.GetTimeline.Response>(new ATProto.Lexicons.App.BSky.Feed.GetTimeline()
        {
          limit = 60
        });
        feedItems = response?.feed;
      }

      if (_followedOnly)
      {
        feedItems = feedItems.Where((s) => {
          if (s?.reply?.parent == null)
          {
            return true;
          }
          var post = s.reply.parent as ATProto.Lexicons.App.BSky.Feed.Defs.PostView;
          if (post == null)
          {
            return true;
          }
          return !string.IsNullOrEmpty(post.author.viewer.following);
        }).ToList();
      }
      FeedItems = new ObservableCollection<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost>(feedItems);

      _mainPage?.EndLoading();

      OnPropertyChanged(nameof(FeedItems));
    }

    public void Flush()
    {
      FeedItems?.Clear();
      OnPropertyChanged(nameof(FeedItems));
    }

    public string FeedURI
    {
      get { return (string)GetValue(FeedURIProperty); }
      set { SetValue(FeedURIProperty, value); }
    }
    public static readonly DependencyProperty FeedURIProperty = DependencyProperty.Register("FeedURI", typeof(string), typeof(FeedInlay), new PropertyMetadata(string.Empty));

    public string ActorDID
    {
      get { return (string)GetValue(ActorDIDProperty); }
      set { SetValue(ActorDIDProperty, value); }
    }
    public static readonly DependencyProperty ActorDIDProperty = DependencyProperty.Register("ActorDID", typeof(string), typeof(FeedInlay), new PropertyMetadata(string.Empty));

    public ObservableCollection<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> FeedItems { get; set; }
    public bool FollowedOnly { get => _followedOnly; set => _followedOnly = value; }

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
