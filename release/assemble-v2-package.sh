#!/bin/bash
# Build Anbarban v2.0 release zip (run from repo root)
set -e
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$ROOT/release/Anbarban-v2.0-Access-2010"
ZIP="$ROOT/release/Anbarban-v2.0-Access-2010.zip"

rm -rf "$OUT"
mkdir -p "$OUT/database"

copy_tree() {
  local src="$1" dst="$2"
  if [ -d "$ROOT/$src" ]; then
    mkdir -p "$OUT/$dst"
    cp -a "$ROOT/$src/." "$OUT/$dst/"
  fi
}

copy_tree build build
copy_tree docs docs
copy_tree forms forms
copy_tree queries queries
copy_tree sql sql
copy_tree vba vba
copy_tree tools tools

cp "$ROOT/release/راهنما-انباربان.txt" "$OUT/راهنما-انباربان.txt"
cp "$ROOT/release/شروع-ساخت-دیتابیس.bat" "$OUT/شروع-ساخت-دیتابیس.bat"
cp "$ROOT/release/RELEASE_NOTES_v2.0.md" "$OUT/RELEASE_NOTES_v2.0.md"
cp "$ROOT/README.md" "$OUT/README.md"
cp "$ROOT/database/README.md" "$OUT/database/README.md" 2>/dev/null || true

cat > "$OUT/VERSION.txt" <<EOF
Anbarban (انباربان) v2.0.0
Target: Microsoft Access 2010 — Windows offline
Build date: $(date -u +%Y-%m-%d)
Git: $(git -C "$ROOT" rev-parse --short HEAD 2>/dev/null || echo unknown)
EOF

rm -f "$ZIP"
(cd "$ROOT/release" && zip -r -q "Anbarban-v2.0-Access-2010.zip" "Anbarban-v2.0-Access-2010")
echo "Created: $ZIP"
du -h "$ZIP"
