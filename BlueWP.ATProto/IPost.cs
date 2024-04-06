using System.Collections.Generic;

namespace BlueWP.ATProto
{
  public interface IPost
  {
    bool IsRepost { get; }
    bool IsReply { get; }
    bool HasQuotedPost { get; }
    bool HasEmbedExternal { get; }
    string PostAuthorAvatarURL { get; }
    string PostAuthorDisplayName { get; }
    string PostAuthorHandle { get; }
    string PostElapsedTime { get; }
    string PostText { get; }
    string PostURI { get; }
    IEnumerable<Lexicons.App.BSky.Embed.Images.ViewImage> PostImages { get; }
  }
}
