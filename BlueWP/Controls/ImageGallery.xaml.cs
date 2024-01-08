using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public partial class ImageGallery : UserControl, INotifyPropertyChanged
  {
    public ImageGallery()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public IEnumerable<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage> Images
    {
      get { return GetValue(ImagesProperty) as IEnumerable<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage>; }
      set { SetValue(ImagesProperty, value); }
    }
    public static readonly DependencyProperty ImagesProperty = DependencyProperty.Register("Images", typeof(IEnumerable<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage>), typeof(ImageGallery), new PropertyMetadata(null, ImagesChanged));

    private static void ImagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var imageGallery = d as ImageGallery;
      if (imageGallery != null)
      {
        imageGallery.OnPropertyChanged(nameof(Images));
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
