using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Uniso.InStat.Game;
using Uniso.InStat.Gui;

namespace Uniso.InStat
{
    public partial class SelectForm : Form
    {
        private UISync sync = null;
        private bool loading = false;
        private int match_id = 0;
        private HockeyIce game = new HockeyIce(HockeyIce.GameTypeEnum.Euro, HockeyIce.KIND_1);

        public SelectForm()
        {
            InitializeComponent();

            HockeyIce.Role = HockeyIce.RoleEnum.Ttd;

            hockeyField1.Game = game;
            HockeyGui.HF = hockeyField1;
            HockeyGui.Mode = HockeyGui.ModeEnum.EditTactics;
        }

        private void teamDataEditor2_ChangedData(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void teamDataEditor1_ChangedData(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private int old_code = 0;
        public void ShowStatus(String msg, int code)
        {
            sync.Execute(() =>
            {
                if (code != old_code)
                {
                    switch (code)
                    {
                        case 0:
                            toolStripStatusLabel1.BackColor = SystemColors.Control;
                            toolStripStatusLabel1.ForeColor = Color.Black;
                            break;
                        case 1:
                            toolStripStatusLabel1.BackColor = Color.Red;
                            toolStripStatusLabel1.ForeColor = Color.White;
                            break;
                    }
                    old_code = code;
                }
                toolStripStatusLabel1.Text = msg;
            });
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!loading && match_id > 0)
            {
                HockeyGui.Mode = HockeyGui.ModeEnum.EditTactics;
                sync.Execute(() => textBox4.Text = String.Empty);

                loading = true;
                UpdateUI();

                var t = new Thread(DoLoadMatchData) {IsBackground = true};
                t.Start(match_id);
            }
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            sync = new UISync(this);

            UpdateUI();
        }

        private void UpdateUI()
        {
            sync.Execute(() =>
                {
                    var red = !String.IsNullOrEmpty(textBox3.Text) && match_id == 0;
                    textBox3.BackColor = red ? Color.Red : Color.White;

                    linkLabel1.Enabled = !loading;
                    linkLabel3.Visible = !loading;
                    linkLabel3.Enabled = match_id > 0;
                    textBox3.Enabled = !loading;

                    button1.Enabled = game != null && game.Match != null && hockeyField1.IsTacticsStartValid && !loading;
                    checkBox1.Enabled = game != null && game.Match != null && !loading;
                    checkBox2.Enabled = game != null && game.Match != null && !loading;

                    comboBox1.Enabled = game != null && game.Match != null && !loading;
                    comboBox2.Enabled = game != null && game.Match != null && !loading;
                });
        }

        private void DoBeginWork()
        {
            var t = new Thread(() =>
                {
                    try
                    {
                        for (var i = 0; i < 3; i++)
                            game.HalfList[i].Periods[0].Length = Options.G.Game_LengthPrimaryHalf * 60000;

                        foreach (Game.Marker mk in game.Markers)
                            mk.row = null;

                        if (changed_colors)
                        {
                            ShowStatus("Сохранение вариантов формы...", 0);
                            Web.SaveMatchColors(game.Match);
                        }

                        ShowStatus("Запуск...", 0);
                        
                        if (changeTeam1)
                            PrepareTactics(game.Match.Team1);
                        
                        if (changeTeam2)
                            PrepareTactics(game.Match.Team2);

                        if (changeTeam1 || changeTeam2)
                            MarkerList.SaveToFile(game.Match.Id, game.Markers);

                        game.Match.Team1.FinePlaces.Clear();
                        game.Match.Team1.FinePlayers.Clear();

                        game.Match.Team2.FinePlaces.Clear();
                        game.Match.Team2.FinePlayers.Clear();

                        ShowStatus(String.Empty, 0);

                        sync.Execute(() =>
                        {
                            WindowVisible(false);

                            if (HockeyIce.Role != HockeyIce.RoleEnum.Online)
                            {
                                var form = new MainForm(game);
                                form.Game = game;
                                form.ShowDialog();
                            }
                            else
                            {
                                var form = new OnlinerForm(game);
                                form.ShowDialog();
                            }

                            checkBox1.Checked = false;
                            checkBox2.Checked = false;

                            WindowVisible(true);
                        });
                    }
                    catch (Exception ex)
                    {
                        ShowStatus(ex.Message, 1);
                        Log.WriteException(ex);
                    }
                    finally
                    {
                        loading = false;
                        UpdateUI();
                    }
                });
            t.IsBackground = true;
            t.Start();
        }

        private void PrepareTactics(Team tm)
        {
            //Удаление старых расстановок
            var ami_removed =
                game.GetTacticsTime((Uniso.InStat.Game.Team)tm, game.HalfList[0], 0);

            foreach (var id in ami_removed.Keys)
            {
                var t = tm.Tactics[id];
                var tt = ami_removed[id];
                foreach (Marker mk in tt)
                {
                    if (mk.FlagSaved)
                        mk.FlagDel = true;
                    else
                        game.Markers.Remove(mk);
                }
            }

            var rem14 = game.Markers.Where(o => o.ActionId == 14 && o.Half.Index == game.HalfList[0].Index && o.TimeVideo == 0).ToList<Marker>();
            foreach (var mk in rem14)
            {
                if (mk.FlagSaved)
                    mk.FlagDel = true;
                else
                    game.Markers.Remove(mk);
            }

            var tint = new List<int>();
            foreach (var i in tm.Tactics.Keys)
            {
                if (i > 0 && !tm.Tactics[i].IsEmpty)
                    tint.Add(i);
            }

            var ami = ((Uniso.InStat.Game.Team)tm).GetCurrentTacticsMarker(game, game.HalfList[0], 0, tint);
            foreach (Marker mk in ami)
            { 
                game.Insert(mk);
            }

            /*ami = ((Uniso.InStat.Game.Team)tm).GetCurrentTacticsMarker(game.HalfList[0], 0, new List<int>() { 0 });
            foreach (Marker mk in ami)
            {
                Marker mki = new Game.Marker(14, mk.ActionType, game.HalfList[0], 0);
                mki.Player1 = mk.Player1;
                game.Insert(mki);
            }*/
        }

        public void WindowVisible(bool value)
        {
            sync.Execute(() => { Visible = value; }); 
        }
        
        private Team GetTeam(int team_id, Marker mk)
        {
            if (game.Match.Team1.Id == team_id)
                return game.Match.Team1;
            if (game.Match.Team2.Id == team_id)
                return game.Match.Team2;

            throw new TeamMarkerException(String.Format("Ошибка в маркере {0}. Указана команда {1}, которая не играет в матче", mk.Id, team_id));
        }

        private Team GetTeam(int team_id, int player_id, Marker mk)
        {
            if (mk.ActionId == 16 || mk.ActionId == 14)
            {
                if (team_id == game.Match.Team1.Id)
                    return game.Match.Team1;
                if (team_id == game.Match.Team2.Id)
                    return game.Match.Team2;

                mk.FlagDel = true;
                return null;
            }

            if (game.Match.Team1.Players.Exists(o => o.Id == player_id))
                return game.Match.Team1;
            if (game.Match.Team2.Players.Exists(o => o.Id == player_id))
                return game.Match.Team2;

            mk.FlagDel = true;

            return null;

            //throw new Exception(String.Format("Ошибка в маркере {0}. Игрок {1} не был указан в маркерах расстановки", mk.Id, player_id));
        }

        private Player GetPlayer(int player_id, Team team, List<Player> pl)
        {
            var pli = pl.Where(o => o.Id == player_id).ToList<Player>();
            return pli.Count == 0 ? MsSql.GetPlayer(player_id, team) : pli[0];
        }

        private bool changed_colors = false;

        private void DoLoadMatchData(object ids)
        {
            try
            {
                sync.Execute(() => pictureBox1.Visible = true);
                ShowStatus("Загрузка данных матча...", 0);

                Web.GetTeamColorsKind();

                var idm = Int32.Parse(ids.ToString());
                SizeF sizef;
                game = Web.GetMatchInfo(idm, out sizef);

                if (game == null)
                    throw new Exception("Нет матча или выбран неверный тип игры!");

                if (game.Match.Team1 == null || game.Match.Team1.Id == 0)
                    throw new Exception("Нет команды 1");
                if (game.Match.Team2 == null || game.Match.Team2.Id == 0)
                    throw new Exception("Нет команды 2");

                if (!sizef.IsEmpty)
                    game.FieldSize = sizef;

                ShowStatus("Загрузка игроков команды " + game.Match.Team2.ToString() + "...", 0);
                Web.GetPlayers(game.Match.Team2);

                if (game.Match.Team2.Players.Count == 0)
                    throw new Exception("Нет состава команды " + game.Match.Team2);

                ShowStatus("Загрузка игроков команды " + game.Match.Team1.ToString() + "...", 0);
                Web.GetPlayers(game.Match.Team1);

                if (game.Match.Team1.Players.Count == 0)
                    throw new Exception("Нет состава команды " + game.Match.Team1);

                ShowStatus("Загрузка параметров матча...", 0);

                game.Match.Team1.FinePlaces.Clear();
                game.Match.Team1.FinePlayers.Clear();

                game.Match.Team2.FinePlaces.Clear();
                game.Match.Team2.FinePlayers.Clear();

                Web.GetTeamColors(game.Match.Team1);
                Web.GetTeamColors(game.Match.Team2);

                var txt = game.Match.Team1.Name + " - " + game.Match.Team2.Name;

                game.Markers.Clear();

                sync.Execute(() =>
                {
                    label1.Text = game.Match.Tournament + " " + 
                        game.Match.Date.ToShortDateString() + " " + 
                        game.Match.Date.ToLongTimeString();

                    textBox4.Text = txt;
                });

                ShowStatus("Загрузка маркеров...", 0);

                var lstSrv = Web.GetMarkers(game, game.Match.Id);
                List<Marker> lstLoc = MarkerList.LoadFromFile(game, game.Match.Id);

                foreach (Game.Marker mk in lstLoc)
                    mk.game = game;

                foreach (var mk in lstLoc.Where(o => !o.FlagDel).OrderBy(o => o.Half.Index).ThenBy(o => o.TimeVideo))
                {
                    var period = mk.Half.Index;
                    var half = game.HalfList.FirstOrDefault(o => o.Index == period);
                    mk.Half = half;
                }

                game.Union(lstLoc, lstSrv);

                foreach (Game.Marker mk in game.Markers)
                {
                    mk.row = null;
                }

                ShowStatus("Синхронизация...", 0);
                var pl = game.Match.Team1.Players.Concat(game.Match.Team2.Players).ToList<Player>();

                //Восстановление и контроль составов
                foreach (var mk in game.Markers.Where(o => o.ActionId == 16))
                {
                    Team tm = null;
                    try
                    {
                        tm = GetTeam(mk.team1_id, mk);
                        mk.Team1 = tm;
                    }
                    catch (TeamMarkerException ex)
                    {
                        mk.FlagDel = true;
                        ShowStatus(ex.Message, 1);
                        Thread.Sleep(100);
                        continue;
                    }

                    if (pl.Exists(o => o.Id == mk.player1_id))
                    {
                        var p = pl.First(o => o.Id == mk.player1_id);

                        if (mk is Game.Marker)
                        {
                            var mkg = (Game.Marker)mk;
                            if (mkg.Num > 0)
                                p.Number = mkg.Num;
                        }

                        var tm2 = game.Match.Team1 == tm ? game.Match.Team2 : game.Match.Team1;
                        if (tm2.Players.Exists(o => o.Id == p.Id))
                            throw new Exception(String.Format("Ошибка в маркере расстановки {0}. Игрок {1} был в составе {2}, а теперь указан в команде соперника", mk.Id, p, tm2));

                        p.Team = tm;
                        mk.Player1 = p;

                        if (!tm.Players.Exists(o => o.Id == p.Id)) 
                            tm.Players.Add(p);                        
                    }
                    else
                    {
                        if (mk.player1_id <= 0)
                            continue;

                        var p = Web.GetPlayer(mk.player1_id);
                        if (p != null)
                        {
                            p.Team = tm;
                            mk.Player1 = p;

                            if (!tm.Players.Exists(o => o.Id == p.Id))
                                tm.Players.Add(p);
                            if (!pl.Exists(o => o.Id == p.Id))
                                pl.Add(p);
                        }
                        else
                            throw new Exception("Критическая ошибка: не могу загрузить игрока ID=" + mk.player1_id + " из состава команды " + tm.Name);
                    }
                }

                game.Match.Team1.Players.AddRange(
                    pl.Where(o 
                        => o.Team != null
                        && o.Team.Id == game.Match.Team1.Id
                        && !game.Match.Team1.Players.Exists(o1 => o1.Id == o.Id)));

                game.Match.Team2.Players.AddRange(
                    pl.Where(o
                        => o.Team != null
                        && o.Team.Id == game.Match.Team2.Id
                        && !game.Match.Team2.Players.Exists(o1 => o1.Id == o.Id)));

                var errmklist = new List<Marker>();
                foreach (var mk in game.Markers.Where(o => o.ActionId != 16))
                {
                    if (mk.player1_id > 0)
                    {
                        var pla = pl.FirstOrDefault(o => o.Id == mk.player1_id);

                        if (pla == null)
                        {
                            try
                            {
                                mk.Player1 = Web.GetPlayer(mk.player1_id);
                            }
                            catch
                            {
                                mk.FlagDel = true;
                                errmklist.Add(mk);
                            }
                        }
                        else
                            mk.Player1 = pla;
                    }
                    else
                        if (mk.team1_id > 0)
                        {
                            try
                            {
                                mk.Team1 = GetTeam(mk.team1_id, mk);
                            }
                            catch (Exception ex)
                            { }
                        }

                    if (mk.player2_id > 0)
                    {
                        var opp = pl.FirstOrDefault(o => o.Id == mk.player2_id);

                        if (opp == null)
                        {
                            try
                            {
                                mk.Player2 = Web.GetPlayer(mk.player2_id); ;
                            }
                            catch
                            {
                                mk.FlagDel = true;
                                errmklist.Add(mk);
                            }
                        }
                        else
                            mk.Player2 = opp;
                    }
                    else
                        if (mk.team2_id > 0)
                        {
                            try
                            {
                                mk.Team2 = GetTeam(mk.team2_id, mk);
                            }
                            catch (Exception ex)
                            { }
                        }
                }

                game.RefreshOvertimes();

                sync.Execute(() =>
                {
                    label1.Text = game.Match.Tournament + " " + game.Match.Date.ToShortDateString() + " " + game.Match.Date.ToLongTimeString();
                    textBox4.Text = txt;
                });

                TeamDataRestore(game.Match.Team1);
                TeamDataRestore(game.Match.Team2);

                PlayerConverter.RefreshData(game.Match);
                TeamConverter.RefreshData(game.Match);

                sync.Execute(() =>
                {
                    checkBox1.Checked = !game.Match.Team1.Tactics[1].IsValid;
                    checkBox2.Checked = !game.Match.Team1.Tactics[1].IsValid;

                    comboBox1.Items.Clear();
                    foreach (var tc in game.Match.Team1.TeamColorsKind.Values)
                    {
                        comboBox1.Items.Add(tc);
                        if (tc.Kind == game.Match.Team1.Color.Kind)
                            comboBox1.SelectedItem = tc;
                    }

                    comboBox2.Items.Clear();
                    foreach (var tc in game.Match.Team2.TeamColorsKind.Values)
                    {
                        comboBox2.Items.Add(tc);
                        if (tc.Kind == game.Match.Team2.Color.Kind)
                            comboBox2.SelectedItem = tc;
                    }

                    changed_colors = false;
                });

                if (errmklist.Count > 0)
                {
                    var errmks = String.Empty;
                    for (var i = 0; i < errmklist.Count; i++)
                    {
                        if (i <= 2)
                        {
                            if (i > 0)
                                errmks += ",";
                            errmks += errmklist[i].Id;
                            if (i == 2)
                                errmks += "...";
                        }
                    }

                    ShowStatus(String.Format("Ошибочно указаны некоторые игроки. Эти маркеры {0} помечены на удаление.", errmks), 1);
                }
                else
                {
                }

                game.RecalcActualTime(game.Markers, null);

                sync.Execute(() => hockeyField1.Game = game);
                ShowStatus("Готово", 0);
            }
            catch (Exception ex)
            {
                game = null;
                ShowStatus(ex.Message, 1);
                HockeyGui.InvalidateRect();
            }
            finally
            {
                sync.Execute(() =>
                {
                    HockeyGui.InvalidateRect();
                    pictureBox1.Visible = false;
                });
                loading = false;
                UpdateUI();
                
            }
        }

        private void TeamDataRestore(Team team)
        {
            var ami = 
                game.GetTacticsTime((Uniso.InStat.Game.Team)team, game.HalfList[0], 0);

            foreach (var id in ami.Keys)
            {
                var t = team.Tactics[id];
                var tt = ami[id];
                foreach (Marker mk in tt)
                {
                    var code = mk.ActionType % 100;
                    var place = t.GetPlace(game, code);
                    if (place != null)
                        place.Player = mk.Player1;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new OptionsForm();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoBeginWork();
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                linkLabel3_LinkClicked(null, null);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            match_id = 0;
            Int32.TryParse(textBox3.Text, out match_id);
            UpdateUI();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void hockeyField1_ChangedPlace_1(object sender, HockeyGui.ChangedPlaceEventArgs e)
        {
            if (e.Team == game.Match.Team1)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox2.Checked = true;
            }

            UpdateUI();
        }

        private bool changeTeam1 = false;
        private bool changeTeam2 = false;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            changeTeam1 = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            changeTeam2 = checkBox2.Checked;
        }

        private void hockeyField1_ChangedPlayers(object sender, EventArgs e)
        {
            foreach (var cpp in HockeyGui.ChangedPlayersList)
            {
                if (cpp.Place2 != null)
                    cpp.Place2.Player = cpp.Place1.Player;
            }

            HockeyGui.InvalidateRect();
            HockeyGui.ChangedPlayersList.Clear();
            UpdateUI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is TeamColors)
            {
                changed_colors = true;
                game.Match.Team1.Color = (TeamColors)comboBox1.SelectedItem;
                hockeyField1.gdi.InvalidateRect();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem is TeamColors)
            {
                changed_colors = true;
                game.Match.Team2.Color = (TeamColors)comboBox2.SelectedItem;
                hockeyField1.gdi.InvalidateRect();
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            HockeyIce.Role = (HockeyIce.RoleEnum)Convert.ToInt32(((RadioButton)sender).Tag);
        }
    }
}
