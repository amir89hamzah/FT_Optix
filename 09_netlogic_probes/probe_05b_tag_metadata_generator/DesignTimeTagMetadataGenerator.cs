// Probe 05B - Tag source intent metadata generator
//
// Paste this into a FactoryTalk Optix DesignTime NetLogic class.
// It reads a hardened Probe 05A JSON file and creates Model nodes while
// preserving PLC tag intent as helper metadata variables.
//
// This probe intentionally does NOT create real DynamicLinks yet.
// DynamicLink creation is reserved for Probe 05C after the smallest reliable
// FT Optix API pattern is discovered.

#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class DesignTimeTagMetadataGenerator : BaseNetLogic
{
    private const string DefaultJsonPath = @"C:\Temp\ftoptix_probe05b_tag_backed_variables.json";

    [ExportMethod]
    public void GenerateTagMetadataModel()
    {
        if (!File.Exists(DefaultJsonPath))
            throw new FileNotFoundException("Probe 05B JSON file was not found.", DefaultJsonPath);

        var json = File.ReadAllText(DefaultJsonPath);
        var spec = JsonSerializer.Deserialize<TagBackedSpec>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        ValidateSpec(spec);

        var model = Project.Current.Get("Model");
        if (model == null)
            throw new InvalidOperationException("Could not find Project.Current.Get(\"Model\").");

        DeleteChildIfExists(model, spec.RootName);

        var root = InformationModel.MakeObject(spec.RootName);
        model.Add(root);

        if (!string.IsNullOrWhiteSpace(spec.Description))
            AddStringVariable(root, "Description", spec.Description);

        AddStringVariable(root, "Probe", spec.Probe);
        AddStringVariable(root, "SpecVersion", spec.SpecVersion);
        AddStringVariable(root, "GeneratedBy", "Probe 05B DesignTimeTagMetadataGenerator");
        AddStringVariable(root, "GenerationRule", "plcTag sources are metadata only; DynamicLinks are not created in Probe 05B.");

        foreach (var objSpec in spec.Objects)
        {
            var obj = InformationModel.MakeObject(objSpec.Name);
            root.Add(obj);

            if (!string.IsNullOrWhiteSpace(objSpec.Description))
                AddStringVariable(obj, "Description", objSpec.Description);

            foreach (var variable in objSpec.Variables)
                AddVariableWithMetadata(obj, variable);
        }
    }

    private static void AddVariableWithMetadata(IUANode parent, VariableSpec variable)
    {
        // Main variable.
        // For plcTag sources, no runtime value is written because the value should
        // eventually come from a real tag / DynamicLink.
        // For mock/static sources, writing a value is acceptable because the JSON
        // explicitly states the value's purpose.
        var main = MakeVariable(variable.Name, variable.DataType);
        parent.Add(main);

        if (variable.Source.Kind == "mock" || variable.Source.Kind == "static")
            SetVariableValue(main, variable.DataType, variable.Source.Value);

        AddStringVariable(parent, variable.Name + "_Role", variable.Role);
        AddStringVariable(parent, variable.Name + "_SourceKind", variable.Source.Kind);

        if (!string.IsNullOrWhiteSpace(variable.Source.Tag))
            AddStringVariable(parent, variable.Name + "_SourceTag", variable.Source.Tag);

        if (!string.IsNullOrWhiteSpace(variable.Source.Mode))
            AddStringVariable(parent, variable.Name + "_SourceMode", variable.Source.Mode);

        if (!string.IsNullOrWhiteSpace(variable.Description))
            AddStringVariable(parent, variable.Name + "_Description", variable.Description);

        if (variable.Display != null)
        {
            if (!string.IsNullOrWhiteSpace(variable.Display.Unit))
                AddStringVariable(parent, variable.Name + "_DisplayUnit", variable.Display.Unit);

            if (variable.Display.Decimals.HasValue)
                AddUInt32Variable(parent, variable.Name + "_DisplayDecimals", (uint)variable.Display.Decimals.Value);

            if (variable.Display.Min.HasValue)
                AddDoubleVariable(parent, variable.Name + "_DisplayMin", variable.Display.Min.Value);

            if (variable.Display.Max.HasValue)
                AddDoubleVariable(parent, variable.Name + "_DisplayMax", variable.Display.Max.Value);

            if (!string.IsNullOrWhiteSpace(variable.Display.Format))
                AddStringVariable(parent, variable.Name + "_DisplayFormat", variable.Display.Format);
        }
    }

    private static IUAVariable MakeVariable(string name, string dataType)
    {
        if (dataType == "Boolean")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.Boolean);
        if (dataType == "Int32")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.Int32);
        if (dataType == "UInt32")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.UInt32);
        if (dataType == "Float")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.Float);
        if (dataType == "Double")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.Double);
        if (dataType == "String")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.String);
        if (dataType == "DateTime")
            return InformationModel.MakeVariable(name, OpcUa.DataTypes.DateTime);

        throw new InvalidOperationException("Unsupported dataType: " + dataType);
    }

    private static void SetVariableValue(IUAVariable variable, string dataType, object rawValue)
    {
        if (rawValue == null)
            return;

        if (rawValue is JsonElement element)
        {
            switch (dataType)
            {
                case "Boolean":
                    variable.Value = element.GetBoolean();
                    return;
                case "Int32":
                    variable.Value = element.GetInt32();
                    return;
                case "UInt32":
                    variable.Value = element.GetUInt32();
                    return;
                case "Float":
                    variable.Value = element.GetSingle();
                    return;
                case "Double":
                    variable.Value = element.GetDouble();
                    return;
                case "String":
                case "DateTime":
                    variable.Value = element.GetString() ?? string.Empty;
                    return;
            }
        }

        // Fallback for manually constructed objects, not expected for JsonSerializer.
        if (dataType == "Boolean")
            variable.Value = Convert.ToBoolean(rawValue, CultureInfo.InvariantCulture);
        else if (dataType == "Int32")
            variable.Value = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
        else if (dataType == "UInt32")
            variable.Value = Convert.ToUInt32(rawValue, CultureInfo.InvariantCulture);
        else if (dataType == "Float")
            variable.Value = Convert.ToSingle(rawValue, CultureInfo.InvariantCulture);
        else if (dataType == "Double")
            variable.Value = Convert.ToDouble(rawValue, CultureInfo.InvariantCulture);
        else
            variable.Value = Convert.ToString(rawValue, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static void AddStringVariable(IUANode parent, string name, string value)
    {
        var variable = InformationModel.MakeVariable(name, OpcUa.DataTypes.String);
        variable.Value = value ?? string.Empty;
        parent.Add(variable);
    }

    private static void AddUInt32Variable(IUANode parent, string name, uint value)
    {
        var variable = InformationModel.MakeVariable(name, OpcUa.DataTypes.UInt32);
        variable.Value = value;
        parent.Add(variable);
    }

    private static void AddDoubleVariable(IUANode parent, string name, double value)
    {
        var variable = InformationModel.MakeVariable(name, OpcUa.DataTypes.Double);
        variable.Value = value;
        parent.Add(variable);
    }

    private static void DeleteChildIfExists(IUANode parent, string childName)
    {
        var existing = parent.Get(childName);
        if (existing != null)
            existing.Delete();
    }

    private static void ValidateSpec(TagBackedSpec spec)
    {
        if (spec == null)
            throw new InvalidOperationException("JSON could not be parsed as a Probe 05B spec.");

        if (spec.SpecVersion != "0.5a")
            throw new InvalidOperationException("specVersion must be 0.5a.");

        if (spec.Probe != "Probe 05A - tag-backed variables")
            throw new InvalidOperationException("probe must be 'Probe 05A - tag-backed variables'.");

        if (string.IsNullOrWhiteSpace(spec.RootName))
            throw new InvalidOperationException("rootName is required.");

        if (spec.Objects == null || spec.Objects.Count == 0)
            throw new InvalidOperationException("objects must contain at least one object.");

        var objectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var obj in spec.Objects)
        {
            if (string.IsNullOrWhiteSpace(obj.Name))
                throw new InvalidOperationException("Every object must have a name.");

            if (!objectNames.Add(obj.Name))
                throw new InvalidOperationException("Duplicate object name: " + obj.Name);

            if (obj.Variables == null || obj.Variables.Count == 0)
                throw new InvalidOperationException("Object " + obj.Name + " must contain at least one variable.");

            var variableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var variable in obj.Variables)
            {
                if (string.IsNullOrWhiteSpace(variable.Name))
                    throw new InvalidOperationException("Variable name is required in object " + obj.Name + ".");

                if (!variableNames.Add(variable.Name))
                    throw new InvalidOperationException("Duplicate variable name in object " + obj.Name + ": " + variable.Name);

                if (string.IsNullOrWhiteSpace(variable.DataType))
                    throw new InvalidOperationException("dataType is required for " + obj.Name + "/" + variable.Name);

                if (string.IsNullOrWhiteSpace(variable.Role))
                    throw new InvalidOperationException("role is required for " + obj.Name + "/" + variable.Name);

                if (variable.Source == null || string.IsNullOrWhiteSpace(variable.Source.Kind))
                    throw new InvalidOperationException("source.kind is required for " + obj.Name + "/" + variable.Name);

                if (variable.Source.Kind == "plcTag")
                {
                    if (string.IsNullOrWhiteSpace(variable.Source.Tag))
                        throw new InvalidOperationException("source.tag is required for plcTag variable " + obj.Name + "/" + variable.Name);
                    if (string.IsNullOrWhiteSpace(variable.Source.Mode))
                        throw new InvalidOperationException("source.mode is required for plcTag variable " + obj.Name + "/" + variable.Name);
                }
                else if (variable.Source.Kind == "mock" || variable.Source.Kind == "static")
                {
                    if (variable.Source.Value == null)
                        throw new InvalidOperationException("source.value is required for " + variable.Source.Kind + " variable " + obj.Name + "/" + variable.Name);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported source.kind for " + obj.Name + "/" + variable.Name + ": " + variable.Source.Kind);
                }
            }
        }
    }

    private class TagBackedSpec
    {
        public string SpecVersion { get; set; }
        public string Probe { get; set; }
        public string RootName { get; set; }
        public string Description { get; set; }
        public List<ModelObjectSpec> Objects { get; set; }
    }

    private class ModelObjectSpec
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<VariableSpec> Variables { get; set; }
    }

    private class VariableSpec
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string Role { get; set; }
        public SourceSpec Source { get; set; }
        public DisplaySpec Display { get; set; }
    }

    private class SourceSpec
    {
        public string Kind { get; set; }
        public string Tag { get; set; }
        public string Mode { get; set; }
        public object Value { get; set; }
    }

    private class DisplaySpec
    {
        public string Unit { get; set; }
        public int? Decimals { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string Format { get; set; }
    }
}
