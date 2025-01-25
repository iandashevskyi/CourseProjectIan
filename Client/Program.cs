namespace SortClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    class Client
    {
        private static readonly HttpClient client = new HttpClient();
        private const string BaseUrl = "http://localhost:5190";
        private static string token = null;
        private static string login = null;
        private static List<string> userResults = new List<string>();
        private const string CookieFolder = "Cookies";

        static async Task Main(string[] args)
        {
            try
            {
                if (!Directory.Exists(CookieFolder))
                {
                    Directory.CreateDirectory(CookieFolder);
                }

                while (true)
                {
                    Console.Clear();
                    if (string.IsNullOrEmpty(token))
                    {
                        ShowGuestMenu();
                    }
                    else
                    {
                        ShowUserMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                PressAnyKeyToContinue();
            }
        }

        static void ShowGuestMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Register().Wait();
                    break;
                case "2":
                    Authenticate().Wait();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    PressAnyKeyToContinue();
                    break;
            }
        }

        static void ShowUserMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Send Array to Server");
            Console.WriteLine("2. Sort Array");
            Console.WriteLine("3. Get Sorted Array");
            Console.WriteLine("4. Get Sorted Part of Array");
            Console.WriteLine("5. Add Elements to Array");
            Console.WriteLine("6. Generate Random Array");
            Console.WriteLine("7. Clear Array");
            Console.WriteLine("8. Show Results History");
            Console.WriteLine("9. Change Password");
            Console.WriteLine("10. Delete User");
            Console.WriteLine("11. Logout");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SendArrayToServer().Wait();
                    break;
                case "2":
                    SortArray().Wait();
                    break;
                case "3":
                    GetSortedArray().Wait();
                    break;
                case "4":
                    GetSortedPartOfArray().Wait();
                    break;
                case "5":
                    AddElementsToArray().Wait();
                    break;
                case "6":
                    GenerateRandomArray().Wait();
                    break;
                case "7":
                    ClearArray().Wait();
                    break;
                case "8":
                    ShowUserResults();
                    break;
                case "9":
                    ChangePassword().Wait();
                    break;
                case "10":
                    DeleteUser().Wait();
                    break;
                case "11":
                    token = null;
                    login = null;
                    userResults.Clear();
                    Console.WriteLine("Logged out.");
                    PressAnyKeyToContinue();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    PressAnyKeyToContinue();
                    break;
            }
        }

        static async Task Register()
        {
            try
            {
                Console.Write("Enter username: ");
                var login = Console.ReadLine();
                Console.Write("Enter password: ");
                var password = Console.ReadLine();

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Username and password cannot be empty.");
                    return;
                }

                var hashedPassword = HashPassword(password);

                var registerData = new { Login = login, Password = hashedPassword };
                var content = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BaseUrl}/auth/signup", content);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Registration successful!");
                }
                else
                {
                    Console.WriteLine($"Registration error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task Authenticate()
        {
            try
            {
                Console.Write("Enter username: ");
                login = Console.ReadLine();
                Console.Write("Enter password: ");
                var password = Console.ReadLine();

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Username and password cannot be empty.");
                    return;
                }

                var hashedPassword = HashPassword(password);

                var authData = new { Login = login, Password = hashedPassword };
                var content = new StringContent(JsonConvert.SerializeObject(authData), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BaseUrl}/auth/login", content);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
                    token = loginResponse.Token;
                    Console.WriteLine("Login successful!");

                    LoadUserResults();
                }
                else
                {
                    Console.WriteLine($"Login error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task ChangePassword()
        {
            try
            {
                Console.Write("Enter old password: ");
                var oldPassword = Console.ReadLine();
                Console.Write("Enter new password: ");
                var newPassword = Console.ReadLine();

                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                {
                    Console.WriteLine("Old and new passwords cannot be empty.");
                    return;
                }

                if (oldPassword == newPassword)
                {
                    Console.WriteLine("New password must be different from the old password.");
                    return;
                }

                var hashedOldPassword = HashPassword(oldPassword);
                var hashedNewPassword = HashPassword(newPassword);

                var changePasswordData = new
                {
                    Login = login,
                    OldPassword = hashedOldPassword,
                    NewPassword = hashedNewPassword
                };

                var content = new StringContent(JsonConvert.SerializeObject(changePasswordData), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync($"{BaseUrl}/auth/changepassword", content);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Password changed successfully.");

                    token = null;
                    login = null;
                    userResults.Clear();

                    Console.WriteLine("Session ended. Please log in again with the new password.");
                    PressAnyKeyToContinue();

                    await Authenticate();
                }
                else
                {
                    Console.WriteLine($"Password change error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during password change: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task SendArrayToServer()
        {
            try
            {
                Console.Write("Enter numbers separated by commas: ");
                var input = Console.ReadLine();
                var numbers = input.Split(',').Select(int.Parse).ToArray();

                var arrayData = new { Array = numbers };
                var content = new StringContent(JsonConvert.SerializeObject(arrayData), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("Sending array to server...");
                var response = await client.PostAsync($"{BaseUrl}/array/send", content);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var serverResponse = JsonConvert.DeserializeObject<ArrayResponse>(responseData);
                    Console.WriteLine($"Server response: {serverResponse.Message}");
                    Console.WriteLine($"Received array: {string.Join(",", serverResponse.Array)}");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sending array: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task SortArray()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("Sending sorting request...");
                var response = await client.PostAsync($"{BaseUrl}/array/sort", null);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var sortResponse = JsonConvert.DeserializeObject<ArrayResponse>(responseData);
                    Console.WriteLine($"Sorted array: {string.Join(",", sortResponse.Array)}");
                }
                else
                {
                    Console.WriteLine($"Sorting error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sorting: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task GetSortedArray()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("Requesting sorted array...");
                var response = await client.GetAsync($"{BaseUrl}/array/sorted");
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var sortedResponse = JsonConvert.DeserializeObject<ArrayResponse>(responseData);
                    Console.WriteLine($"Sorted array: {string.Join(",", sortedResponse.Array)}");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during getting sorted array: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task AddElementsToArray()
{
    try
    {
        Console.Write("Enter elements to add separated by commas: ");
        var elements = Console.ReadLine().Split(',').Select(int.Parse).ToArray();
        Console.Write("Enter index to add after (leave empty to add to end): ");
        var indexInput = Console.ReadLine();
        int? index = string.IsNullOrEmpty(indexInput) ? null : (int?)int.Parse(indexInput);

        // Создаем объект запроса
        var addElementsData = new AddElementsRequest
        {
            Elements = elements,
            Index = index
        };

        // Сериализуем запрос в JSON
        var content = new StringContent(JsonConvert.SerializeObject(addElementsData), Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Создаем PATCH-запрос
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{BaseUrl}/array/addelements")
        {
            Content = content
        };

        Console.WriteLine("Adding elements to array...");
        var response = await client.SendAsync(request); // Отправляем PATCH-запрос
        var responseData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var addResponse = JsonConvert.DeserializeObject<ArrayResponse>(responseData);
            Console.WriteLine($"Updated array: {string.Join(",", addResponse.Array)}");
        }
        else
        {
            Console.WriteLine($"Error: {responseData}");
        }
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid input. Please enter valid integers for elements and index.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during adding elements: {ex.Message}");
    }
    finally
    {
        PressAnyKeyToContinue();
    }
}

        static async Task GenerateRandomArray()
        {
            try
            {
                Console.Write("Enter size of the random array: ");
                var size = int.Parse(Console.ReadLine());

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("Generating random array...");
                var response = await client.PostAsync($"{BaseUrl}/array/generate?size={size}", null);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var generateResponse = JsonConvert.DeserializeObject<ArrayResponse>(responseData);
                    Console.WriteLine($"Generated array: {string.Join(",", generateResponse.Array)}");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during generating random array: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task ClearArray()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("Clearing array...");
                var response = await client.DeleteAsync($"{BaseUrl}/array/clear");
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Array cleared successfully.");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during clearing array: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static void ShowUserResults()
        {
            try
            {
                if (userResults.Count == 0)
                {
                    Console.WriteLine("No results found.");
                }
                else
                {
                    Console.WriteLine($"Result history for user {login}:");
                    for (int i = 0; i < userResults.Count; i++)
                    {
                        Console.WriteLine($"Calculation {i + 1} - {userResults[i]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying results: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static async Task DeleteUser()
        {
            try
            {
                Console.Write("Are you sure you want to delete your account? (y/n): ");
                var confirm = Console.ReadLine();
                if (confirm.ToLower() == "y")
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var deleteData = new { Login = login };
                    var content = new StringContent(JsonConvert.SerializeObject(deleteData), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{BaseUrl}/auth/deleteuser", content);
                    var responseData = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("User deleted successfully.");
                        token = null;
                        login = null;
                        userResults.Clear();
                        DeleteUserCookie();
                        Console.WriteLine("User cookies deleted.");
                    }
                    else
                    {
                        Console.WriteLine($"User deletion error: {responseData}");
                    }
                }
                else
                {
                    Console.WriteLine("Deletion canceled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user deletion: {ex.Message}");
            }
            finally
            {
                PressAnyKeyToContinue();
            }
        }

        static void SaveUserResults()
        {
            try
            {
                var filePath = Path.Combine(CookieFolder, $"{login}.txt");
                File.WriteAllLines(filePath, userResults);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving results: {ex.Message}");
            }
        }

        static void LoadUserResults()
        {
            try
            {
                var filePath = Path.Combine(CookieFolder, $"{login}.txt");
                if (File.Exists(filePath))
                {
                    userResults = new List<string>(File.ReadAllLines(filePath));
                    Console.WriteLine("Results loaded from cookies.");
                }
                else
                {
                    Console.WriteLine("No cookies found for this user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading results: {ex.Message}");
            }
        }

        static void DeleteUserCookie()
        {
            try
            {
                var filePath = Path.Combine(CookieFolder, $"{login}.txt");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting cookies: {ex.Message}");
            }
        }

        static async Task GetSortedPartOfArray()
{
    try
    {
        Console.Write("Enter start index: ");
        var startIndex = int.Parse(Console.ReadLine());
        Console.Write("Enter end index: ");
        var endIndex = int.Parse(Console.ReadLine());

        // Проверка на валидность индексов
        if (startIndex < 0 || endIndex < startIndex)
        {
            Console.WriteLine("Invalid indices. Start index must be >= 0, and end index must be >= start index.");
            return;
        }

        // Создаем объект запроса
        var sortPartRequest = new SortPartRequest
        {
            StartIndex = startIndex,
            EndIndex = endIndex
        };

        // Сериализуем запрос в JSON
        var content = new StringContent(JsonConvert.SerializeObject(sortPartRequest), Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        Console.WriteLine("Requesting sorted part of array...");
        var response = await client.PostAsync($"{BaseUrl}/array/sortpart", content); // Используем POST-запрос
        var responseData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var sortedPartResponse = JsonConvert.DeserializeObject<ArrayResponse>(responseData);
            Console.WriteLine($"Sorted part of array: {string.Join(",", sortedPartResponse.Array)}");
        }
        else
        {
            Console.WriteLine($"Error: {responseData}");
        }
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid input. Please enter valid integers for start and end indices.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during getting sorted part of array: {ex.Message}");
    }
    finally
    {
        PressAnyKeyToContinue();
    }
}

        static void PressAnyKeyToContinue()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }

    public class SortResponse
    {
        public string SortedArray { get; set; }
    }

    public class ArrayResponse
    {
        public string Message { get; set; }
        public int[] Array { get; set; }
    }
    public class SortPartRequest
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
    public class AddElementsRequest
    {
        public int[] Elements { get; set; }
        public int? Index { get; set; } // Для вставки после указанного индекса
    }
}