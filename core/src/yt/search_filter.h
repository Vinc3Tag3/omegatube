#pragma once

#include <string>

#include <nlohmann/json_fwd.hpp>

namespace omega::yt {

// InnerTube search `params`: base64(SearchFilter protobuf). Empty when no filters set.
std::string encode_search_filters(const nlohmann::json& filters);

bool search_filter_encode_selfcheck(std::string& error);

}  // namespace omega::yt
