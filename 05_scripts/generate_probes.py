#!/usr/bin/env python3
"""Generate FT Optix XML probe files.

These probes are intentionally Model-folder first.
Tested manually with FT Optix Studio v1.7.2.51.
"""

from __future__ import annotations

import uuid
from pathlib import Path
from xml.etree.ElementTree import Element, SubElement, ElementTree, register_namespace, indent

NS_UA = "http://opcfoundation.org/UA/2011/03/UANodeSet.xsd"
NS_UAX = "http://opcfoundation.org/UA/2008/02/Types.xsd"
MODEL_URI = "AI_Generated_FTOptix_Test"

register_namespace("", NS_UA)
register_namespace("uax", NS_UAX)

DATA_TYPES = {
    "Boolean": ("i=1", "Boolean"),
    "Int32": ("i=6", "Int32"),
    "UInt32": ("i=7", "UInt32"),
    "Float": ("i=10", "Float"),
    "Double": ("i=11", "Double"),
    "String": ("i=12", "String"),
    "DateTime": ("i=294", "DateTime"),
}

BASE_OBJECT_TYPE = "i=58"
FOLDER_TYPE = "i=61"
BASE_DATA_VARIABLE_TYPE = "i=63"
HAS_TYPE_DEFINITION = "i=40"
HAS_SUBTYPE = "i=45"
HAS_COMPONENT = "i=47"
HAS_MODELLING_RULE = "i=37"
MODELLING_RULE_MANDATORY = "i=78"


def q(tag: str, ns: str = NS_UA) -> str:
    return f"{{{ns}}}{tag}"


def uax(tag: str) -> str:
    return f"{{{NS_UAX}}}{tag}"


