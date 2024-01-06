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
        private WebHeaderCollection _responseHeaders = null;
        private string _cookieDomain = string.Empty;

        public async Task<MemoryStream> DoHTTPRequestStreamAsync(string url, byte[] data, NameValueCollection headers = null, string method = "POST", Func<long, long, bool> callback = null)
        {
            HttpWebRequest _httpWReq = (HttpWebRequest)WebRequest.Create(url);

            _httpWReq.Method = method;
            if (method == "POST" && data != null)
            {
                _httpWReq.ContentType = "application/x-www-form-urlencoded";
                //_httpWReq.ContentLength = data.Length;
            }

            if (headers != null)
            {
                if (headers["Content-Type"] != null)
                {
                    _httpWReq.ContentType = headers["Content-Type"];
                    headers.Remove("Content-Type");
                }
                //         if (headers["User-Agent"] != null)
                //         {
                //           _httpWReq.UserAgent = headers["User-Agent"];
                //           headers.Remove("User-Agent");
                //         }
                if (headers["Accept"] != null)
                {
                    _httpWReq.Accept = headers["Accept"];
                    headers.Remove("Accept");
                }
                //         if (headers["Referer"] != null)
                //         {
                //           _httpWReq.Referer = headers["Referer"];
                //           headers.Remove("Referer");
                //         }
                foreach (var key in headers.AllKeys)
                {
                    _httpWReq.Headers[key] = headers[key];
                }
            }
            _httpWReq.CookieContainer = _cookieContainer;

            if (method == "POST")
            {
                using (var stream = await _httpWReq.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }

            // May throw exception
            var response = (HttpWebResponse)await _httpWReq.GetResponseAsync();

            /*
                        HttpWebResponse response = null;
                        try
                        {
                            _responseHeaders = response.Headers;
                        }
                        catch (WebException ex)
                        {
                            var webResponse = ex.Response as HttpWebResponse;
                            var error = ex.Response != null ? new StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : ex.ToString();
            #if DEBUG
                            System.Diagnostics.Debug.WriteLine($"[HTTP ERROR {webResponse.StatusCode}] {error}");
            #endif
                            return null;
                        }
            */

            int bytesRead;
            byte[] buffer = new byte[1024 * 1024];
            MemoryStream memoryStream = null;
            if (response.ContentLength > 0)
            {
                memoryStream = new MemoryStream((int)response.ContentLength);
            }
            else
            {
                memoryStream = new MemoryStream();
            }
            if (callback != null)
            {
                bool keepRunning = callback(0, response.ContentLength);
                if (keepRunning == false)
                {
                    return null;
                }
            }
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var responseStream = response.GetResponseStream();

            while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
                if (callback != null && stopwatch.ElapsedMilliseconds > 1000)
                {
                    stopwatch.Restart();
                    bool keepRunning = callback(memoryStream.Length, response.ContentLength);
                    if (keepRunning == false)
                    {
                        return null;
                    }
                }
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
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

        public async Task<string> DoGETRequestAsync(string _url, NameValueCollection _params = null, NameValueCollection _headers = null)
        {
            if (_params != null && _params.Count > 0)
            {
                _url += "?" + _params.ToString();
            }

            return await DoHTTPRequestAsync(_url, (byte[])null, _headers, "GET");
        }

        public async Task<MemoryStream> DoHTTPRequestStream(string url, byte[] data, NameValueCollection headers = null, string method = "POST", Func<long, long, bool> callback = null)
        {
            HttpWebRequest _httpWReq = (HttpWebRequest)WebRequest.Create(url);

            _httpWReq.Method = method;
            if (method == "POST" && data != null)
            {
                _httpWReq.ContentType = "application/x-www-form-urlencoded";
                //         _httpWReq.ContentLength = data.Length;
            }

            if (headers != null)
            {
                if (headers["Content-Type"] != null)
                {
                    _httpWReq.ContentType = headers["Content-Type"];
                    headers.Remove("Content-Type");
                }
                //         if (headers["User-Agent"] != null)
                //         {
                //           _httpWReq.UserAgent = headers["User-Agent"];
                //           headers.Remove("User-Agent");
                //         }
                if (headers["Accept"] != null)
                {
                    _httpWReq.Accept = headers["Accept"];
                    headers.Remove("Accept");
                }
                //         if (headers["Referer"] != null)
                //         {
                //           _httpWReq.Referer = headers["Referer"];
                //           headers.Remove("Referer");
                //         }
                foreach (var key in headers.AllKeys)
                {
                    _httpWReq.Headers[key] = headers[key];
                }
            }
            _httpWReq.CookieContainer = _cookieContainer;

            if (method == "POST")
            {
                using (var stream = await _httpWReq.GetRequestStreamAsync())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)await _httpWReq.GetResponseAsync();
                _responseHeaders = response.Headers;
            }
            catch (WebException ex)
            {
                string error = ex.Response != null ? new StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : ex.ToString();
                return null;
            }

            int bytesRead;
            byte[] buffer = new byte[1024 * 1024];
            MemoryStream memoryStream = null;
            if (response.ContentLength > 0)
            {
                memoryStream = new MemoryStream((int)response.ContentLength);
            }
            else
            {
                memoryStream = new MemoryStream();
            }
            if (callback != null)
            {
                bool keepRunning = callback(0, response.ContentLength);
                if (keepRunning == false)
                {
                    return null;
                }
            }
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var responseStream = response.GetResponseStream();

            while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
                if (callback != null && stopwatch.ElapsedMilliseconds > 1000)
                {
                    stopwatch.Restart();
                    bool keepRunning = callback(memoryStream.Length, response.ContentLength);
                    if (keepRunning == false)
                    {
                        return null;
                    }
                }
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public async Task<Stream> DoHTTPRequestStream(string _url, MemoryStream _stream, NameValueCollection _headers = null, string _method = "POST", Func<long, long, bool> callback = null)
        {
            byte[] bytes = _stream.ToArray();
            return await DoHTTPRequestStream(_url, bytes, _headers);
        }

        public async Task<string> DoHTTPRequest(string _url, byte[] _data, NameValueCollection _headers = null, string _method = "POST", Func<long, long, bool> callback = null)
        {
            var stream = await DoHTTPRequestStream(_url, _data, _headers, _method);
            return stream != null ? new StreamReader(stream).ReadToEnd() : null;
        }

        public async Task<string> DoHTTPRequest(string _url, MemoryStream _stream, NameValueCollection _headers = null, string _method = "POST", Func<long, long, bool> callback = null)
        {
            var stream = await DoHTTPRequestStream(_url, _stream.ToArray(), _headers);
            return stream != null ? new StreamReader(stream).ReadToEnd() : null;
        }

        public async Task<string> DoPOSTRequest(string _url, NameValueCollection _params, NameValueCollection _headers = null)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(_params.ToString());

            return await DoHTTPRequest(_url, data, _headers, "POST");
        }

        public async Task<string> DoGETRequest(string _url, NameValueCollection _params = null, NameValueCollection _headers = null)
        {
            if (_params != null && _params.Count > 0)
            {
                _url += "?" + _params.ToString();
            }

            return await DoHTTPRequest(_url, (byte[])null, _headers, "GET");
        }
    }
}