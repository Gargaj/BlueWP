using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class SettingsInlay : UserControl, INotifyPropertyChanged
  {
    private App _app;
    protected Pages.MainPage _mainPage;
    public SettingsInlay()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += SettingsInlay_Loaded;
      SettingsLayoutRoot.DataContext = this;
    }

    protected async void SettingsInlay_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();

      await Refresh();
    }

    public string Platform { get { return Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily; } }

    public List<AccountInfo> Accounts { get; set; }
    public string Handle { get { return _app.Client.Handle; } }
    public string DID { get { return _app.Client.DID; } }

    public async Task Refresh()
    {
      var response = await _app.Client.GetAsync<ATProto.Lexicons.App.BSky.Actor.GetProfilesResponse>(new ATProto.Lexicons.App.BSky.Actor.GetProfiles()
      {
        actors = _app.Client.Settings.AccountSettings.Select(s => s.Credentials.DID).ToList()
      });

      Accounts = new List<AccountInfo>();

      foreach (var account in _app.Client.Settings.AccountSettings)
      {
        var profile = response.profiles.FirstOrDefault(s => s.did == account.Credentials.DID);
        Accounts.Add(new AccountInfo() {
          AccountAvatarURL = profile?.avatar,
          Handle = profile?.handle,
          Name = profile?.DisplayName,
          DID = profile?.did,
          IsSelected = _app.Client.CurrentAccountSettings == account
        });
      }

      OnPropertyChanged(nameof(Accounts));
    }

    private async void RemoveAccount_Click(object sender, RoutedEventArgs e)
    {
      var button = e.OriginalSource as Button;
      if (button == null)
      {
        return;
      }
      var dataContext = button.DataContext as AccountInfo;
      if (dataContext == null)
      {
        return;
      }

      var dialog = new Windows.UI.Popups.MessageDialog("Are you sure?");
      dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes", new Windows.UI.Popups.UICommandInvokedHandler(async (s) => {
        await _app.Client.Settings.DeleteAccountSettings(dataContext.DID);
      })));
      dialog.Commands.Add(new Windows.UI.Popups.UICommand("No", new Windows.UI.Popups.UICommandInvokedHandler((s) => { })));
      dialog.DefaultCommandIndex = 1;
      dialog.CancelCommandIndex = 1;
      await dialog.ShowAsync();
    }

    private void AddAccount_Click(object sender, RoutedEventArgs e)
    {
      _app.NavigateToLoginScreen(string.Empty);
    }

    private async void SwitchAccount_Click(object sender, RoutedEventArgs e)
    {
      var button = e.OriginalSource as Button;
      if (button == null)
      {
        return;
      }
      var dataContext = button.DataContext as AccountInfo;
      if (dataContext == null)
      {
        return;
      }

      _app.Client.Settings.SelectedDID = dataContext.DID;
      await _mainPage.RefreshNotificationCounter();
      _mainPage.SwitchToProfileInlay(dataContext.DID);
    }

    public class AccountInfo
    {
      public string AccountAvatarURL { get; set; }
      public string Name { get; set; }
      public string Handle { get; set; }
      public string DID { get; set; }
      public bool IsSelected { get; set; }
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
  }
}
