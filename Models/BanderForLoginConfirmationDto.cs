namespace SnatchItAPI
{
    public partial class BanderForLoginConfirmation
    {
        byte[] PasswordHash { get; set;} = [];
        byte[] PasswordSalt {get; set; } = [];
    }
}