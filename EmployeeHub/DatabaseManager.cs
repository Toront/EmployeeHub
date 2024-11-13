using System.Data.SqlClient;

public class DatabaseManager : IDisposable
{
    private static readonly Lazy<DatabaseManager> _instance = new Lazy<DatabaseManager>(() => new DatabaseManager());
    private readonly SqlConnection _connection;

    private DatabaseManager()
    {
        _connection = new SqlConnection("Server=your_server_name;Database=EmployeeDB;Trusted_Connection=True;");
        _connection.Open();
    }

    public static DatabaseManager Instance => _instance.Value;

    public int ExecuteNonQuery(string query, object parameters = null)
    {
        using (var command = new SqlCommand(query, _connection))
        {
            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters, null) ?? DBNull.Value);
                }
            }

            return command.ExecuteNonQuery();
        }
    }

    public SqlDataReader ExecuteReader(string query)
    {
        var command = new SqlCommand(query, _connection);
        return command.ExecuteReader(); // Не используем using, чтобы читатель мог быть возвращен
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}