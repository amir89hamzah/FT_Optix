# Git Commands

Clone the public repo:

```bash
git clone https://github.com/amir89hamzah/FT_Optix.git
cd FT_Optix
```

Generate XML probes locally:

```bash
python 05_scripts/generate_probes.py
```

Validate duplicate NodeId values:

```bash
python 05_scripts/validate_nodes.py 04_generated/AI_ModelProbe_01.xml
```

Compare generated XML with FT Optix exported-back XML:

```bash
python 05_scripts/compare_ftoptix_export.py generated.xml exported_back_from_ftoptix.xml
```

Normal manual Git workflow:

```bash
git status
git add .
git commit -m "Add FT Optix XML probe result"
git push
```