class ProbeBuilder:
    def __init__(self, probe_name: str):
        self.probe_name = probe_name
        self.root = Element(q("UANodeSet"))
        namespace_uris = SubElement(self.root, q("NamespaceUris"))
        SubElement(namespace_uris, q("Uri")).text = MODEL_URI
        models = SubElement(self.root, q("Models"))
        model = SubElement(models, q("Model"), {"ModelUri": MODEL_URI})
        SubElement(model, q("RequiredModel"), {"ModelUri": "http://opcfoundation.org/UA/"})
        SubElement(self.root, q("Aliases"))

    def nodeid(self, path: str) -> str:
        return "ns=1;g=" + str(uuid.uuid5(uuid.NAMESPACE_URL, MODEL_URI + ":" + path))

    def _refs(self, parent, type_def: str, parent_path: str | None = None):
        refs = SubElement(parent, q("References"))
        SubElement(refs, q("Reference"), {"ReferenceType": HAS_TYPE_DEFINITION}).text = type_def
        if parent_path:
            SubElement(refs, q("Reference"), {"ReferenceType": HAS_COMPONENT, "IsForward": "false"}).text = self.nodeid(parent_path)
        return refs

    def _write_value(self, var, dtype: str, value):
        val_el = SubElement(var, q("Value"))
        child = SubElement(val_el, uax(DATA_TYPES[dtype][1]))
        if dtype == "Boolean":
            child.text = "true" if bool(value) else "false"
        elif dtype in {"Float", "Double"}:
            child.text = f"{float(value):.6g}"
        elif dtype in {"Int32", "UInt32"}:
            child.text = str(int(value))
        else:
            child.text = str(value)

    def add_folder(self, path: str, desc: str | None = None):
        parent_path = "/".join(path.split("/")[:-1]) if "/" in path else None
        attrs = {"NodeId": self.nodeid(path), "BrowseName": "1:" + path.split("/")[-1], "WriteMask": "69876"}
        if parent_path:
            attrs["ParentNodeId"] = self.nodeid(parent_path)
        obj = SubElement(self.root, q("UAObject"), attrs)
        SubElement(obj, q("DisplayName")).text = path.split("/")[-1]
        if desc:
            SubElement(obj, q("Description")).text = desc
        self._refs(obj, FOLDER_TYPE, parent_path)

    def add_object(self, path: str, type_def: str = BASE_OBJECT_TYPE, desc: str | None = None):
        """Add a plain UAObject instance.

        If type_def is a custom NodeId, the object becomes an instance of that object type.
        """
        parent_path = "/".join(path.split("/")[:-1]) if "/" in path else None
        attrs = {"NodeId": self.nodeid(path), "BrowseName": "1:" + path.split("/")[-1], "WriteMask": "69876"}
        if parent_path:
            attrs["ParentNodeId"] = self.nodeid(parent_path)
        obj = SubElement(self.root, q("UAObject"), attrs)
        SubElement(obj, q("DisplayName")).text = path.split("/")[-1]
        if desc:
            SubElement(obj, q("Description")).text = desc
        self._refs(obj, type_def, parent_path)

    def add_object_type(self, type_name: str, desc: str | None = None) -> str:
        """Add a minimal custom UAObjectType under BaseObjectType.

        The returned NodeId can be used as the HasTypeDefinition target for instances.
        """
        type_path = f"ObjectTypes/{type_name}"
        obj_type = SubElement(
            self.root,
            q("UAObjectType"),
            {
                "NodeId": self.nodeid(type_path),
                "BrowseName": "1:" + type_name,
                "WriteMask": "0",
                "IsAbstract": "false",
            },
        )
        SubElement(obj_type, q("DisplayName")).text = type_name
        if desc:
            SubElement(obj_type, q("Description")).text = desc
        refs = SubElement(obj_type, q("References"))
        SubElement(refs, q("Reference"), {"ReferenceType": HAS_SUBTYPE, "IsForward": "false"}).text = BASE_OBJECT_TYPE
        return self.nodeid(type_path)

    def add_type_var(self, type_name: str, name: str, dtype: str, value=None, desc: str | None = None):
        """Add a variable declaration below a custom object type.

        FT Optix import behavior for this pattern is intentionally being probed.
        """
        parent_path = f"ObjectTypes/{type_name}"
        path = f"{parent_path}/{name}"
        data_id, _ = DATA_TYPES[dtype]
        var = SubElement(
            self.root,
            q("UAVariable"),
            {
                "NodeId": self.nodeid(path),
                "BrowseName": "1:" + name,
                "ParentNodeId": self.nodeid(parent_path),
                "WriteMask": "594038",
                "DataType": data_id,
                "AccessLevel": "3",
                "UserAccessLevel": "3",
            },
        )
        SubElement(var, q("DisplayName")).text = name
        if desc:
            SubElement(var, q("Description")).text = desc
        refs = self._refs(var, BASE_DATA_VARIABLE_TYPE, parent_path)
        SubElement(refs, q("Reference"), {"ReferenceType": HAS_MODELLING_RULE}).text = MODELLING_RULE_MANDATORY
        if value is not None:
            self._write_value(var, dtype, value)

    def add_var(self, path: str, dtype: str, value=None, desc: str | None = None):
        parent_path = "/".join(path.split("/")[:-1]) if "/" in path else None
        data_id, _ = DATA_TYPES[dtype]
        attrs = {
            "NodeId": self.nodeid(path),
            "BrowseName": "1:" + path.split("/")[-1],
            "WriteMask": "594038",
            "DataType": data_id,
            "AccessLevel": "3",
            "UserAccessLevel": "3",
        }
        if parent_path:
            attrs["ParentNodeId"] = self.nodeid(parent_path)
        var = SubElement(self.root, q("UAVariable"), attrs)
        SubElement(var, q("DisplayName")).text = path.split("/")[-1]
        if desc:
            SubElement(var, q("Description")).text = desc
        self._refs(var, BASE_DATA_VARIABLE_TYPE, parent_path)
        if value is not None:
            self._write_value(var, dtype, value)

    def write(self, path: Path):
        path.parent.mkdir(parents=True, exist_ok=True)
        indent(self.root, space="  ")
        ElementTree(self.root).write(path, encoding="UTF-8", xml_declaration=True)
        print(path)


def probe_01(out_dir: Path):
    p = ProbeBuilder("AI_ModelProbe_01")
    root = "AI_ModelProbe_01"
    p.add_folder(root, "Basic generated XML probe for FT Optix Model import.")
    p.add_var(f"{root}/StatusText", "String", "Hello from generated XML")
    p.add_var(f"{root}/TestNumber", "Float", 123.45)
    p.add_var(f"{root}/TestBool", "Boolean", True)
    p.add_var(f"{root}/Notes", "String", "Generated by script. Import under Model folder.")
    p.write(out_dir / "AI_ModelProbe_01.xml")


