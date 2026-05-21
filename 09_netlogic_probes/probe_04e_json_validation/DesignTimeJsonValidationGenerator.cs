#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class DesignTimeNetLogic1 : BaseNetLogic
{
    [ExportMethod]
    public void Method1()
    {
        GenerateAfterValidation();
    }

    [ExportMethod]
    public void GenerateAfterValidation()
    {
        const string jsonPath = @"C:\Temp\ftoptix_model_spec.json";
        if (!File.Exists(jsonPath))
        {
            Log.Error("Probe04E", "JSON file not found: " + jsonPath);
            return;
        }

        ModelSpec spec;
        try
        {
            spec = JsonSerializer.Deserialize<ModelSpec>(File.ReadAllText(jsonPath));
        }
        catch (Exception ex)
        {
            Log.Error("Probe04E", "JSON read/parse failed: " + ex.Message);
            return;
        }

        var validationErrors = new List<string>();
        ValidateSpec(spec, validationErrors);
        if (validationErrors.Count > 0)
        {
            foreach (var error in validationErrors)
                Log.Error("Probe04E", "validation failed: " + error);
            return;
        }

        var model = Project.Current.Get("Model");
        if (model == null)
        {
            Log.Error("Probe04E", "Model folder not found.");
            return;
        }

        if (model.Get(spec.rootName) != null)
        {
            Log.Warning("Probe04E", spec.rootName + " already exists. Delete it manually or use a later recreate-mode probe.");
            return;
        }

        var root = InformationModel.MakeObject(spec.rootName);
        model.Add(root);
        AddVariables(root, spec.variables);
        AddObjects(root, spec.objects);
        Log.Info("Probe04E", spec.rootName + " generated after validation.");
    }

    private void ValidateSpec(ModelSpec spec, List<string> errors)
    {
        if (spec == null)
        {
            errors.Add("JSON spec is empty.");
            return;
        }
        if (string.IsNullOrWhiteSpace(spec.rootName))
            errors.Add("rootName is required.");
        if (!IsValidGenerationMode(spec.generationMode))
            errors.Add("generationMode must be skip or deleteAndRecreate.");
        if (!string.IsNullOrWhiteSpace(spec.rootName))
            ValidateChildren(spec.rootName, spec.variables, spec.objects, errors);
    }

    private void ValidateChildren(string parentPath, List<VariableSpec> variables, List<ObjectSpec> objects, List<string> errors)
    {
        var childNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (variables != null)
        {
            foreach (var variable in variables)
            {
                if (variable == null)
                {
                    errors.Add("Null variable under " + parentPath + ".");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(variable.name))
                {
                    errors.Add("Variable name is required under " + parentPath + ".");
                    continue;
                }
                if (!childNames.Add(variable.name))
                    errors.Add("Duplicate child name '" + variable.name + "' under " + parentPath + ".");
                if (!IsKnownDataType(variable.dataType))
                    errors.Add("Unknown dataType '" + variable.dataType + "' at " + parentPath + "/" + variable.name + ".");
                if (variable.value.ValueKind == JsonValueKind.Undefined)
                    errors.Add("Variable '" + variable.name + "' is missing value at " + parentPath + ".");
            }
        }

        if (objects != null)
        {
            foreach (var obj in objects)
            {
                if (obj == null)
                {
                    errors.Add("Null object under " + parentPath + ".");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(obj.name))
                {
                    errors.Add("Object name is required under " + parentPath + ".");
                    continue;
                }
                if (!childNames.Add(obj.name))
                    errors.Add("Duplicate child name '" + obj.name + "' under " + parentPath + ".");
                ValidateChildren(parentPath + "/" + obj.name, obj.variables, obj.objects, errors);
            }
        }
    }

    private bool IsValidGenerationMode(string mode)
    {
        if (string.IsNullOrWhiteSpace(mode)) return true;
        return mode.Equals("skip", StringComparison.OrdinalIgnoreCase) || mode.Equals("deleteAndRecreate", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsKnownDataType(string dataType)
    {
        switch ((dataType ?? "String").Trim().ToLowerInvariant())
        {
            case "string":
            case "float":
            case "double":
            case "boolean":
            case "bool":
            case "int32":
            case "int":
            case "uint32":
            case "uint":
                return true;
            default:
                return false;
        }
    }

    private void AddObjects(IUANode parent, List<ObjectSpec> objects)
    {
        if (objects == null) return;
        foreach (var objectSpec in objects)
        {
            var obj = InformationModel.MakeObject(objectSpec.name);
            parent.Add(obj);
            AddVariables(obj, objectSpec.variables);
            AddObjects(obj, objectSpec.objects);
        }
    }

    private void AddVariables(IUANode parent, List<VariableSpec> variables)
    {
        if (variables == null) return;
        foreach (var variableSpec in variables)
        {
            var variable = InformationModel.MakeVariable(variableSpec.name, ResolveDataType(variableSpec.dataType));
            SetVariableValue(variable, variableSpec.value, variableSpec.dataType);
            parent.Add(variable);
        }
    }

    private NodeId ResolveDataType(string dataType)
    {
        switch ((dataType ?? "String").Trim().ToLowerInvariant())
        {
            case "boolean":
            case "bool": return OpcUa.DataTypes.Boolean;
            case "float": return OpcUa.DataTypes.Float;
            case "double": return OpcUa.DataTypes.Double;
            case "int32":
            case "int": return OpcUa.DataTypes.Int32;
            case "uint32":
            case "uint": return OpcUa.DataTypes.UInt32;
            default: return OpcUa.DataTypes.String;
        }
    }

    private void SetVariableValue(IUAVariable variable, JsonElement value, string dataType)
    {
        switch ((dataType ?? "String").Trim().ToLowerInvariant())
        {
            case "boolean":
            case "bool": variable.Value = value.ValueKind == JsonValueKind.True || (value.ValueKind == JsonValueKind.String && bool.Parse(value.GetString())); break;
            case "float": variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetSingle() : float.Parse(value.GetString()); break;
            case "double": variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetDouble() : double.Parse(value.GetString()); break;
            case "int32":
            case "int": variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetInt32() : int.Parse(value.GetString()); break;
            case "uint32":
            case "uint": variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetUInt32() : uint.Parse(value.GetString()); break;
            default: variable.Value = value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString(); break;
        }
    }

    private class ModelSpec
    {
        public string generationMode { get; set; }
        public string rootName { get; set; }
        public List<VariableSpec> variables { get; set; }
        public List<ObjectSpec> objects { get; set; }
    }
    private class ObjectSpec
    {
        public string name { get; set; }
        public List<VariableSpec> variables { get; set; }
        public List<ObjectSpec> objects { get; set; }
    }
    private class VariableSpec
    {
        public string name { get; set; }
        public string dataType { get; set; }
        public JsonElement value { get; set; }
    }
}
