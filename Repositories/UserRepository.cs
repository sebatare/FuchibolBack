using System.Data;
using Fuchibol.ChatService.Models;
using Microsoft.Data.SqlClient;

namespace Fuchibol.ChatService.Repositories
{
    public class UserRepository
    {
        string connectionString;

        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            User user;

            var data = GetUserDetailsFromDb();
            foreach (DataRow row in data.Rows)
            {
                user = new User
                {
                    Id = row["Id"].ToString(),
                    Name = row["Name"].ToString(),
                    Age = Convert.ToInt32(row["Age"])
                };
                users.Add(user);
            }
            return users;
        }
        public DataTable GetUserDetailsFromDb()
        {
            var query = "SELECT Id, Name, Age FROM [User]";
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dataTable.Load(reader);
                        }
                    }
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}