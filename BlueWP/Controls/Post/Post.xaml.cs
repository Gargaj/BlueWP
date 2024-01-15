namespace BlueWP.Controls.Post
{
  public partial class Post : PostBase
  {
    public Post() : base()
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
