namespace BlueWP.ATProto.Lexicons.COM.ATProto.Repo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/repo/deleteRecord.json"/>
  public class DeleteRecord : ILexiconRequest
  {
    public string EndpointID => "com.atproto.repo.deleteRecord";

    public string repo;
    public string collection;
    public string rkey;
    public string swapRecord;
    public string swapCommit;

    public class Response : ILexiconResponse
    {
    }
  }
}
