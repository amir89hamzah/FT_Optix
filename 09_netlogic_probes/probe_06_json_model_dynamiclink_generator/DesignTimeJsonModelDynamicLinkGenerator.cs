// Probe 06 - JSON -> Model variables -> DynamicLinks generator
//
// Purpose:
// Combine the proven Probe 05B JSON/metadata generator pattern with the proven
// Probe 05C DynamicLink API pattern.
//
// Scope limit:
// - Reads one JSON file from C:\Temp
// - Processes ONLY the first object in objects[]
// - Creates Model variables
// - source.kind == mock/static      -> writes value directly
// - source.kind == localSource      -> creates DynamicLink to another generated/local node
// - source.kind == plcTag           -> creates DynamicLink only if the target node already exists
// - Stores trace metadata beside each generated variable
//
// FT Echo is NOT required for this probe.
// Batch import is intentionally NOT implemented yet.

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
    private const string DefaultJsonPath = @"C:\Temp\ftoptix_probe06_json_model_dynamiclink.json";

    [ExportMethod]
    public void GenerateFirstObjectOnly()
    {
        if (!File.Exists(DefaultJsonPath))
            throw new FileNotFoundException("Probe 06 JSON file was not found.", DefaultJsonPath);

        var json = File.ReadAllText(DefaultJsonPath);
        var spec = JsonSerializer.Deserialize<ImportSpec>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        ValidateSpec(spec);

        var model = Project.Current.Get("Model");
        if (model == null)
            throw new InvalidOperationException("Could not find Project.Current.Get(\"Model\").");

        var rootName = string.IsNullOrWhiteSpace(spec.RootName)
            ? "AI_JSON_DynamicLinkProbe_01"
            : spec.RootName;

        DeleteChildIfExists(model, rootName);

        var root = InformationModel.MakeObject(rootName);
        model.Add(root);

        AddStringVariable(root, "Probe", "Probe 06 - JSON to Model variables to DynamicLinks");
        AddStringVariable(root, "GeneratedBy", "DesignTimeJsonModelDynamicLinkGenerator");
        AddStringVariable(root, "SourceJsonPath", DefaultJsonPath);
        AddStringVariable(root, "ScopeLimit", "Only objects[0] is processed in this probe.");

        var objectSpec = spec.Objects[0];
        var objectNode = InformationModel.MakeObject(objectSpec.Name);
        root.Add(objectNode);

        if (!string.IsNullOrWhiteSpace(objectSpec.Description))
            AddStringVariable(objectNode, "Description", objectSpec.Description);

        var generatedVariables = new Dictionary<string, IUAVariable>(StringComparer.OrdinalIgnoreCase);

        foreach (var variableSpec in objectSpec.Variables)
        {
            var main = MakeVariable(variableSpec.Name, variableSpec.DataType);
            objectNode.Add(main);
            generatedVariables[variableSpec.Name] = main;

            ApplySource(main, objectNode, generatedVariables, variableSpec);
            AddVariableMetadata(objectNode, variableSpec);
        }
    }

    private static void ApplySource(
        IUAVariable targetVariable,
        IUANode objectNode,
        Dictionary<string, IUAVariable> generatedVariables,
        VariableSpec variableSpec)
    {
        var sourceKind = variableSpec.Source.Kind;

        if (sourceKind == "mock" || sourceKind == "static")
        {
            SetVariableValue(targetVariable, variableSpec.DataType, variableSpec.Source.Value);
            return;
        }

        if (sourceKind == "localSource" || sourceKind == "plcTag")
        {
            var linkTarget = ResolveLinkTarget(objectNode, generatedVariables, variableSpec.Source);
            if (linkTarget == null)
            {
                var requestedPath = GetSourcePath(variableSpec.Source);
                throw new InvalidOperationException(
                    "Could not resolve DynamicLink target for variable " + variableSpec.Name +
                    ". source.kind=" + sourceKind +
                    ", requested path/tag=" + requestedPath +
                    ". For the first test, use localSource pointing to a generated sibling variable. For plcTag, the tag node must already exist in the project.");
            }

            // Working local API from Probe 05C-3.
            targetVariable.SetDynamicLink(linkTarget);
            return;
        }

        throw new InvalidOperationException("Unsupported source.kind for " + variableSpec.Name + ": " + sourceKind);
    }

    private static IUAVariable ResolveLinkTarget(
        IUANode objectNode,
        Dictionary<string, IUAVariable> generatedVariables,
        SourceSpec source)
    {
        var path = GetSourcePath(source);
        if (string.IsNullOrWhiteSpace(path))
            return null;

        if (generatedVariables.TryGetValue(path, out var generatedByName))
            return generatedByName;

        if (path.StartsWith("../", StringComparison.Ordinal))
        {
            var siblingName = path.Substring(3);
            if (generatedVariables.TryGetValue(siblingName, out var generatedByRelativeName))
                return generatedByRelativeName;

            return objectNode.Get(siblingName) as IUAVariable;
        }

        var localChild = objectNode.Get(path) as IUAVariable;
        if (localChild != null)
            return localChild;

        return Project.Current.Get(path) as IUAVariable;
    }

    private static string GetSourcePath(SourceSpec source)
    {
        if (!string.IsNullOrWhiteSpace(source.Path))
            return source.Path;

        if (!string.IsNullOrWhiteSpace(source.Tag))
            return source.Tag;

        return string.Empty;
    }

    private static void AddVariableMetadata(IUANode parent, VariableSpec variable)
    {
        AddStringVariable(parent, variable.Name + "_SourceKind", variable.Source.Kind);

        var path = GetSourcePath(variable.Source);
        if (!string.IsNullOrWhiteSpace(path))
            AddStringVariable(parent, variable.Name + "_SourcePath", path);

        if (!string.IsNullOrWhiteSpace(variable.Source.Mode))
            AddStringVariable(parent, variable.Name + "_SourceMode", variable.Source.Mode);

        if (!string.IsNullOrWhiteSpace(variable.Role))
            AddStringVariable(parent, variable.Name + "_Role", variable.Role);

        if (!string.IsNullOrWhiteSpace(variable.Description))
            AddStringVariable(parent, variable.Name + "_Description", variable.Description);

        AddStringVariable(parent, variable.Name + "_OriginalJson", JsonSerializer.Serialize(variable));
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

    private static void DeleteChildIfExists(IUANode parent, string childName)
    {
        var existing = parent.Get(childName);
        if (existing != null)
            existing.Delete();
    }

    private static void ValidateSpec(ImportSpec spec)
    {
        if (spec == null)
            throw new InvalidOperationException("JSON could not be parsed as a Probe 06 import spec.");

        if (spec.Objects == null || spec.Objects.Count == 0)
            throw new InvalidOperationException("objects must contain at least one object.");

        var firstObject = spec.Objects[0];
        if (string.IsNullOrWhiteSpace(firstObject.Name))
            throw new InvalidOperationException("objects[0].name is required.");

        if (firstObject.Variables == null || firstObject.Variables.Count == 0)
            throw new InvalidOperationException("objects[0].variables must contain at least one variable.");

        var variableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var variable in firstObject.Variables)
        {
            if (string.IsNullOrWhiteSpace(variable.Name))
                throw new InvalidOperationException("Every variable must have a name.");

            if (!variableNames.Add(variable.Name))
                throw new InvalidOperationException("Duplicate variable name in first object: " + variable.Name);

            if (string.IsNullOrWhiteSpace(variable.DataType))
                throw new InvalidOperationException("dataType is required for variable " + variable.Name);

            if (variable.Source == null || string.IsNullOrWhiteSpace(variable.Source.Kind))
                throw new InvalidOperationException("source.kind is required for variable " + variable.Name);

            var kind = variable.Source.Kind;
            if (kind == "mock" || kind == "static")
            {
                if (variable.Source.Value == null)
                    throw new InvalidOperationException("source.value is required for " + kind + " variable " + variable.Name);
            }
            else if (kind == "localSource" || kind == "plcTag")
            {
                if (string.IsNullOrWhiteSpace(GetSourcePath(variable.Source)))
                    throw new InvalidOperationException("source.path or source.tag is required for " + kind + " variable " + variable.Name);
            }
            else
            {
                throw new InvalidOperationException("Unsupported source.kind for " + variable.Name + ": " + kind);
            }
        }
    }

    private class ImportSpec
    {
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
    }

    private class SourceSpec
    {
        public string Kind { get; set; }
        public string Path { get; set; }
        public string Tag { get; set; }
        public string Mode { get; set; }
        public object Value { get; set; }
    }
}
