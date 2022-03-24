namespace WebApplication.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public bool ConfirmPasswordSuccess { get; set; }
    }
}