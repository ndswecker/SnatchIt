namespace SnatchItAPI.Models
{
    public partial class BanderForRegistrationDto : BanderForLoginDto
    {
        public string PasswordConfirm { get; set;} = "";

    }
}