namespace BlueWP.ATProto.Lexicons.COM.ATProto.Repo
{
  /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/com/atproto/repo/createRecord.json"/>
  public class CreateRecord : LexiconBase
  {
    public override string EndpointID => "com.atproto.repo.createRecord";

    public string repo;
    public string collection;
    public string rkey;
    public bool? validate;
    public object record;
    public string swapCommit;
  }
  public class CreateRecordResponse : LexiconBase
  {
    public override string EndpointID => "com.atproto.repo.createRecord";

    public string uri;
    public string cid;
  }
}
