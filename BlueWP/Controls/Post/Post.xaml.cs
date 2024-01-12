using Windows.UI.Xaml.Controls;

namespace BlueWP.Controls.Post
{
  public partial class Post : PostBase
  {
    public Post() : base()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    private void ViewProfile_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
      // TODO
    }
  }
}
