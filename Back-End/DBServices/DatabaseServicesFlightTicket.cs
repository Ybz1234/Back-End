using Back_End.Models;
using System.Data.SqlClient;

namespace Back_End.DBServices
{
    public class DatabaseServicesFlightTicket
    {
        private static readonly string sqlConnectionStr = "workstation id=FlyAndTravelNirDb.mssql.somee.com;packet size=4096;user id=Yonix_SQLLogin_1;pwd=vl6fwdn3rp;data source=FlyAndTravelNirDb.mssql.somee.com;persist security info=False;initial catalog=FlyAndTravelNirDb;TrustServerCertificate=True";
        //private static readonly string sqlConnectionStr = "Data Source=DESKTOP-476LUOR\\SQLEXPRESS;Initial Catalog=FlyAndTravel;Integrated Security=True";
        private static readonly string getAllFlightTicketsQuery = @"
            SELECT 
                ft.Id AS FlightTicketId, 
                u.Id AS UserId, 
                u.First_Name, 
                u.Last_Name, 
                u.Mail_Address, 
                u.Passwd,
                f.Id AS FlightId, 
                a1.Id AS FromAirportId, 
                a1.Name AS FromAirportName, 
                c1.Id AS FromCityId, 
                c1.Name AS FromCityName, 
                co1.Id AS FromCountryId, 
                co1.Name AS FromCountryName, 
                a2.Id AS ToAirportId, 
                a2.Name AS ToAirportName, 
                c2.Id AS ToCityId, 
                c2.Name AS ToCityName, 
                co2.Id AS ToCountryId, 
                co2.Name AS ToCountryName, 
                f.From_Time AS DepartureTime, 
                f.Until_Time AS ArrivalTime
            FROM Flight_Ticket ft
            INNER JOIN Users u ON ft.User_Id = u.Id
            INNER JOIN Flight f ON ft.Flight_Id = f.Id
            INNER JOIN Airport a1 ON f.Departure_Id = a1.Id
            INNER JOIN City c1 ON a1.City_Id = c1.Id
            INNER JOIN Country co1 ON a1.Country_Id = co1.Id
            INNER JOIN Airport a2 ON f.Arrival_Id = a2.Id
            INNER JOIN City c2 ON a2.City_Id = c2.Id
            INNER JOIN Country co2 ON a2.Country_Id = co2.Id;";
        private static readonly string getFlightTicketByIdQuery = "SELECT * FROM Flight_Ticket WHERE Id = @Id";
        private static readonly string createFlightTicketQuery = "INSERT INTO Flight_Ticket (User_Id, Flight_Id) VALUES (@User_Id, @Flight_Id)";
        private static readonly string updateFlightTicketQuery = "UPDATE Flight_Ticket SET User_Id = @User_Id, Flight_Id = @Flight_Id WHERE Id = @Id";
        private static readonly string deleteFlightTicketQuery = "DELETE FROM Flight_Ticket WHERE Id = @Id";

        public static List<FlightTicket> GetAllFlightTickets()
        {
            List<FlightTicket> flightTickets = new List<FlightTicket>();

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(getAllFlightTicketsQuery, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    FlightTicket flightTicket = new FlightTicket
                    {
                        Id = Convert.ToInt32(reader["FlightTicketId"]),
                        User = new User
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            FirstName = reader["First_Name"].ToString(),
                            LastName = reader["Last_Name"].ToString(),
                            Email = reader["Mail_Address"].ToString(),
                            Password = reader["Passwd"].ToString()
                        },
                        Flight = new Flight
                        {
                            Id = Convert.ToInt32(reader["FlightId"]),
                            From = new Airport
                            {
                                Id = Convert.ToInt32(reader["FromAirportId"]),
                                Name = reader["FromAirportName"].ToString(),
                                City = new City
                                {
                                    Id = Convert.ToInt32(reader["FromCityId"]),
                                    Name = reader["FromCityName"].ToString()
                                },
                                Country = new Country
                                {
                                    Id = Convert.ToInt32(reader["FromCountryId"]),
                                    Name = reader["FromCountryName"].ToString()
                                }
                            },
                            To = new Airport
                            {
                                Id = Convert.ToInt32(reader["ToAirportId"]),
                                Name = reader["ToAirportName"].ToString(),
                                City = new City
                                {
                                    Id = Convert.ToInt32(reader["ToCityId"]),
                                    Name = reader["ToCityName"].ToString()
                                },
                                Country = new Country
                                {
                                    Id = Convert.ToInt32(reader["ToCountryId"]),
                                    Name = reader["ToCountryName"].ToString()
                                }
                            },
                            Departure = Convert.ToDateTime(reader["DepartureTime"]),
                            Arrival = Convert.ToDateTime(reader["ArrivalTime"])
                        }
                    };
                    flightTickets.Add(flightTicket);
                }
                reader.Close();
            }

            return flightTickets;
        }

        public static FlightTicket GetFlightTicketById(int id)
        {
            FlightTicket flightTicket = null;

            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(getFlightTicketByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    flightTicket = new FlightTicket
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        User = DatabaseServicesUsers.GetUserById(Convert.ToInt32(reader["User_Id"])),
                        Flight = DatabaseServicesFlight.GetFlightById(Convert.ToInt32(reader["Flight_Id"]))
                    };
                }
                reader.Close();
            }

            return flightTicket;
        }

        public static void CreateFlightTicket(FlightTicket flightTicket)
        {
            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(createFlightTicketQuery, connection);
                command.Parameters.AddWithValue("@User_Id", flightTicket.User.Id);
                command.Parameters.AddWithValue("@Flight_Id", flightTicket.Flight.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static bool UpdateFlightTicket(int id, FlightTicket flightTicket)
        {
            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(updateFlightTicketQuery, connection);
                command.Parameters.AddWithValue("@User_Id", flightTicket.User.Id);
                command.Parameters.AddWithValue("@Flight_Id", flightTicket.Flight.Id);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public static bool DeleteFlightTicket(int id)
        {
            using (var connection = new SqlConnection(sqlConnectionStr))
            {
                var command = new SqlCommand(deleteFlightTicketQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
