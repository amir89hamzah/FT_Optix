#!/usr/bin/env python3
"""Basic XML compare helper for FT Optix experiments.

Usage:
    python compare_ftoptix_export.py generated.xml exported_back_from_ftoptix.xml
"""

import sys
import xml.etree.ElementTree as ET
from pathlib import Path

if len(sys.argv) != 3:
    raise SystemExit("Usage: python compare_ftoptix_export.py generated.xml exported.xml")


def count_nodes(path: Path) -> dict[str, int]:
    tree = ET.parse(path)
    counts = {}
    for elem in tree.getroot().iter():
        tag = elem.tag.split("}", 1)[-1]
        counts[tag] = counts.get(tag, 0) + 1
    return counts


a = Path(sys.argv[1])
b = Path(sys.argv[2])
ca = count_nodes(a)
cb = count_nodes(b)
for key in sorted(set(ca) | set(cb)):
    print(f"{key:24s} generated={ca.get(key,0):5d} exported={cb.get(key,0):5d}")
