namespace BlueWP.ATProto.Lexicons.App.Video
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/video/uploadVideo.json"/>
  public class UploadVideo : ILexiconRequest
  {
    public string EndpointID => "app.bsky.video.uploadVideo";

    public class Response : ILexiconResponse
    {
      public Defs.JobStatus jobStatus;
    }
  }
}
