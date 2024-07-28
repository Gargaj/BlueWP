using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls
{
  public sealed partial class MainPagePivotHeader : UserControl
  {
    public MainPagePivotHeader()
    {
      InitializeComponent();
      DataContext = this;
    }

    public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(MainPagePivotHeader), null);

    public string Glyph
    {
      get { return GetValue(GlyphProperty) as string; }
      set { SetValue(GlyphProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(MainPagePivotHeader), null);

    public string Label
    {
      get { return GetValue(LabelProperty) as string; }
      set { SetValue(LabelProperty, value); }
    }
  }
}
