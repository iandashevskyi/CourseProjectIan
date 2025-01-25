namespace Prog.Model;

public class ChangePasswordRequest
{
    public string Login { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}