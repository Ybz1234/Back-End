using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesAttraction
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string insertAttractionQuery = "INSERT INTO Attraction_Recommendation_Details (Name, City_Id, Country_Id) OUTPUT INSERTED.Id VALUES (@Name, @City_Id, @Country_Id)";
        private static readonly string getAllAttractionsByCityQuery = "SELECT A.Id, A.Name, C.Id AS CityId, C.Name AS CityName, CO.Id AS CountryId, CO.Name AS CountryName " +
                                                                      "FROM Attraction_Recommendation_Details A " +
                                                                      "INNER JOIN City C ON A.City_Id = C.Id " +
                                                                      "INNER JOIN Country CO ON A.Country_Id = CO.Id " +
                                                                      "WHERE C.Id = @City_Id";
        private static readonly string getAllAttractionsByCountryQuery = "SELECT A.Id, A.Name, C.Id AS CityId, C.Name AS CityName, CO.Id AS CountryId, CO.Name AS CountryName " +
                                                                         "FROM Attraction_Recommendation_Details A " +
                                                                         "INNER JOIN City C ON A.City_Id = C.Id " +
                                                                         "INNER JOIN Country CO ON A.Country_Id = CO.Id " +
                                                                         "WHERE CO.Id = @Country_Id";
        private static readonly string getAttractionByNameQuery = "SELECT A.Id, A.Name, C.Id AS CityId, C.Name AS CityName, CO.Id AS CountryId, CO.Name AS CountryName " +
                                                                  "FROM Attraction_Recommendation_Details A " +
                                                                  "INNER JOIN City C ON A.City_Id = C.Id " +
                                                                  "INNER JOIN Country CO ON A.Country_Id = CO.Id " +
                                                                  "WHERE A.Name = @Name";
        private static readonly string updateAttractionQuery = "UPDATE Attraction_Recommendation_Details SET Name = @Name, City_Id = @City_Id, Country_Id = @Country_Id WHERE Id = @Id";
        private static readonly string deleteAttractionQuery = "DELETE FROM Attraction_Recommendation_Details WHERE Id = @Id";

        public static int InsertAttraction(Attraction attraction)
        {
            attraction.City.Id = GetOrInsertCity(attraction.City);
            attraction.Country.Id = GetOrInsertCountry(attraction.Country);

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertAttractionQuery, connection);
                command.Parameters.AddWithValue("@Name", attraction.Name);
                command.Parameters.AddWithValue("@City_Id", attraction.City.Id);
                command.Parameters.AddWithValue("@Country_Id", attraction.Country.Id);

                connection.Open();
                attraction.Id = (int)command.ExecuteScalar();
            }

            return attraction.Id;
        }

        private static int GetOrInsertCity(City city)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                string selectCityQuery = "SELECT Id FROM City WHERE Name = @Name";
                SqlCommand selectCityCommand = new SqlCommand(selectCityQuery, connection);
                selectCityCommand.Parameters.AddWithValue("@Name", city.Name);

                connection.Open();
                var cityId = selectCityCommand.ExecuteScalar();

                if (cityId != null)
                {
                    return (int)cityId;
                }

                string insertCityQuery = "INSERT INTO City (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                SqlCommand insertCityCommand = new SqlCommand(insertCityQuery, connection);
                insertCityCommand.Parameters.AddWithValue("@Name", city.Name);

                city.Id = (int)insertCityCommand.ExecuteScalar();
            }

            return city.Id;
        }

        private static int GetOrInsertCountry(Country country)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                string selectCountryQuery = "SELECT Id FROM Country WHERE Name = @Name";
                SqlCommand selectCountryCommand = new SqlCommand(selectCountryQuery, connection);
                selectCountryCommand.Parameters.AddWithValue("@Name", country.Name);

                connection.Open();
                var countryId = selectCountryCommand.ExecuteScalar();

                if (countryId != null)
                {
                    return (int)countryId;
                }

                string insertCountryQuery = "INSERT INTO Country (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                SqlCommand insertCountryCommand = new SqlCommand(insertCountryQuery, connection);
                insertCountryCommand.Parameters.AddWithValue("@Name", country.Name);

                country.Id = (int)insertCountryCommand.ExecuteScalar();
            }

            return country.Id;
        }

        public static List<Attraction> GetAllAttractionsByCity(int cityId)
        {
            List<Attraction> attractions = new List<Attraction>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getAllAttractionsByCityQuery, connection);
                command.Parameters.AddWithValue("@City_Id", cityId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Attraction attraction = new Attraction
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
                        attractions.Add(attraction);
                    }
                    reader.Close();
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw new Exception("Database query error: " + sqlEx.Message, sqlEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    throw new Exception("General error: " + ex.Message, ex);
                }
            }

            return attractions;
        }


        public static List<Attraction> GetAllAttractionsByCountry(int countryId)
        {
            List<Attraction> attractions = new List<Attraction>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getAllAttractionsByCountryQuery, connection);
                command.Parameters.AddWithValue("@Country_Id", countryId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Attraction attraction = new Attraction
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
                    attractions.Add(attraction);
                }
                reader.Close();
            }

            return attractions;
        }

        public static Attraction GetAttractionByName(string name)
        {
            Attraction attraction = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(getAttractionByNameQuery, connection);
                command.Parameters.AddWithValue("@Name", name);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    attraction = new Attraction
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

            return attraction;
        }

        public static int UpdateAttraction(Attraction attraction)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateAttractionQuery, connection);
                command.Parameters.AddWithValue("@Id", attraction.Id);
                command.Parameters.AddWithValue("@Name", attraction.Name);
                command.Parameters.AddWithValue("@City_Id", attraction.City.Id);
                command.Parameters.AddWithValue("@Country_Id", attraction.Country.Id);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public static bool DeleteAttraction(int id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteAttractionQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
