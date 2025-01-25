using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Prog.Services
{
    public class DBManager : IDBManager
    {
        private readonly PasswordHasher<string> _passwordHasher;
        private SqliteConnection? connection = null;
        private readonly ILogger<DBManager> _logger;

        public DBManager(ILogger<DBManager> logger)
        {
            _logger = logger;
            _passwordHasher = new PasswordHasher<string>();
        }

        public bool ConnectToDB(string path)
        {
            try
            {
                connection = new SqliteConnection("Data Source=" + path);
                connection.Open();

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    _logger.LogError("Failed to open connection to database.");
                    return false;
                }

                _logger.LogInformation("Connected to database successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to database.");
                return false;
            }
        }

        public void Disconnect()
        {
            if (connection == null)
                return;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    _logger.LogInformation("Disconnected from database.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting from database.");
            }
            finally
            {
                connection = null;
            }
        }

        public bool AddUser(string login, string clientHashedPassword)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                _logger.LogError("Database connection is not open.");
                return false;
            }
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(clientHashedPassword))
            {
                _logger.LogError("Login or password is empty.");
                return false;
            }
            if (CheckIfUserExists(login))
            {
                _logger.LogWarning($"User with login '{login}' already exists.");
                return false;
            }
            string serverHashedPassword = _passwordHasher.HashPassword(login, clientHashedPassword);
            string REQUEST = "INSERT INTO users (Login, Password) VALUES (@Login, @Password)";
            var command = new SqliteCommand(REQUEST, connection);
            command.Parameters.AddWithValue("@Login", login);
            command.Parameters.AddWithValue("@Password", serverHashedPassword);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user.");
                return false;
            }
        }

        public bool CheckUser(string login, string clientHashedPassword)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                _logger.LogError("Database connection is not open.");
                return false;
            }

            string REQUEST = "SELECT Password FROM users WHERE Login = @Login";
            var command = new SqliteCommand(REQUEST, connection);
            command.Parameters.AddWithValue("@Login", login);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var serverHashedPassword = reader["Password"].ToString();
                    var result = _passwordHasher.VerifyHashedPassword(login, serverHashedPassword, clientHashedPassword);
                    return result == PasswordVerificationResult.Success;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user.");
                return false;
            }
        }

        public bool DeleteUser(string login)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                _logger.LogError("Database connection is not open.");
                return false;
            }

            if (!CheckIfUserExists(login))
            {
                _logger.LogWarning($"User with login '{login}' does not exist.");
                return false;
            }

            string REQUEST = "DELETE FROM users WHERE Login = @Login";
            var command = new SqliteCommand(REQUEST, connection);
            command.Parameters.AddWithValue("@Login", login);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user.");
                return false;
            }
        }

        public bool UpdatePassword(string login, string newPassword)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                _logger.LogError("Database connection is not open.");
                return false;
            }

            if (!CheckIfUserExists(login))
            {
                _logger.LogWarning($"User with login '{login}' does not exist.");
                return false;
            }

            // Хешируем новый пароль перед сохранением
            string hashedPassword = _passwordHasher.HashPassword(login, newPassword);

            string REQUEST = "UPDATE users SET Password = @Password WHERE Login = @Login";
            var command = new SqliteCommand(REQUEST, connection);
            command.Parameters.AddWithValue("@Login", login);
            command.Parameters.AddWithValue("@Password", hashedPassword);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password.");
                return false;
            }
        }

        private bool CheckIfUserExists(string login)
        {
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                _logger.LogError("Database connection is not open.");
                return false;
            }

            string REQUEST = "SELECT COUNT(*) FROM users WHERE Login = @Login";
            var command = new SqliteCommand(REQUEST, connection);
            command.Parameters.AddWithValue("@Login", login);

            try
            {
                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists.");
                return false;
            }
        }
    }
}