def probe_02(out_dir: Path):
    p = ProbeBuilder("AI_ModelProbe_02")
    root = "AI_ModelProbe_02"
    p.add_folder(root, "Nested model structure for future FT Optix UI bindings.")
    p.add_var(f"{root}/ImportStatus", "String", "AI_ModelProbe_02 imported successfully")
    p.add_var(f"{root}/ProbeVersion", "String", "02 - nested model and process values")
    p.add_folder(f"{root}/Navigation")
    p.add_var(f"{root}/Navigation/SelectedPage", "String", "Dashboard")
    p.add_var(f"{root}/Navigation/MenuIndex", "UInt32", 0)
    p.add_folder(f"{root}/Dashboard")
    for name, value in {
        "MainThruster": 10.0,
        "LateralThrusters": 15.0,
        "Oxygen": 5.0,
        "WaterTreatment": 15.0,
        "Drilling": 5.0,
        "Stabilization": 10.0,
    }.items():
        p.add_var(f"{root}/Dashboard/{name}", "Float", value)
    p.add_var(f"{root}/Dashboard/LastUpdateUtc", "DateTime", "2026-05-18T09:30:00Z")
    p.add_folder(f"{root}/ProductionLine")
    for name, running, speed, alarms in [
        ("Drilling", True, 75.0, 2),
        ("Slurry", True, 68.5, 0),
        ("Coating", False, 0.0, 1),
        ("Drying", True, 82.0, 0),
        ("Calendaring", True, 60.0, 0),
        ("Slitting", False, 0.0, 3),
    ]:
        base = f"{root}/ProductionLine/{name}"
        p.add_folder(base)
        p.add_var(base + "/StageName", "String", name)
        p.add_var(base + "/Running", "Boolean", running)
        p.add_var(base + "/SpeedPercent", "Float", speed)
        p.add_var(base + "/ActiveAlarmCount", "Int32", alarms)
    p.add_folder(f"{root}/AlarmSummary")
    p.add_var(f"{root}/AlarmSummary/ActiveCount", "Int32", 6)
    p.add_var(f"{root}/AlarmSummary/AckedCount", "Int32", 3)
    p.add_var(f"{root}/AlarmSummary/HighestSeverity", "UInt32", 700)
    p.add_var(f"{root}/AlarmSummary/LastMessage", "String", "Generated test alarm message")
    p.write(out_dir / "AI_ModelProbe_02_ProcessModel.xml")


def probe_03(out_dir: Path):
    p = ProbeBuilder("AI_ModelProbe_03")
    root = "AI_ModelProbe_03"
    p.add_folder(root, "Dashboard-ready runtime model data, repeated objects, alarm rows, and page description paths.")
    p.add_var(f"{root}/ImportStatus", "String", "AI_ModelProbe_03 imported successfully")
    p.add_var(f"{root}/ProbeVersion", "String", "03 - dashboard runtime data model")
    p.add_var(f"{root}/GeneratedFor", "String", "FT Optix v1.7.2.51 / import under Model folder")
    p.add_folder(f"{root}/PageDescriptions")
    for page in ["Dashboard", "Drilling", "Slurry", "Coating", "Drying", "Calendaring", "Slitting"]:
        p.add_var(f"{root}/PageDescriptions/{page}HtmlPath", "String", f"%PROJECTDIR%/Descriptions/{page}.html")
    p.add_folder(f"{root}/Dashboard")
    p.add_folder(f"{root}/Dashboard/EnergyConsumption")
    energy = {"MainThruster": 10.0, "LateralThrusters": 15.0, "Oxygen": 5.0, "WaterTreatment": 15.0, "Drilling": 5.0, "Stabilization": 10.0}
    for name, val in energy.items():
        base = f"{root}/Dashboard/EnergyConsumption/{name}"
        p.add_folder(base)
        p.add_var(base + "/Value", "Float", val)
        p.add_var(base + "/Min", "Float", 0)
        p.add_var(base + "/Max", "Float", 20)
        p.add_var(base + "/Unit", "String", "kWh")
    p.add_folder(f"{root}/ProductionLine")
    for name, running, speed, alarms in [("Drilling", True, 72.5, 2), ("Slurry", True, 64, 0), ("Coating", False, 0, 1), ("Drying", True, 88.2, 0), ("Calendaring", True, 55.7, 3), ("Slitting", True, 69.9, 0)]:
        base = f"{root}/ProductionLine/{name}"
        p.add_folder(base)
        p.add_var(base + "/StageName", "String", name)
        p.add_var(base + "/Running", "Boolean", running)
        p.add_var(base + "/SpeedPercent", "Float", speed)
        p.add_var(base + "/ActiveAlarmCount", "UInt32", alarms)
    p.add_folder(f"{root}/AlarmTable")
    for i, row in enumerate([
        ("2026-05-18 09:00:00", "Drilling.SpeedPercent", "High speed warning", True, False, False, 500),
        ("2026-05-18 09:05:00", "Calendaring.ActiveAlarmCount", "Alarm count above expected value", True, False, False, 700),
        ("2026-05-18 09:10:00", "Coating.Running", "Coating section stopped", True, True, False, 600),
    ], 1):
        base = f"{root}/AlarmTable/Row{i:02d}"
        p.add_folder(base)
        ts, var, msg, active, acked, confirmed, sev = row
        p.add_var(base + "/Timestamp", "String", ts)
        p.add_var(base + "/Variable", "String", var)
        p.add_var(base + "/Message", "String", msg)
        p.add_var(base + "/Active", "Boolean", active)
        p.add_var(base + "/Acked", "Boolean", acked)
        p.add_var(base + "/Confirmed", "Boolean", confirmed)
        p.add_var(base + "/Severity", "UInt32", sev)
    p.write(out_dir / "AI_ModelProbe_03_DashboardRuntimeData.xml")


