namespace SnatchItAPI.Models
{
    public partial class Bander
    {
        public int BanderId { get; set;}
        public string FirstName { get; set;} = "";
        public string LastName { get; set;} = "";
        public string Email { get; set;} = "";
        public string Initials {get; set;} = "";
    }
}