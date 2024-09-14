﻿using Newtonsoft.Json;
using System.Collections.Specialized;

namespace BlueWP.ATProto
{
  public interface ILexiconRequest
  {
    [JsonIgnore]
    string EndpointID { get; }
  }

  public interface ILexiconResponse
  {
  }

  public interface IRawPost
  {
    [JsonIgnore]
    byte[] PostData { get; set; }
    [JsonIgnore]
    string MimeType { get; set; }
  }

  public interface ICustomHeaderProvider
  {
    void SetCustomHeaders(NameValueCollection headers, Settings.AccountSettingsData accountSettings);
  }
}