def probe_04(out_dir: Path):
    """Probe custom object type + object instance.

    Inspired by BoilerDemo's Model/Pumps pattern.
    This deliberately tests whether FT Optix accepts a minimal UAObjectType and
    an instance of that type through NodeSet import under Model.
    """
    p = ProbeBuilder("AI_ModelProbe_04")
    root = "AI_ModelProbe_04"

    pump_type_id = p.add_object_type(
        "AI_MyPump",
        "Minimal generated pump object type for FT Optix import probing.",
    )
    for name, dtype, value, desc in [
        ("SetSpeed", "Float", 0.0, "Requested pump speed percentage."),
        ("CurrentSpeed", "Float", 0.0, "Measured pump speed percentage."),
        ("Command", "String", "Stop", "Simple command text for probe testing."),
        ("Alarm", "Boolean", False, "Pump alarm state."),
        ("MinSpeed", "Float", 0.0, "Minimum allowed speed."),
        ("MaxSpeed", "Float", 100.0, "Maximum allowed speed."),
        ("UseRunFeedback", "Boolean", True, "Whether run feedback is expected."),
    ]:
        p.add_type_var("AI_MyPump", name, dtype, value, desc)

    p.add_folder(root, "Object type and instance probe based on the BoilerDemo pump pattern.")
    p.add_var(f"{root}/ImportStatus", "String", "AI_ModelProbe_04 imported successfully")
    p.add_var(f"{root}/ProbeVersion", "String", "04 - model object type and instance")
    p.add_folder(f"{root}/Pumps")

    for pump_name, set_speed, current_speed, command, alarm in [
        ("Pump1", 50.0, 47.5, "RunCw", False),
        ("Pump2", 35.0, 0.0, "Stop", True),
    ]:
        base = f"{root}/Pumps/{pump_name}"
        p.add_object(base, pump_type_id, f"{pump_name} instance of generated AI_MyPump object type.")
        p.add_var(base + "/SetSpeed", "Float", set_speed)
        p.add_var(base + "/CurrentSpeed", "Float", current_speed)
        p.add_var(base + "/Command", "String", command)
        p.add_var(base + "/Alarm", "Boolean", alarm)
        p.add_var(base + "/MinSpeed", "Float", 0.0)
        p.add_var(base + "/MaxSpeed", "Float", 100.0)
        p.add_var(base + "/UseRunFeedback", "Boolean", True)

    p.write(out_dir / "AI_ModelProbe_04_PumpObjectType.xml")


if __name__ == "__main__":
    output = Path("04_generated")
    probe_01(output)
    probe_02(output)
    probe_03(output)
    probe_04(output)
