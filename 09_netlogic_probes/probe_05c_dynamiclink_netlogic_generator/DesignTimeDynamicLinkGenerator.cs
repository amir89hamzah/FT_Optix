// Probe 05C-3 - DesignTime NetLogic DynamicLink generator
//
// Purpose:
// Create the same local DynamicLink pattern that was proven manually in 05C-1
// and exported in 05C-2.
//
// Expected generated structure:
// Model
// └─ AI_DynamicLinkNetLogicProbe_01
//    ├─ SourceSpeed
//    └─ LinkedSpeed -> DynamicLink ../SourceSpeed
//
// FT Echo is NOT required for this probe.
// This is still a local Model-to-Model DynamicLink test.

#region Using directives
using System;
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class DesignTimeDynamicLinkGenerator : BaseNetLogic
{
    [ExportMethod]
    public void GenerateLocalDynamicLinkProbe()
    {
        var model = Project.Current.Get("Model");
        if (model == null)
            throw new InvalidOperationException("Could not find Project.Current.Get(\"Model\").");

        DeleteChildIfExists(model, "AI_DynamicLinkNetLogicProbe_01");

        var root = InformationModel.MakeObject("AI_DynamicLinkNetLogicProbe_01");
        model.Add(root);

        var sourceSpeed = InformationModel.MakeVariable("SourceSpeed", OpcUa.DataTypes.Float);
        sourceSpeed.Value = 12.3f;
        root.Add(sourceSpeed);

        var linkedSpeed = InformationModel.MakeVariable("LinkedSpeed", OpcUa.DataTypes.Float);
        linkedSpeed.Value = 0.0f;
        root.Add(linkedSpeed);

        // Working API confirmed in FT Optix manual test:
        // This creates a functional local DynamicLink from LinkedSpeed to SourceSpeed.
        // The earlier attempt with DynamicLinkMode.ReadWrite did not compile because
        // DynamicLinkMode was not available in the tested FT Optix C# context.
        linkedSpeed.SetDynamicLink(sourceSpeed);

        AddStringVariable(root, "Probe", "Probe 05C-3 - DesignTime NetLogic DynamicLink generator");
        AddStringVariable(root, "ExpectedLinkPath", "../SourceSpeed");
        AddStringVariable(root, "ExpectedModeFromManualExport", "2");
        AddStringVariable(root, "Note", "If runtime follows SourceSpeed, DesignTime NetLogic DynamicLink creation works.");
    }

    private static void DeleteChildIfExists(IUANode parent, string childName)
    {
        var existing = parent.Get(childName);
        if (existing != null)
            existing.Delete();
    }

    private static void AddStringVariable(IUANode parent, string name, string value)
    {
        var variable = InformationModel.MakeVariable(name, OpcUa.DataTypes.String);
        variable.Value = value ?? string.Empty;
        parent.Add(variable);
    }
}
