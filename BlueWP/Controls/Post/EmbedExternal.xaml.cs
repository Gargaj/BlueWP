using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls.Post
{
  public partial class EmbedExternal : UserControl, INotifyPropertyChanged
  {
    public EmbedExternal()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public ATProto.Lexicons.App.BSky.Embed.External.View External
    {
      get { return GetValue(ExternalProperty) as ATProto.Lexicons.App.BSky.Embed.External.View; }
      set { SetValue(ExternalProperty, value); }
    }
    public static readonly DependencyProperty ExternalProperty = DependencyProperty.Register("External", typeof(ATProto.Lexicons.App.BSky.Embed.External.View), typeof(EmbedExternal), new PropertyMetadata(null, ImagesChanged));

    private static void ImagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var embedExternal = d as EmbedExternal;
      if (embedExternal != null)
      {
        embedExternal.OnPropertyChanged(nameof(External));
      }
    }

    public async void OpenExternalURL_Click(object sender, RoutedEventArgs e)
    {
      var button = e.OriginalSource as Button;
      if (button == null)
      {
        return;
      }
      var dataContext = button.DataContext as EmbedExternal;
      if (dataContext == null)
      {
        return;
      }
      if (!string.IsNullOrEmpty(dataContext.External.URL))
      {
        await Windows.System.Launcher.LaunchUriAsync(new Uri(dataContext.External.URL));
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
  }
}
