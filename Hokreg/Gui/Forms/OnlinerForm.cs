using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uniso.InStat.Classes;
using Uniso.InStat.Game;
using Uniso.InStat.Server;

namespace Uniso.InStat.Gui.Forms
{
    public partial class OnlinerForm : Form
    {
        private Uniso.InStat.Gui.UISync sync = null;
        private int game_length = 20 * 60000;
        private int table_time = 20 * 60000;
        private HockeyIce game;

        private Player player_goal = null;
        private Player player_assist1 = null;
        private Player player_assist2 = null;
        private Player player_foul1 = null;
        private int player_foul_type = 0;
        private int player_foul_del = 0;
        private int player_foul_disc = 0;

        private bool import_process = false;
        private Object locker = new object();

        public OnlinerForm(HockeyIce game)
        {
            InitializeComponent();
            this.game = game;
        }

        private void OnlinerForm_Load(object sender, EventArgs e)
        {
            sync = new Gui.UISync(this);

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(game.HalfList.ToArray());
            comboBox1.SelectedIndex = 0;

            var players = new List<Player>();
            players.AddRange(game.Match.Team1.Players);
            players.AddRange(game.Match.Team2.Players);

            comboBox2.Items.AddRange(players.ToArray<Player>());
            comboBox3.Items.Add("НЕТ");
            comboBox3.Items.AddRange(players.ToArray<Player>());
            comboBox4.Items.Add("НЕТ");
            comboBox4.Items.AddRange(players.ToArray<Player>());
            comboBox6.Items.AddRange(players.ToArray<Player>());

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("col0", "#", 55, HorizontalAlignment.Left, String.Empty);
            dataGridView1.Columns.Add("col1", "Действие", 170, HorizontalAlignment.Left, String.Empty);
            dataGridView1.Columns.Add("col2", "Период", 65, HorizontalAlignment.Center, String.Empty);
            dataGridView1.Columns.Add("col3", "Время", 120, HorizontalAlignment.Center, String.Empty);
            dataGridView1.Columns.Add("col4", "Игрок", 140, HorizontalAlignment.Left, String.Empty);
            dataGridView1.Columns.Add("col5", "Команда", 100, HorizontalAlignment.Left, String.Empty);
            dataGridView1.Columns.Add("col6", "Оппонент", 140, HorizontalAlignment.Left, String.Empty);
            dataGridView1.Columns.Add("col7", "Команда", 100, HorizontalAlignment.Left, String.Empty);
            dataGridView1.Columns.Add("col8", "Pos", 80, HorizontalAlignment.Center, String.Empty);
            dataGridView1.Columns.Add("col9", "Pos Dest", 70, HorizontalAlignment.Center, String.Empty);

            ReloadDataGridView();

            UpdateUI();
        }

        private int old_code = 0;
        public void ShowStatus(String msg, int code)
        {
            sync.Execute(() =>
            {
                try
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

                    if (msg.Length > 300)
                        msg = msg.Substring(0, 300);

                    toolStripStatusLabel1.Text = msg;
                }
                catch
                { }
            });
        }

