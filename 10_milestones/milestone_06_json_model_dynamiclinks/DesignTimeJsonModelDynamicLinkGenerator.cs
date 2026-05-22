// Milestone 06 - JSON to Model variables to local DynamicLinks
//
// Purpose:
// Combine the proven 05A/05B/05C path:
// hardened JSON -> Model variables -> local DynamicLinks.
//
// FT Echo is NOT required for this milestone.
// PLC live tag verification is reserved for the next milestone/probe.

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

public class DesignTimeJsonModelDynamicLinkGenerator : BaseNetLogic
{
    private const string DefaultJsonPath = @"C:\Temp\ftoptix_milestone06_tag_backed_variables.json";
    private const string GeneratedRootName = "AI_JsonDynamicLinkProbe_01";
    private const string LocalSourcesName = "_LocalSources";

    [ExportMethod]
    public void GenerateJsonModelWithLocalDynamicLinks()
    {
        if (!File.Exists(DefaultJsonPath))
            throw new FileNotFoundException("Milestone 06 JSON file was not found.", DefaultJsonPath);

        var json = File.ReadAllText(DefaultJsonPath);
        var spec = JsonSerializer.Deserialize<TagBackedSpec>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        ValidateSpec(spec);

        var model = Project.Current.Get("Model");
        if (model == null)
            throw new InvalidOperationException("Could not find Project.Current.Get(\"Model\").");

        DeleteChildIfExists(model, GeneratedRootName);

        var root = InformationModel.MakeObject(GeneratedRootName);
        model.Add(root);

        AddStringVariable(root, "Milestone", "Milestone 06 - JSON to Model variables to local DynamicLinks");
        AddStringVariable(root, "SourceJson", DefaultJsonPath);
        AddStringVariable(root, "OriginalJsonRootName", spec.RootName);
        AddStringVariable(root, "Rule", "plcTag sources are simulated locally under _LocalSources for this milestone.");

        var localSources = InformationModel.MakeObject(LocalSourcesName);
        root.Add(localSources);

        foreach (var objSpec in spec.Objects)
        {
            var generatedObject = InformationModel.MakeObject(objSpec.Name);
            root.Add(generatedObject);

            if (!string.IsNullOrWhiteSpace(objSpec.Description))
                AddStringVariable(generatedObject, "Description", objSpec.Description);

            IUANode localSourceObject = null;

            foreach (var variableSpec in objSpec.Variables)
            {
                if (variableSpec.Source.Kind == "plcTag")
                {
                    if (localSourceObject == null)
                    {
                        localSourceObject = InformationModel.MakeObject(objSpec.Name);
                        localSources.Add(localSourceObject);
                    }

                    var sourceVariable = MakeVariable(variableSpec.Name, variableSpec.DataType);
                    SetDefaultSimulatedValue(sourceVariable, variableSpec);
                    localSourceObject.Add(sourceVariable);

                    var generatedVariable = MakeVariable(variableSpec.Name, variableSpec.DataType);
                    generatedVariable.Value = GetNeutralValue(variableSpec.DataType);
                    generatedObject.Add(generatedVariable);

                    generatedVariable.SetDynamicLink(sourceVariable);

                    AddMetadata(generatedObject, variableSpec, "localDynamicLink", "../" + LocalSourcesName + "/" + objSpec.Name + "/" + variableSpec.Name);
                }
                else if (variableSpec.Source.Kind == "mock" || variableSpec.Source.Kind == "static")
                {
                    var generatedVariable = MakeVariable(variableSpec.Name, variableSpec.DataType);
                    SetVariableValue(generatedVariable, variableSpec.DataType, variableSpec.Source.Value);
                    generatedObject.Add(generatedVariable);

                    AddMetadata(generatedObject, variableSpec, variableSpec.Source.Kind, string.Empty);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported source.kind: " + variableSpec.Source.Kind);
                }
            }
        }
    }

