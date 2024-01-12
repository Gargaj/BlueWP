using Newtonsoft.Json;

namespace BlueWP.ATProto
{
  public interface ILexicon
  {
    [JsonIgnore]
    string EndpointID { get; }
  }

  public interface IRawPost
  {
    [JsonIgnore]
    byte[] PostData { get; set; }
    [JsonIgnore]
    string MimeType { get; set; }
  }

  public interface ICustomAuthorizationHeaderProvider
  {
    string GetAuthorizationHeader(Client.Credentials credentials);
  }
}
