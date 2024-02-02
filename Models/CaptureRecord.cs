namespace SnatchItAPI.Models
{
    public partial class CaptureRecord 
    {
        public int SheetId { get; set; }
        public int BandNumber { get; set; }
        public string BandSize { get; set; } = "";
        public string Scribe { get; set; } = "";
        public string SpeciesCommon { get; set; } = "";
        public string SpeciesAlpha { get; set; } = "";
        public DateTime SheetDate { get; set; }
        public string Station { get; set; } = "";
        public string Net { get; set; } = "";
        public int WingChord { get; set; }
        public char Sex { get; set; } = 'U';
        public string AgeYear { get; set; } = "";
        public string AgeWRP { get; set; } = "";
        public decimal BodyMass { get; set; }
        public string Notes { get; set; } = "";
    }

}