namespace BlueWP.ATProto.Lexicons.App.Video
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/video/getUploadLimits.json"/>
  public class GetUploadLimits : ILexiconRequest
  {
    public string EndpointID => "app.bsky.video.getUploadLimits";

    public class Response : ILexiconResponse
    {
      public bool canUpload;
      public uint remainingDailyVideos;
      public uint remainingDailyBytes;
      public string message;
      public string error;
    }
  }
}
