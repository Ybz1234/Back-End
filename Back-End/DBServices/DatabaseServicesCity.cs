using Back_End.Models;
using System.Data.SqlClient;

namespace BackEndDemo.DBServices
{
    public class DatabaseServicesCity
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string allCitiesQuery = "SELECT * FROM City";
        private static readonly string cityByIdQuery = "SELECT * FROM City WHERE Id = @Id";
        private static readonly string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
        private static readonly string updateCityQuery = "UPDATE City SET Name = @Name WHERE Id = @Id";
        private static readonly string deleteCityQuery = "DELETE FROM City WHERE Id = @Id";

        public static List<City> GetAllCities()
        {
            List<City> cities = new List<City>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allCitiesQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    City city = new City
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString()
                    };
                    cities.Add(city);
                }
                reader.Close();
            }

            return cities;
        }

        public static City GetCityById(int id)
        {
            City city = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(cityByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    city = new City
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString()
                    };
                }
                reader.Close();
            }

            return city;
        }

        public static int InsertCity(City city)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertCityQuery, connection);
                command.Parameters.AddWithValue("@Name", city.Name);
                connection.Open();
                city.Id = (int)command.ExecuteScalar();
            }

            return city.Id;
        }

        public static int UpdateCity(City city)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateCityQuery, connection);
                command.Parameters.AddWithValue("@Id", city.Id);
                command.Parameters.AddWithValue("@Name", city.Name);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public static bool DeleteCity(int id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteCityQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
