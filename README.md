# FT_Optix XML Pattern Lab

Public learning repo for FT Optix XML / OPC UA `UANodeSet` import-export experiments.

This repo started from real FT Optix Studio testing using:

- FT Optix Studio v1.7.2.51
- Import target: `Model` folder
- Test method: generate XML → import into FT Optix → check result → export back → compare

## Goal

The goal is to build an open reference so humans and LLMs can learn how to generate valid FT Optix XML structures.

Current focus:

1. Generate valid Model-folder XML.
2. Understand folders, variables, data types, values, `NodeId`, `ParentNodeId`, `BrowseName`, and references.
3. Build repeatable probes that can be imported into FT Optix.
4. Later: move from Model-only into UI/page binding once the Model pattern is stable.

## Confirmed working so far

| Probe | Description | Import Target | Result |
|---|---|---|---|
| Probe 01 | Basic folder + variables | Model | Success |
| Probe 02 | Nested Model folder structure | Model | Success |
| Probe 03 | Dashboard/runtime data model structure | Model | Pending / next test |

## Important import rule discovered

Current FT Optix import flow only allows XML import under:

```text
Model folder
```

Because of that, this repo starts with Model structures first. UI/page generation will be attempted later.

## Experiment loop

```text
Prompt / idea
   ↓
Generate XML
   ↓
Import into FT Optix Model folder
   ↓
Check screenshot/result
   ↓
Export back from FT Optix
   ↓
Compare generated XML vs FT Optix export
   ↓
Update generator/template
```

## Repository structure

```text
FT_Optix/
├─ 00_notes/        Notes, rules, observations
├─ 02_probes/       Probe descriptions and import results
├─ 03_templates/    Reusable XML pattern notes
├─ 04_generated/    Generated XML output, created locally by scripts
├─ 05_scripts/      Generator and validation scripts
└─ NEXT_STEPS.md    Current work plan
```

## Public scope

This repo is intended to be public and educational.

Do not commit:

- passwords
- API keys
- real customer project files
- internal PLC/HMI backups
- proprietary plant screenshots
- files containing real usernames, emails, IP addresses, or production paths

Use sanitized probes and generated examples first.

## How to generate the probe XML files

Run:

```bash
python 05_scripts/generate_probes.py
```

This creates XML files under:

```text
04_generated/
```

Then import the XML into FT Optix under the `Model` folder.
