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

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      try
      {
        var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Notification.ListNotificationsResponse>(new ATProto.Lexicons.App.BSky.Notification.ListNotifications()
        {
          limit = 60
        });
        Notifications = response?.notifications;
      }
      catch (WebException ex)
      {
        var webResponse = ex.Response as HttpWebResponse;
        _mainPage?.TriggerError($"HTTP ERROR {(int)webResponse.StatusCode}\n\n{ex.Message}");
      }

      _mainPage?.EndLoading();

      OnPropertyChanged(nameof(Notifications));
    }

    public List<ATProto.Lexicons.App.BSky.Notification.Notification> Notifications { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises this object's PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property that has a new value.</param>
    public virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Post_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
      var post = sender as Controls.Post.PostBase;
      if (post != null)
      {
        _mainPage.SwitchToThreadViewInlay(post.PostData.PostURI);
      }
    }
  }
}
