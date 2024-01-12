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
