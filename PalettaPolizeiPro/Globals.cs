global using static PalettaPolizeiPro.Globals;
using MudBlazor;
namespace PalettaPolizeiPro
{
    public static class Globals
    {
        
        public static MudTheme MainTheme = new MudTheme
        {
            Palette = new Palette
            {
                Primary = Colors.BlueGrey.Darken1,
                Secondary = Colors.BlueGrey.Darken4,
                Error = Colors.Purple.Darken4,
                Dark = Colors.Grey.Darken4,
                
            }
        };

        public static bool ProgramRunning = true;
        public static readonly bool SIMULATION = true;

        public static readonly int PALETTA_CHECK_DATABLOCK_INDEX = 0;
        public static readonly int PALETTA_CHECK_DATABLOCK_SIZE = 16;

        public static readonly int PALETTA_CHECK_ACTUALCYCLE_INDEX= 12; //int
        public static readonly int PALETTA_CHECK_PREDEFINIED_INDEX = 14; //int


        public static readonly int MOKANY_INDEX = 240;
        public static readonly int MOKANY_SIZE = 9;

    }
}
