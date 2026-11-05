using System.Collections.Generic;
using OmegaTube.Util;

namespace OmegaTube.Data
{
    // Maps to core search filter JSON (contracts/shell-json.md).
    public class SearchFilters
    {
        public string Date = "any";
        public string Duration = "any";
        public string Type = "any";
        public string Sort = "relevance";
        public bool Live;
        public bool Hd;
        public bool Subtitles;

        public bool IsDefault =>
            Date == "any" && Duration == "any" && Type == "any" && Sort == "relevance" && !Live && !Hd && !Subtitles;

        public string ToJson()
        {
            if (IsDefault)
            {
                return null;
            }
            var d = new Dictionary<string, object>
            {
                ["date"] = Date,
                ["duration"] = Duration,
                ["type"] = Type,
                ["sort"] = Sort,
                ["live"] = Live,
                ["hd"] = Hd,
                ["subtitles"] = Subtitles,
            };
            return Json.Write(d);
        }
    }
}
