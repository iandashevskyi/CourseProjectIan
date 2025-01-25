namespace Prog.Services;

public interface IDBManager
{
    bool ConnectToDB(string path);
    void Disconnect();
    bool AddUser(string login, string password);
    bool CheckUser(string login, string password);
    bool DeleteUser(string login);
    bool UpdatePassword(string login, string newPassword);
}