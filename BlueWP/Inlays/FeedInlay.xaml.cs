using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class FeedInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    private Pages.MainPage _mainPage;

    public FeedInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      DataContext = this;
    }

    public async Task Refresh()
    {
      try
      {
        if (string.IsNullOrEmpty(URI))
        {
          var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetTimelineResponse>(new ATProto.Lexicons.App.BSky.Feed.GetTimeline()
          {
            limit = 60
          });
          FeedItems = response?.feed;
        }
        else
        {
          var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Feed.GetFeedResponse>(new ATProto.Lexicons.App.BSky.Feed.GetFeed()
          {
            limit = 60,
            feed = URI
          });
          FeedItems = response?.feed;
        }
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        _mainPage.TriggerError($"HTTP ERROR {(int)webResponse.StatusCode}\n\n{ex.Message}");
      }
      OnPropertyChanged(nameof(FeedItems));
    }

    public string URI
    {
      get { return (string)GetValue(URIProperty); }
      set { SetValue(URIProperty, value); }
    }

    public static readonly DependencyProperty URIProperty = DependencyProperty.Register("URI", typeof(string), typeof(FeedInlay), new PropertyMetadata(string.Empty));

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
