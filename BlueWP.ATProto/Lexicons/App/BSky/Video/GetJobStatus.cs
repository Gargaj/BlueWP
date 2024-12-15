namespace BlueWP.ATProto.Lexicons.App.Video
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/video/getJobStatus.json"/>
  public class GetJobStatus : ILexiconRequest
  {
    public string EndpointID => "app.bsky.video.getJobStatus";

    public string jobId;

    public class Response : ILexiconResponse
    {
      public Defs.JobStatus jobStatus;
    }
  }
}
