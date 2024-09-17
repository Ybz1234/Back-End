using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesAirport
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string allAirportsQuery = "SELECT A.Id, A.Name, A.City_Id, A.Country_Id, " +
                                                          "C.Id AS CityId, C.Name AS CityName, " +
                                                          "CO.Id AS CountryId, CO.Name AS CountryName " +
                                                          "FROM Airport A " +
                                                          "INNER JOIN City C ON A.City_Id = C.Id " +
                                                          "INNER JOIN Country CO ON A.Country_Id = CO.Id";
        private static readonly string airportByIdQuery = "SELECT A.Id, A.Name, A.City_Id, A.Country_Id, " +
                                                          "C.Id AS CityId, C.Name AS CityName, " +
                                                          "CO.Id AS CountryId, CO.Name AS CountryName " +
                                                          "FROM Airport A " +
                                                          "INNER JOIN City C ON A.City_Id = C.Id " +
                                                          "INNER JOIN Country CO ON A.Country_Id = CO.Id " +
                                                          "WHERE A.Id = @Id";
        private static readonly string insertAirportQuery = "INSERT INTO Airport (Name, City_Id, Country_Id) OUTPUT INSERTED.Id VALUES (@Name, @City_Id, @Country_Id)";
        private static readonly string updateAirportQuery = "UPDATE Airport SET Name = @Name, City_Id = @City_Id, Country_Id = @Country_Id WHERE Id = @Id";
        private static readonly string deleteAirportQuery = "DELETE FROM Airport WHERE Id = @Id";

        public static List<Airport> GetAllAirports()
        {
            List<Airport> airports = new List<Airport>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allAirportsQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Airport airport = new Airport
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        City = new City
                        {
                            Id = Convert.ToInt32(reader["CityId"]),
                            Name = reader["CityName"].ToString()
                        },
                        Country = new Country
                        {
                            Id = Convert.ToInt32(reader["CountryId"]),
                            Name = reader["CountryName"].ToString()
                        }
                    };
                    airports.Add(airport);
                }
                reader.Close();
            }

            return airports;
        }

        public static Airport GetAirportById(int id)
        {
            Airport airport = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(airportByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    airport = new Airport
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        City = new City
                        {
                            Id = Convert.ToInt32(reader["CityId"]),
                            Name = reader["CityName"].ToString()
                        },
                        Country = new Country
                        {
                            Id = Convert.ToInt32(reader["CountryId"]),
                            Name = reader["CountryName"].ToString()
                        }
                    };
                }
                reader.Close();
            }

            return airport;
        }

        public static int InsertAirport(Airport airport)
        {
            airport.City.Id = GetOrInsertCity(airport.City);
            airport.Country.Id = GetOrInsertCountry(airport.Country);

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertAirportQuery, connection);
                command.Parameters.AddWithValue("@Name", airport.Name);
                command.Parameters.AddWithValue("@City_Id", airport.City.Id);
                command.Parameters.AddWithValue("@Country_Id", airport.Country.Id);

                connection.Open();
                airport.Id = (int)command.ExecuteScalar();
            }

            return airport.Id;
        }

        public static int GetOrInsertCity(City city)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                // Check if the city already exists
                string selectCityQuery = "SELECT Id FROM City WHERE Name = @Name";
                SqlCommand selectCityCommand = new SqlCommand(selectCityQuery, connection);
                selectCityCommand.Parameters.AddWithValue("@Name", city.Name);

                connection.Open();
                var cityId = selectCityCommand.ExecuteScalar();

                if (cityId != null)
                {
                    return (int)cityId;
                }

                // Insert the city if it doesn't exist
                string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                SqlCommand insertCityCommand = new SqlCommand(insertCityQuery, connection);
                insertCityCommand.Parameters.AddWithValue("@Name", city.Name);

                city.Id = (int)insertCityCommand.ExecuteScalar();
            }

            return city.Id;
        }

        public static int GetOrInsertCountry(Country country)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                // Check if the country already exists
                string selectCountryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                SqlCommand selectCountryCommand = new SqlCommand(selectCountryQuery, connection);
                selectCountryCommand.Parameters.AddWithValue("@Name", country.Name);

                connection.Open();
                var countryId = selectCountryCommand.ExecuteScalar();

                if (countryId != null)
                {
                    return (int)countryId;
                }

                // Insert the country if it doesn't exist
                string insertCountryQuery = "INSERT INTO Country (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                SqlCommand insertCountryCommand = new SqlCommand(insertCountryQuery, connection);
                insertCountryCommand.Parameters.AddWithValue("@Name", country.Name);

                country.Id = (int)insertCountryCommand.ExecuteScalar();
            }

            return country.Id;
        }


        public static int UpdateAirport(Airport airport)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateAirportQuery, connection);
                command.Parameters.AddWithValue("@Id", airport.Id);
                command.Parameters.AddWithValue("@Name", airport.Name);
                command.Parameters.AddWithValue("@City_Id", airport.City.Id);
                command.Parameters.AddWithValue("@Country_Id", airport.Country.Id);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public static bool DeleteAirport(int id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteAirportQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
