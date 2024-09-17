using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesTripTicket
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        private const string GetFlightQuery = @"
            SELECT 
                ft.Id AS TicketId,
                u.Id AS UserId,
                u.First_Name AS UserFirstName,
                u.Last_Name AS UserLastName,
                u.Mail_Address AS UserEmail,
                f.Id AS FlightId,
                a1.Id AS FromAirportId,
                a1.Name AS FromAirportName,
                a1.City_Id AS FromCityId,
                a1.Country_Id AS FromCountryId,
                a2.Id AS ToAirportId,
                a2.Name AS ToAirportName,
                a2.City_Id AS ToCityId,
                a2.Country_Id AS ToCountryId,
                f.From_Time AS Departure,
                f.Until_Time AS Arrival,
                c1.Name AS FromCityName,
                c2.Name AS ToCityName,
                co1.Name AS FromCountryName,
                co2.Name AS ToCountryName
            FROM Flight_Ticket ft
            JOIN Flight f ON ft.Flight_Id = f.Id
            JOIN Airport a1 ON f.Departure_Id = a1.Id
            JOIN Airport a2 ON f.Arrival_Id = a2.Id
            JOIN Users u ON ft.User_Id = u.Id
            JOIN City c1 ON a1.City_Id = c1.Id
            JOIN City c2 ON a2.City_Id = c2.Id
            JOIN Country co1 ON a1.Country_Id = co1.Id
            JOIN Country co2 ON a2.Country_Id = co2.Id
            WHERE c1.Name = @FromCity AND c2.Name = @ToCity";

        public FlightTicket[] GetFlightTickets(string[] cities, int userId)
        {
            if (cities == null || cities.Length < 2)
            {
                return new FlightTicket[0];
            }

            var flightTickets = new List<FlightTicket>();

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                connection.Open();

                for (int i = 0; i < cities.Length - 1; i++)
                {
                    var fromCity = cities[i];
                    var toCity = cities[i + 1];

                    var tickets = GetFlightTicketsForRoute(connection, fromCity, toCity, userId);
                    flightTickets.AddRange(tickets);
                }
            }

            return flightTickets.ToArray();
        }

        private IEnumerable<FlightTicket> GetFlightTicketsForRoute(SqlConnection connection, string fromCity, string toCity, int userId)
        {
            var flightTickets = new List<FlightTicket>();

            using (var command = new SqlCommand(GetFlightQuery, connection))
            {
                command.Parameters.AddWithValue("@FromCity", fromCity);
                command.Parameters.AddWithValue("@ToCity", toCity);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var flight = new Flight
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("FlightId")),
                            From = new Airport
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("FromAirportId")),
                                Name = reader.GetString(reader.GetOrdinal("FromAirportName")),
                                City = new City
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("FromCityId")),
                                    Name = reader.GetString(reader.GetOrdinal("FromCityName"))
                                },
                                Country = new Country
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("FromCountryId")),
                                    Name = reader.GetString(reader.GetOrdinal("FromCountryName"))
                                }
                            },
                            To = new Airport
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ToAirportId")),
                                Name = reader.GetString(reader.GetOrdinal("ToAirportName")),
                                City = new City
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ToCityId")),
                                    Name = reader.GetString(reader.GetOrdinal("ToCityName"))
                                },
                                Country = new Country
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ToCountryId")),
                                    Name = reader.GetString(reader.GetOrdinal("ToCountryName"))
                                }
                            },
                            Departure = reader.GetDateTime(reader.GetOrdinal("Departure")),
                            Arrival = reader.GetDateTime(reader.GetOrdinal("Arrival"))
                        };

                        var flightTicket = new FlightTicket
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TicketId")),
                            Flight = flight,
                            User = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                FirstName = reader.GetString(reader.GetOrdinal("UserFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("UserLastName")),
                                Email = reader.GetString(reader.GetOrdinal("UserEmail"))
                            }
                        };

                        flightTickets.Add(flightTicket);
                    }
                }
            }

            return flightTickets;
        }
    }
}
