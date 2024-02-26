namespace PalettaPolizeiPro.Data.Palettas
{
    public class QueryState : EntityObject
    {
        public string? PalettaName { get; set; }
        public byte? OperationStatus { get; set; }
        public byte? ControlFlag { get; set; }
    }
}
