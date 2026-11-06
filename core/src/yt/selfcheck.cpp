#include "yt/selfcheck.h"

#include "yt/search_filter.h"

#include <cctype>
#include <cstdlib>

namespace omega::yt {

bool resume_watch_eligible(int64_t pos_ms, int64_t dur_ms) {
    if (dur_ms <= 0) return false;
    if (pos_ms <= 10'000) return false;
    if (pos_ms >= dur_ms - 30'000) return false;
    return true;
}

static bool parse_time_part(const char* p, int* h, int* m, int* s) {
    *h = *m = *s = 0;
    int a = 0, b = 0, c = 0;
    int n = 0;
    if (std::isdigit(static_cast<unsigned char>(p[0]))) {
        while (std::isdigit(static_cast<unsigned char>(*p))) {
            a = a * 10 + (*p - '0');
            ++p;
        }
        if (*p == ':') {
            ++p;
            while (std::isdigit(static_cast<unsigned char>(*p))) {
                b = b * 10 + (*p - '0');
                ++p;
            }
            if (*p == ':') {
                ++p;
                while (std::isdigit(static_cast<unsigned char>(*p))) {
                    c = c * 10 + (*p - '0');
                    ++p;
                }
                *h = a;
                *m = b;
                *s = c;
                n = 3;
            } else {
                *m = a;
                *s = b;
                n = 2;
            }
        }
    }
    return n >= 2;
}

bool parse_description_timestamp_ms(const std::string& text, int64_t* out_ms) {
    if (!out_ms) return false;
    for (size_t i = 0; i + 2 < text.size(); ++i) {
        if (!std::isdigit(static_cast<unsigned char>(text[i]))) continue;
        int h = 0, m = 0, s = 0;
        if (!parse_time_part(text.c_str() + i, &h, &m, &s)) continue;
        *out_ms = (static_cast<int64_t>(h) * 3600 + m * 60 + s) * 1000;
        return true;
    }
    return false;
}

bool run_selfcheck(std::string& error) {
    if (!resume_watch_eligible(15'000, 120'000)) {
        error = "resume_watch_eligible positive";
        return false;
    }
    if (resume_watch_eligible(5'000, 120'000)) {
        error = "resume_watch_eligible too early";
        return false;
    }
    if (resume_watch_eligible(100'000, 120'000)) {
        error = "resume_watch_eligible too late";
        return false;
    }
    int64_t ms = 0;
    if (!parse_description_timestamp_ms("Intro at 1:02:03 then outro 12:34", &ms) || ms != 3723'000) {
        error = "parse_description_timestamp_ms";
        return false;
    }
    if (!search_filter_encode_selfcheck(error)) return false;
    return true;
}

}  // namespace omega::yt
