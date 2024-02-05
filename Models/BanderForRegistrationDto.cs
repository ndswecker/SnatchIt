namespace SnatchItAPI.Models
{
    public partial class BanderForRegistrationDto : BanderForLoginDto
    {
        public string PasswordConfirm { get; set;} = "";
        public string FirstName { get; set;} = "";
        public string LastName { get; set;} = "";

    }
}