        private void FillRow(Game.Marker mk)
        {
            if (mk.row == null)
            {
                mk.row = new System.Windows.Forms.ListViewItem(mk.Id.ToString());
                for (var i = 1; i < 10; i++)
                    try
                    {
                        mk.row.SubItems.Add(String.Empty);
                    }
                    catch
                    { }
            }
            else
                mk.row.SubItems[0].Text = mk.Id.ToString();

            var action = mk.Action;
            var name = Convert.ToString(HockeyIce.convAction.ConvertTo(action, typeof(string)));

            if (game.IsRestoreTimeMarker(mk) || game.IsStopTimeMarker(mk))
                mk.row.Font = new System.Drawing.Font(Font, FontStyle.Italic);
            else
                mk.row.Font = Font;

            var time = game.TimeToString(mk.TimeVideo);
            if (Options.G.Game_ShowActualTime)
                time += "/" + game.TimeToString(mk.TimeActual);

            mk.row.SubItems[1].Text = name;
            mk.row.SubItems[2].Text = mk.Half.ToString();
            mk.row.SubItems[3].Text = time;
            mk.row.SubItems[4].Text = mk.Player1 != null ? mk.Player1.ToString() : "NULL";
            mk.row.SubItems[5].Text = mk.Team1 != null ? mk.Team1.ToString() : "";
            mk.row.SubItems[6].Text = mk.Player2 != null ? mk.Player2.ToString() : "NULL";
            mk.row.SubItems[7].Text = mk.Team2 != null ? mk.Team2.ToString() : "";
            mk.row.SubItems[8].Text = mk.Point1.IsEmpty ? "" : String.Format("{0}, {1}", mk.Point1.X.ToString("0.00"), mk.Point1.Y.ToString("0.00"));
            mk.row.SubItems[9].Text = mk.Point2.IsEmpty ? "" : String.Format("{0}, {1}", mk.Point2.X.ToString("0.00"), mk.Point2.Y.ToString("0.00"));

            mk.row.Tag = mk;
            if (mk.FlagDel)
            {
                mk.row.ForeColor = Color.LightGray;
                mk.row.BackColor = Color.White;
            }
            else
            {
                mk.row.ForeColor = Color.Black;
                if (mk.Exception == null)
                {
                    if (mk.FlagSaved)
                    {
                        if (mk.FlagUpdate)
                            mk.row.BackColor = Color.Gray;
                        else
                            mk.row.BackColor = Color.LightGray;
                    }
                    else
                        mk.row.BackColor = Color.White;
                }
                if (mk.Exception is HockeyIce.CheckValidMarkerException)
                {
                    var ex = (HockeyIce.CheckValidMarkerException)mk.Exception;
                    switch (ex.CheckValidLevel)
                    {
                        case HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.WARNING:
                            mk.row.BackColor = Color.Yellow;
                            break;

                        case HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.CRITICAL:
                            mk.row.BackColor = Color.Red;
                            break;
                    }
                }
            }
        }

        public void ReloadDataGridView()
        {
            lock (locker)
            {
                var aaa = new List<ListViewItem>();

                sync.Execute(() =>
                {
                    dataGridView1.BeginUpdate();
                    dataGridView1.Items.Clear();
                });

                var list = new List<Marker>();
                lock (game.Markers)
                {
                    list.AddRange(game.Markers.OrderBy(o => o.Half.Index)
                    .ThenBy(o => o.TimeVideo)
                    .ThenBy(o => o.TimeActual)
                    .ThenBy(o => o.Id).ToList<Marker>());
                }

                foreach (Game.Marker mk in list)
                {
                    if (mk.FlagGuiUpdate || mk.row == null)
                    {
                        FillRow(mk);
                        mk.FlagGuiUpdate = false;
                    }

                    aaa.Insert(0, mk.row);
                }

                sync.Execute(() =>
                {
                    foreach (var lvi in aaa)
                        try
                        {
                            dataGridView1.Items.Add(lvi);
                        }
                        catch
                        { }

                    dataGridView1.EndUpdate();
                });

                SlideDatagridToCurrentTime();

                locker = new object();
            }
            //});
        }

