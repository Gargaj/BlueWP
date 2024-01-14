namespace BlueWP.Controls.Post
{
  public partial class PostSelected : PostBase
  {
    public PostSelected() : base()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }
  }
}
