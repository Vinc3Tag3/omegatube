using System.Collections.Generic;

namespace OmegaTube.Data
{
    // In-memory play queue for this app session (cleared on restart). Player consumes in US7.
    public static class SessionQueue
    {
        private static readonly List<VideoItem> items = new List<VideoItem>();

        public static IReadOnlyList<VideoItem> Items => items;

        public static void Add(VideoItem item)
        {
            if (item == null || item.Kind != "video")
            {
                return;
            }
            items.Add(item);
        }
    }
}
