using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesCountry
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string allCountriesQuery = "SELECT * FROM Country";
        private static readonly string CountryByIdQuery = "SELECT * FROM Country WHERE Id = @Id";
        private static readonly string insertCountryQuery = "INSERT INTO Country (Name) OUTPUT INSERTED.Id VALUES (@Name)";
        private static readonly string updateCountryQuery = "UPDATE Country SET Name = @Name WHERE Id = @Id";
        private static readonly string deleteCountryQuery = "DELETE FROM Country WHERE Id = @Id";

        public static List<Country> GetAllCountries()
        {
            List<Country> countries = new List<Country>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allCountriesQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Country country = new Country
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString()
                    };
                    countries.Add(country);
                }
                reader.Close();
            }

            return countries;
        }

        public static Country GetCountryById(int id)
        {
            Country country = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(CountryByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    country = new Country
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString()
                    };
                }
                reader.Close();
            }
            return country;
        }

        public static int InsertCountry(Country country)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertCountryQuery, connection);
                command.Parameters.AddWithValue("@Name", country.Name);
                connection.Open();
                country.Id = (int)command.ExecuteScalar();
            }

            return country.Id;
        }

        public static int UpdateCountry(Country country)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateCountryQuery, connection);
                command.Parameters.AddWithValue("@Id", country.Id);
                command.Parameters.AddWithValue("@Name", country.Name);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public static bool DeleteCountry(int id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteCountryQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
