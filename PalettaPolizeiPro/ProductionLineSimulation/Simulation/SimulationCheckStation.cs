using Sharp7;
using System;
using System.Text;

namespace ProductionLineSimulation.Simulation;

public class SimulationCheckStation : SimulationStation
{
    public float GetSerivcePercentage()
    {
        var actualCycleBytes = Plc.GetBytes(DB,12,2);
        var predefieniedCycleBytes = Plc.GetBytes(DB, 14, 2);

        int predefiniedCycle = S7.GetIntAt(predefieniedCycleBytes,0);
        int actualCycle = S7.GetIntAt(actualCycleBytes, 0);

        if (predefiniedCycle == 0 || actualCycle == 0) { return 0; }
        float p = actualCycle / (float)predefiniedCycle * 100;
        return p;
    }
    public string? GetPalettaName()
    {
        byte[] buffer = Plc.GetBytes(DB, 0, 16);
        bool fullnull = AllZero(buffer);
        return fullnull ? null : GetIdentifier(buffer, Loop);
    }
    public string? GetEngineId()
    {
        byte[] mokanyBytes = Plc.GetBytes(DB, 240, 9);
        bool fullnull = AllZero(mokanyBytes);
        return fullnull ? null : S7.GetCharsAt(mokanyBytes, 0, 9);
    }

    public override void PalettaIn(SimulationPaletta paletta)
    {
        Paletta = paletta;  
        byte[] bytes = new byte[16];
        var buffer = SetIdentifierNummer(paletta.Name.Split('W')[1]);
        for(int i = 0;i<buffer.Length;i++)
        {
            bytes[i] = buffer[i];
        }
        S7.SetIntAt(bytes, 12, (short)paletta.ActualCycle);
        S7.SetIntAt(bytes, 14, (short)paletta.PredefiniedCycle);
        Plc.SetBytes(DB,0,16,bytes);

        if (paletta.Engine is not null)
        {
            var mokanyBytes = Encoding.UTF8.GetBytes(paletta.Engine);
            Plc.SetBytes(DB,240,9,mokanyBytes);
        }
    }

    public override void PalettaOut()
    {
        Paletta = null;
        byte[] bytes = new byte[16];
        Plc.SetBytes(DB, 0, 16, bytes);
        Plc.SetBytes(DB, 240, 9, new byte[9]);

    }
}

