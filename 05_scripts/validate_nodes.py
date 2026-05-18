#!/usr/bin/env python3
"""Quick check for duplicated NodeId values in an FT Optix XML file."""

import sys
import xml.etree.ElementTree as ET
from collections import Counter
from pathlib import Path

if len(sys.argv) != 2:
    raise SystemExit("Usage: python validate_nodes.py file.xml")

path = Path(sys.argv[1])
tree = ET.parse(path)
ids = []
for elem in tree.getroot().iter():
    node_id = elem.attrib.get("NodeId")
    if node_id:
        ids.append(node_id)

dupes = [node_id for node_id, count in Counter(ids).items() if count > 1]
print(f"NodeId count: {len(ids)}")
print(f"Duplicate NodeId count: {len(dupes)}")
for item in dupes[:20]:
    print("DUPLICATE", item)
