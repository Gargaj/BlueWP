using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Inlays
{
  public sealed partial class FeedInlay : UserControl
  {
    public FeedInlay()
    {
      InitializeComponent();
    }

    public List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost> FeedItems
    {
      get { return GetValue(FeedItemsProperty) as List<ATProto.Lexicons.App.BSky.Feed.Defs.FeedViewPost>; }
      set { SetValue(FeedItemsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FeedItems.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FeedItemsProperty = DependencyProperty.Register("FeedItems", typeof(int), typeof(FeedInlay), new PropertyMetadata(null));
  }
}
