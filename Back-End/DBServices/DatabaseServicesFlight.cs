using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesFlight
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string getAllFlightsQuery = "SELECT * FROM Flight";
        private static readonly string getFlightByIdQuery = "SELECT * FROM Flight WHERE Id = @Id";
        private static readonly string getFlightByDatesAndDestinations = @"
            SELECT f.Id, f.From_Time, f.Until_Time, 
                   depAirport.Id AS DepartureId, depAirport.Name AS DepartureName, 
                   arrAirport.Id AS ArrivalId, arrAirport.Name AS ArrivalName,
                   depCity.Id AS DepartureCityId, depCity.Name AS DepartureCityName,
                   arrCity.Id AS ArrivalCityId, arrCity.Name AS ArrivalCityName,
                   depCountry.Id AS DepartureCountryId, depCountry.Name AS DepartureCountryName,
                   arrCountry.Id AS ArrivalCountryId, arrCountry.Name AS ArrivalCountryName
            FROM Flight f
            INNER JOIN Airport depAirport ON f.Departure_Id = depAirport.Id
            INNER JOIN City depCity ON depAirport.City_Id = depCity.Id
            INNER JOIN Country depCountry ON depAirport.Country_Id = depCountry.Id
            INNER JOIN Airport arrAirport ON f.Arrival_Id = arrAirport.Id
            INNER JOIN City arrCity ON arrAirport.City_Id = arrCity.Id
            INNER JOIN Country arrCountry ON arrAirport.Country_Id = arrCountry.Id
            WHERE CAST(f.From_Time AS DATE) = @Date
            AND depCity.Name = @DepartureCity
            AND arrCity.Name = @ArrivalCity";
        private static readonly string createFlightQuery = "INSERT INTO Flight (Departure_Id, Arrival_Id, From_Time, Until_Time) VALUES (@Departure_Id, @Arrival_Id, @From_Time, @Until_Time)";
        private static readonly string updateFlightQuery = "UPDATE Flight SET Departure_Id = @Departure_Id, Arrival_Id = @Arrival_Id, From_Time = @From_Time, Until_Time = @Until_Time WHERE Id = @Id";
        private static readonly string deleteFlightQuery = "DELETE FROM Flight WHERE Id = @Id";

        public static List<Flight> GetAllFlights()
        {
            List<Flight> flights = new List<Flight>();

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(getAllFlightsQuery, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Flight flight = new Flight
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        From = GetAirportById(Convert.ToInt32(reader["Departure_Id"])),
                        To = GetAirportById(Convert.ToInt32(reader["Arrival_Id"])),
                        Departure = Convert.ToDateTime(reader["From_Time"]),
                        Arrival = Convert.ToDateTime(reader["Until_Time"])
                    };
                    flights.Add(flight);
                }
                reader.Close();
            }

            return flights;
        }

        public static Flight GetFlightById(int id)
        {
            Flight flight = null;

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(getFlightByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    flight = new Flight
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        From = GetAirportById(Convert.ToInt32(reader["Departure_Id"])),
                        To = GetAirportById(Convert.ToInt32(reader["Arrival_Id"])),
                        Departure = Convert.ToDateTime(reader["From_Time"]),
                        Arrival = Convert.ToDateTime(reader["Until_Time"])
                    };
                }
                reader.Close();
            }

            return flight;
        }

        public static List<Flight> GetFlightsByDateCityToCity(DateTime date, string departureCity, string arrivalCity)
        {
            List<Flight> flights = new List<Flight>();

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(getFlightByDatesAndDestinations, connection);
                command.Parameters.AddWithValue("@Date", date.Date);
                command.Parameters.AddWithValue("@DepartureCity", departureCity);
                command.Parameters.AddWithValue("@ArrivalCity", arrivalCity);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Flight flight = new Flight
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        From = new Airport
                        {
                            Id = Convert.ToInt32(reader["DepartureId"]),
                            Name = reader["DepartureName"].ToString(),
                            City = new City
                            {
                                Id = Convert.ToInt32(reader["DepartureCityId"]),
                                Name = reader["DepartureCityName"].ToString()
                            },
                            Country = new Country
                            {
                                Id = Convert.ToInt32(reader["DepartureCountryId"]),
                                Name = reader["DepartureCountryName"].ToString()
                            }
                        },
                        To = new Airport
                        {
                            Id = Convert.ToInt32(reader["ArrivalId"]),
                            Name = reader["ArrivalName"].ToString(),
                            City = new City
                            {
                                Id = Convert.ToInt32(reader["ArrivalCityId"]),
                                Name = reader["ArrivalCityName"].ToString()
                            },
                            Country = new Country
                            {
                                Id = Convert.ToInt32(reader["ArrivalCountryId"]),
                                Name = reader["ArrivalCountryName"].ToString()
                            }
                        },
                        Departure = Convert.ToDateTime(reader["From_Time"]),
                        Arrival = Convert.ToDateTime(reader["Until_Time"])
                    };
                    flights.Add(flight);
                }
                reader.Close();
            }

            return flights;
        }


        public static void CreateFlight(Flight flight)
        {
            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(createFlightQuery, connection);
                command.Parameters.AddWithValue("@Departure_Id", flight.From.Id);
                command.Parameters.AddWithValue("@Arrival_Id", flight.To.Id);
                command.Parameters.AddWithValue("@From_Time", flight.Departure);
                command.Parameters.AddWithValue("@Until_Time", flight.Arrival);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static bool UpdateFlight(int id, Flight flight)
        {
            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(updateFlightQuery, connection);
                command.Parameters.AddWithValue("@Departure_Id", flight.From.Id);
                command.Parameters.AddWithValue("@Arrival_Id", flight.To.Id);
                command.Parameters.AddWithValue("@From_Time", flight.Departure);
                command.Parameters.AddWithValue("@Until_Time", flight.Arrival);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public static bool DeleteFlight(int id)
        {
            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(deleteFlightQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        private static Airport GetAirportById(int airportId) // Helper func
        {
            Airport airport = null;

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                string query = @"
                    SELECT a.Id, a.Name, c.Id AS CityId, c.Name AS CityName, co.Id AS CountryId, co.Name AS CountryName
                    FROM Airport a
                    INNER JOIN City c ON a.City_Id = c.Id
                    INNER JOIN Country co ON a.Country_Id = co.Id
                    WHERE a.Id = @Id";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", airportId);
                connection.Open();
                var reader = command.ExecuteReader();
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
    }
}
