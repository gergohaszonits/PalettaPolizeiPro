﻿using System.ComponentModel.DataAnnotations.Schema;

namespace PalettaPolizeiPro.Data.Palettas
{
    public class PalettaProperty : EntityObject
    {
        public required string Identifier { get; set; }
        public long PalettaId { get; set; }
        public Paletta Paletta { get; set; }
        public int PredefiniedCycle { get; set; }
        public int ActualCycle { get; set; }
        public string? EngineNumber { get; set; }
        public DateTime ReadTime { get; set; }

        [NotMapped]
        public float ServicePercentage
        {
            get
            {
                if (PredefiniedCycle == 0 || ActualCycle == 0) { return 0; }
                float p = ActualCycle / (float)PredefiniedCycle * 100;
                return p;
            }
        }
    }
}