    private static void AddMetadata(IUANode parent, VariableSpec variableSpec, string generationSourceKind, string localLinkPath)
    {
        AddStringVariable(parent, variableSpec.Name + "_Role", variableSpec.Role);
        AddStringVariable(parent, variableSpec.Name + "_SourceKind", variableSpec.Source.Kind);
        AddStringVariable(parent, variableSpec.Name + "_GenerationSourceKind", generationSourceKind);

        if (!string.IsNullOrWhiteSpace(variableSpec.Source.Tag))
            AddStringVariable(parent, variableSpec.Name + "_SourceTag", variableSpec.Source.Tag);

        if (!string.IsNullOrWhiteSpace(variableSpec.Source.Mode))
            AddStringVariable(parent, variableSpec.Name + "_SourceMode", variableSpec.Source.Mode);

        if (!string.IsNullOrWhiteSpace(localLinkPath))
            AddStringVariable(parent, variableSpec.Name + "_LocalLinkPath", localLinkPath);

        if (variableSpec.Display != null)
        {
            if (!string.IsNullOrWhiteSpace(variableSpec.Display.Unit))
                AddStringVariable(parent, variableSpec.Name + "_DisplayUnit", variableSpec.Display.Unit);

            if (variableSpec.Display.Decimals.HasValue)
                AddUInt32Variable(parent, variableSpec.Name + "_DisplayDecimals", (uint)variableSpec.Display.Decimals.Value);

            if (variableSpec.Display.Min.HasValue)
                AddDoubleVariable(parent, variableSpec.Name + "_DisplayMin", variableSpec.Display.Min.Value);

            if (variableSpec.Display.Max.HasValue)
                AddDoubleVariable(parent, variableSpec.Name + "_DisplayMax", variableSpec.Display.Max.Value);
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

    private static void SetDefaultSimulatedValue(IUAVariable variable, VariableSpec spec)
    {
        if (spec.DataType == "Boolean")
            variable.Value = spec.Role == "command" ? false : true;
        else if (spec.DataType == "Int32")
            variable.Value = 1;
        else if (spec.DataType == "UInt32")
            variable.Value = 1u;
        else if (spec.DataType == "Float")
            variable.Value = spec.Role == "setpoint" ? 50.0f : 12.3f;
        else if (spec.DataType == "Double")
            variable.Value = spec.Role == "setpoint" ? 50.0 : 12.3;
        else if (spec.DataType == "String")
            variable.Value = "SIM_" + (spec.Source.Tag ?? spec.Name);
        else if (spec.DataType == "DateTime")
            variable.Value = "2026-05-22T00:00:00Z";
    }

    private static object GetNeutralValue(string dataType)
    {
        if (dataType == "Boolean")
            return false;
        if (dataType == "Int32")
            return 0;
        if (dataType == "UInt32")
            return 0u;
        if (dataType == "Float")
            return 0.0f;
        if (dataType == "Double")
            return 0.0;
        if (dataType == "String")
            return string.Empty;
        if (dataType == "DateTime")
            return "";
        return string.Empty;
    }

    private static void SetVariableValue(IUAVariable variable, string dataType, object rawValue)
    {
        if (rawValue == null)
            return;

        if (rawValue is JsonElement element)
        {
            if (dataType == "Boolean")
                variable.Value = element.GetBoolean();
            else if (dataType == "Int32")
                variable.Value = element.GetInt32();
            else if (dataType == "UInt32")
                variable.Value = element.GetUInt32();
            else if (dataType == "Float")
                variable.Value = element.GetSingle();
            else if (dataType == "Double")
                variable.Value = element.GetDouble();
            else
                variable.Value = element.GetString() ?? string.Empty;
            return;
        }

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
            throw new InvalidOperationException("JSON could not be parsed.");
        if (spec.SpecVersion != "0.5a")
            throw new InvalidOperationException("specVersion must be 0.5a.");
        if (spec.Objects == null || spec.Objects.Count == 0)
            throw new InvalidOperationException("objects must contain at least one object.");

        foreach (var obj in spec.Objects)
        {
            if (string.IsNullOrWhiteSpace(obj.Name))
                throw new InvalidOperationException("Every object must have a name.");
            if (obj.Variables == null || obj.Variables.Count == 0)
                throw new InvalidOperationException("Object " + obj.Name + " must contain at least one variable.");

            foreach (var variable in obj.Variables)
            {
                if (string.IsNullOrWhiteSpace(variable.Name))
                    throw new InvalidOperationException("Variable name is required in object " + obj.Name + ".");
                if (string.IsNullOrWhiteSpace(variable.DataType))
                    throw new InvalidOperationException("dataType is required for " + obj.Name + "/" + variable.Name);
                if (variable.Source == null || string.IsNullOrWhiteSpace(variable.Source.Kind))
                    throw new InvalidOperationException("source.kind is required for " + obj.Name + "/" + variable.Name);
                if (variable.Source.Kind == "plcTag" && string.IsNullOrWhiteSpace(variable.Source.Tag))
                    throw new InvalidOperationException("source.tag is required for plcTag variable " + obj.Name + "/" + variable.Name);
                if ((variable.Source.Kind == "mock" || variable.Source.Kind == "static") && variable.Source.Value == null)
                    throw new InvalidOperationException("source.value is required for " + variable.Source.Kind + " variable " + obj.Name + "/" + variable.Name);
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
