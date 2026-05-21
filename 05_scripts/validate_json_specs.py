#!/usr/bin/env python3
"""Validate FT Optix AI JSON specs.

Probe 05A intentionally avoids external dependencies so the validator can run on
basic Python installs. It performs the semantic checks that matter most before a
DesignTime NetLogic generator creates FT Optix nodes.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path
from typing import Any

NAME_RE = re.compile(r"^[A-Za-z_][A-Za-z0-9_]*$")
DATA_TYPES = {"Boolean", "Int32", "UInt32", "Float", "Double", "String", "DateTime"}
ROLES = {"measured", "setpoint", "command", "status", "config", "diagnostic"}
SOURCE_KINDS = {"plcTag", "mock", "static"}
SOURCE_MODES = {"read", "write", "readWrite"}


class ValidationError(Exception):
    pass


def fail(errors: list[str], path: str, message: str) -> None:
    errors.append(f"{path}: {message}")


def require_object(errors: list[str], value: Any, path: str) -> dict[str, Any] | None:
    if not isinstance(value, dict):
        fail(errors, path, "must be an object")
        return None
    return value


def require_name(errors: list[str], value: Any, path: str) -> None:
    if not isinstance(value, str) or not NAME_RE.match(value):
        fail(errors, path, "must be a valid FT Optix-safe name like PumpA or CurrentSpeed")


def check_no_extra(errors: list[str], obj: dict[str, Any], path: str, allowed: set[str]) -> None:
    extra = sorted(set(obj) - allowed)
    if extra:
        fail(errors, path, f"has unsupported properties: {', '.join(extra)}")


def check_value_matches_datatype(errors: list[str], value: Any, data_type: str, path: str) -> None:
    if data_type == "Boolean":
        if not isinstance(value, bool):
            fail(errors, path, "must be boolean for Boolean variable")
    elif data_type in {"Int32", "UInt32"}:
        if not isinstance(value, int) or isinstance(value, bool):
            fail(errors, path, f"must be integer for {data_type} variable")
        elif data_type == "UInt32" and value < 0:
            fail(errors, path, "must be >= 0 for UInt32 variable")
    elif data_type in {"Float", "Double"}:
        if not isinstance(value, (int, float)) or isinstance(value, bool):
            fail(errors, path, f"must be numeric for {data_type} variable")
    elif data_type in {"String", "DateTime"}:
        if not isinstance(value, str):
            fail(errors, path, f"must be string for {data_type} variable")


def validate_display(errors: list[str], display: Any, path: str, role: str) -> None:
    display_obj = require_object(errors, display, path)
    if display_obj is None:
        return
    check_no_extra(errors, display_obj, path, {"unit", "decimals", "min", "max", "format"})

    if "decimals" in display_obj:
        decimals = display_obj["decimals"]
        if not isinstance(decimals, int) or isinstance(decimals, bool) or decimals < 0 or decimals > 6:
            fail(errors, f"{path}.decimals", "must be an integer from 0 to 6")

    for key in ("min", "max"):
        if key in display_obj and (not isinstance(display_obj[key], (int, float)) or isinstance(display_obj[key], bool)):
            fail(errors, f"{path}.{key}", "must be numeric")

    if "min" in display_obj and "max" in display_obj and display_obj["min"] >= display_obj["max"]:
        fail(errors, path, "min must be less than max")

    if role == "setpoint":
        if "min" not in display_obj or "max" not in display_obj:
            fail(errors, path, "setpoint variables must declare display.min and display.max")


def validate_source(errors: list[str], source: Any, path: str, data_type: str, role: str) -> None:
    source_obj = require_object(errors, source, path)
    if source_obj is None:
        return
    kind = source_obj.get("kind")
    if kind not in SOURCE_KINDS:
        fail(errors, f"{path}.kind", "must be one of plcTag, mock, static")
        return

    if kind == "plcTag":
        check_no_extra(errors, source_obj, path, {"kind", "tag", "mode"})
        tag = source_obj.get("tag")
        mode = source_obj.get("mode")
        if not isinstance(tag, str) or not tag.strip():
            fail(errors, f"{path}.tag", "must be a non-empty PLC tag path")
        if mode not in SOURCE_MODES:
            fail(errors, f"{path}.mode", "must be read, write, or readWrite")
        if role in {"measured", "status", "diagnostic"} and mode != "read":
            fail(errors, path, f"role {role} should use source.mode read")
        if role == "setpoint" and mode != "readWrite":
            fail(errors, path, "role setpoint should use source.mode readWrite")
        if role == "command" and mode not in {"write", "readWrite"}:
            fail(errors, path, "role command should use source.mode write or readWrite")
    elif kind == "mock":
        check_no_extra(errors, source_obj, path, {"kind", "value"})
        if "value" not in source_obj:
            fail(errors, path, "mock source must include value")
        else:
            check_value_matches_datatype(errors, source_obj["value"], data_type, f"{path}.value")
    elif kind == "static":
        check_no_extra(errors, source_obj, path, {"kind", "value"})
        if role not in {"config", "diagnostic"}:
            fail(errors, path, "static source is only allowed for config or diagnostic variables")
        if "value" not in source_obj:
            fail(errors, path, "static source must include value")
        else:
            check_value_matches_datatype(errors, source_obj["value"], data_type, f"{path}.value")


def validate_variable(errors: list[str], var: Any, path: str) -> None:
    var_obj = require_object(errors, var, path)
    if var_obj is None:
        return
    check_no_extra(errors, var_obj, path, {"name", "description", "dataType", "role", "source", "display"})

    for key in ("name", "dataType", "role", "source"):
        if key not in var_obj:
            fail(errors, path, f"missing required property {key}")

    if "name" in var_obj:
        require_name(errors, var_obj["name"], f"{path}.name")

    data_type = var_obj.get("dataType")
    role = var_obj.get("role")
    if data_type not in DATA_TYPES:
        fail(errors, f"{path}.dataType", f"must be one of {', '.join(sorted(DATA_TYPES))}")
    if role not in ROLES:
        fail(errors, f"{path}.role", f"must be one of {', '.join(sorted(ROLES))}")

    if data_type in DATA_TYPES and role in ROLES and "source" in var_obj:
        validate_source(errors, var_obj["source"], f"{path}.source", data_type, role)

    if "display" in var_obj and role in ROLES:
        validate_display(errors, var_obj["display"], f"{path}.display", role)
    elif role == "setpoint":
        fail(errors, path, "setpoint variables must include display with min and max")


def validate_spec(spec: Any) -> list[str]:
    errors: list[str] = []
    root = require_object(errors, spec, "$")
    if root is None:
        return errors

    check_no_extra(errors, root, "$", {"specVersion", "probe", "rootName", "description", "objects"})
    if root.get("specVersion") != "0.5a":
        fail(errors, "$.specVersion", "must be 0.5a")
    if root.get("probe") != "Probe 05A - tag-backed variables":
        fail(errors, "$.probe", "must be Probe 05A - tag-backed variables")
    if "rootName" not in root:
        fail(errors, "$", "missing required property rootName")
    else:
        require_name(errors, root["rootName"], "$.rootName")

    objects = root.get("objects")
    if not isinstance(objects, list) or not objects:
        fail(errors, "$.objects", "must be a non-empty array")
        return errors

    seen_objects: set[str] = set()
    for i, obj in enumerate(objects):
        obj_path = f"$.objects[{i}]"
        obj_dict = require_object(errors, obj, obj_path)
        if obj_dict is None:
            continue
        check_no_extra(errors, obj_dict, obj_path, {"name", "description", "variables"})
        name = obj_dict.get("name")
        if name is None:
            fail(errors, obj_path, "missing required property name")
        else:
            require_name(errors, name, f"{obj_path}.name")
            if isinstance(name, str):
                if name in seen_objects:
                    fail(errors, f"{obj_path}.name", "duplicate object name")
                seen_objects.add(name)

        variables = obj_dict.get("variables")
        if not isinstance(variables, list) or not variables:
            fail(errors, f"{obj_path}.variables", "must be a non-empty array")
            continue

        seen_vars: set[str] = set()
        for j, var in enumerate(variables):
            var_path = f"{obj_path}.variables[{j}]"
            if isinstance(var, dict) and isinstance(var.get("name"), str):
                var_name = var["name"]
                if var_name in seen_vars:
                    fail(errors, f"{var_path}.name", "duplicate variable name in object")
                seen_vars.add(var_name)
            validate_variable(errors, var, var_path)

    return errors


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate Probe 05A JSON specs.")
    parser.add_argument("files", nargs="+", type=Path, help="JSON spec files to validate")
    args = parser.parse_args()

    exit_code = 0
    for file_path in args.files:
        try:
            spec = json.loads(file_path.read_text(encoding="utf-8"))
        except Exception as exc:  # noqa: BLE001 - CLI should print any parse/read issue
            print(f"FAIL {file_path}: {exc}")
            exit_code = 1
            continue

        errors = validate_spec(spec)
        if errors:
            print(f"FAIL {file_path}")
            for error in errors:
                print(f"  - {error}")
            exit_code = 1
        else:
            print(f"OK   {file_path}")

    return exit_code


if __name__ == "__main__":
    sys.exit(main())
