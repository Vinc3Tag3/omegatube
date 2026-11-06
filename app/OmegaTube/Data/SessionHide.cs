using System.Collections.Generic;

namespace OmegaTube.Data
{
    // In-memory hides for "Not interested" / "Don't recommend channel" until app restart.
    public static class SessionHide
    {
        private static readonly HashSet<string> hiddenVideos = new HashSet<string>();
        private static readonly HashSet<string> hiddenChannels = new HashSet<string>();

        public static void HideVideo(string videoId)
        {
            if (!string.IsNullOrEmpty(videoId))
            {
                hiddenVideos.Add(videoId);
            }
        }

        public static void HideChannel(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName))
            {
                hiddenChannels.Add(channelName);
            }
        }

        public static bool IsHidden(VideoItem item)
        {
            if (item == null)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(item.Id) && hiddenVideos.Contains(item.Id))
            {
                return true;
            }
            return !string.IsNullOrEmpty(item.Channel) && hiddenChannels.Contains(item.Channel);
        }

        public static List<VideoItem> Filter(List<VideoItem> list)
        {
            if (list == null || list.Count == 0)
            {
                return list;
            }
            var outList = new List<VideoItem>(list.Count);
            foreach (VideoItem item in list)
            {
                if (!IsHidden(item))
                {
                    outList.Add(item);
                }
            }
            return outList;
        }
    }
}
