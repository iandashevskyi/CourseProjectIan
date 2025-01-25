namespace Prog.Services;

public interface IAuthJwt
{
    string LogIn(string login, string password);
}