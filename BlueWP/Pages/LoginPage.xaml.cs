﻿using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BlueWP.Pages
{
  public partial class LoginPage : Page, INotifyPropertyChanged
  {
    private App _app;
    private string _serviceHost = "bsky.social";
    private string _handle = string.Empty;
    private string _appPassword = string.Empty;
    private string _params = string.Empty;

    public LoginPage()
    {
      InitializeComponent();
      _app = (App)Windows.UI.Xaml.Application.Current;
      DataContext = this;

      OnPropertyChanged(nameof(ServiceHost));
      OnPropertyChanged(nameof(Handle));
      OnPropertyChanged(nameof(AppPassword));
    }

    public string ServiceHost { get { return _serviceHost; } set { _serviceHost = value; OnPropertyChanged(nameof(ServiceHost)); } }
    public string Handle { get { return _handle; } set { _handle = value; OnPropertyChanged(nameof(Handle)); } }
    public string AppPassword { get { return _appPassword; } set { _appPassword = value; OnPropertyChanged(nameof(AppPassword)); } }

    public event PropertyChangedEventHandler PropertyChanged;

    private async void Login_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      if (await _app.Client.AuthenticateWithPassword(_serviceHost, _handle, _appPassword))
      {
        _app.NavigateToMainScreen(_params);
      }
      else
      {
        var dialog = new ContentDialog
        {
          Content = new TextBlock { Text = $"Login failed!" },
          Title = $"Login failed!",
          PrimaryButtonText = "Ok :(",
        };
        await dialog.ShowAsync();
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      _params = e.Parameter as string;
      base.OnNavigatedTo(e);
    }

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
