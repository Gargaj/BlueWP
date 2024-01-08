using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
  }
}
