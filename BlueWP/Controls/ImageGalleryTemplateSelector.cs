using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  class ImageGalleryTemplateSelector : DataTemplateSelector
  {
    public DataTemplate Image0 { get; set; }
    public DataTemplate Image1 { get; set; }
    public DataTemplate Image2 { get; set; }
    public DataTemplate Image3 { get; set; }
    public DataTemplate Image4 { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
      var images = item as Post.ImageGallery.ImageGalleryProvider;
      switch (images?.ImageCount ?? 0)
      {
        case 1: return Image1;
        case 2: return Image2;
        case 3: return Image3;
        case 4: return Image4;
      }
      return Image0;
    }
  }
}
