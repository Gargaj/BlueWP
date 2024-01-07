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
}
