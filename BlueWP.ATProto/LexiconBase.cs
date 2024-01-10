using Newtonsoft.Json;

namespace BlueWP.ATProto
{
  public abstract class LexiconBase
  {
    [JsonIgnore]
    public abstract string EndpointID { get; }
    [JsonIgnore]
    public virtual bool RequiresAuthorization { get { return true; } }
  }

  public interface IRawPost
  {
    [JsonIgnore]
    byte[] PostData { get; set; }
    [JsonIgnore]
    string MimeType { get; set; }
  }
}
