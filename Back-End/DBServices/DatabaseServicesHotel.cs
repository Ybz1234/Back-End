using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesHotel
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string allHotelsQuery = "SELECT H.Id, H.Name, H.Address, H.City_Id, H.Country_Id, " +
                                                        "C.Id AS CityId, C.Name AS CityName, " +
                                                        "CO.Id AS CountryId, CO.Name AS CountryName " +
                                                        "FROM Hotel H " +
                                                        "INNER JOIN City C ON H.City_Id = C.Id " +
                                                        "INNER JOIN Country CO ON H.Country_Id = CO.Id";
        private static readonly string hotelByIdQuery = "SELECT H.Id, H.Name, H.Address, H.City_Id, H.Country_Id, " +
                                                        "C.Id AS CityId, C.Name AS CityName, " +
                                                        "CO.Id AS CountryId, CO.Name AS CountryName " +
                                                        "FROM Hotel H " +
                                                        "INNER JOIN City C ON H.City_Id = C.Id " +
                                                        "INNER JOIN Country CO ON H.Country_Id = CO.Id " +
                                                        "WHERE H.Id = @Id";
        private static readonly string hotelsByCitiesQuery = @"
            SELECT H.Id, H.Name, H.Address, C.Name AS CityName, CO.Name AS CountryName
            FROM Hotel H
            INNER JOIN City C ON H.City_Id = C.Id
            INNER JOIN Country CO ON H.Country_Id = CO.Id
            WHERE C.Name IN (@Cities)";
        private static readonly string insertHotelQuery = "INSERT INTO Hotel (Name, Address, City_Id, Country_Id) OUTPUT INSERTED.Id VALUES (@Name, @Address, @City_Id, @Country_Id)";
        private static readonly string updateHotelQuery = "UPDATE Hotel SET Name = @Name, Address = @Address, City_Id = @City_Id, Country_Id = @Country_Id WHERE Id = @Id";
        private static readonly string deleteHotelQuery = "DELETE FROM Hotel WHERE Id = @Id";

        public static List<Hotel> GetAllHotels()
        {
            List<Hotel> hotels = new List<Hotel>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(allHotelsQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Hotel hotel = new Hotel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
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
                    hotels.Add(hotel);
                }
                reader.Close();
            }

            return hotels;
        }

        public static Hotel GetHotelById(int id)
        {
            Hotel hotel = null;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(hotelByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    hotel = new Hotel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
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

            return hotel;
        }

        public static List<Hotel> GetHotelsByCities(string[] cities)
        {
            List<Hotel> hotels = new List<Hotel>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                // Build dynamic query with parameters
                var citiesParam = string.Join(",", cities.Select((_, i) => $"@City{i}"));

                string query = hotelsByCitiesQuery.Replace("@Cities", citiesParam);
                SqlCommand command = new SqlCommand(query, connection);

                for (int i = 0; i < cities.Length; i++)
                {
                    command.Parameters.AddWithValue($"@City{i}", cities[i]);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Hotel hotel = new Hotel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = new City
                        {
                            Name = reader["CityName"].ToString()
                        },
                        Country = new Country
                        {
                            Name = reader["CountryName"].ToString()
                        }
                    };
                    hotels.Add(hotel);
                }
                reader.Close();
            }

            return hotels;
        }

        public static int InsertHotel(Hotel hotel)
        {
            hotel.City.Id = GetOrInsertCity(hotel.City);
            hotel.Country.Id = GetOrInsertCountry(hotel.Country);

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(insertHotelQuery, connection);
                command.Parameters.AddWithValue("@Name", hotel.Name);
                command.Parameters.AddWithValue("@Address", hotel.Address);
                command.Parameters.AddWithValue("@City_Id", hotel.City.Id);
                command.Parameters.AddWithValue("@Country_Id", hotel.Country.Id);

                connection.Open();
                hotel.Id = (int)command.ExecuteScalar();
            }

            return hotel.Id;
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


        public static int UpdateHotel(Hotel hotel)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(updateHotelQuery, connection);
                command.Parameters.AddWithValue("@Id", hotel.Id);
                command.Parameters.AddWithValue("@Name", hotel.Name);
                command.Parameters.AddWithValue("@Address", hotel.Address);
                command.Parameters.AddWithValue("@City_Id", hotel.City.Id);
                command.Parameters.AddWithValue("@Country_Id", hotel.Country.Id);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            return rowsAffected;
        }

        public static bool DeleteHotel(int id)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionStr))
            {
                SqlCommand command = new SqlCommand(deleteHotelQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
