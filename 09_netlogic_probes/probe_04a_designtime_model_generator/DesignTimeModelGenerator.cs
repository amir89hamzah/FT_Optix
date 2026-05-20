// Probe 04A - DesignTime NetLogic model generator
//
// This is a FT Optix NetLogic template, not a standalone C# console app.
// Create a DesignTime NetLogic in FT Optix Studio, then adapt/paste this
// class body into the generated NetLogic file.
//
// If the generated class name in FT Optix is different, keep the class name
// created by Studio and copy only the GenerateModelProbe method and helpers.

#region Using directives
using FTOptix.Core;
using FTOptix.HMIProject;
using UAManagedCore;
#endregion

public class DesignTimeModelGenerator : BaseNetLogic
{
    [ExportMethod]
    public void GenerateModelProbe()
    {
        var model = Project.Current.Get("Model");
        if (model == null)
        {
            Log.Error("Probe04A", "Model folder not found.");
            return;
        }

        // Keep the first version simple and explicit.
        // If the node already exists, stop instead of deleting anything.
        // This avoids accidental removal while we are still proving the API path.
        if (model.Get("AI_NetLogicProbe_01") != null)
        {
            Log.Warning("Probe04A", "AI_NetLogicProbe_01 already exists. Delete it manually before running again.");
            return;
        }

        var root = InformationModel.MakeObject("AI_NetLogicProbe_01");
        model.Add(root);

        AddVariable(root, "StatusText", OpcUa.DataTypes.String, "Created by DesignTime NetLogic");
        AddVariable(root, "TestNumber", OpcUa.DataTypes.Float, 123.45f);
        AddVariable(root, "Running", OpcUa.DataTypes.Boolean, true);

        var pump1 = InformationModel.MakeObject("Pump1");
        root.Add(pump1);
        AddVariable(pump1, "SetSpeed", OpcUa.DataTypes.Float, 50.0f);
        AddVariable(pump1, "CurrentSpeed", OpcUa.DataTypes.Float, 47.5f);

        Log.Info("Probe04A", "AI_NetLogicProbe_01 generated under Model.");
    }

    private IUAVariable AddVariable(IUANode parent, string browseName, NodeId dataType, object value)
    {
        var variable = InformationModel.MakeVariable(browseName, dataType);
        variable.Value = value;
        parent.Add(variable);
        return variable;
    }
}
