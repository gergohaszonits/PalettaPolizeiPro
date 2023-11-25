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
            }
        };

        public static bool ProgramRunning = true;
    }
}
