using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
          var response = await client.GetAsync<Lexicons.COM.ATProto.Identity.ResolveHandleResponse>(new Lexicons.COM.ATProto.Identity.ResolveHandle()
          {
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
      var timespan = DateTime.Now - dateTime.ToLocalTime();
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
        return dateTime.ToLocalTime().ToString("MMM d");
      }
      return "'" + dateTime.ToLocalTime().ToString("yy MMM d");
    }

    public static uint ConvertCharacterPositionToBytePositionInString(string s, int characterPosition)
    {
      return (uint)System.Text.Encoding.UTF8.GetBytes(s.Substring(0, characterPosition)).Length;
    }

    public static async Task<List<Lexicons.App.BSky.RichText.Facet>> ParseTextForFacets(Client client, string postText)
    {
      var results = new List<Lexicons.App.BSky.RichText.Facet>();

      // regex based on: https://atproto.com/specs/handle#handle-identifier-syntax
      // but with added "?:"-s to not capture stuff that shouldnt be
      var mentionRegex = new Regex(@"[$|\W](@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)");
      await FindFacet(postText, results, mentionRegex, async (facet, matchText) =>
      {
        Lexicons.COM.ATProto.Identity.ResolveHandleResponse response = null;
        try
        {
          response = await client.GetAsync<Lexicons.COM.ATProto.Identity.ResolveHandleResponse>(new Lexicons.COM.ATProto.Identity.ResolveHandle()
          {
            handle = matchText.Substring(1) // chop off @
          });
        }
        catch (Exception)
        {
          return;
        }
        if (response != null)
        {
          facet.features = new List<object>()
              {
              new Lexicons.App.BSky.RichText.Facet.Mention()
              {
                did = response.did,
              }
              };
        }
      });

      var linkRegex = new Regex(@"[$|\W](https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&//=]*[-a-zA-Z0-9@%_\+~#//=])?)");
      await FindFacet(postText, results, linkRegex, async (facet, matchText) =>
      {
        facet.features = new List<object>()
            {
            new Lexicons.App.BSky.RichText.Facet.Link()
            {
              uri = matchText
            }
            };
      });

      var hashtagRegex = new Regex(@"(#\w+)");
      await FindFacet(postText, results, hashtagRegex, async (facet, matchText) =>
      {
        facet.features = new List<object>()
            {
            new Lexicons.App.BSky.RichText.Facet.Tag()
            {
              tag = matchText.Substring(1)
            }
            };
      });

      return results.Count == 0 ? null : results;
    }

    public static async Task FindFacet(string postText, List<Lexicons.App.BSky.RichText.Facet> facets, Regex regex, Func<Lexicons.App.BSky.RichText.Facet, string, Task> perform)
    {
      var matches = regex.Matches(postText);
      if (matches.Count <= 0)
      {
        return;
      }
      foreach (Match m in matches)
      {
        var facet = new Lexicons.App.BSky.RichText.Facet();

        var group = m.Groups[1];
        await perform(facet, group.Value.ToString());

        facet.index = new Lexicons.App.BSky.RichText.Facet.ByteSlice()
        {
          byteStart = ConvertCharacterPositionToBytePositionInString(postText, group.Index),
          byteEnd = ConvertCharacterPositionToBytePositionInString(postText, group.Index + group.Length),
        };
        facets.Add(facet);
      }
    }
  }
}