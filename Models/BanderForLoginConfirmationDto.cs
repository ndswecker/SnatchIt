namespace SnatchItAPI
{
    public partial class BanderForLoginConfirmation
    {
        public byte[] PasswordHash { get; set;} = [];
        public byte[] PasswordSalt {get; set; } = [];
    }
}