using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls.Post
{
  public partial class ImageGallery : UserControl, INotifyPropertyChanged
  {
    public ImageGallery()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public ImageGalleryProvider Gallery { get; set; }

    public List<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage> Images
    {
      get { return GetValue(ImagesProperty) as List<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage>; }
      set { SetValue(ImagesProperty, value); }
    }
    public static readonly DependencyProperty ImagesProperty = DependencyProperty.Register("Images", typeof(List<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage>), typeof(ImageGallery), new PropertyMetadata(null, ImagesChanged));

    private static void ImagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var imageGallery = d as ImageGallery;
      if (imageGallery != null)
      {
        imageGallery.Gallery = new ImageGalleryProvider(imageGallery.Images);
        imageGallery.OnPropertyChanged(nameof(Images));
        imageGallery.OnPropertyChanged(nameof(Gallery));
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

    public class ImageGalleryProvider
    {
      private List<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage> _images;
      public ImageGalleryProvider(List<ATProto.Lexicons.App.BSky.Embed.Images.ViewImage> images)
      {
        _images = images;
      }
      public int ImageCount => _images?.Count ?? 0;
      public string Image1ThumbURL => _images != null && _images.Count > 0 ? _images[0]?.ThumbURL : null;
      public string Image2ThumbURL => _images != null && _images.Count > 1 ? _images[1]?.ThumbURL : null;
      public string Image3ThumbURL => _images != null && _images.Count > 2 ? _images[2]?.ThumbURL : null;
      public string Image4ThumbURL => _images != null && _images.Count > 3 ? _images[3]?.ThumbURL : null;
    }
  }
}
