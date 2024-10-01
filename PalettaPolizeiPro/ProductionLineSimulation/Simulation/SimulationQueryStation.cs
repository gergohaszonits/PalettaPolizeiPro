using PalettaPolizeiPro.Data.Palettas;
using Sharp7;
using static System.Collections.Specialized.BitVector32;

namespace ProductionLineSimulation.Simulation;

public class SimulationQueryStation : SimulationStation
{
    private QueryState? _lastState = null;
    public QueryState? LastState { get { return _lastState; } }
    public override void PalettaIn(SimulationPaletta paletta)
    {
        Paletta = paletta;


        SetControlFlag(1);
        SetPalettaName(Paletta.Name);

    }
    public override void PalettaOut()
    {
        Paletta = null;
        SetControlFlag(0);
        ClearPalettaName();
    }
    public QueryState GetQueryState()
    {
        byte[] bytes = Plc.GetBytes(DB, 0, 11);
        QueryState query = new QueryState
        {
            OperationStatus = bytes[0],
            ControlFlag = bytes[1],
            PalettaName = S7.GetCharsAt(bytes, 2, 9)
        };
        _lastState = query;
        return query;
    }

    public void SetControlFlag(byte flag)
    {
        Plc.SetBytes(DB, 1, 1, [flag]);
    }

    public void SetOperationStatus(byte val)
    {
        Plc.SetBytes(DB, 0, 1, [val]);
    }
    public void SetPalettaName(string name)
    {
        var bytes = new byte[name.Length];
        S7.SetCharsAt(bytes, 0, name);
        Plc.SetBytes(DB, 2, bytes.Length, bytes);
    }
    public void ClearPalettaName()
    {
        var bytes = new byte[9];
        Plc.SetBytes(DB, 2, bytes.Length, bytes);
    }
}

