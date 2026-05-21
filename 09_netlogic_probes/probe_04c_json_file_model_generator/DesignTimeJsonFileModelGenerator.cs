// Probe 04C - DesignTime NetLogic reads JSON from file
//
// This is a FT Optix NetLogic template, not a standalone C# console app.
// For easiest testing, paste this into the FT Optix generated file:
//
//   DesignTimeNetLogic1.cs
//
// Important:
// - The C# class name must exactly match the FT Optix NetLogic object name.
// - If your NetLogic object is named DesignTimeNetLogic1, keep this class name.
// - If your NetLogic object has a different name, rename this class to match it.
// - The first file-read test uses a fixed path: C:\Temp\ftoptix_model_spec.json

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
        GenerateFromJsonFile();
    }

    [ExportMethod]
    public void GenerateFromJsonFile()
    {
        const string jsonPath = @"C:\Temp\ftoptix_model_spec.json";

        if (!File.Exists(jsonPath))
        {
            Log.Error("Probe04C", "JSON file not found: " + jsonPath);
            return;
        }

        string jsonSpec;
        try
        {
            jsonSpec = File.ReadAllText(jsonPath);
        }
        catch (Exception ex)
        {
            Log.Error("Probe04C", "Could not read JSON file: " + ex.Message);
            return;
        }

        ModelSpec spec;
        try
        {
            spec = JsonSerializer.Deserialize<ModelSpec>(jsonSpec);
        }
        catch (Exception ex)
        {
            Log.Error("Probe04C", "Could not parse JSON file: " + ex.Message);
            return;
        }

        if (spec == null || string.IsNullOrWhiteSpace(spec.rootName))
        {
            Log.Error("Probe04C", "JSON spec is empty or missing rootName.");
            return;
        }

        var model = Project.Current.Get("Model");
        if (model == null)
        {
            Log.Error("Probe04C", "Model folder not found.");
            return;
        }

        if (model.Get(spec.rootName) != null)
        {
            Log.Warning("Probe04C", spec.rootName + " already exists. Delete it manually before running again.");
            return;
        }

        var root = InformationModel.MakeObject(spec.rootName);
        model.Add(root);

        AddVariables(root, spec.variables);
        AddObjects(root, spec.objects);

        Log.Info("Probe04C", spec.rootName + " generated from JSON file: " + jsonPath);
    }

    private void AddObjects(IUANode parent, List<ObjectSpec> objects)
    {
        if (objects == null)
            return;

        foreach (var objectSpec in objects)
        {
            if (objectSpec == null || string.IsNullOrWhiteSpace(objectSpec.name))
                continue;

            var obj = InformationModel.MakeObject(objectSpec.name);
            parent.Add(obj);

            AddVariables(obj, objectSpec.variables);
            AddObjects(obj, objectSpec.objects);
        }
    }

    private void AddVariables(IUANode parent, List<VariableSpec> variables)
    {
        if (variables == null)
            return;

        foreach (var variableSpec in variables)
        {
            if (variableSpec == null || string.IsNullOrWhiteSpace(variableSpec.name))
                continue;

            var dataType = ResolveDataType(variableSpec.dataType);
            var variable = InformationModel.MakeVariable(variableSpec.name, dataType);
            SetVariableValue(variable, variableSpec.value, variableSpec.dataType);
            parent.Add(variable);
        }
    }

    private NodeId ResolveDataType(string dataType)
    {
        switch ((dataType ?? "String").Trim().ToLowerInvariant())
        {
            case "boolean":
            case "bool":
                return OpcUa.DataTypes.Boolean;
            case "float":
                return OpcUa.DataTypes.Float;
            case "double":
                return OpcUa.DataTypes.Double;
            case "int32":
            case "int":
                return OpcUa.DataTypes.Int32;
            case "uint32":
            case "uint":
                return OpcUa.DataTypes.UInt32;
            case "string":
            default:
                return OpcUa.DataTypes.String;
        }
    }

    private void SetVariableValue(IUAVariable variable, JsonElement value, string dataType)
    {
        switch ((dataType ?? "String").Trim().ToLowerInvariant())
        {
            case "boolean":
            case "bool":
                variable.Value = value.ValueKind == JsonValueKind.True ||
                                 (value.ValueKind == JsonValueKind.String && bool.Parse(value.GetString()));
                break;
            case "float":
                variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetSingle() : float.Parse(value.GetString());
                break;
            case "double":
                variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetDouble() : double.Parse(value.GetString());
                break;
            case "int32":
            case "int":
                variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetInt32() : int.Parse(value.GetString());
                break;
            case "uint32":
            case "uint":
                variable.Value = value.ValueKind == JsonValueKind.Number ? value.GetUInt32() : uint.Parse(value.GetString());
                break;
            case "string":
            default:
                variable.Value = value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
                break;
        }
    }

    private class ModelSpec
    {
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
