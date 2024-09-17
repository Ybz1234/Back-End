using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesUsers
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private static readonly string allUsersQuery = "SELECT * FROM Users";
        private static readonly string userByIdQuery = "SELECT * FROM Users WHERE Id = @Id";
        private static readonly string insertUserQuery = "INSERT INTO Users (First_Name, Last_Name, Mail_Address, Passwd) OUTPUT INSERTED.Id VALUES (@First_Name, @Last_Name, @Mail_Address, @Passwd)";
        private static readonly string updateUserQuery = "UPDATE Users SET First_Name = @First_Name, Last_Name = @Last_Name, Mail_Address = @Mail_Address, Passwd = @Passwd WHERE Id = @Id";
        private static readonly string deleteUserQuery = "DELETE FROM Users WHERE Id = @Id";

        public static string GetSqlConnectionStr()
        {
            return sqlConnectionStr;
        }

        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allUsersQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["First_Name"].ToString(),
                        LastName = reader["Last_Name"].ToString(),
                        Email = reader["Mail_Address"].ToString(),
                        Password = reader["Passwd"].ToString()
                    };
                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }

        public static User GetUserById(int id)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(userByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["First_Name"].ToString(),
                        LastName = reader["Last_Name"].ToString(),
                        Email = reader["Mail_Address"].ToString()
                    };
                }
                reader.Close();
            }

            return user;
        }

        public static int InsertUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertUserQuery, connection);
                command.Parameters.AddWithValue("@First_Name", user.FirstName);
                command.Parameters.AddWithValue("@Last_Name", user.LastName);
                command.Parameters.AddWithValue("@Mail_Address", user.Email);
                command.Parameters.AddWithValue("@Passwd", user.Password);
                connection.Open();
                user.Id = (int)command.ExecuteScalar();
            }

            return user.Id;
        }

        public static int UpdateUser(User user)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateUserQuery, connection);
                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@First_Name", user.FirstName);
                command.Parameters.AddWithValue("@Last_Name", user.LastName);
                command.Parameters.AddWithValue("@Mail_Address", user.Email);
                command.Parameters.AddWithValue("@Passwd", user.Password);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public static bool DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteUserQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
