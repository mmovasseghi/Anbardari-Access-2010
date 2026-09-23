#!/usr/bin/env python3
"""Offline unlock code generator (same algorithm as modUnlock.bas)."""
import sys

ALPHABET = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"
SALT = "Anbarban-Offline-2010"


def compute_unlock_code(doc_kind: str, document_id: int, document_number: str) -> str:
    seed = f"{doc_kind.upper().strip()}|{document_id}|{document_number.strip()}|{SALT}"
    h = 5381
    for ch in seed:
        h = ((h * 33) ^ ord(ch)) & 0x7FFFFFFF
    out = []
    for _ in range(6):
        out.append(ALPHABET[h % 32])
        h = (h * 1103515245 + 12345) & 0x7FFFFFFF
    return "".join(out)


def main() -> None:
    if len(sys.argv) != 4:
        print("Usage: generate_unlock_code.py INCOMING|OUTGOING <DocumentID> <DocumentNumber>")
        sys.exit(1)
    print(compute_unlock_code(sys.argv[1], int(sys.argv[2]), sys.argv[3]))


if __name__ == "__main__":
    main()
