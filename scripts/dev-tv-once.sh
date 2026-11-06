#!/usr/bin/env bash
# One-time TV dev setup: connect, print DUID, create Samsung cert profile "omegatube".
# Usage: TV_IP=192.168.x.x scripts/dev-tv-once.sh [author name]
set -euo pipefail
cd "$(dirname "$0")/.."

: "${TV_IP:?set TV_IP to the TV address}"

STUDIO="${TIZEN_STUDIO:-$HOME/tizen-studio}"
SDB="$STUDIO/tools/sdb"
TARGET="$TV_IP:26101"

printf '\nHost PC IP for TV Developer Mode: %s\n' "$(ipconfig getifaddr en0 2>/dev/null || ipconfig getifaddr en1 2>/dev/null || echo '?')"
printf 'TV must have Developer Mode On, Host PC IP set, and a full restart.\n\n'

"$SDB" connect "$TARGET"
DUID=$("$SDB" -s "$TARGET" shell 0 getduid | tr -d '\r\n')
printf 'DUID: %s\n\n' "$DUID"

if [ -f "$HOME/SamsungCertificate/omegatube/author.p12" ]; then
  printf 'Certificate already in ~/SamsungCertificate/omegatube — skip gen-cert or delete to recreate.\n'
else
  scripts/gen-cert.sh "$DUID" "${1:-OmegaTube Dev}"
fi

printf '\nNext: scripts/build-app.sh && TV_IP=%s scripts/deploy.sh\n' "$TV_IP"
