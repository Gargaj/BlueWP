using System.Collections.Generic;

namespace BlueWP.ATProto.Lexicons.App.BSky.Embed
{
    /// <see cref="https://github.com/bluesky-social/atproto/blob/main/lexicons/app/bsky/embed/images.json"/>
    public class Images
    {
        public class AspectRatio
        {
            public uint width;
            public uint height;
        }

        public class View
        {
            public List<ViewImage> images;
        }

        public class ViewImage
        {
            public string thumb;
            public string fullsize;
            public string alt;
            public AspectRatio aspectRatio;
        }
    }
}
