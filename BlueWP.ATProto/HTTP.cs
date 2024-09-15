using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlueWP.ATProto
{
  public class HTTP
  {
    private CookieContainer _cookieContainer = new CookieContainer();
    private string _cookieDomain = string.Empty;
    private System.Net.Http.HttpResponseMessage _response;

    public System.Net.Http.HttpResponseMessage Response => _response;

    public async Task<MemoryStream> DoHTTPRequestStreamAsync(string url, byte[] data, NameValueCollection headers = null, string method = "POST", Func<long, long, bool> callback = null)
    {
      var httpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler
      {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
      });
      httpClient.Timeout = TimeSpan.FromSeconds(5);
      _response = null;
      try
      {
        System.Net.Http.HttpMethod httpMethod = System.Net.Http.HttpMethod.Get;
        if (method == "POST")
        {
          httpMethod = System.Net.Http.HttpMethod.Post;
        }
        using (var requestMessage = new System.Net.Http.HttpRequestMessage(httpMethod, url))
        {
          if (httpMethod == System.Net.Http.HttpMethod.Post)
          {
            var content = new System.Net.Http.ByteArrayContent(data);
            if (headers["Content-Type"] != null)
            {
              var types = headers["Content-Type"].Split(';');
              headers.Remove("Content-Type");
              content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(types[0]);
            }
            requestMessage.Content = content;
          }
          else
          {
            if (headers["Content-Type"] != null)
            {
              headers.Remove("Content-Type");
            }
          }

            if (headers["Authorization"] != null)
          {
            requestMessage.Headers.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(headers["Authorization"]);
            headers.Remove("Authorization");
          }

          foreach (var key in headers.AllKeys)
          {
            requestMessage.Headers.Add(key, headers[key]);
          }

          _response = await httpClient.SendAsync(requestMessage);
        }
      }
      catch (TaskCanceledException)
      {
        System.Diagnostics.Debug.WriteLine($"[TIMEOUT] {url}");
        return null;
      }
      return new MemoryStream(await _response.Content.ReadAsByteArrayAsync());
    }

    public async Task<Stream> DoHTTPRequestStreamAsync(string _url, MemoryStream _stream, NameValueCollection _headers = null, string _method = "POST", Func<long, long, bool> callback = null)
    {
      byte[] bytes = _stream.ToArray();
      return await DoHTTPRequestStreamAsync(_url, bytes, _headers);
    }

    public async Task<string> DoHTTPRequestAsync(string _url, byte[] _data, NameValueCollection _headers = null, string _method = "POST", Func<long, long, bool> callback = null)
    {
      var stream = await DoHTTPRequestStreamAsync(_url, _data, _headers, _method);
      return stream != null ? new StreamReader(stream).ReadToEnd() : null;
    }

    public async Task<string> DoHTTPRequestAsync(string _url, MemoryStream _stream, NameValueCollection _headers = null, string _method = "POST", Func<long, long, bool> callback = null)
    {
      var stream = await DoHTTPRequestStreamAsync(_url, _stream.ToArray(), _headers);
      return stream != null ? new StreamReader(stream).ReadToEnd() : null;
    }

    public async Task<string> DoPOSTRequestAsync(string _url, NameValueCollection _params, NameValueCollection _headers = null)
    {
      ASCIIEncoding encoding = new ASCIIEncoding();
      byte[] data = encoding.GetBytes(_params.ToString());

      return await DoHTTPRequestAsync(_url, data, _headers, "POST");
    }

    public async Task<string> DoPOSTRequestAsync(string _url, string _params, NameValueCollection _headers = null)
    {
      byte[] data = Encoding.UTF8.GetBytes(_params);
      return await DoHTTPRequestAsync(_url, data, _headers, "POST");
    }

    public async Task<string> DoPOSTRequestAsync(string _url, byte[] _data, NameValueCollection _headers = null)
    {
      return await DoHTTPRequestAsync(_url, _data, _headers, "POST");
    }

    public async Task<string> DoGETRequestAsync(string _url, NameValueCollection _params = null, NameValueCollection _headers = null)
    {
      if (_params != null && _params.Count > 0)
      {
        _url += "?" + _params.ToString();
      }

      return await DoHTTPRequestAsync(_url, (byte[])null, _headers, "GET");
    }
  }
}