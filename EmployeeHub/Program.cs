using System;
using System.Data;
using System.Data.SqlClient;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            try
            {
                HandleUserChoice(choice);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Ошибка базы данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("1. Добавить нового сотрудника");
        Console.WriteLine("2. Посмотреть всех сотрудников");
        Console.WriteLine("3. Обновить информацию о сотруднике");
        Console.WriteLine("4. Удалить сотрудника");
        Console.WriteLine("5. Выйти из приложения");
        Console.Write("Выберите опцию: ");
    }

    private static void HandleUserChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                AddEmployee();
                break;
            case "2":
                ViewEmployees();
                break;
            case "3":
                UpdateEmployee();
                break;
            case "4":
                DeleteEmployee();
                break;
            case "5":
                Cleanup();
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Некорректный выбор. Пожалуйста, попробуйте снова.");
                break;
        }
    }

    private static void AddEmployee()
    {
        var employeeData = GetEmployeeDataFromUser();
        string query = "INSERT INTO Employees (FirstName, LastName, Email, DateOfBirth, Salary) VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @Salary)";
        DatabaseManager.Instance.ExecuteNonQuery(query, employeeData);
        Console.WriteLine("Сотрудник добавлен.");
    }

    private static void ViewEmployees()
    {
        string query = "SELECT * FROM Employees";
        using (var reader = DatabaseManager.Instance.ExecuteReader(query))
        {
            Console.WriteLine("\nСписок сотрудников:");
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["EmployeeID"]}, Имя: {reader["FirstName"]}, Фамилия: {reader["LastName"]}, Email: {reader["Email"]}, Дата Рождения: {reader["DateOfBirth"]}, Зарплата: {reader["Salary"]}");
            }
        }
    }

    private static void UpdateEmployee()
    {
        int employeeId = GetValidInt("Введите ID сотрудника для обновления: ");
        var employeeData = GetEmployeeDataFromUser(isUpdating: true);

        string query = @"
UPDATE Employees 
SET FirstName = COALESCE(NULLIF(@FirstName, ''), FirstName),
    LastName = COALESCE(NULLIF(@LastName, ''), LastName),
    Email = COALESCE(NULLIF(@Email, ''), Email),
    DateOfBirth = COALESCE(NULLIF(@DateOfBirth, ''), DateOfBirth),
    Salary = COALESCE(NULLIF(@Salary, ''), Salary)
WHERE EmployeeID = @EmployeeID";

        var parameters = new
        {
            FirstName = employeeData.FirstName,
            LastName = employeeData.LastName,
            Email = employeeData.Email,
            DateOfBirth = (object)employeeData.DateOfBirth ?? DBNull.Value,
            Salary = (object)employeeData.Salary ?? DBNull.Value,
            EmployeeID = employeeId
        };

        DatabaseManager.Instance.ExecuteNonQuery(query, parameters);
        Console.WriteLine("Информация о сотруднике обновлена.");
    }

    private static void DeleteEmployee()
    {
        int employeeId = GetValidInt("Введите ID сотрудника для удаления: ");
        string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
        DatabaseManager.Instance.ExecuteNonQuery(query, new { EmployeeID = employeeId });
        Console.WriteLine("Сотрудник удален.");
    }

    private static (string FirstName, string LastName, string Email, DateTime? DateOfBirth, decimal? Salary) GetEmployeeDataFromUser(bool isUpdating = false)
    {
        string firstName = isUpdating ? GetNullableInput("Введите новое имя (или 'Сбросить' для сброса): ") : GetStringInput("Введите имя: ");
        string lastName = isUpdating ? GetNullableInput("Введите новую фамилию (или 'Сбросить' для сброса): ") : GetStringInput("Введите фамилию: ");
        string email = isUpdating ? GetNullableInput("Введите новый email (или 'Сбросить' для сброса): ") : GetStringInput("Введите адрес электронной почты: ");
        DateTime? dateOfBirth = GetValidNullableDateTime("Введите новую дату рождения (dd.mm.yyyy) (или оставьте пустым для пропуска, 'Сбросить' для сброса): ");
        decimal? salary = GetValidNullableDecimal("Введите новую зарплату (или оставьте пустым для пропуска, 'Сбросить' для сброса): ");
        return (firstName, lastName, email, dateOfBirth, salary);
    }

    private static string GetNullableInput(string prompt)
    {
        string input = GetStringInput(prompt);
        return input.Equals("Сбросить", StringComparison.OrdinalIgnoreCase) ? null : input;
    }

    private static string GetStringInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    private static DateTime? GetValidNullableDateTime(string prompt)
    {
        string input = "";
        while (true)
        {
            Console.Write(prompt);
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return null; // Если пустое значение
            }
            if (DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            Console.WriteLine("Некорректный формат даты. Пожалуйста, попробуйте снова.");
        }
    }

    private static decimal? GetValidNullableDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return null; // Если пустое значение
            }
            if (decimal.TryParse(input, out decimal result))
            {
                return result;
            }
            Console.WriteLine("Некорректный формат. Пожалуйста, попробуйте снова.");
        }
    }

    private static int GetValidInt(string prompt)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out result))
            {
                return result;
            }
            Console.WriteLine("Некорректный формат. Пожалуйста, попробуйте снова.");
        }
    }

    private static void Cleanup()
    {
        DatabaseManager.Instance.Dispose();
    }
}