# FT_Optix - LLM-to-FT-Optix HMI Build Path

Public learning and implementation repo for building runnable FactoryTalk Optix HMI projects from natural-language intent and hardened JSON specifications using LLMs.

This repo is not only an XML experiment log. Its purpose is to teach humans and LLMs the proven FactoryTalk Optix patterns needed to build an HMI without guessing unsupported APIs.

## Core idea

```text
User gives HMI intent
→ LLM reads this repo when it does not know the FT Optix pattern
→ LLM follows proven JSON / NetLogic / DynamicLink rules
→ LLM generates or updates the FT Optix project assets
→ HMI can be opened, run, and tested in FT Optix
```

Example target workflow:

```text
User:
"Build an HMI for 2 pumps with speed, setpoint, running status, commands, alarms, and trends."

LLM:
1. Reads the repo status and build guide.
2. Converts the request into a hardened JSON spec.
3. Validates the JSON against the documented rules.
4. Generates FT Optix Model nodes using DesignTime NetLogic.
5. Creates DynamicLinks using tested FT Optix API patterns.
6. Connects PLC-backed variables to FT Echo or a real controller once live-tag binding is proven.
7. Generates screens, controls, alarms, and trends only after the required lower-level proofs pass.
```

## Why this repo exists

FactoryTalk Optix project generation has details that are easy for an LLM to hallucinate:

- XML import limitations.
- Model-folder structure.
- OPC UA / FT Optix node patterns.
- DesignTime NetLogic object creation.
- DynamicLink creation APIs.
- PLC tag versus mock/static data semantics.

This repo records what has actually been tested, what compiled, what failed, and what result was observed in FT Optix.

## Current verified path

The current proven path is:

```text
Hardened JSON
→ DesignTime NetLogic C# generator
→ FT Optix Model objects and variables
→ _LocalSources for plcTag-intent simulation
→ local DynamicLinks
→ runtime/emulator value update
```

Milestone 06 is tested and passed.

Confirmed runtime proof:

```text
Label linked to:
Model/AI_JsonDynamicLinkProbe_01/PumpA/CurrentSpeed

Initial display: 12.3
After local source change: 45.6
```

## Current priority

```text
Milestone 07 / Probe 05D
FT Echo or dummy PLC live tag verification
```

Milestone 07 is the next checkpoint because the repo must prove that `source.kind = plcTag` can connect to live or dummy controller tags, not only to local `_LocalSources`.

FT Echo is not required for the completed Milestone 06 local DynamicLink proof. FT Echo becomes required when running Milestone 07 live tag verification.

## Proven checkpoints

| Checkpoint | Purpose | Status |
|---|---|---|
| Probe 05A | Harden JSON schema so tag-backed variables are not ambiguous | PASS |
| Probe 05B | Generate FT Optix Model nodes from hardened JSON and preserve source metadata | PASS |
| Probe 05C | Discover and prove local DynamicLink creation pattern | PASS |
| Milestone 06 | Combine JSON → Model → `_LocalSources` → local DynamicLinks → runtime update | PASS |
| Probe 05D / Milestone 07 | Verify FT Echo or dummy PLC live tag binding | PLANNED |

## LLM operating rule

An LLM using this repo should not invent FT Optix behavior. It should:

1. Read the current status.
2. Use only patterns that are recorded as tested.
3. Treat untested patterns as probes.
4. Record compile/runtime results.
5. Move to UI generation, alarms, trends, and full HMI generation only after the lower-level Model, DynamicLink, and live tag paths are proven.

## Important DynamicLink finding

Known working DesignTime NetLogic API from Probe 05C:

```csharp
linkedSpeed.SetDynamicLink(sourceSpeed);
```

Known not compiling in the current tested context:

```csharp
linkedSpeed.SetDynamicLink(sourceSpeed, DynamicLinkMode.ReadWrite);
```

## Repository structure

```text
FT_Optix/
├─ 00_notes/        Notes, rules, observations
├─ 02_probes/       Small proof probes with objective, steps, and result
├─ 03_templates/    Reusable pattern notes
├─ 04_generated/    Generated output created locally by scripts
├─ 05_scripts/      Generator and validation scripts
├─ 06_specs/        JSON specs and examples
├─ 09_netlogic_probes/
│                   FT Optix DesignTime NetLogic probe code
├─ 10_milestones/   Larger combined checkpoints
├─ PROJECT_VISION.md
├─ LLM_BUILD_GUIDE.md
└─ NEXT_STEPS.md
```

## Recommended reading order for an LLM

1. `PROJECT_VISION.md`
2. `LLM_BUILD_GUIDE.md`
3. `NEXT_STEPS.md`
4. `10_milestones/milestone_06_json_model_dynamiclinks/README.md`
5. `02_probes/probe_05a_json_schema_hardening/README.md`
6. `02_probes/probe_05b_tag_metadata_generator/README.md`
7. `02_probes/probe_05c_dynamiclink_pattern_discovery/README.md`
8. `02_probes/probe_05d_ft_echo_live_tag_verification/README.md`

## Public scope

This repo is intended to be public and educational.

Do not commit:

- passwords
- API keys
- real customer project files
- internal PLC/HMI backups
- proprietary plant screenshots
- files containing real usernames, emails, IP addresses, or production paths

Use sanitized probes, dummy PLC tags, and generated examples first.
