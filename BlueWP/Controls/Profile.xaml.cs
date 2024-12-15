using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public partial class Profile : UserControl, INotifyPropertyChanged
  {
    private App _app;
    protected Pages.MainPage _mainPage;

    public Profile()
    {
      InitializeComponent();
      _app = (App)Application.Current;
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
      Loaded += ProfileList_Loaded;
      LayoutRoot.DataContext = this;
    }

    private void ProfileList_Loaded(object sender, RoutedEventArgs e)
    {
      _mainPage = _app.GetCurrentFrame<Pages.MainPage>();
    }

    public string DisplayName => ProfileData?.DisplayName;
    public string Handle => ProfileData?.Handle;
    public string AvatarURL => ProfileData?.AvatarURL;
    public string Description => ProfileData?.Description;

    private void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
      var b = sender as Button;
      var profile = b?.DataContext as Profile;
      if (profile?.ProfileData?.did != null)
      {
        _mainPage.SwitchToProfileInlay(profile.ProfileData.did);
      }
    }

    public ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView ProfileData
    {
      get { return GetValue(ProfileDataProperty) as ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView; }
      set { SetValue(ProfileDataProperty, value); }
    }
    public static readonly DependencyProperty ProfileDataProperty = DependencyProperty.Register("ProfileData", typeof(ATProto.Lexicons.App.BSky.Actor.Defs.ProfileView), typeof(Profile), new PropertyMetadata(null, OnPostDataChanged));

    private static void OnPostDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var profile = d as Profile;
      if (profile != null)
      {
        profile.OnPropertyChanged(nameof(DisplayName));
        profile.OnPropertyChanged(nameof(Handle));
        profile.OnPropertyChanged(nameof(AvatarURL));
        profile.OnPropertyChanged(nameof(Description));
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
