namespace PalettaPolizeiPro.Data.Palettas
{
    public class OrderEventArgs
    {
        public required Order Order { get; set; }
        public required ChangeState State { get; set; }
        public required DateTime Time { get; set; }

    }
}
