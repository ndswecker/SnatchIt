namespace SnatchItAPI.Models
{
    public partial class CaptureRecordDto
    {
        public int BandNumber { get; set; }
        public string BandSize { get; set; } = "";
        public string Bander { get; set; } = "";
        public string Scribe { get; set; } = "";
        public string Code { get; set; } = "";
        public string SpeciesCommon { get; set; } = "";
        public string SpeciesAlpha { get; set; } = "";
        public DateTime SheetDate { get; set; }
        public string Station { get; set; } = "";
        public string Net { get; set; } = "";
        public string CloacaShape { get; set; } = "";
        public int CPScore { get; set; }
        public int BPScore { get; set; }
        public int FatScore { get; set; }
        public int BodyMoltScore { get; set; } 
        public string FFMolt { get; set;} = "";
        public int FFWearScore { get; set; }
        public int JuvBodyPlumScore { get; set;}
        public int SkullScore { get; set; }
        public int WingChord { get; set; }
        public char Sex { get; set; } = 'U';
        public string AgeYear { get; set; } = "";
        public string AgeWRP { get; set; } = "";
        public decimal BodyMass { get; set; }
        public string Notes { get; set; } = "";
        public int ReleaseCondition { get; set;}
    }

}