        public void SlideDatagridToCurrentTime()
        {
            sync.Execute(() =>
            {
                if (dataGridView1.Items.Count > 0)
                    dataGridView1.Items[0].Selected = true;
            });
            return;

            sync.Execute(() =>
            {
                var mk_sel = game.GetLastByTime(game.Half, game.Time);
                if (mk_sel != null)
                {
                    var sel = 0;
                    foreach (System.Windows.Forms.ListViewItem row in dataGridView1.Items)
                    {
                        if (row.Tag is Marker)
                        {
                            var mks = (Marker)row.Tag;
                            if (mks == mk_sel)
                            {
                                sel = row.Index;
                                break;
                            }
                        }
                    }

                    try
                    {
                        var h = dataGridView1.GetItemRect(0).Height;
                        var a = dataGridView1.Bounds.Height / h;
                        a = 4;
                        dataGridView1.EnsureVisible(sel + a - 4);
                        dataGridView1.SelectedIndices.Clear();
                        dataGridView1.SelectedIndices.Add(sel);
                    }
                    catch
                    { }
                }
            });
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            game_length = Convert.ToInt32(numericUpDown4.Value) * 60000;

            game.Time = GetTime();
            UpdateUI();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            table_time = Convert.ToInt32(numericUpDown1.Value) * 60000
                + Convert.ToInt32(numericUpDown2.Value) * 1000
                + Convert.ToInt32(numericUpDown3.Value);

            game.Time = GetTime();
            UpdateUI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            game.Half = (Half)comboBox1.SelectedItem;
            UpdateUI();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            player_goal = (Player)comboBox2.SelectedItem;
            UpdateUI();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem is Player)
            {
                player_assist1 = (Player)comboBox3.SelectedItem;
            }
            else
                player_assist1 = null;

