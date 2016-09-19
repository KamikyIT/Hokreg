using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Uniso.InStat.Server
{
    public class MsSql
    {
        public static bool Login(User u, String pass)
        {
            using (var conn = GetConnection())
            {
                var cmdGet = new SqlCommand();
                cmdGet.Connection = conn;
                cmdGet.CommandType = CommandType.StoredProcedure;
                cmdGet.CommandText = "prc_check_user_id_password";
                cmdGet.Parameters.Add(new SqlParameter("id", u.Id));
                cmdGet.Parameters.Add(new SqlParameter("pass", pass));

                var readerGet = cmdGet.ExecuteReader();

                while (readerGet.Read())
                {
                    var res = 0;
                    if (readerGet.FieldCount > 0 && readerGet[0] is Int32 && Int32.TryParse(readerGet[0].ToString(), out res))
                        return res == 1;
                }
            }

            return false;
        }

        public static List<User> GetUserList()
        {
            using (var conn = GetConnection())
            {
                var cmdGet = new SqlCommand();
                cmdGet.Connection = conn;
                cmdGet.CommandType = CommandType.StoredProcedure;
                cmdGet.CommandText = "lst_c_users";

                var readerGet = cmdGet.ExecuteReader();

                var res = new List<User>();
                while (readerGet.Read())
                {
                    var login = (String)readerGet["login"];
                    res.Add(new User
                    {
                        Id = (int)readerGet["id"],
                        Login = login,
                        Name = Convert.ToString(readerGet["name"]),
                        Surname = Convert.ToString(readerGet["surname"]),
                    });
                }

                return res;
            }
        }

        public static void GetPlayers(Team team)
        {
            var idp = team.Id * 100;
            for (var i = 1; i < 35; i++)
                team.Players.Add(new Player { Id = idp++, IsGk = i <= 5, Name = "Player" + i, Number = i, LastName = "Lastname" + i, Team = team });
        }

        public static void LoadTeamColors(Match m)
        {
            
        }

        public static IEnumerable<Marker> LoadMarkers(Match selectMatch)
        {
            return new List<Marker>();
        }

        public static Player GetPlayer(int p, Team tm)
        {
            var player = new Player { Id = p, Team = tm };
            tm.Players.Add(player);
            return player;
        }

        private static SqlConnection GetConnection()
        {
            var conn = new SqlConnection("Data Source=www.instatfootball.com;user=instat_fitness_client;password=base_fitness_6731;database=instat_football");
            conn.Open();

            return conn;
        }
    }
}
