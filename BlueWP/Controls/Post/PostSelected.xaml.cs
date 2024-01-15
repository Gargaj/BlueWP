namespace BlueWP.Controls.Post
{
  public partial class PostSelected : PostBase
  {
    public PostSelected() : base()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    protected override void UpdateText()
    {
      postText.Inlines.Clear();
      GenerateInlines().ForEach(s => postText.Inlines.Add(s));
    }
  }
}
