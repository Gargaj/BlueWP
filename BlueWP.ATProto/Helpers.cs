using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueWP.ATProto
{
  public class Helpers
  {
    public static bool ParseATURI(string atUri, ref string repo, ref string collection, ref string rkey)
    {
      var regex = new System.Text.RegularExpressions.Regex("at://(.*)/(.*)/(.*)");
      var result = regex.Match(atUri);
      if (!result.Success)
      {
        return false;
      }
      repo = result.Groups[1].Value;
      collection = result.Groups[2].Value;
      rkey = result.Groups[3].Value;
      return true;
    }
    public static bool ParseHTTPURI(string httpUri, ref string repo, ref string collection, ref string rkey)
    {
      var regex = new System.Text.RegularExpressions.Regex("https?://.*/profile/(.*)/post/(.*)");
      var result = regex.Match(httpUri);
      if (!result.Success)
      {
        return false;
      }
      repo = result.Groups[1].Value;
      collection = "app.bsky.feed.post";
      rkey = result.Groups[2].Value;
      return true;
    }
    public static async Task<string> HTTPToATURI(Client client, string httpUri)
    {
      string repo = string.Empty;
      string collection = string.Empty;
      string rkey = string.Empty;
      if (ParseHTTPURI(httpUri, ref repo, ref collection, ref rkey))
      {
        if (!repo.StartsWith("did:"))
        {
          var response = await client.GetAsync<Lexicons.COM.ATProto.Identity.ResolveHandleResponse>(new Lexicons.COM.ATProto.Identity.ResolveHandle() {
            handle = repo
          });
          repo = response.did;
        }
        return $"at://{repo}/{collection}/{rkey}";
      }
      return null;
    }
    public static string ToElapsedTime(DateTime dateTime)
    {
      var timespan = DateTime.Now - dateTime;
      if (timespan.TotalSeconds < 60)
      {
        return timespan.ToString("%s") + "s";
      }
      if (timespan.TotalSeconds < 60 * 60)
      {
        return timespan.ToString("%m") + "m";
      }
      if (timespan.TotalSeconds < 60 * 60 * 24)
      {
        return timespan.ToString("%h") + "h";
      }
      if (timespan.TotalSeconds < 60 * 60 * 24 * 7)
      {
        return timespan.ToString("%d") + "d";
      }
      if (dateTime.Year == DateTime.Now.Year)
      {
        return dateTime.ToString("MMM d");
      }
      return "'" + dateTime.ToString("yy MMM d");
    }
  }
}
