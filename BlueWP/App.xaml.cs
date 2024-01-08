using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BlueWP
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  sealed partial class App : Application
  {
    private ATProto.Client _client;
    private List<object> _preferences;
    private ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref _savedFeedsPref;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      _client = new ATProto.Client();

      this.InitializeComponent();
      this.Suspending += OnSuspending;
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
      Frame rootFrame = Window.Current.Content as Frame;

      // Do not repeat app initialization when the Window already has content,
      // just ensure that the window is active
      if (rootFrame == null)
      {
        // Create a Frame to act as the navigation context and navigate to the first page
        rootFrame = new Frame();

        rootFrame.NavigationFailed += OnNavigationFailed;

        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
          //TODO: Load state from previously suspended application
        }

        // Place the frame in the current Window
        Window.Current.Content = rootFrame;
      }

      if (e.PrelaunchActivated == false)
      {
        if (rootFrame.Content == null)
        {
          // When the navigation stack isn't restored navigate to the first page,
          // configuring the new page by passing required information as a navigation
          // parameter
          if (!await _client.Authenticate())
          {
            rootFrame.Navigate(typeof(Pages.LoginPage), e.Arguments);
          }
          else
          {
            await NavigateToMainScreen();
          }
        }
        // Ensure the current window is active
        Window.Current.Activate();
      }
    }

    public void NavigateToSettings()
    {
      //rootFrame.Navigate(typeof(Pages.LoginPage), e.Arguments);
    }

    public async Task NavigateToMainScreen()
    {
      Frame rootFrame = Window.Current.Content as Frame;
      if (rootFrame == null)
      {
        return;
      }

      var preferences = await _client.GetAsync<ATProto.Lexicons.App.BSky.Actor.GetPreferencesResponse>(new ATProto.Lexicons.App.BSky.Actor.GetPreferences());
      if (preferences != null)
      {
        _preferences = preferences.preferences;
        _savedFeedsPref = _preferences?.FirstOrDefault(s => s is ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref) as ATProto.Lexicons.App.BSky.Actor.Defs.SavedFeedsPref;
      }

      rootFrame.Navigate(typeof(Pages.FeedPage));
    }

    public async void Logout()
    {
      Frame rootFrame = Window.Current.Content as Frame;
      if (rootFrame == null)
      {
        return;
      }

      await _client.DeleteCredentials();
      rootFrame.Navigate(typeof(Pages.LoginPage));
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
      var deferral = e.SuspendingOperation.GetDeferral();
      //TODO: Save application state and stop any background activity
      deferral.Complete();
    }

    public ATProto.Client Client => _client;
  }
}
