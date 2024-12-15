using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls.ProfileList
{
  public partial class ProfileListBase : UserControl, INotifyPropertyChanged
  {
    private App _app;
    protected Pages.MainPage _mainPage;

    public ProfileListBase()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += ProfileList_Loaded;
      DataContext = this;
    }

    private void ProfileList_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public virtual Task<List<ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView>> GetProfileItems()
    {
      return null;
    }

    public async Task Refresh()
    {
      _mainPage?.StartLoading();

      var profileItems = await GetProfileItems();
      ProfileItems = profileItems == null ? new ObservableCollection<ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView>() : new ObservableCollection<ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView>(profileItems);

      _mainPage?.EndLoading();

      OnPropertyChanged(nameof(ProfileItems));
    }

    public void Flush()
    {
      ProfileItems?.Clear();
      OnPropertyChanged(nameof(ProfileItems));
    }

    public ObservableCollection<ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView> ProfileItems { get; set; }

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

    private void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      var b = sender as Button;
      var profile = b?.DataContext as ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView;
      if (profile != null)
      {
        _mainPage.SwitchToProfileInlay(profile.did);
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