            UpdateUI();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedItem is Player)
            {
                player_assist2 = (Player)comboBox4.SelectedItem;
            }
            else
                player_assist2 = null;

            UpdateUI();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            player_foul1 = (Player)comboBox6.SelectedItem;
            UpdateUI();
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            player_foul_type = comboBox9.SelectedIndex + 1;
            UpdateUI();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            player_foul_del = comboBox7.SelectedIndex + 1;
            if (player_foul_del > 2)
                player_foul_del++;
            UpdateUI();
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            player_foul_disc = comboBox8.SelectedIndex + 5;
            UpdateUI();
        }

        private bool CheckTime()
        {
            return GetTime() < 0;
        }

        private int GetTime()
        {
            return game_length - table_time;
        }

        private void UpdateUI()
        {
            sync.Execute(() => UpdateUIThread());
        }

        private void UpdateUIThread()
        {
            var error_time = CheckTime();

            if (error_time)
            {
                label10.BackColor = Color.Red;
                label10.ForeColor = Color.White;
            }
            else
            {
                label10.BackColor = SystemColors.Control;
                label10.ForeColor = Color.Black;
            }
            label10.Text = Utils.TimeFormat(game.Time);

            button1.Enabled = !error_time && player_goal != null && !import_process;
            button2.Enabled = !error_time && (player_assist1 != null || player_assist2 != null) && !import_process;
            button3.Enabled = !error_time && player_foul1 != null && player_foul_type > 0 && (player_foul_disc >= 2 || player_foul_disc >= 6);

            comboBox2.Enabled = !error_time && !import_process;
            comboBox3.Enabled = !error_time && !import_process;
            comboBox4.Enabled = !error_time && !import_process;
        }

        //Гол
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => 
            {
                try
                {
                    import_process = true;
                    UpdateUI();

                    ShowStatus("Send GOAL to instatfootball.com...", 0);

                    var mk = new Game.Marker(game, 8, 1, game.Half, -1) { Player1 = player_goal, TimeActualSrv = game.Time };
                    Web.SaveMarker(game, mk);
                    game.Insert(mk);
                    game.SaveLocal();

                    ShowStatus("Success", 0);
                }
                catch (Exception ex)
                {
                    ShowStatus(ex.Message, 1);
                }
                finally
                {
                    import_process = false;
                    ReloadDataGridView();
                    UpdateUI();
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => 
            {
                try
                {
                    import_process = true;
                    UpdateUI();

                    ShowStatus("Send ASSISTS to instatfootball.com...", 0);

                    if (player_assist1 != null)
                    {
                        var mk = new Game.Marker(game, 1, 2, game.Half, -1) { Player1 = player_assist1, TimeActualSrv = game.Time };
                        Web.SaveMarker(game, mk);
                        game.Insert(mk);
                    }

                    if (player_assist2 != null)
                    {
                        var mk = new Game.Marker(game, 1, 2, game.Half, -1) { Player1 = player_assist2, TimeActualSrv = game.Time };
                        Web.SaveMarker(game, mk);
                        game.Insert(mk);
                    }

                    game.SaveLocal();

                    ShowStatus("Success", 0);
                }
                catch (Exception ex)
                {
                    ShowStatus(ex.Message, 1);
                }
                finally
                {
                    import_process = false;
                    ReloadDataGridView();
                    UpdateUI();
                }
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    import_process = true;
                    UpdateUI();

                    ShowStatus("Send FOUL to instatfootball.com...", 0);

                    if (player_foul_del > 1)
                    {
                        var mk = new Game.Marker(game, 5, player_foul_del, game.Half, -1) { Player1 = player_foul1, TimeActualSrv = game.Time };
                        Web.SaveMarker(game, mk);
                        game.Insert(mk);
                    }

                    if (player_foul_disc > 5)
                    {
                        var mk = new Game.Marker(game, 5, player_foul_disc, game.Half, -1) { Player1 = player_foul1, TimeActualSrv = game.Time };
                        Web.SaveMarker(game, mk);
                        game.Insert(mk);
                    }

                    var mk1 = new Game.Marker(game, 9, player_foul_type, game.Half, -1) { Player1 = player_foul1, TimeActualSrv = game.Time };
                    Web.SaveMarker(game, mk1);
                    game.Insert(mk1);

                    game.SaveLocal();

                    ShowStatus("Success", 0);
                }
                catch (Exception ex)
                {
                    ShowStatus(ex.Message, 1);
                }
                finally
                {
                    import_process = false;
                    ReloadDataGridView();
                    UpdateUI();
                }
            });
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dataGridView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(String.Format("Удалить маркеры ({0}) ?", dataGridView1.SelectedItems.Count), 
                    "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    return;

                var del = new List<Marker>();

                foreach (ListViewItem row in dataGridView1.SelectedItems)
                    {
                        var mk = (Game.Marker)row.Tag;
                        del.Add(mk);
                    }

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        import_process = true;
                        UpdateUI();

                        ShowStatus("DELETE from instatfootball.com...", 0);

                        var tasks = new List<Task>();
                        Exception exception = null;

                        foreach (Game.Marker mk in del)
                        {
                            tasks.Add(Task.Factory.StartNew(() => 
                            {
                                try
                                {
                                    var mk1 = new Game.Marker(game);
                                    mk1.Assign(mk);
                                    mk1.FlagDel = !mk.FlagDel;

                                    Web.SaveMarker(game, mk1);

                                    lock (game.Markers)
                                        game.Markers.Remove(mk);
                                }
                                catch (Exception ex)
                                {
                                    exception = ex;
                                }
                            }));
                        }

                        Task.WaitAll(tasks.ToArray<Task>());

                        if (exception != null)
                            throw exception;

                        ShowStatus("Success", 0);
                    }
                    catch (Exception ex)
                    {
                        ShowStatus(ex.Message, 1);
                    }
                    finally
                    {
                        import_process = false;
                        ReloadDataGridView();
                        UpdateUI();
                    }
                });
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedItems.Count > 0)
            {
                var mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;


                var mke = new Game.Marker(game);
                mke.Assign(mk);
                var mef = new MarkerEditForm();
                mef.Edit(mke);
                if (mef.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mk.Assign(mke);
                    mk.FlagUpdate = true;


                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            import_process = true;
                            UpdateUI();

                            ShowStatus("UPDATE instatfootball.com...", 0);

                            Web.SaveMarker(game, mk);


                            ShowStatus("Success", 0);
                        }
                        catch (Exception ex)
                        {
                            ShowStatus(ex.Message, 1);
                        }
                        finally
                        {
                            import_process = false;
                            ReloadDataGridView();
                            UpdateUI();
                        }
                    });
                }
            }
        }
    }
}
