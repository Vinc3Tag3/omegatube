#include "yt/search_filter.h"

#include <nlohmann/json.hpp>

namespace omega::yt {
namespace {

using json = nlohmann::json;

void pb_varint(std::string& out, uint64_t v) {
    while (v >= 0x80) {
        out.push_back(static_cast<char>((v & 0x7f) | 0x80));
        v >>= 7;
    }
    out.push_back(static_cast<char>(v));
}

void pb_key(std::string& out, int field, int wire) { pb_varint(out, static_cast<uint64_t>((field << 3) | wire)); }

void pb_int(std::string& out, int field, int value) {
    pb_key(out, field, 0);
    pb_varint(out, static_cast<uint64_t>(value));
}

void pb_bool(std::string& out, int field, bool on) {
    if (on) pb_int(out, field, 1);
}

void pb_embedded(std::string& out, int field, const std::string& msg) {
    pb_key(out, field, 2);
    pb_varint(out, msg.size());
    out += msg;
}

std::string b64_encode(const std::string& in) {
    static const char* k =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    std::string out;
    out.reserve((in.size() + 2) / 3 * 4);
    for (size_t i = 0; i < in.size(); i += 3) {
        uint32_t n = static_cast<unsigned char>(in[i]) << 16;
        if (i + 1 < in.size()) n |= static_cast<unsigned char>(in[i + 1]) << 8;
        if (i + 2 < in.size()) n |= static_cast<unsigned char>(in[i + 2]);
        out.push_back(k[(n >> 18) & 63]);
        out.push_back(k[(n >> 12) & 63]);
        out.push_back(i + 1 < in.size() ? k[(n >> 6) & 63] : '=');
        out.push_back(i + 2 < in.size() ? k[n & 63] : '=');
    }
    return out;
}

std::string str_field(const json& j, const char* key) {
    if (!j.contains(key) || !j[key].is_string()) return "any";
    return j[key].get<std::string>();
}

}  // namespace

std::string encode_search_filters(const json& filters) {
    if (!filters.is_object()) return "";
    std::string inner;
    std::string date = str_field(filters, "date");
    if (date == "hour" || date == "today") pb_int(inner, 1, 2);
    else if (date == "week") pb_int(inner, 1, 3);
    else if (date == "month") pb_int(inner, 1, 4);
    else if (date == "year") pb_int(inner, 1, 5);

    std::string type = str_field(filters, "type");
    if (type == "video") pb_int(inner, 2, 1);
    else if (type == "channel") pb_int(inner, 2, 2);
    else if (type == "playlist") pb_int(inner, 2, 3);

    std::string duration = str_field(filters, "duration");
    if (duration == "short") pb_int(inner, 3, 4);
    else if (duration == "medium") pb_int(inner, 3, 5);
    else if (duration == "long") pb_int(inner, 3, 2);

    if (filters.value("hd", false)) pb_bool(inner, 4, true);
    if (filters.value("subtitles", false)) pb_bool(inner, 5, true);
    if (filters.value("live", false)) pb_bool(inner, 8, true);

    std::string root;
    std::string sort = str_field(filters, "sort");
    if (sort == "views") pb_int(root, 1, 3);

    if (inner.empty() && root.empty()) return "";
    if (!inner.empty()) pb_embedded(root, 2, inner);
    return b64_encode(root);
}

bool search_filter_encode_selfcheck(std::string& error) {
    json f = {{"duration", "long"}};
    std::string params = encode_search_filters(f);
    if (params.empty()) {
        error = "encode_search_filters empty for duration long";
        return false;
    }
    // misc.Filters field 3 (duration) = OVER_TWENTY_MINS (2) -> tag 0x18 value 2
    std::string raw;
    // minimal decode: we only need to spot 18 02 inside payload
    const std::string& b64 = params;
    auto dec = [&](char c) -> int {
        if (c >= 'A' && c <= 'Z') return c - 'A';
        if (c >= 'a' && c <= 'z') return c - 'a' + 26;
        if (c >= '0' && c <= '9') return c - '0' + 52;
        if (c == '+') return 62;
        if (c == '/') return 63;
        return -1;
    };
    for (size_t i = 0; i + 3 < b64.size(); i += 4) {
        int a = dec(b64[i]), b = dec(b64[i + 1]), c = b64[i + 2] == '=' ? -1 : dec(b64[i + 2]),
            d = b64[i + 3] == '=' ? -1 : dec(b64[i + 3]);
        if (a < 0 || b < 0) break;
        raw.push_back(static_cast<char>((a << 2) | (b >> 4)));
        if (c >= 0) raw.push_back(static_cast<char>(((b & 15) << 4) | (c >> 2)));
        if (d >= 0) raw.push_back(static_cast<char>(((c & 3) << 6) | d));
    }
    if (raw.find("\x18\x02") == std::string::npos) {
        error = "duration long missing proto enum 2";
        return false;
    }
    return true;
}

}  // namespace omega::yt
