namespace BlueWP.ATProto.Lexicons.App.Video
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/video/defs.json"/>
  public class Defs
  {
    public class JobStatus
    {
      public string jobId;
      public string did;
      public string state;
      public string progress;
      public Blob blob;
      public string error;
      public string message;
    }
  }
}
