using Newtonsoft.Json;

namespace BlueWP.ATProto.Lexicons.COM.ATProto.Repo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/repo/uploadBlob.json"/>
  public class UploadBlob : ILexiconRequest, IRawPost
  {
    public string EndpointID => "com.atproto.repo.uploadBlob";

    [JsonIgnore]
    public byte[] PostData { get; set; }
    [JsonIgnore]
    public string MimeType { get; set; }

    public class Response : ILexiconResponse
    {
      public Blob blob;
    }
  }
}
