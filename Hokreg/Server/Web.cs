using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Uniso.InStat
{
    public class Web
    {
        public enum Method
        {
            POST, GET,
        }

        public static String FOOT_PRE = "\"server\":\"instatfootball.com\",\"base\":\"football\",\"login\":\"registrator\",\"pass\":\"BLhD9_vk-u\"";
        public static String HOCK_PRE = "\"server\":\"instatfootball.com\",\"base\":\"hokkey\",\"login\":\"registrator\",\"pass\":\"BLhD9_vk-u\"";

        /*registrator_save_del_f_match_event

        Параметры:
        @action		tinyint		- 1 - сохранить, 2 - удалить
        @id			int		- id маркера (для сохранения и удаления)
        @match_id		int		- id матча
        @half			tinyint		- номер тайма
        @second		float		- секунда
        @c_action		int		- id действия
        @f_team		int		- id команды
        @f_player		int		- id игрока
        @opponent_f_team	int		- id команды оппонента
        @opponent_f_player	int		- id игрока-оппонента
        @pos_x		float		- позиция точки (X)
        @pos_y		float		- позиция точки (Y)
        @opponent_pos_x	float		- позиция точки назначения (X)
        @opponent_pos_y	float		- позиция точки назначения (Y)
        @c_zone		tinyint		- id зоны
        @opponent_c_zone	tinyint		- id зоны назначения
        @f_user		int		- id пользователя, сохраняющего/удаляющего данные
        @ret			int OUTPUT	- результат операции: вернет id маркера при удаче и отрицательное значение при ошибке
        @msg			varchar(500) OUTPUT	- вернет текст ошибки
        */

        public static void SaveMarker(Game.HockeyIce game, Game.Marker mk)
        {
            if (mk.Compare(3, new int[] { 1, 2 }) || mk.Compare(8, 1))
                mk.Win = 0;

            var datain = new Dictionary<string, object>();
            datain.Add("action", mk.FlagDel ? 2 : 1);

            if (mk.Id > 0)
                datain.Add("id", mk.Id);
            
            datain.Add("match_id", game.Match.Id);
            datain.Add("half", mk.Half.Index);

            var time = Math.Round((float)mk.TimeVideoReal / 1000f, 3);
            if (Uniso.InStat.Game.HockeyIce.Role == Game.HockeyIce.RoleEnum.Online && !mk.Compare(18) && !mk.Compare(16))
                datain.Add("second_online", time);
            else
                datain.Add("second", time);

            datain.Add("second_clear", Math.Round((float)mk.TimeActual / 1000f, 3));
                        
            datain.Add("c_action", mk.ActionCode);

            if (mk.Team1 != null)
                datain.Add("f_team", mk.Team1.Id);

            if (mk.Player1 != null)
                datain.Add("f_player", mk.Player1.Id);

            if (mk.Team2 != null)
                datain.Add("opponent_f_team", mk.Team2.Id);

            if (mk.Player2 != null)
                datain.Add("opponent_f_player", mk.Player2.Id);

            if (!mk.Point1.IsEmpty)
            {
                datain.Add("pos_x", mk.Point1.X);
                datain.Add("pos_y", mk.Point1.Y);
            }

            if (!mk.Point2.IsEmpty)
            {
                datain.Add("opponent_pos_x", mk.Point2.X);
                datain.Add("opponent_pos_y", mk.Point2.Y);
            }

            datain.Add("data1_int", mk.Num);
            datain.Add("data2_int", mk.Sync);
            datain.Add("f_user", mk.user_id);
            datain.Add("link", mk.Link);

            var dataout = new Dictionary<string, object>();
            dataout.Add("ret", "int");
            dataout.Add("msg", "varchar(500)");

            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_save_del_f_match_event_2", datain, dataout, out rqst);
            if (!req.ContainsKey("variables"))
                throw new Exception("Error parsing saving");

            var vars = (Dictionary<string, object>)req["variables"];

            if (!vars.ContainsKey("@ret"))
                throw new Exception("Error parsing saving");

            var ret = Convert.ToInt32(vars["@ret"]);
            if (ret <= 0)
                throw new Exception("Ошибка при сохранении маркера. MESSAGE: " + Convert.ToString(vars["@msg"]));

            mk.Id = ret;
            mk.FlagUpdate = false;
        }

        /*registrator_ask_f_match_event

        Параметры:
        @match_id	int	- id матча
        @dl		tinyint	- 1 - выдать все маркеры, включая удаленные, 0 - только не удаленные

        Возвращает поля:
        id			- id записи
        half			- номер тайма
        second			- секунда
        c_action		- id действия
        action_name		- наименование действия
        f_team			- id команды
        team_name		- наименование команды
        f_player			- id игрока
        player_name		- имя+фамилия игрока
        opponent_f_team	- id команды оппонента
        opponent_team_name	- наименование команды оппонента
        opponent_f_player	- id игрока-оппонента
        opponent_player_name	- имя+фамилия игрока-оппонента
        pos_x			
        pos_y
        opponent_pos_x
        opponent_pos_y
        f_user			- id последнего изменившего запись пользователя
        user_name		- имя+фамилия последнего изменившего запись пользователя
        dl			- флаг удаления записи
        */  

        public static List<Marker> GetMarkers(Uniso.InStat.Game.HockeyIce game, int match_id)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("match_id", match_id);
            datain.Add("dl", 0);

            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_ask_f_match_event", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];
            var res = new List<Marker>();

            foreach (Dictionary<string, object> u in objlist)
            {
                try
                {
                    var period = Convert.ToInt32(u["half"]);
                    var half = game.HalfList.FirstOrDefault(o => o.Index == period);
                    if (half == null)
                        throw new Exception(String.Format("Указанный период игры ({0}) не найден в системе", period));

                    var c_action = Convert.ToInt32(u["c_action"]);

                    var sync = 0;
                    if (u.ContainsKey("data2_int"))
                        sync = Convert.ToInt32(u["data2_int"]);

                    var mk = new Game.Marker(game)
                    {
                        Id = Convert.ToInt32(u["id"]),
                        ActionCode = c_action,
                        Half = half,
                        Link = u.ContainsKey("link") && u["link"] != null ? Convert.ToInt32(u["link"]) : 0,
                        player1_id = u["f_player"] != null ? Convert.ToInt32(u["f_player"]) : 0,
                        team1_id = u["f_team"] != null ? Convert.ToInt32(u["f_team"]) : 0,
                        player2_id = u["opponent_f_player"] != null ? Convert.ToInt32(u["opponent_f_player"]) : 0,
                        team2_id = u["opponent_f_team"] != null ? Convert.ToInt32(u["opponent_f_team"]) : 0,
                        Point1 = new System.Drawing.PointF
                        {
                            X = u["pos_x"] != null ? Convert.ToSingle(u["pos_x"]) : 0,
                            Y = u["pos_y"] != null ? Convert.ToSingle(u["pos_y"]) : 0,
                        },
                        Point2 = new System.Drawing.PointF
                        {
                            X = u["opponent_pos_x"] != null ? Convert.ToSingle(u["opponent_pos_x"]) : 0,
                            Y = u["opponent_pos_y"] != null ? Convert.ToSingle(u["opponent_pos_y"]) : 0,
                        },
                        user_id = u["f_user"] != null ? Convert.ToInt32(u["f_user"]) : 0,
                        Sync = sync,
                    };

                    if (u.ContainsKey("second_clear") && u["second_clear"] != null)
                        try
                        {
                            var sav = 0.0f;
                            var sa = u["second_clear"].ToString().Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                            if (Single.TryParse(sa, out sav))
                                mk.TimeActualSrv = Convert.ToInt32(sav * 1000f);
                        }
                        catch
                        { }
                    if (Uniso.InStat.Game.HockeyIce.Role == Game.HockeyIce.RoleEnum.Online)
                    {
                        if (!mk.Compare(1, 1)
                            && !mk.Compare(1, 2) 
                            && !mk.Compare(3, 8) 
                            && !mk.Compare(8, 1) 
                            && !mk.Compare(5) 
                            && !mk.Compare(9) 
                            && !mk.Compare(3, 1) 
                            && !mk.Compare(16)
                            && !mk.Compare(18))
                            continue;

                        if (mk.Compare(18, 9))
                            continue;
                    }

                    var time1 = -1;
                    if (u.ContainsKey("second") && u["second"] != null)
                        time1 = Convert.ToInt32(Convert.ToSingle(u["second"].ToString().Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)) * 1000f);

                    var time2 = -1;
                    if (u.ContainsKey("second_online") && u["second_online"] != null)
                        time2 = Convert.ToInt32(Convert.ToSingle(u["second_online"].ToString().Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator)) * 1000f);

                    /*if (Uniso.InStat.Game.HockeyIce.Role != Game.HockeyIce.RoleEnum.Online && time1 == -1 && mk.Sync == 1 && !mk.Compare(new int[] { 16, 18 }))
                        continue;

                    if (Uniso.InStat.Game.HockeyIce.Role != Game.HockeyIce.RoleEnum.Online && time1 == time2 && mk.Sync == 1 && !mk.Compare(new int[] { 16, 18 }))
                        continue;

                    if (Uniso.InStat.Game.HockeyIce.Role == Game.HockeyIce.RoleEnum.Online && time2 == -1 && !mk.Compare(new int[] { 16, 18 }))
                        continue;*/

                    if (Uniso.InStat.Game.HockeyIce.Role == Game.HockeyIce.RoleEnum.Online)
                        mk.TimeVideoReal = time2;
                    else
                        mk.TimeVideoReal = time1;

                    if (u.ContainsKey("players_num_team1"))
                        mk.NumTeam1 = Convert.ToInt32(u["players_num_team1"]);

                    if (u.ContainsKey("players_num_team1"))
                        mk.NumTeam2 = Convert.ToInt32(u["players_num_team2"]);

                    if (u.ContainsKey("data1_int"))
                        mk.Num = Convert.ToInt32(u["data1_int"]);

                    res.Add(mk);
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex);
                    throw new Exception("Ошибка в формате данных registrator_ask_f_match_event");
                }
            }

            return res;
        }

        public static void CopyMarkers(int match_id_from, int match_id_to)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("match_id_from", match_id_from);
            datain.Add("match_id_to", match_id_to);

            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "very_secret_function_copy_markers_to_new_match", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];
            if (objlist.Length == 0)
                throw new Exception("Пользователь не найден");

            var data = (Dictionary<string, object>)objlist[0];

            if (!data.ContainsKey("result"))
                throw new Exception("Нет параметра result");
        }

        public static bool Login(User u, String pass)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("id", u.Id);
            datain.Add("pass", pass);

            var rqst = String.Empty;
            var req = Request(FOOT_PRE, "prc_check_user_id_password", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];
            if (objlist.Length == 0)
                throw new Exception("Пользователь не найден");

            var data = (Dictionary<string, object>)objlist[0];

            if (!data.ContainsKey("result"))
                throw new Exception("Нет параметра result");

            return Convert.ToInt32(data["result"]) == 1;
        }

        public static List<User> GetUserList()
        {
            var rqst = String.Empty;
            var req = Request(FOOT_PRE, "lst_c_users", new Dictionary<string, object>(), new Dictionary<string, object>(), out rqst);
            var objlist = (Object[])req["data"];

            var res = new List<User>();
            foreach (Dictionary<string, object> u in objlist)
            {
                res.Add(new User 
                { 
                 Id = Convert.ToInt32(u["id"]),
                 Name = Convert.ToString(u["name"]),
                 Surname = Convert.ToString(u["surname"]),
                 Login = Convert.ToString(u["login"])
                });
            }

            return res;
        }

        public static void GetPlayers(Team team)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("team_id", team.Id);
            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_ask_players_by_team", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];
            team.Players.Clear();

            foreach (Dictionary<string, object> u in objlist)
            {
                var p = new Player 
                { 
                    Team = team,
                    Id = Convert.ToInt32(u["f_player"]),
                    Number = Convert.ToInt32(u["sweater_num"]),
                    Name = Convert.ToString(u["firstname_eng"]),
                    LastName = Convert.ToString(u["lastname_eng"]),
                    MiddleName = Convert.ToString(u["middlename_eng"]),
                    NickName = Convert.ToString(u["nickname_eng"]),
                    Gender = (Gender)Convert.ToInt32(u["c_gender"]),
                    IsGk = Convert.ToInt32(u["c_amplua1"]) == 1,
                };

                team.Players.Add(p);
            }
        }

        public static Player GetPlayer(int id)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("player_id", id);
            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_get_f_player", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];

            if (objlist.Length == 0)
                throw new Exception("Игрок id=" + id + " не найден");

            var u = (Dictionary<string, object>)objlist[0];
            var p = new Player
            {
                Id = id,
                Number = Convert.ToInt32(u["club_sweater_num"]),
                Name = Convert.ToString(u["firstname_eng"]),
                LastName = Convert.ToString(u["lastname_eng"]),
                MiddleName = Convert.ToString(u["middlename_eng"]),
                NickName = Convert.ToString(u["nickname_eng"]),
                Gender = (Gender)Convert.ToInt32(u["c_gender"]),
                IsGk = Convert.ToInt32(u["c_amplua1"]) == 1,
            };

            return p;
        }

        public static void GetTeamColorsKind()
        {
            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "lst_c_uniform_type", new Dictionary<string, object>(), new Dictionary<string, object>(), out rqst);
            
            var objlist = (Object[])req["data"];
            var res = new List<Marker>();

            TeamColors.KindList = new Dictionary<int, string>();
            foreach (Dictionary<string, object> u in objlist)
            {
                if (u.ContainsKey("id") && u.ContainsKey("name"))
                    TeamColors.KindList.Add(Convert.ToInt32(u["id"]), Convert.ToString(u["name"]));
            }
        }

        public static void SaveMatchColors(Match match)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("match_id", match.Id);
            datain.Add("f_team_color_standart_team1", match.Team1.Color.Id);
            datain.Add("f_team_color_standart_team2", match.Team2.Color.Id);

            var dataout = new Dictionary<string, object>();
            dataout.Add("ret", "int");
            dataout.Add("msg", "varchar(500)");

            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_set_match_team_uniform", datain, dataout, out rqst);
            if (!req.ContainsKey("variables"))
                throw new Exception("Error parsing saving");

            var vars = (Dictionary<string, object>)req["variables"];

            if (!vars.ContainsKey("@ret"))
                throw new Exception("Error parsing saving");

            var ret = Convert.ToInt32(vars["@ret"]);
            if (ret < 0)
                throw new Exception("Ошибка при сохранении варинта формы команд");
        }

        public static void GetTeamColors(Team team)
        {
            var datain = new Dictionary<string, object>();
            datain.Add("team_id", team.Id);

            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_ask_f_team_color_standart_by_team", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];
            var res = new List<Marker>();

            team.TeamColorsKind.Clear();
            foreach (Dictionary<string, object> u in objlist)
            {
                if (u.ContainsKey("id") 
                    && u.ContainsKey("c_uniform_type") 
                    && u.ContainsKey("sweater_color_1")
                    && u.ContainsKey("sweater_color_2")
                    && u.ContainsKey("number_color"))
                {
                    var tc = new TeamColors();
                    tc.Id = Convert.ToInt32(u["id"]);
                    tc.Kind = Convert.ToInt32(u["c_uniform_type"]);
                    tc.SelfColor1 = HexToColor(Convert.ToString(u["sweater_color_1"]));
                    tc.SelfColor2 = HexToColor(Convert.ToString(u["sweater_color_2"]));
                    tc.NumberColor = HexToColor(Convert.ToString(u["number_color"]));
                    tc.Name = Convert.ToString(u["uniform_type"]);
                    team.TeamColorsKind.Add(tc.Kind, tc);
                }
            }

            if (team.Color == null)
            {
                if (team.TeamColorsKind.Count > 0)
                    team.Color = team.TeamColorsKind[team.TeamColorsKind.Keys.First()];
                else
                    team.Color = new TeamColors { Kind = 3, NumberColor = Color.White, SelfColor1 = Color.Red, SelfColor2 = Color.Red };
            }
            else
            {
                if (!team.TeamColorsKind.ContainsKey(team.Color.Kind))
                    throw new Exception(String.Format("Отсутствует цвет формы {0} команды {1}", team.Color, team));

                team.Color.Id = team.TeamColorsKind[team.Color.Kind].Id;
            }
        }

        public static String ColorToHex(Color c)
        {
            return String.Format("{0:X6}", c.ToArgb() & 0x00ffffff);
        }

        public static Color HexToColor(String c)
        {
            c = "FF" + c.Replace("0x", "");
            var inVal = System.Int32.Parse(c, System.Globalization.NumberStyles.AllowHexSpecifier);
            return Color.FromArgb(inVal);
        }

        public static Uniso.InStat.Game.HockeyIce GetMatchInfo(int id, out System.Drawing.SizeF size)
        {
            size = new System.Drawing.SizeF(60f, 30f);

            var datain = new Dictionary<string, object>();
            datain.Add("match_id", id);
            var rqst = String.Empty;
            var req = Request(HOCK_PRE, "registrator_get_match_info", datain, new Dictionary<string, object>(), out rqst);

            var objlist = (Object[])req["data"];
            if (objlist.Length == 0)
                throw new Exception("Матч не найден");

            var data = (Dictionary<string, object>)objlist[0];

            var match = new Match();
            match.Id = id;
            
            match.Team1Native = new Game.Team 
            { 
                Id = Convert.ToInt32(data["f_team1"]), 
                Name = Convert.ToString(data["team1_name_rus"]), 
                Color = null,
            };

            var kind = 0;
            if (data["c_uniform_type_team1"] != null && Int32.TryParse(data["c_uniform_type_team1"].ToString(), out kind))
            {
                match.Team1.Color = new TeamColors
                    {
                        Kind = kind,
                        NumberColor = HexToColor(Convert.ToString(data["number_color_team1"])),
                        SelfColor1 = HexToColor(Convert.ToString(data["sweater_color_1_team1"])),
                        SelfColor2 = HexToColor(Convert.ToString(data["sweater_color_2_team1"])),
                        Name = Convert.ToString(data["uniform_type_team1"]),
                    };
            }

            match.Team2Native = new Game.Team 
            { 
                Id = Convert.ToInt32(data["f_team2"]), 
                Name = Convert.ToString(data["team2_name_rus"]), 
                Color = null,
            };

            if (data["c_uniform_type_team2"] != null && Int32.TryParse(data["c_uniform_type_team2"].ToString(), out kind))
            {
                match.Team2.Color = new TeamColors
                    {
                        Kind = kind,
                        NumberColor = HexToColor(Convert.ToString(data["number_color_team2"])),
                        SelfColor1 = HexToColor(Convert.ToString(data["sweater_color_1_team2"])),
                        SelfColor2 = HexToColor(Convert.ToString(data["sweater_color_2_team2"])),
                        Name = Convert.ToString(data["uniform_type_team2"]),
                    };
            }

            match.Tournament = Convert.ToString(data["tournament_name_rus"]);
            match.Season = Convert.ToString(data["c_season"]);

            var date = Convert.ToString(data["match_date"]);
            match.Date = DateTime.Parse(date);
            var gte = Game.HockeyIce.GameTypeEnum.Euro;

            switch (Convert.ToInt32(data["c_field_type"]))
            { 
                case 1:
                    size = new System.Drawing.SizeF(61f, 30f);
                    break;

                case 2:
                    size = new System.Drawing.SizeF(60.96f, 25.91f);
                    gte = Game.HockeyIce.GameTypeEnum.NHL;
                    break;
            }

            var game_kind = Convert.ToInt32(data["c_tournament_type"]);
            var game = new Game.HockeyIce(
                gte, 
                game_kind == 1 ? Game.HockeyIce.KIND_1 : Game.HockeyIce.KIND_2);

            game.Match = match;

            return game;
        }

        public static Dictionary<string, object> Request(String pre, String proc, Dictionary<String, object> datain, Dictionary<String, object> dataout, out String request_data_in_out)
        {
            var method = Method.POST;// datain.Count == 0 && dataout.Count == 0 ? Method.GET : Method.POST;

            var url = "http://service.instatfootball.com/ws.php";
            //if (pre.Contains("hokkey"))
                //url = "http://service.instatfootball.com/ws_test.php";

            //GET DATA
            if (method == Method.GET)
            {
                url += "?server=instatfootball.com&base=hokkey&login=registrator&pass=BLhD9_vk-u&" + proc;
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToString();
            request.ContentType = "application/json";
            request.Host = "service.instatfootball.com";

            var request_data = String.Empty;

            //POST DATA
            request_data_in_out = String.Empty;
            if (method == Method.POST)
            {
                request_data = "{" + pre;
                request_data += String.Format(",\"proc\":\"{0}\"", proc);

                foreach (var key in datain.Keys)
                {
                    if (!String.IsNullOrEmpty(request_data_in_out))
                        request_data_in_out += ",";

                    var value = datain[key];
                    if (datain[key] is String)
                        value  = "\"" + datain[key] + "\"";
                    if (datain[key] is Single)
                        value = Convert.ToSingle(datain[key]).ToString("0.000").Replace(",", ".");
                    if (datain[key] is Double)
                        value = Convert.ToDouble(datain[key]).ToString("0.000").Replace(",", ".");

                    request_data_in_out += String.Format("\"@{0}\":[{1},\"in\"]", key, value);
                }
                foreach (var key in dataout.Keys)
                {
                    if (!String.IsNullOrEmpty(request_data_in_out))
                        request_data_in_out += ",";

                    request_data_in_out += String.Format("\"@{0}\":[{1},\"out\"]", key, dataout[key] is String ? "\"" + dataout[key] + "\"" : dataout[key]);
                }

                if (!String.IsNullOrEmpty(request_data_in_out))
                    request_data += ",\"params\":{" + request_data_in_out + "}";

                request_data += "}";

                var EncodedPostParams = Encoding.UTF8.GetBytes(request_data);
                request.ContentLength = EncodedPostParams.Length;
                request.GetRequestStream().Write(EncodedPostParams,
                                                 0,
                                                 EncodedPostParams.Length);
                request.GetRequestStream().Close();
            }

            var response = (HttpWebResponse)request.GetResponse();
            var html = new StreamReader(response.GetResponseStream(),
                                           Encoding.UTF8).ReadToEnd();

            var c = new JavaScriptSerializer();
            c.MaxJsonLength = 16000000;
            var obj = (Dictionary<string, object>)c.DeserializeObject(html);

            if (!obj.ContainsKey("status"))
                throw new Exception("Server error: " + html);

            if (!obj["status"].Equals("Ok"))
                throw new Exception("Server error CODE=" + obj["error_code"].ToString() + ". MESSAGE: " + obj["error_text"].ToString() + ". REQUEST: " + request_data);

            if (!obj.ContainsKey("data"))
                throw new Exception("Responce parse error: " + html);

            return obj;
        }
    }
}
