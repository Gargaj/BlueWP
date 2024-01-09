namespace BlueWP.Controls.Post
{
  public sealed partial class PostQuoted : PostBase
  {
    public PostQuoted()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }
  }
}
