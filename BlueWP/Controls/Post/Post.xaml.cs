using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace BlueWP.Controls.Post
{
  public partial class Post : PostBase
  {
    public Post() : base()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }
  }
}
