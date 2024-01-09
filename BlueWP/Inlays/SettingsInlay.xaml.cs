using System;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public partial class SettingsInlay : UserControl
  {
    private App _app;
    public SettingsInlay()
    {
      InitializeComponent();
      _app = (App)Windows.UI.Xaml.Application.Current;

      LayoutRoot.DataContext = this;
    }

    public string Platform { get { return Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily; } }
    public string Handle { get { return _app.Client.Handle; } }
    public string DID { get { return _app.Client.DID; } }

    private void BackButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      App app = Windows.UI.Xaml.Application.Current as App;
      app.TryGoBack();
    }

    protected async void Logout_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      var dialog = new Windows.UI.Popups.MessageDialog("Are you sure?");
      dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes", new Windows.UI.Popups.UICommandInvokedHandler(LogoutMessageDialogHandler)));
      dialog.Commands.Add(new Windows.UI.Popups.UICommand("No", new Windows.UI.Popups.UICommandInvokedHandler(LogoutMessageDialogHandler)));
      dialog.DefaultCommandIndex = 1;
      dialog.CancelCommandIndex = 1;
      await dialog.ShowAsync();
    }

    protected void LogoutMessageDialogHandler(Windows.UI.Popups.IUICommand command)
    {
      if (command.Label == "Yes")
      {
        _app.Logout();
      }
    }
  }
}
