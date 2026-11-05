#pragma once

#include <cstdint>
#include <string>

namespace omega::yt {

// Resume shelf: started (>10s) and not in last 30s of duration.
bool resume_watch_eligible(int64_t pos_ms, int64_t dur_ms);

// First h:mm:ss or m:ss timestamp in description text; false if none.
bool parse_description_timestamp_ms(const std::string& text, int64_t* out_ms);

// Search filter protobuf encoder sanity (expanded in later tasks).
bool search_filter_encode_selfcheck(std::string& error);

bool run_selfcheck(std::string& error);

}  // namespace omega::yt
