using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uniso.InStat.Classes;
using Uniso.InStat.Game;
using Uniso.InStat.Gui.Controls;
using Uniso.InStat.Models;
using Uniso.InStat.PlayerTypes;
using Uniso.InStat.Server;
using Utils = Uniso.InStat.Classes.Utils;

namespace Uniso.InStat.Gui.Forms
{
    public partial class MainForm : Form
    {
        public HockeyIce Game = null;

        private bool lockUpdateTactics = false;
        private Uniso.InStat.Gui.UISync sync = null;
        private Player[] players = null;
        private bool lock_change_players_in_panel = false;        
        private Stopwatch sw = new Stopwatch();

        public MainForm(HockeyIce game)
        {
            Game = game;
            InitializeComponent();
            checkBox2.Checked = Options.G.IsStopPlayingOnShot;

            if (Options.G.PlaySound == false)
            {
                trackBar2.Value = trackBar2.Minimum;

                vlcStreamPlayer1.Volume = trackBar2.Value;
            }
            

            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ВСЕ МАРКЕРЫ", Rule = new Func<Game.Marker, bool>(o => true) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ВБР+СТОП+ГОЛ", Rule = new Func<Game.Marker, bool>(o => o.Compare(1, 1) || o.Compare(3, 8) || o.Compare(8, 1)) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "БРОСКИ", Rule = new Func<Game.Marker, bool>(o => o.Compare(4, new int[] {1, 2, 3,})) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ГОЛЫ", Rule = new Func<Game.Marker, bool>(o => o.Compare(8) || o.Compare(1, 2)) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ГОЛЫ+БРОСКИ", Rule = new Func<Game.Marker, bool>(o => o.Compare(new int[] { 4, 8 })) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "СИЛОВЫЕ ПРИЕМЫ", Rule = new Func<Game.Marker, bool>(o => o.Compare(6, 1)) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ЗАМЕНЫ", Rule = new Func<Game.Marker, bool>(o => o.Compare(new int[] { 14, 16 })) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ВСЕ КРОМЕ ЗАМЕН", Rule = new Func<Game.Marker, bool>(o => !o.Compare(new int[] { 14, 16 })) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "СИЛОВЫЕ ПРИЕМЫ", Rule = new Func<Game.Marker, bool>(o => o.Compare(6, 1)) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ШТРАФЫ", Rule = new Func<Game.Marker, bool>(o => o.Compare(3, 1) || o.Compare(5)) });
            comboBoxEx1.Items.Add(new SelectedMarker { Name = "ПЕРЕДАЧИ", Rule = new Func<Game.Marker, bool>(o => o.Compare(1, new int[] { 2, 3, 4, 5 })) });
        }

        public Half Half
        {
            get { return Game.Half; }
            set
            {
                sw.Reset();
                sw.Stop();

                Game.Half = value;
                UpdateUI();
            }
        }

        public Marker GetFirstFaceoff()
        {
            Marker mk1 = null;
            lock (Game.Markers)
                mk1 = Game.Markers.Where(o => !o.FlagDel && o.Compare(1, 1) && o.Half.Index == Game.Half.Index)
                    .OrderBy(o => o.TimeVideo).FirstOrDefault();

            return mk1;
        }

        public int Second
        {
            get
            {
                if (HockeyIce.Role == HockeyIce.RoleEnum.Online)
                {
                    return Convert.ToInt32(sw.ElapsedMilliseconds) + offset;
                }

                if (vlcStreamPlayer1 == null)
                    return 0;

                return vlcStreamPlayer1.Position;
            }
            set
            { 
                if (vlcStreamPlayer1 == null)
                    return;

                vlcStreamPlayer1.Position = value;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sync = new Uniso.InStat.Gui.UISync(this);

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

            Game.editMarker = new HockeyIce.EditMrk();
            hockeyField1.Game = Game;
            spareField1.Game = Game;
            spareField1.Team = Game.Match.Team1;
            spareField2.Game = Game;
            spareField2.Team = Game.Match.Team2;

            HockeyGui.ManyPlayersCompleteA = linkLabel_a12;
            HockeyGui.ManyPlayersCompleteB = linkLabel6;
            HockeyGui.HF = hockeyField1;
            HockeyGui.SF1 = spareField1;
            HockeyGui.SF2 = spareField2;
            HockeyGui.Mode = HockeyGui.ModeEnum.View;

            RefreshRole();

            flowLayoutPanel1.Controls.Clear();
            for (var i = 0; i < Game.HalfList.Count; i++)
            {
                var h = Game.HalfList[i];
                if (h.Index <= 3 || h.Index == 255)
                {
                    var label = new LinkLabel();
                    label.AutoSize = true;
                    label.Font = new Font(label.Font.Name, 6.5f, FontStyle.Regular);
                    label.LinkColor = Color.White;
                    label.LinkClicked += linkLabel3_LinkClicked;
                    label.Tag = h;
                    label.Text = h.Name;
                    flowLayoutPanel1.Controls.Add(label);
                }
            }

            var labelo = new LinkLabel();
            labelo.AutoSize = true;
            labelo.Font = new Font(labelo.Font.Name, 6.5f, FontStyle.Regular);
            labelo.LinkColor = Color.White;
            labelo.LinkClicked += linkLabel3_LinkClicked;
            labelo.Tag = 3;
            labelo.Text = "Overtime";
            flowLayoutPanel1.Controls.Add(labelo);

            var label2 = new LinkLabel();
            label2.Font = new Font(label2.Font.Name, 6.5f, FontStyle.Regular);
            label2.LinkColor = Color.White;
            label2.LinkClicked += linkLabel3_LinkClicked;
            label2.Tag = 1;
            label2.Text = "Перерыв";
            flowLayoutPanel1.Controls.Add(label2);

            //HockeyGui.LabelInviteAction = label2;

            var label3 = new LinkLabel();
            label3.Font = new Font(label3.Font.Name, 6.5f, FontStyle.Regular);
            label3.LinkColor = Color.White;
            label3.LinkClicked += linkLabel3_LinkClicked;
            label3.Tag = 2;
            label3.Text = "Конец матча";
            flowLayoutPanel1.Controls.Add(label3);

            lock (Game.Markers)
            {
                //Game.Markers.SortBy();
                Game.RecalcActualTime(Game.Markers, null);

                foreach (var mk in Game.Markers
                    .Where(o => o.Half != null)
                    .OrderBy(o => o.Half.Index)
                    .ThenBy(o => o.TimeVideo)
                    .ThenBy(o => o.Id))
                {
                    if (mk.ActionId == 18)
                    {
                        var mks = Game.HalfList.Where(o => o.ActionType == mk.ActionType).ToList<Half>();
                        if (mks.Count > 0)
                            Half = mks[0];
                        else
                            if (Game.HalfList.Exists(o => o.ActionType == 1))
                                Half = Game.HalfList.First(o => o.ActionType == 1);
                    }
                }
            }

            if (Half == null)
                Half = Game.HalfList[0];

            ReloadDataGridView();

            UpdateTactics();
            UpdateUI();
            ShowAddPanel(false);

            if (HockeyIce.Role == HockeyIce.RoleEnum.Online)
            {
                macTrackBar1.Enabled = false;
                timer1.Start();
            }

            toolStripStatusLabel1.Width = Width - 50;

            UpdateAutoSaveTimer();
            
            SetEditMarker(null, StageEnum.CreateMarker);

            players = Game.Match.Team1.Players.Concat(Game.Match.Team2.Players).ToArray<Player>();

            switch (HockeyIce.Role)
            {
                case HockeyIce.RoleEnum.Ttd:
                    comboBox10.SelectedIndex = 0;
                    break;
                case HockeyIce.RoleEnum.Substitutions:
                    comboBox10.SelectedIndex = 1;
                    break;
                case HockeyIce.RoleEnum.AdvTtd:
                    comboBox10.SelectedIndex = 2;
                    break;
            }
        }

        private void RefreshRole()
        {
            var role = "";
            switch (HockeyIce.Role)
            {
                case HockeyIce.RoleEnum.Substitutions:
                    role = " (ЗАМЕНЫ)";
                    break;

                case HockeyIce.RoleEnum.Ttd:
                    role = " (ТТД)";
                    break;

                case HockeyIce.RoleEnum.Online:
                    role = " (ТТД онлайн)";
                    break;

                case HockeyIce.RoleEnum.AdvTtd:
                    role = " (ПОДРОБНОЕ ТТД)";
                    break;
            }

            Text = Game.Match.ToString() + role;

            panelFullTtd.Visible = HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd;
            panel9.Visible = HockeyIce.Role != HockeyIce.RoleEnum.AdvTtd;
            panel10.Visible = HockeyIce.Role != HockeyIce.RoleEnum.AdvTtd;
        }

        private bool auto_update_datagrid = true;

        private void dataGridView1_MouseLeave(object sender, EventArgs e)
        {
            auto_update_datagrid = true;
        }

        private void dataGridView1_MouseEnter(object sender, EventArgs e)
        {
            auto_update_datagrid = false;
        }

        private String TimeFormat(long ms)
        {
            var ss = ms / 1000L;
            var m = ss / 60L;
            var s = ss % 60L;
            var mss = (ms % 1000L) / 100;
            return String.Format("{0}:{1}.{2}", m.ToString("00"), s.ToString("00"), mss.ToString("0"));
        }

        private void DeleteSelectedMarkers(object sender, EventArgs e)
        {
            var lst = new List<ListViewItem>();
            foreach (ListViewItem item in dataGridView1.SelectedItems)
            {
                lst.Add(item);
            }

            if (lst.Count > 0 &&
                MessageBox.Show("Удалить маркеры (" + lst.Count + "шт.)?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Log.Write("DELETE MANUAL");

                var lstDel = new List<Marker>();
                dataGridView1.BeginUpdate();
                try
                {
                    lock (Game.Markers)
                    {
                        var lstDel1 = new List<Marker>();
                        foreach (var row in lst)
                        {
                            if (row.Tag is Marker)
                            {
                                var mk = (Marker)row.Tag;

                                if (mk.FlagDel)
                                {
                                    mk.FlagDel = false;
                                    mk.FlagGuiUpdate = true;
                                    continue;
                                }

                                lstDel1.Add(mk);
                            }
                        }

                        lstDel1 = lstDel1.OrderByDescending(o => o.TimeVideo).ToList<Marker>();

                        foreach (var mk in lstDel1)
                        {
                            if (lstDel.IndexOf(mk) < 0)
                                lstDel.Add(mk);
                        }

                        var mkd = lstDel.Count > 0 ? lstDel[0] : null;

                        for (var i = 0; i < lstDel.Count; i++)
                        {
                            var mk = lstDel[i];
                            Game.Remove(mk);
                        }

                        if (mkd != null && HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                            Game.RecalcMarkers(mkd.Half);

                        ReloadDataGridView(true);
                        dataGridView1.SelectedItems.Clear();

                        Game.SaveLocal();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.WriteException(ex);
                }
                finally
                {
                    dataGridView1.EndUpdate();
                    //UpdateUI();
                }
            }
        }

        private Object lockerDataGrid = new object();
        private bool hide_subs_markers = false;
        private bool only_current_time = false;
        //private Font font_italic = null;

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

            if (action == ActionEnum._18_05)
                name += String.Format(" #{0}", mk.Half.Index - 3);
            
            var time = Game.TimeToString(mk.TimeVideo);
            if (Options.G.Game_ShowActualTime)
                time += "/" + Game.TimeToString(mk.TimeActual);

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
                    mk.row.BackColor = 
                        mk.FlagSaved ? 
                            (mk.FlagUpdate ? 
                                Color.Gray : 
                                Color.LightGray) : 
                            Color.White;
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

        public void ReloadDataGridView(bool force_update)
        {
            if (!auto_update_datagrid && !force_update)
                return;

            lock (lockerDataGrid)
            {
                var aaa = new List<ListViewItem>();

                sync.Execute(() =>
                {
                    dataGridView1.BeginUpdate();
                    dataGridView1.Items.Clear();
                });

                var list = new List<Marker>();
                lock (Game.Markers)
                {
                    foreach (var mki in Game.Markers
                        .OrderBy(o => o.Half.Index)
                        .ThenBy(o => o.TimeVideo)
                        .ThenBy(o => o.TimeActual)
                        .ThenBy(o => o.Id))
                    {
                        list.Add(mki);
                    }
                }

                foreach (Game.Marker mk in list)
                {
                    if (selectedMarker != null 
                        //&& mk.ActionId != 14
                        && !selectedMarker.Rule(mk))
                        continue;

                    if (only_current_time && (mk.Half.Index != Game.Half.Index || mk.TimeVideo > Game.Time))
                        continue;

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

                SlideDatagridToCurrentTime(force_update);

                lockerDataGrid = new object();
            }
            //});
        }

        public void SlideDatagridToCurrentTime(bool force_update)
        {
            if (!auto_update_datagrid && !force_update)
                return;

            sync.Execute(() =>
            {
                var mk_sel = Game.GetLastByTime(Game.Half, Game.Time);
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

        public void UpdateGridView(Game.Marker mk)
        {
            sync.Execute(() =>
            {
                lock (locker)
                {
                    if (mk.FlagGuiUpdate && mk.row != null)
                    {
                        FillRow(mk);
                        mk.FlagGuiUpdate = false;
                    }

                    locker = new object();
                }
            });
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            var lst = new List<ListViewItem>();
            foreach (ListViewItem item in dataGridView1.SelectedItems)
            {
                lst.Add(item);
            }

            if (lst.Count > 0)
            {
                var row = lst[0];
                if (row.Tag is Marker)
                {
                    var mk = (Game.Marker)row.Tag;

                    if (mk.Compare(1, 1) && mk.Player1 == null && mk.Player2 == null)
                    {
                        ProcessingMarker(mk);
                        return;
                    }

                    if (mk.Compare(6, 1) && mk.Player1 == null && mk.Player2 == null)
                    {
                        ProcessingMarker(mk);
                        return;
                    }

                    if (mk.Compare(8, 1) && mk.Point1.IsEmpty)
                    {
                        mk.Point2 = Point.Empty;
                        ProcessingMarker(mk);
                        return;
                    }

                    if (mk.ActionId == 5 && mk.ActionType > 1 && mk.Num == 0 && HockeyIce.Role != HockeyIce.RoleEnum.Ttd)
                    {
                        lock (Game.Markers)
                        {
                            if (!Game.Markers.Any(o =>
                                  !o.FlagDel
                                && o.Half.Index == mk.Half.Index
                                && o.TimeVideo > mk.TimeVideo - 100
                                && o.TimeVideo < mk.TimeVideo + 100
                                && o.ActionId == 14
                                && o.player2_id == mk.player1_id))
                            {
                                mk.FlagUpdate = true;

                                var placemk = mk.Player1.Team.Tactics[0].GetPlace(mk.Player1);
                                if (placemk == null)
                                    foreach (var t in mk.Player1.Team.Tactics.Values)
                                    {
                                        placemk = t.GetPlace(mk.Player1);
                                        if (placemk != null)
                                            break;
                                    }

                                if (placemk != null)
                                {
                                    if (MessageBox.Show("Установить маркер выхода с площадки для игрока " + mk.Player1, "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                                        return;

                                    var mkc = new Game.Marker(Game, 14, placemk.GetCode(), mk.Half, mk.TimeVideo) { Player1 = null, Player2 = mk.Player1 };
                                    Log.Write((string) ("INSERT FINE " + mkc.Player1));
                                    Game.Insert(mkc);
                                    ReloadDataGridView(false);
                                    return;
                                }
                            }
                        }

                        return;
                    }

                    Second = mk.TimeVideo - 1500;
                }
            }
        }

        private void UpdateAutoSaveTimer()
        {
            sync.Execute(() =>
            {
                if (Options.G.AutoSavePeriod > 0)
                {
                    timer2.Interval = Options.G.AutoSavePeriod * 1000;
                    timer2.Enabled = true;
                }
                else
                    timer2.Enabled = false;
            });
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var l = (LinkLabel)sender;
            Game.Marker mk = null;
            if (l.Tag is Half)
            {
                mk = (Game.Marker)Game.CreateMarkerBegin((Half)l.Tag, Second);
                Half = mk.Half;
            }

            var time = Second;
            var maxplayersnum = 0;
            var overtimelength = 0;

            if (l.Tag is Int32)
            {
                var id = Convert.ToInt32(l.Tag);

                //Overtime
                if (id == 3)
                {
                    var form = new SelectOvertimeForm(Game);
                    form.ShowDialog();
                    if (form.Half != null)
                    {
                        mk = (Game.Marker)Game.CreateMarkerBegin(form.Half, 0);
                        Half = mk.Half;
                    }

                    maxplayersnum = form.MaxPlayersNum;
                    overtimelength = form.OvertimeLength;
                }
                else
                //Завершения
                {
                    if (Half.Index < Game.HalfList[3].Index)
                    {
                        var actual_time = Game.GetActuialTime(Game.Half, Game.Time);
                        if (Options.G.Game_FindActualTimeForHalfFinally)
                        {
                            if (actual_time < Game.HalfList[0].Length)
                            {
                                for (var i = 0; i < 30000; i += 100)
                                {
                                    actual_time = Game.GetActuialTime(Game.Half, Game.Time + i);
                                    if (actual_time >= Game.HalfList[0].Length)
                                    {
                                        time = Game.Time + i;
                                        break;
                                    }

                                    if (i > 30000)
                                        throw new Exception("Не могу определить время установки маркера конца периода! Не найден в пределе ближайших 30 секунд");
                                }
                            }
                            else
                            {
                                if (actual_time > Game.HalfList[0].Length)
                                {
                                    for (var i = 0; i < 30000; i += 100)
                                    {
                                        actual_time = Game.GetActuialTime(Game.Half, Game.Time - i);
                                        if (actual_time - 100 <= Game.HalfList[0].Length)
                                        {
                                            time = Game.Time - i;
                                            break;
                                        }

                                        if (i > 30000)
                                            throw new Exception("Не могу определить время установки маркера конца периода! Не найден в пределе ближайших 30 секунд");
                                    }
                                }
                            }
                        }
                    }

                    switch (id)
                    {
                        case 1:
                            mk = new Game.Marker(Game) { ActionId = 18, ActionType = 3, Half = Half, TimeVideo = time };
                            break;
                        case 2:
                            mk = new Game.Marker(Game) { ActionId = 18, ActionType = 4, Half = Half, TimeVideo = time };
                            break;
                    }
                }
            }

            try
            {
                try
                {
                    if (mk != null)
                    {
                        Game.IsCanCreateMarker(Half, Second, mk);

                        Game.Insert(mk);
                        ReloadDataGridView();
                    }
                }
                catch (Game.GameBase.InsertMarkerException ex)
                { }

                InStat.Marker mkh = null;
                lock (Game.Markers)
                    mkh = Game.Markers.FirstOrDefault(o => o.Compare(18, 5) && o.Half.Index == 4);

                if (mkh != null)
                {
                    var num = overtimelength * 10000 + maxplayersnum;
                    if (mkh.Num != num)
                    {
                        mkh.Num = num;
                        mkh.FlagUpdate = true;

                        Game.RefreshOvertimes();
                        ShowTimer();
                    }
                }

                if (mk.Compare(18, new int[] { 3, 4 }))
                {
                    ReturnAllPlayers(Game.Match.Team1, time, 0);
                    ReturnAllPlayers(Game.Match.Team2, time, 0);
                }

                try
                {
                    Game.SaveLocal();
                }
                catch (Exception ex)
                {
                    ShowStatus(ex.Message, 1);
                }

                
            }
            catch (Game.GameBase.InsertMarkerException imex)
            {
                MessageBox.Show(imex.Message, "Ошибка вставки маркера", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateUI();
            ShowAddPanel(false);
        }

        private void InsertFine(Game.Marker mk, Player player, Half _half, int _second, bool withoutdelete)
        {
            var form = new FineForm(mk, player);
            form.WithoutDelete = withoutdelete;
            form.ShowDialog();

            var no_opponent = false;
            foreach (var mki in form.GetResult())
            {
                if (mki.ActionId == 5 && mki.ActionType >= 2 && mki.ActionType <= 5)
                {
                    var placemk = player.Team.Tactics[0].GetPlace(mki.Player1);

                    if (placemk == null)
                        foreach (var t in player.Team.Tactics.Values)
                        {
                            placemk = t.GetPlace(mki.Player1);
                            if (placemk != null)
                                break;
                        }

                    if (placemk != null)
                    {
                        if (!form.WithoutDelete && (HockeyIce.Role != HockeyIce.RoleEnum.Ttd && HockeyIce.Role != HockeyIce.RoleEnum.Online))
                        {
                            var mkc = new Game.Marker(Game, 14, placemk.GetCode(), _half, _second) { Player1 = null, Player2 = mki.Player1 };
                            Log.Write((string) ("INSERT FINE " + mkc.Player1));
                            Game.Insert(mkc);
                        }
                    }
                }

                if (mki.Compare(9, new int[] {8, 11}))
                    no_opponent = true;

                mki.Num = form.WithoutDelete ? 1 : 0;
                mki.Half = _half;
                mki.TimeVideo = _second;
                Game.Insert(mki);
            }

            var tctnum = new Dictionary<Team, List<int>>();
            tctnum.Add(Game.Match.Team1, new List<int>());
            tctnum.Add(Game.Match.Team2, new List<int>());
            
            foreach (var mki in form.GetResult())
            {                
                if (mki.Compare(5, 9))
                    continue;

                if (mki.ActionId == 9)
                    continue;

                if (mki.Player1 != null)
                {
                    var tm = mki.Player1.Team;

                    if (tm.Tactics[0].Places.Exists(o => o.Player != null && o.Player.Id == mki.Player1.Id))
                    {
                        var place = Enumerable.First<Place>(tm.Tactics[0].Places, o => o.Player != null && o.Player.Id == mki.Player1.Id);

                        if (tctnum[tm].IndexOf(0) < 0)
                            tctnum[tm].Add(0);

                        place.Player = null;
                    }
                }
            }

            ReloadDataGridView();

            if (no_opponent)
            {
                mk.Player2 = null;
                mk.FlagUpdate = true;
                return;
            }

            if (form.IsPair && mk.Player2 != player)
            {
                Game.Insert(new Game.Marker(Game, 3, 1, mk.Half, mk.TimeVideo) { Player1 = mk.Player2, Player2 = mk.Player1, Point1 = mk.Point1 });
                InsertFine(mk, mk.Player2, mk.Half, _second, form.WithoutDelete);                
            }
        }

        private void InsertViolation(Game.Marker mk, Player player, Half _half, int _second)
        {
            var form = new ViolationForm(mk, player);
            form.ShowDialog();

            foreach (var mki in form.GetResult())
            {
                mki.Half = _half;
                mki.TimeVideo = _second;
                Game.Insert(mki);
            }

            ReloadDataGridView();

            if (form.IsPair && mk.Player2 != player)
            {
                Game.Insert(new Game.Marker(Game, 3, 1, mk.Half, mk.TimeVideo) { Player1 = mk.Player2, Player2 = mk.Player1, Point1 = mk.Point1 });
                InsertViolation(mk, mk.Player2, mk.Half, _second);
            }
        }

        private int reg_time = 0;

        public void ProcessingMarker(Game.Marker mk)
        {
            var group = new List<Marker>();
            group.Add(mk);
            group.AddRange(mk.flag_adding);

            var time = fixedTime > 0 ? fixedTime : Second;

            checkBox1.Visible = false;

            var bullet_period = Half.Index == 255;
            var mkct = Game.GetLastControlTimeMarker(Half, time);
            var stop_time = mkct == null || Game.IsStopTimeMarker(mkct);

            HockeyGui.ChangedPlayersList.Clear();

            var prevm = Game.GetPrevousMarkers(Half, mk.TimeVideo);

            if (mk == null || mk.Compare(0, 0))
            {
                mk.FlagGuiUpdate = true;
                SetEditMarker(mk, StageEnum.ScreenPosition);
                return;
            }

            var canceled = new List<StageEnum>();
            if (checkBox1.Checked)
                canceled.Add(StageEnum.Player2);

            var stage = Game.GetNextStage(mk, canceled);

            #region Online HockeyIce.Role

            if (HockeyIce.Role == HockeyIce.RoleEnum.Online)

            #region Online
            {
                if (mk.Compare(1, 1) && stage == StageEnum.Player1)
                    stage = StageEnum.CreateMarker;
                if (mk.Compare(6, 1) && stage == StageEnum.Player1)
                    stage = StageEnum.CreateMarker;
            }
            #endregion

            #endregion

            #region Player1 Player2


            if (stage == StageEnum.Player1 || stage == StageEnum.Player2)
            {
            #region Player1 Player2
                if ((mk.Compare(8, 1) || mk.Compare(3, new int[] {1, 2,}))
                    && stage == StageEnum.Player1 && !prevm.Exists(o => o.Compare(4, 6)))
                {
                    checkBox1.Checked = false;
                    checkBox1.Visible = true;
                }

                if (prevm.Exists(o => o.Compare(4, 6)) && mk.Compare(8, 1))
                {
                    var gmk = prevm.First(o => o.Compare(4, 6));
                    if (gmk.Player1 != null && !gmk.Point1.IsEmpty)
                    {
                        mk.Player1 = gmk.Player1;
                        mk.Player2 = gmk.Player2;
                        mk.Point1 = gmk.Point1;
                        mk.ExtraOptionsExists = true;
                        ProcessingMarker(mk);
                        return;
                    }
                }

                var stageName = mk.GetNameStage(stage);
                var s = Game.TimeToString(mk.TimeVideo) +
                        Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPlayer, mk, s);

                HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectPlayer, mk, s, this.label2);
                RefreshHockeyField();

                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPlayer, mk);
            }

            #endregion

            #endregion

            #region Point

            if (stage == StageEnum.Point)
            #region Point

            {
                Game.editMarker.G = mk;

                if (mk.Compare(1, 1))
                {
                    // Если начало матча или это новое вбрасывание после гола.
                    if (Game.GetActuialTime(mk.Half, mk.TimeVideo) < 1000 || prevm.Exists(o => o.Compare(8, 1)))
                    {
                        mk.Point1 = Game.GetDumpInPoints(new RectangleF(PointF.Empty, Game.FieldSize))[0];
                        ProcessingMarker(mk);
                        return;
                    }
                }

                // Если буллит
                if (mk.Compare(4, 6))
                {
                    mk.Point1 = Game.GetDumpInPoints(new RectangleF(PointF.Empty, Game.FieldSize))[0];
                    ProcessingMarker(mk);
                    return;
                }

                //var stageName = mk.GetNameStage(stage);
                //var s = Game.TimeToString(mk.TimeVideo) + Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPoint, mk, s);

                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPoint, mk);
                var stageName = mk.GetNameStage(stage);
                var s = Game.TimeToString(mk.TimeVideo) +
                        Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectPoint, mk, s, label2);
                RefreshHockeyField();
                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPoint, mk);


            }
            #endregion

            #endregion

            #region Point1 & Point2

            if (stage == StageEnum.PointAndDest)
            #region Point1 & Point2
            {
                Game.editMarker.G = mk;

                if (mk.Compare(4, 6))
                {
                    mk.Point1 = Game.GetDumpInPoints(new RectangleF(PointF.Empty, Game.FieldSize))[0];
                }

                //var stageName = mk.GetNameStage(stage);
                //var s = Game.TimeToString(mk.TimeVideo) + Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPointAndDest, mk, s);

                //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPointAndDest, mk);

                var stageName = mk.GetNameStage(stage);
                var s = Game.TimeToString(mk.TimeVideo) +
                        Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;

                HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectPointAndDest, mk, s, label2);
                RefreshHockeyField();
            }
            #endregion

            #endregion

            #region Extra Options

            if (stage == StageEnum.ExtraOptions)
            #region Extra Options
            {
                if (mk.Compare(12, 0))
                {
                    var stageName = mk.GetNameStage(stage);
                    var s = Game.TimeToString(mk.TimeVideo) +
                            Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;

                    //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectManyPlayers, mk);
                    HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectManyPlayers, mk, s, label2);
                }

                if (mk.Compare(2, 11))
                {
                    var stageName = mk.GetNameStage(stage);
                    var s = Game.TimeToString(mk.TimeVideo) +
                            Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                    //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectManyPlayers, mk);
                    HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectManyPlayers, mk, s, label2);
                }

                if (mk.Compare(6, 2))
                {
                    var stageName = mk.GetNameStage(stage);
                    var s = Game.TimeToString(mk.TimeVideo) +
                            Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                    //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectManyPlayers, mk);
                    HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectManyPlayers, mk, s, label2);
                }

                if (mk.Compare(8, 1))
                {
                    Game.Insert(mk);
                    lock (Game.Markers)
                        Game.RecalcActualTime(Game.Markers, Game.Half);
                    ReloadDataGridView();
                    var stageName = mk.GetNameStage(stage);
                    var s = Game.TimeToString(mk.TimeVideo) +
                            Convert.ToString(HockeyIce.convAction.ConvertTo(mk.Action, typeof(string))) + stageName;
                    //HockeyGui.SetMode(HockeyGui.ModeEnum.SelectManyPlayers, mk);
                    HockeyGui.SetInviteLabel(HockeyGui.ModeEnum.SelectManyPlayers, mk, s, label2);

                }

                if (mk.Compare(3, new int[] {1}))
                {
                    var mkf_time = mk.TimeVideo;
                    /*int mkf_time_abs = Game.GetActuialTime(mk.Half, mk.TimeVideo, true);
                    lock (Game.Markers)
                    {
                        InStat.Marker mks = Game.Markers.Where(o => o.TimeActualTotal >= mkf_time_abs)
                            .OrderBy(o => o.TimeActualTotal)
                            .FirstOrDefault(o => Game.IsStopTimeMarker(o));

                        if (mks != null)
                        {
                            mkf_time = mks.TimeVideo;
                        }
                    }

                    InsertFine(mk, mk.Player1, Half, mkf_time, false);*/

                    InsertViolation(mk, mk.Player1, mk.Half, mk.TimeVideo);

                    mk.ExtraOptionsExists = true;
                    ProcessingMarker(mk);
                    return;
                }
            }

            #endregion

            #endregion


            mk.FlagGuiUpdate = true;

            #region Create Marker

            if (stage == StageEnum.CreateMarker)
            #region Create Marker
            {
                fixedTime = 0;

                mk.FlagUpdate = true;

                //Выставляем необходимость синхронизации
                if (HockeyIce.Role == HockeyIce.RoleEnum.Online && mk.Sync == 0)
                    mk.Sync = 1;

                if (mk.Compare(3, new int[] {1, 2}))
                    mk.ExtraOptionsExists = true;

                lock (Game.editMarker)
                {
                    if (Game.editMarker.G != null && Game.editMarker.G.Compare(8, 1) &&
                        !prevm.Exists(o => o.Compare(4, 6)) && checkBox1.Checked)
                    {
                        checkBox1.Checked = false;
                        mk.Player2 = null;
                    }
                }

                HockeyGui.SetMode(HockeyGui.ModeEnum.View, null);
                SetEditMarker((Game.Marker) null, stage);

                if (mk.Compare(2, 4))
                {
                    if (prevm.Any(o => o.Compare(new int[] {1, 2, 4, 7, 8})))
                    {
                        var mkp = prevm.First(o => o.Compare(new int[] {1, 2, 4, 7, 8}));
                        if (mk.team1_id == mkp.team1_id && mkp.Compare(1, new int[] {3, 4, 5}))
                            mk.ActionType = 4;
                        else
                            mk.ActionType = 5;
                    }

                    if (prevm.Any(o => o.Compare(2, new int[] {3, 4}) && o.player1_id == mk.player1_id))
                    {
                        mk.ActionType = 3;
                    }
                }

                if (!mk.Compare(6, 2))
                {
                    /*если в маркере предыдущего действия не задействован игрок, выполняющий текущее действие
                    Если предыдущее действе - не Вбрасывание, КП, ПП, ОП или Наброс игрока той же команды, что совершает действие
                    Если текущее действие - ни подбор, ни прием, ни единоборство, ни перехват, ни перехват неудачный, ни фол,Подбор в борьбе
 
                    то игроку, совершающему действие, ставится автоматический подбор*/

                    //ПОДБОР
                    if (Game.IsInsertPickUp(mk))
                    {
                        var action_type = 5;
                        Game.Insert(new Game.Marker(Game, 2, action_type, mk.Half, mk.TimeVideo - 100)
                        {
                            Player1 = mk.Player1,
                            Point1 = mk.Point1
                        });
                    }

                    var foul_after_stop = mk.Compare(3, 1) && mkct != null && mkct.Compare(3, 8);
                    if (mk.Compare(3, 8) || foul_after_stop)
                    {
                        //Если стоп-игра, то ищем фол и причины, для выставления штрафа
                        var mk11 = Game.GetLastControlTimeMarker(mk.Half, mk.TimeVideo);
                        if (mk11 != null &&
                            ((mk11.Compare(1, 1) && !foul_after_stop) || (mk11.Compare(3, 8) && foul_after_stop)))
                        {
                            List<Marker> mk9_list = null;
                            lock (Game.Markers)
                                mk9_list = Game.Markers.Where(o
                                        => !o.FlagDel
                                           && o.Half.Index == mk.Half.Index
                                           && o.TimeVideo <= mk.TimeVideo
                                           && o.TimeVideo > mk11.TimeVideo
                                           &&
                                           (!Game.GetSiblings(o)
                                               .Any(o1 => o1.Compare(5) && o1.player2_id == o.player1_id))
                                           && o.Compare(9))
                                    .ToList<Marker>();

                            var withoutdelete = false;
                            foreach (var mk9 in mk9_list)
                            {
                                var player = mk9.Player1;
                                var form = new PenaltyForm(mk, player);
                                form.WithoutDelete = withoutdelete;
                                form.ShowDialog();

                                withoutdelete = form.WithoutDelete;
                                var no_opponent = false;
                                foreach (var mki in form.GetResult())
                                {
                                    if (mki.Compare(5, new int[] {2, 3, 4, 5}))
                                    {
                                        var placemk = player.Team.Tactics[0].GetPlace(mki.Player1);
                                        mki.Num = 1;

                                        /*if (placemk == null)
                                            foreach (Tactics t in player.Team.Tactics.Values)
                                            {
                                                placemk = t.GetPlace(mki.Player1);
                                                if (placemk != null)
                                                    break;
                                            }*/

                                        if (placemk != null)
                                        {
                                            if (!form.WithoutDelete &&
                                                (HockeyIce.Role != HockeyIce.RoleEnum.Ttd &&
                                                 HockeyIce.Role != HockeyIce.RoleEnum.Online))
                                            {
                                                var mkc = new Game.Marker(Game, 14, placemk.GetCode(), mk.Half,
                                                    mk.TimeVideo)
                                                {
                                                    Player1 = null,
                                                    Player2 = mki.Player1
                                                };

                                                Log.Write((string) ("INSERT FINE " + mkc.Player1));
                                                Game.Insert(mkc);
                                                mki.Num = 0;
                                            }
                                        }
                                    }

                                    if (mki.Compare(9, new int[] {8, 11}))
                                        no_opponent = true;

                                    mki.Half = mk.Half;
                                    mki.TimeVideo = mk.TimeVideo;
                                    Game.Insert(mki);
                                }

                                var tctnum = new Dictionary<Team, List<int>>();
                                tctnum.Add(Game.Match.Team1, new List<int>());
                                tctnum.Add(Game.Match.Team2, new List<int>());

                                foreach (var mki in form.GetResult())
                                {
                                    if (mki.Compare(5, 9))
                                        continue;

                                    if (mki.ActionId == 9)
                                        continue;

                                    if (mki.Player1 != null)
                                    {
                                        var tm = mki.Player1.Team;

                                        if (
                                            tm.Tactics[0].Places.Exists(
                                                o => o.Player != null && o.Player.Id == mki.Player1.Id))
                                        {
                                            var place = Enumerable.First<Place>(tm.Tactics[0].Places,
                                                o => o.Player != null && o.Player.Id == mki.Player1.Id);

                                            if (tctnum[tm].IndexOf(0) < 0)
                                                tctnum[tm].Add(0);

                                            place.Player = null;
                                        }
                                    }
                                }

                                if (no_opponent)
                                {
                                    mk.Player2 = null;
                                    mk.FlagUpdate = true;
                                    return;
                                }
                            }
                        }
                    }

                    if (!mk.Compare(8, 1) && !mk.Compare(2, 11))
                    {
                        var foul_list = Game.GetFoulEmpty(mk.Half, mk.TimeVideo);

                        Game.Insert(mk);
                    }

                    //Выброс (-)
                    if (group.Any(o => ((Game.Marker)o).Compare(1, 6, 2)))
                    {
                        // Если Выброс(-) БЕЗ Неточная
                        // ТО нихуя не делаем, иначе создаем Выброс(-) И Перехват через 0.1 сек.
                        if (group.Any(o => o.Compare(0, 0) && o.Win == 1) == false)
                        {
                            //Формируем перехват
                            var pt = new PointF(Game.FieldSize.Width - mk.Point2.X, Game.FieldSize.Height - mk.Point2.Y);
                            Game.Insert(new Game.Marker(Game, 2, 7, mk.Half, mk.TimeVideo + 100)
                            {
                                Player1 = mk.Player2,
                                Point1 = pt
                            });
                        }
                    }

                    //Передачи
                    foreach (Marker mka in mk.flag_adding)
                    {
                        if (mka.Compare(1, 6) && mka.Win == 2 && mk.Compare(1, new int[] {3, 4, 5, 7,}))
                        {
                            mka.Player1 = mk.Player1;
                            mka.Player2 = mk.Player2;
                            mka.Point1 = mk.Point1;
                            mka.Point2 = mk.Point2;
                            mka.Half = mk.Half;
                            mka.TimeVideo = mk.TimeVideo;
                            Game.Insert(mka);
                        }
                        else
                        {


                            MarkersWomboCombo.AddChildMarkerRule addrule = MarkersWomboCombo.CheckRuleForeExtraMarker(
                                mk, (Game.Marker) mka);

                            if (addrule == MarkersWomboCombo.AddChildMarkerRule.None)
                            {
                                // Если Выброс(-) И Неточная, то не добавляем дополнительный маркер.
                                if (mk.Compare(1, 6, 2) && ((Game.Marker) mka).Compare(0, 0, 1))
                                {
                                    continue;
                                }
                                mka.Player1 = mk.Player1;
                                mka.Point1 = mk.Point1;
                                mka.Half = mk.Half;
                                mka.TimeVideo = mk.TimeVideo;
                                Game.Insert(mka);
                            }
                            else if (addrule == MarkersWomboCombo.AddChildMarkerRule.Add100ms)
                            {
                                mka.Player1 = mk.Player1;
                                mka.Point1 = mk.Point1;
                                mka.Half = mk.Half;
                                mka.TimeVideo = mk.TimeVideo + 100;
                                Game.Insert(mka);
                            }


                        }

                        //Вставка доп маркера помеха
                        if (mk.flag_hitch)
                        {
                            var pt = new PointF(Game.FieldSize.Width - mk.Point1.X, Game.FieldSize.Height - mk.Point1.Y);
                            if (mk.Compare(1, 6) && mk.Win == 2)
                                pt = new PointF(Game.FieldSize.Width - mk.Point2.X, Game.FieldSize.Height - mk.Point2.Y);

                            ProcessingMarker(new Game.Marker(Game, 2, 9, mk.Half, mk.TimeVideo + 100)
                            {
                                Point1 = pt,
                                Player2 = mk.Player1
                            });
                            return;
                        }

                        //Вставка доп маркера проброс
                        if (mk.flag_icing)
                        {
                            Game.Insert(new Game.Marker(Game, 3, 3, mk.Half, mk.TimeVideo + 100)
                            {
                                Point1 = mk.Point1,
                                Player1 = mk.Player1,
                                Point2 = mk.Point2
                            });
                        }

                        lock (Game.Markers)
                            Game.RecalcActualTime(Game.Markers, Half);
                    }

                    Game.SaveLocal();

                    var bullet = false;

                    //Bullet
                    if (mk.Compare(3, new int[] {1, 2}))
                    {
                        var sibl = Game.GetSiblings(Half, mk.TimeVideo);
                        if (sibl.Exists(o => o.Compare(5, 9)))
                        {
                            var mksb = sibl.First(o => o.Compare(5, 9));
                            var mkb = new Game.Marker(Game, 4, 6) {Half = mk.Half, TimeVideo = mk.TimeVideo};
                            mkb.Team1 = mksb.Team1;
                            ProcessingMarker(mkb);
                            bullet = true;
                            return;
                        }
                    }

                    if (mk.Compare(4, 3) && HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                    {
                        ProcessingMarker(new Game.Marker(Game, 2, 5, mk.Half, mk.TimeVideo + 50));
                        return;
                    }

                    if (Game.IsStopTimeMarker(mk))
                    {
                        //Обработка фола отложенного после остановки времени
                        var mkfi_list = Game.GetFoulEmpty(Half, mk.TimeVideo);
                        if (mkfi_list.Count > 0)
                        {
                            ProcessingMarker((Game.Marker) mkfi_list[0]);
                            return;
                        }
                    }

                    if (mk.Compare(8, 1) && Half.Index >= 4 && Half.Index < 255)
                    {
                        Game.Insert(new Game.Marker(Game) {ActionId = 18, ActionType = 4, Half = Half, TimeVideo = time});
                    }

                    if (!bullet && vlcStreamPlayer1 != null)
                        vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;

                    //SendStreamTicket();
                    ReloadDataGridView();
                    UpdateTactics();

                    if (bullet_period)
                    {
                        RegisterBegin(0, 0);
                        vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
                    }
                }
                else
                    SetEditMarker(mk, stage);
            }
            #endregion

            #endregion

            #region Player2 GoalKeeper

            if (stage == StageEnum.Player2Gk)
            #region Player2 GoalKeeper
            {
                if (Half.Index == 255 && mk.Compare(4, 6))
                {
                    HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPlayer, mk);
                    return;
                }

                if (Half.Index == 255 && mk.Compare(8, 1) && prevm.Exists(o => o.Compare(4, 6)))
                {
                    var boolmk = prevm.First(o => o.Compare(4, 6));
                    mk.Player2 = boolmk.Player2;
                    return;
                }

                if (mk.Team1 != null && mk.Player2 == null)
                {
                    var team = Game.Match.Team1 == mk.Team1 ? Game.Match.Team2 : Game.Match.Team1;
                    var gk = team.Tactics[0].GetGK();
                    if (gk != null)
                        mk.Player2 = gk.Player;
                }

                if (mk.Player2 == null)
                {
                    //MessageBox.Show("Не удалось определить оппонента этому действию. Укажите его вручную",
                    //"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    HockeyGui.SetMode(HockeyGui.ModeEnum.SelectPlayer, mk);
                }
                else
                    ProcessingMarker(mk);
            }
                
            #endregion

            #endregion

            UpdateUI();
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAddPanel(false);
        }

        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAddPanel(true);
        }

        //Открыть доп панель
        public void ShowAddPanel(bool value)
        {
            linkLabel11.Visible = value;
            linkLabel12.Visible = !value;
            panel5.Visible = value;
        }

        public void ReloadDataGridView()
        {
            ReloadDataGridView(false);
        }

        /*public void ReloadDataGridView(bool force_update)
        {
            sync.Execute(() =>
                {
                    if (dataGridForm1 == null)
                        return;

                    dataGridForm1.ReloadDataGridView(force_update);
                });
        }

        public void UpdateGridView(Game.Marker mk)
        {
            sync.Execute(() =>
            {
                if (dataGridForm1 == null)
                    return;

                dataGridForm1.UpdateGridView(mk);
            });
        }*/

        private Object locker = new object();

        private void UpdateUIThread()
        {
            buttonOpen.Enabled = HockeyIce.Role != HockeyIce.RoleEnum.Online;

            label1.Text = Half != null ? Half.ToString() : "НЕТ ПЕРИОДА";

            Game.Marker edit = null;
            lock (Game.editMarker)
                edit = Game.editMarker.G;

            var lastmk = Game.GetSiblings(Half, Second);
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            var proc_and_clear = (edit != null && edit.Compare(0, 0)) || HockeyIce.Role == HockeyIce.RoleEnum.Online;
            var video_load = (vlcStreamPlayer1 != null && vlcStreamPlayer1.Mode != StreamPlayer.PlayerMode.Stop) || HockeyIce.Role == HockeyIce.RoleEnum.Online;
            var bullet_period = Half != null && Half.Index == 255;
            var foul_empty = Game.GetFoulEmpty(Half, Second).Count > 0;
            var mklct = Game.GetLastControlTimeMarker(Half, Second);
            var rest_time = mklct != null && Game.IsRestoreTimeMarker(mklct);
            var stop_time = mklct == null || Game.IsStopTimeMarker(mklct);

            var fine_bullet = false;
            if (stop_time && prevmk.Count > 0)
            {
                var siblprev = Game.GetSiblings(prevmk[0].Half, prevmk[0].TimeVideo);
                if (siblprev.Any(o => o.Compare(5, 9)))
                    fine_bullet = true;
            }

            var mk_ed_wait_add_1 = edit != null && edit.Compare(2, new int[] { 1, 10 }) && edit.flag_adding.Count == 0;
            var mk_ed_wait_add_2 = edit != null && edit.Compare(1, new int[] { 4, 5 }) && edit.flag_adding.Count == 0;

            button19.Enabled = !saving;
            linkLabel8.Enabled = true;// stop_time || prevmk.Count == 0;
            linkLabel9.Enabled = true;// stop_time || prevmk.Count == 0;
            macTrackBar1.Enabled = true;

            var mode_keys = proc_and_clear && Half != null && video_load && (!stop_time) && !bullet_period && !fine_bullet && prevmk.Count > 0;
            var mode_keys_add = edit != null && edit.ActionId > 0 && Half != null && 
                video_load && !stop_time && !bullet_period && !fine_bullet && prevmk.Count > 0 && !edit.Compare(2, 9);

            linkLabel_a12_0.Visible = false;
            linkLabel_a12_1.Visible = false;

            //фол
            button300100.Enabled = proc_and_clear
                && Half != null
                && video_load
                && !foul_empty
                && !bullet_period
                && !fine_bullet
                && prevmk.Count > 0;

            //Правка бросков и голов
            if ((HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                && prevmk.Any(o => o.TimeVideo > -1 && (o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
                && (Game.editMarker == null || edit.ActionId == 0))
            {
                var mk4 = (Game.Marker)prevmk
                    .Where(o => o.TimeVideo > -1 && (o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
                    .OrderByDescending(o => o.TimeVideo).First();

                if (!groupBoxEditBrosok.Visible)
                {
                    var pls = new List<Player>();

                    if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                    {
                        pls.AddRange(Game.Match.Team1.Tactics[0].GetPlayers());
                        pls.AddRange(Game.Match.Team2.Tactics[0].GetPlayers());
                    }
                    else
                    {
                        pls.AddRange(Game.Match.Team1.Players);
                        pls.AddRange(Game.Match.Team2.Players);
                    }

                    comboBox2.Items.Clear();
                    comboBox2.Items.AddRange(pls.ToArray<Player>());

                    comboBox3.Items.Clear();
                    comboBox3.Items.AddRange(pls.ToArray<Player>());

                    comboBox7.Items.Clear();
                    comboBox8.Items.Clear();

                    if (mk4.Compare(8, 1))
                    {
                        comboBox7.Enabled = true;
                        comboBox8.Enabled = true;

                        comboBox7.Items.AddRange(pls.ToArray<Player>());
                        comboBox8.Items.AddRange(pls.ToArray<Player>());
                    }
                    else
                    {
                        comboBox7.Enabled = false;
                        comboBox8.Enabled = false;
                    }

                    UpdateCombo29(mk4);
                }

                groupBoxEditBrosok.Visible = true;
                groupBoxEditBrosok.Text = Convert.ToString(HockeyIce.convAction.ConvertTo(mk4.Action, typeof(string))) + " " + Utils.TimeFormat(mk4.TimeVideo);
                groupBoxEditGoalKeeper.Visible = mk4.Compare(8, 1) || mk4.Compare(4, new int[] { 2, 3, 4, 5 });
                
                var sibl4 = Game.GetSiblings(mk4.Half, mk4.TimeVideo);


                button300600.Enabled = mk4.Compare(4, 2);
                button100900.Enabled = mk4.Compare(4, 2);

                FormatAddButton(button300600, sibl4.Any(o => ((Game.Marker)o).Compare(3, 6, 0)));
                FormatAddButton(button100900, sibl4.Any(o => ((Game.Marker)o).Compare(1, 9, 0)));

                FormatAddButton(button401000, sibl4.Any(o => o.Compare(4, 10)));
                FormatAddButton(button401100, sibl4.Any(o => o.Compare(4, 11)));

                FormatAddButton(button1100101, sibl4.Any(o => ((Game.Marker)o).Compare(11, 1, 1)));
                FormatAddButton(button1100102, sibl4.Any(o => ((Game.Marker)o).Compare(11, 1, 2)));

                FormatAddButton(button1100201, sibl4.Any(o => ((Game.Marker)o).Compare(11, 2, 1)));
                FormatAddButton(button1100202, sibl4.Any(o => ((Game.Marker)o).Compare(11, 1, 2)));
                FormatAddButton(button1100203, sibl4.Any(o => ((Game.Marker)o).Compare(11, 1, 3)));
                FormatAddButton(button1100204, sibl4.Any(o => ((Game.Marker)o).Compare(11, 1, 4)));
                FormatAddButton(button1100205, sibl4.Any(o => ((Game.Marker)o).Compare(11, 1, 5)));

                FormatAddButton(button1100301, sibl4.Any(o => ((Game.Marker)o).Compare(11, 3, 1)));
                FormatAddButton(button1100302, sibl4.Any(o => ((Game.Marker)o).Compare(11, 3, 2)));
                FormatAddButton(button1100303, sibl4.Any(o => ((Game.Marker)o).Compare(11, 3, 3)));

                FormatAddButton(button1100401, sibl4.Any(o => ((Game.Marker)o).Compare(11, 4, 1)));

                var pomexa = sibl4.FirstOrDefault(o => o.Compare(2, 9));

                var assist = sibl4.Where(o => o.Compare(1, 2)).ToList<Marker>();

                lock_change_players_in_panel = true;

                if (assist.Count > 0)
                    comboBox7.SelectedItem = assist[0].Player1;

                if (assist.Count > 1)
                    comboBox8.SelectedItem = assist[1].Player1;
                
                comboBox2.SelectedItem = mk4.Player1;
                comboBox3.SelectedItem = mk4.Player2;

                if (pomexa != null)
                    comboBox6.SelectedItem = pomexa.Player1;
                else
                    if (comboBox6.Items.Count > 0)
                        comboBox6.SelectedIndex = 0;

                lock_change_players_in_panel = false;
            }
            else
                groupBoxEditBrosok.Visible = false;

            //Правка вбрасываний
            if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd
                && prevmk.Any(o => o.TimeVideo > -1 && o.Compare(1, 1))
                && (Game.editMarker == null || edit.ActionId == 0))
            {
                var mk11 = (Game.Marker)prevmk
                    .Where(o => o.TimeVideo > -1 && o.Compare(1, 1))
                    .OrderByDescending(o => o.TimeVideo).First();

                if (!groupBoxEditVbrasivanije.Visible)
                {
                    var pls = new List<Player>();
                    pls.AddRange(Game.Match.Team1.Tactics[0].GetPlayers());
                    pls.AddRange(Game.Match.Team2.Tactics[0].GetPlayers());

                    comboBox5.Items.Clear();
                    comboBox5.Items.AddRange(pls.ToArray<Player>());

                    comboBox4.Items.Clear();
                    comboBox4.Items.AddRange(pls.ToArray<Player>());
                }

                groupBoxEditVbrasivanije.Visible = true;
                groupBoxEditVbrasivanije.Text = Convert.ToString(HockeyIce.convAction.ConvertTo(mk11.Action, typeof(string))) + " " + Utils.TimeFormat(mk11.TimeVideo);

                var sibl4 = Game.GetSiblings(mk11.Half, mk11.TimeVideo);

                lock_change_players_in_panel = true;
                comboBox5.SelectedItem = mk11.Player1;
                comboBox4.SelectedItem = mk11.Player2;
                lock_change_players_in_panel = false;
            }
            else
                groupBoxEditVbrasivanije.Visible = false;

            //Правка силовых
            if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd
                && prevmk.Any(o => o.TimeVideo > -1 && o.Compare(6, 1))
                && (edit == null || edit.ActionId == 0))
            {
                var mk11 = (Game.Marker)prevmk
                    .Where(o => o.TimeVideo > -1 && o.Compare(6, 1))
                    .OrderByDescending(o => o.TimeVideo).First();

                if (!groupBoxEditVbrasivanie.Visible)
                {
                    var pls = new List<Player>();
                    pls.AddRange(Game.Match.Team1.Tactics[0].GetPlayers());
                    pls.AddRange(Game.Match.Team2.Tactics[0].GetPlayers());

                    comboBox9.Items.Clear();
                    comboBox9.Items.AddRange(pls.ToArray<Player>());

                    comboBox1.Items.Clear();
                    comboBox1.Items.AddRange(pls.ToArray<Player>());
                }

                groupBoxEditVbrasivanie.Visible = true;
                groupBoxEditVbrasivanie.Text = Convert.ToString(HockeyIce.convAction.ConvertTo(mk11.Action, typeof(string))) + " " + Utils.TimeFormat(mk11.TimeVideo);

                var sibl4 = Game.GetSiblings(mk11.Half, mk11.TimeVideo);

                lock_change_players_in_panel = true;
                comboBox9.SelectedItem = mk11.Player1;
                comboBox1.SelectedItem = mk11.Player2;
                lock_change_players_in_panel = false;
            }
            else
                groupBoxEditVbrasivanie.Visible = false;

            //Гол
            if (bullet_period || fine_bullet)
            {
                button800100.Enabled = proc_and_clear && Half != null && video_load && !prevmk.Exists(o => o.Compare(8, 1));
            }
            else
            {
                button800100.Enabled = (proc_and_clear)
                    && !lastmk.Exists(o => o.Compare(8, 1))
                    && Half != null && video_load && (!stop_time || prevmk.Exists(o => o.Compare(4, 6)))
                    && prevmk.Count > 0;
            }

            //Adding buttons

            //Текущая - редактируемый пас
            var pas = edit != null && edit.Compare(1, new int[] { 3, 4, 5, 7, 8, 9 });


            //Проброс
            var marker_vibros_plus = edit != null && (edit.Compare(1, 6, 1));
            //button300300.Enabled = pas;
            button300300.Enabled = pas || marker_vibros_plus;
            //FormatAddButton(button300300, pas && edit.flag_icing);
            FormatAddButton(button300300, (pas || marker_vibros_plus) && edit.flag_icing);

            //Неточная     
            var marker_vibros_minus = edit != null && edit.Compare(1, 6) && edit.Win == 2;
            button000001.Enabled = pas || marker_vibros_minus;
            var btn000001_clicked = false;
            if (edit != null && edit.flag_adding.Any())
            {
                btn000001_clicked = edit.flag_adding.Any(x => x.Compare(0, 0) && x.Win == 2);
            }
            FormatAddButton(button000001, pas && edit.Win == 2 || marker_vibros_minus && btn000001_clicked);

            //Primary
            // ReSharper disable once InconsistentNaming
            var marker_perehvat =  edit != null && edit.Compare(2, 7, 0);
            var marker_ediniborstvo = edit != null && edit.Compare(2, 1, 0);
            var marker_otbor = edit != null && edit.Compare(2, 6, 0);
            button100300.Enabled = mode_keys || mk_ed_wait_add_1 || marker_perehvat || marker_ediniborstvo || marker_otbor;
            button100400.Enabled = mode_keys || mk_ed_wait_add_1 || marker_perehvat || marker_ediniborstvo || marker_otbor;
            button100500.Enabled = mode_keys || mk_ed_wait_add_1 || marker_perehvat || marker_ediniborstvo || marker_otbor;
            button100601.Enabled = mode_keys || mk_ed_wait_add_2 || marker_perehvat || marker_ediniborstvo || marker_otbor;
            button100602.Enabled = mode_keys || mk_ed_wait_add_2;
            button100800.Enabled = mode_keys || mk_ed_wait_add_1 || marker_perehvat || marker_ediniborstvo || marker_otbor;

            button1200000.Enabled = mode_keys;
            button1300100.Enabled = mode_keys;
            button1300200.Enabled = mode_keys;

            button200100.Enabled = mode_keys;
            button200201.Enabled = mode_keys;
            button200202.Enabled = mode_keys;
            button200300.Enabled = mode_keys;
            button200400.Enabled = mode_keys;
            button200600.Enabled = mode_keys;
            button200700.Enabled = mode_keys;
            button200702.Enabled = mode_keys;
            button200800.Enabled = mode_keys;
            button201100.Enabled = mode_keys;
            button300900.Enabled = mode_keys;
            button700100.Enabled = mode_keys;

            button300800.Enabled = mode_keys;
            button600100.Enabled = mode_keys;
            button400200.Enabled = mode_keys;
            button400300.Enabled = mode_keys;
            button400100.Enabled = mode_keys;

            //Буллит
            button400600.Enabled = proc_and_clear && (bullet_period || fine_bullet) && Half != null && video_load;
            button400600.Visible = bullet_period || fine_bullet;

            //Счет буллитов
            label14.Visible = bullet_period;

            //Вбрасывание
            button100100.Visible = !bullet_period && !fine_bullet && (!rest_time);
            button100100.Enabled = proc_and_clear && Half != null
                && video_load
                && !rest_time
                && (stop_time || prevmk.Count == 0)
                && !bullet_period && !fine_bullet;

            linkLabel2.Enabled = true;// prevmk.Count == 0;

            button300800.Visible = !stop_time;
            FormatButton(button300800);

            FormatButton(button300100);
            FormatButton(button800100);
            FormatButton(button600100);
            FormatButton(button400200);
            FormatButton(button400300);
            FormatButton(button100100);
            FormatButton(button400100);

            FormatButton(button100300);
            FormatButton(button100400);
            FormatButton(button100500);
            FormatButton(button100601);
            FormatButton(button100800);

            FormatButton(button1200000);

            FormatButton(button200100);
            FormatButton(button200201);
            FormatButton(button200400);
            FormatButton(button200600);
            FormatButton(button200700);
            FormatButton(button200800);


            button1400000.Visible = HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd ? false : true;
        }

        public void UpdateUI()
        {
            UpdateTactics();
            ShowTimer();

            sync.Execute(() =>
            {
                UpdateUIThread();
            });
        }

        private void UpdateCombo29(Marker mk4)
        {
            var pls = new List<Player>();
            if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
            {
                pls.AddRange(Game.Match.Team1.Tactics[0].GetPlayers());
                pls.AddRange(Game.Match.Team2.Tactics[0].GetPlayers());
            }
            else
            {
                pls.AddRange(Game.Match.Team1.Players);
                pls.AddRange(Game.Match.Team2.Players);
            }

            comboBox6.Items.Clear();
            var pl = new List<Object>(pls.Where(o => !o.IsGk && o.Team != mk4.Player1.Team).ToArray<Player>());
            pl.Insert(0, "НЕ УСТАНОВЛЕНА");
            comboBox6.Items.AddRange(pl.ToArray<Object>());
        }

        private void FormatAddButton(Button btn, bool value)
        {
            btn.BackColor = value
                ? Color.LightGray
                : SystemColors.Control;
        }

        private void FormatButton(Button btn)
        { 
            var select = new List<string>();
            lock (Game.editMarker)
            {
                if (Game.editMarker.G != null)
                {
                    select.Add(Game.editMarker.G.ActionCode.ToString());
                    foreach (var mk in Game.editMarker.G.flag_adding)
                        select.Add(mk.ActionCode.ToString());
                }
            }

            btn.BackColor = select.Any(o => btn.Tag.ToString().Equals(o))
                ? Color.LightGray
                : SystemColors.Control;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label5.Text = Utils.TimeFormat(Second);
            label8.Text = "---";

            if (hockeyField1.Visible && HockeyIce.Role != HockeyIce.RoleEnum.AdvTtd)
                hockeyField1.RefreshRgn();

            Game.Half = Half;
            Game.Time = Second;

            UpdateUI();
        }

        private void RegisterBegin(int action_code)
        {
            var mk = new InStat.Game.Marker(Game) { ActionCode = action_code };

            lock (Game.editMarker)
            {
                if (MarkersWomboCombo.CheckPrevMarkerNeedsExtraMarker(Game.editMarker.G, mk) == false)
                {
                    if (Game.editMarker.G != null)
                        if (
                            // Единоборство || Подбор в борьбе
                            Game.editMarker.G.Compare(2, new int[] {1, 10})
                            ||
                            // Пас (конструктивная передача) || ОП (острая передача)
                            Game.editMarker.G.Compare(1, new int[] {4, 5})
                        )
                        {
                            Game.editMarker.G.flag_adding.Add(mk);
                            UpdateUI();
                            return;
                        }
                }
            }

            RegisterBegin(mk.ActionId, mk.ActionType, mk.Win);
        }


        private void RegisterBegin(int action_id, int action_type, int win = 0)
        {
            try
            {
                Log.Write(String.Format("CREATE NEW 1: {0}-{1}-{2}", action_id, action_type, win));

                if (HockeyIce.Role != HockeyIce.RoleEnum.Online || (HockeyIce.Role == HockeyIce.RoleEnum.Online && !Options.G.Game_NoStopVideoInOnline))   
                    vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;

                var time = fixedTime > 0 ? fixedTime : Second;

                ShowStatus("", 0);

                lock (Game.editMarker)
                {
                    var newMarker = new Game.Marker(Game, action_id, action_type)
                    {
                        Half = this.Half,
                        TimeVideo = time,
                        user_id = HockeyIce.User.Id,
                        Win = win
                    };


                    if (MarkersWomboCombo.CheckPrevMarkerNeedsExtraMarker(Game.editMarker.G, newMarker))
                    {
                        MarkersWomboCombo.AddSingleNewExtraMarker(Game.editMarker.G, newMarker);
                    }
                    else
                    {
                        Game.editMarker.G = newMarker;
                    }

                    Game.IsCanCreateMarker(Half, time, Game.editMarker.G);

                    if (HockeyIce.Role == HockeyIce.RoleEnum.Online)
                    {
                        fixedTime = Second;
                        if (action_id == 1 && action_type == 1)
                            sw.Start();
                    }

                    ProcessingMarker(Game.editMarker.G);
                }
            }
            catch (Game.GameBase.InsertMarkerException imex)
            {
                lock (Game.editMarker)
                    Game.editMarker.G = null;

                MessageBox.Show(imex.Message, "Ошибка вставки маркера", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                lock (Game.editMarker)
                    Game.editMarker.G = null;
            }
            finally
            {
                ShowTimer();
                UpdateUI();
            }
        }

        //Маркер
        private void button1_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var tagid = 0;
            if (button.Tag is String && Int32.TryParse(button.Tag.ToString(), out tagid))
            {
#if DEBUG
                if (tagid == 200400 || tagid == 100601 || tagid == 800100 || tagid == 300600 || tagid == 101000 || tagid == 101100)
                {
                    var p = 5;
                }

#endif

                RegisterBegin(tagid);
            }
        }

        private void RegisterAddons(int tagid)
        {
            lock (Game.editMarker)
            {
                if (Game.editMarker.G == null)
                    return;

                switch (tagid)
                {
                    case 000001:
                        if (Game.editMarker.G.Compare(1, new int[] { 3, 4, 5, 7, 8, 9 }))
                            Game.editMarker.G.Win = Game.editMarker.G.Win == 2 ? 0 : 2;

                        if (Game.editMarker.G.Compare(1, 6) && Game.editMarker.G.Win == 2)
                        {
                            

                            if (Game.editMarker.G != null)
                            {
                                if (Game.editMarker.G.flag_adding.Exists(x => x.Compare(0, 0) && x.Win == 1))
                                {
                                    var flag000001 = Game.editMarker.G.flag_adding.FirstOrDefault(x => x.Compare(0, 0) && x.Win == 1);

                                    Game.editMarker.G.flag_adding.Remove(flag000001);
                                }
                                else
                                {
                                    Game.editMarker.G.flag_adding.Add(new Game.Marker(this.Game, 0, 0, Half, Game.editMarker.G.TimeVideo)
                                    {
                                        Win = 1,
                                    });
                                }
                            }
                            
                        }
                        
                        break;
                    case 200900:
                        Game.editMarker.G.flag_hitch = !Game.editMarker.G.flag_hitch;
                        break;

                    case 300300:
                        Game.editMarker.G.flag_icing = !Game.editMarker.G.flag_icing;
                        break;
                }
            }

            UpdateUI();
        }

        //Доп маркер
        private void button1a_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var tagid = 0;
            lock (Game.editMarker)
            {
                if (Game.editMarker.G != null && button.Tag is String && Int32.TryParse(button.Tag.ToString(), out tagid))
                {
#if DEBUG

                    if (button.Tag.ToString() == "300300")
                    {
                        var p = 5;
                    }

#endif

                    if (Game.editMarker.G.ActionId > 0)
                    {
                        RegisterAddons(tagid);
                    }
                    else
                    {
                        if (tagid == 200900)
                            RegisterBegin(tagid);
                    }
                }
            }
        }

        private void hockeyField1_Paint(object sender, PaintEventArgs e)
        {
            var gdi = hockeyField1.gdi;

            if (Game == null)
                return;
        }



        private void hockeyField1_RestorePlayer(object sender, HockeyGui.RestorePlayerEventArgs e)
        {
            //RestorePlayer(e.Player);
        }


        private void RestorePlayer(Player player, List<Marker> mklist)
        {
            var video_second = Second;
            var mkf = mklist.First();
            var ft = mklist.Sum(o => Game.GetFineTime(o));

            var mkf_act_time = mkf.TimeActualTotal;
            var mkct = Game.GetLastControlTimeMarker(Half, Second);
            var t1 = 0;

            if (mkct != null && mkct.Compare(8, 1))
            {
                video_second = mkct.TimeVideo;
                t1 = mkct.TimeActualTotal;
            }
            else
            {
                lock (Game.Markers)
                {
                    var mks = Game.Markers.Where(o => o.TimeActualTotal >= mkf.TimeActualTotal)
                        .OrderBy(o => o.TimeActualTotal)
                        .FirstOrDefault(o => Game.IsStopTimeMarker(o));

                    if (mks != null)
                    {
                        mkf_act_time = mks.TimeActualTotal;
                    }
                }
                                
                do
                {
                    t1 = Game.GetActuialTime(Game.Half, video_second, true);
                    if (video_second < 0)
                        return;
                    video_second -= 200;
                }
                while (t1 >= mkf_act_time + ft);
                video_second += 200;
            }

            lock (Game.Markers)
                if (Game.Markers
                        .Exists(o
                        => (o.TimeActualTotal < t1 + 1000 && o.TimeActualTotal > t1 - 200 && !o.FlagDel)
                        && (o.Compare(5, 1))
                        && (o.Player1 != null && o.Player1.Id == player.Id)))
                return;

            try
                {
                    var mkc = new Game.Marker(Game, 5, 1) { Player1 = player, Half = Half, TimeVideo = video_second };
                    Game.Insert(mkc);

                    if (Enumerable.Any<Place>(player.Team.Tactics[0].Places, o => (o.Amplua.Id == 1 && player.IsGk) || (o.Amplua.Id > 1 && !player.IsGk) && o.Player == null))
                    {
                        ShowStatus("", 0);
                        
                        var mkm = mklist[0];

                        var place = Enumerable.First<Place>(player.Team.Tactics[0].Places, o => (o.Amplua.Id == 1 && player.IsGk) || (o.Amplua.Id > 1 && !player.IsGk) && o.Player == null);
                        var mkc1 = new Game.Marker(Game, 14, place.GetCode()) { Player1 = player, Player2 = null, Half = Half, TimeVideo = video_second };
                        Log.Write(String.Format((string) "RESTORE PLAYER {0}", (object) player));
                        Game.Insert(mkc1);
                    }
                    else
                    {
                        ShowStatus("Для вывода игрока " + player.ToString() + " после штрафа нет свободных ячеек", 1);
                        return;
                    }
                }
                catch 
                { }

            lock (Game.Markers)
            {
                Game.RecalcActualTime(Game.Markers, Half);                
            }

            Game.SaveLocal();

            HockeyGui.InvalidateRect();
            ReloadDataGridView();
        }

        private void UpdateTactics()
        {
            if (lockUpdateTactics)
                return;

            //Оштрафованные игроки
            /*lock (HockeyGui.RestorePlayers)
            lock (Game.FinePlayers)
            lock (Game.FinePlaces)
            {
                HockeyGui.RestorePlayers.Clear();
                Game.FinePlayers.Clear();
                Game.FinePlaces.Clear();

                if (Half != null && Half.Index >= Game.HalfList[3].Index && Half.Index < Game.HalfList[4].Index && Half.MaxPlayersNum == 5)
                {
                    Game.FinePlaces.Add(Game.Match.Team1.Tactics[0].GetPlace(Game, 23));
                    Game.FinePlaces.Add(Game.Match.Team2.Tactics[0].GetPlace(Game, 23));
                }

                List<Marker> fine = null;
                List<Marker> fine_lost = null;
                int curr_time = 0;
                int half_time = 0;

                lock (Game.Markers)
                {
                    fine = Game.Markers
                        .Where(o
                        => (o.Half.Index < Half.Index || (o.Half.Index == Half.Index && o.TimeVideo <= Second) && !o.FlagDel)
                        && (o.ActionId == 5 && o.ActionType > 1))
                        .OrderBy(o => o.Half.Index)
                        .ThenBy(o => o.TimeVideo)
                        .ThenBy(o => o.Id)
                        .ToList<Uniso.InStat.Marker>();

                    fine_lost = Game.Markers
                        .Where(o
                        => (o.Compare(5, 1) && !o.FlagDel))
                        .OrderBy(o => o.Half.Index)
                        .ThenBy(o => o.TimeVideo)
                        .ThenBy(o => o.Id)
                        .ToList<Uniso.InStat.Marker>();
                }

                curr_time = Game.GetActuialTime(Half, Second);
                half_time = curr_time;

                foreach (Half h in Game.HalfList)
                {
                    if (h == Half)
                        break;

                    curr_time += Convert.ToInt32(h.Length);
                }

                Dictionary<Team, List<Diap>> diap = new Dictionary<Team, List<Diap>>();
                diap.Add(Game.Match.Team1, new List<Diap>());
                diap.Add(Game.Match.Team2, new List<Diap>());

                foreach (Marker mki in fine.Where(o => o.Compare(5, new int[] { 2, 3, 4, 5 }) && o.Team1 != null))
                {
                    int mk_time = mki.TimeActual;

                    foreach (Half h in Game.HalfList)
                    {
                        if (h.Index == mki.Half.Index)
                            break;

                        mk_time += Convert.ToInt32(h.Length);
                    }

                    Diap d = new Diap { Mk = mki, Time1 = mk_time, Time2 = mk_time + Game.GetFineTime(mki) };

                    if (diap[mki.Team1].Count(o => mk_time >= o.Time1 && mk_time <= o.Time2) >= 2)
                        d.NoBlockedPlace = true;

                    diap[mki.Team1].Add(d);
                }

                for (int i = fine.Count - 1; i >= 0; i--)
                {
                    Marker mk = fine[i];
                    int mk_time = mk.TimeActual;

                    foreach (Half h in Game.HalfList)
                    {
                        if (h.Index == mk.Half.Index)
                            break;

                        mk_time += Convert.ToInt32(h.Length);
                    }

                    int dt = curr_time - mk_time;

                    if (mk.Compare(5, new int[] { 2, 3, 4, 5 }))
                    { 
                        
                    }

                    if ((mk.Compare(5, new int[] { 2, 3 }) && dt <= 2 * 60000)
                        || (mk.Compare(5, 4) && dt <= 4 * 60000)
                        || (mk.Compare(5, 5) && dt <= 5 * 60000)
                        || (mk.Compare(5, 6) && dt <= 10 * 60000)
                        || (mk.Compare(5, new int[] { 7, 8 })))
                    {
                        if (!Game.FinePlayers.Contains(mk) && mk.Player1 != null)
                            Game.FinePlayers.Add(mk);
                    }

                    if ((mk.Compare(5, new int[] { 2, 3 }) && dt <= 2 * 60000)
                        || (mk.Compare(5, 4) && dt <= 4 * 60000)
                        || (mk.Compare(5, 5) && dt <= 5 * 60000))
                    {
                        Dictionary<int, HockeyIce.TacticsTime> ami =
                            Game.GetTacticsTime((Uniso.InStat.Game.Team)mk.Team1, mk.Half, mk.TimeVideo - 1000);

                        if (ami.ContainsKey(0))
                        {
                            HockeyIce.TacticsTime tt = ami[0];
                            if (tt.Exists(o => o.Player1 != null && o.Player1.Id == mk.Player1.Id))
                            {
                                Game.Marker mkt = tt.First(o => o.Player1 != null && o.Player1.Id == mk.Player1.Id);
                                Place place = mkt.Team1.Tactics[0].GetPlace(Game, mkt.ActionType);

                                if (place != null && !Game.FinePlaces.Contains(place) && diap[mk.Team1].Exists(o => o.Mk == mk && !o.NoBlockedPlace))
                                    Game.FinePlaces.Add(place);
                            }
                        }
                    }
                }

                //Восстановление
                for (int i = fine.Count - 1; i >= 0; i--)
                {
                    Marker mk = fine[i];
                    int mk_time = mk.TimeActual;

                    foreach (Half h in Game.HalfList)
                    {
                        if (h.Index == mk.Half.Index)
                            break;

                        mk_time += Convert.ToInt32(h.Length);
                    }

                    int dt = curr_time - mk_time;
                    
                    if ((mk.Compare(5, new int[] { 2, 3 }) && dt > 2 * 60000 && dt < (2 * 60000) + 5000)
                        || (mk.Compare(5, 4) && dt > 4 * 60000 && dt < (4 * 60000) + 5000)
                        || (mk.Compare(5, 5) && dt > 5 * 60000 && dt < (5 * 60000) + 5000))
                    {
                        int fine_time = Game.GetFineTime(mk);
                        fine_time += mk.TimeActual;

                        if (!Game.FinePlayers.Exists(o => o.Player1 == mk.Player1) && mk.Player1 != null
                            && !fine_lost.Exists(
                                o => o.Player1 != null 
                                && o.Player1.Id == mk.Player1.Id 
                                && o.Half.Index == mk.Half.Index 
                                && o.TimeActual > fine_time - 2000 
                                && o.TimeActual < fine_time + 10000))
                        {
                            RestorePlayer(mk.Player1);
                        }
                    }
                }
            }*/

            
            UpdateTactics(Game.Match.Team1);
            UpdateTactics(Game.Match.Team2);
            HockeyGui.InvalidateRect();
        }

        private void UpdateTactics(Team team)
        {
            List<Uniso.InStat.Marker> finePlayers;
            List<Place> finePlaces;
            team.Tactics[0] = Game.GetTactics((Uniso.InStat.Game.Team)team, Half, Second, out finePlayers, out finePlaces);

            if (Game.Half.Index == 255)
                return;

            lock (team.FinePlayers)
            {
                team.FinePlayers.Clear();
                team.FinePlayers.AddRange(finePlayers);
            }

            lock (team.FinePlaces)
            {
                team.FinePlaces.Clear();
                team.FinePlaces.AddRange(finePlaces);
            }

            var actual_time_total = Game.GetActuialTime(Half, Second, true);

            //Восстановление
            List<Uniso.InStat.Marker> mlist = null;

            lock (Game.Markers)
                mlist = Game.Markers
                .Where(o
                    => (o.Team1 == team)
                    && (o.Half.Index < Half.Index || (o.Half.Index == Half.Index && o.TimeVideo <= Second))
                    && (o.Compare(5, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }))
                    && (!o.FlagDel))
                .OrderByDescending(o => o.TimeActualTotal)
                .ToList<Uniso.InStat.Marker>();

            Marker first_small_fine = null;

            var ami = new List<Player>();
            var res = Game.GetFineList(team, Half, Second);
            var time = Int32.MaxValue;
            foreach (var mkl in res.Values)
            { 
                foreach (var mki in mkl.Where(o => o.Compare(5, new int[] { 2, 3, 4, 5 })))
                {
                    var sibl2 = Game.GetSiblings(mki);
                    if (mki.TimeActualTotal < time && !sibl2.Exists(o => o.ActionId == 14 && o.Player2 != mki.Player1))
                    {
                        if (mkl.Exists(o => o.Compare(5, 6) && !o.FlagDel))
                            continue;

                        first_small_fine = mki;
                        time = mki.TimeActualTotal;
                    }
                }
            }

            /*List<Marker> sibl2 = Game.GetSiblings(mk);
                            if (!sibl2.Exists(o => o.ActionId == 14 && o.Player2 != mk.Player1))
                                first_small_fine = mk;*/
            var mkct = Game.GetLastControlTimeMarker(Half, Second);

            foreach (var player in res.Keys)
            {
                var mklist = res[player];
                if (mklist.Count == 0)
                    continue;

                if (mklist.Exists(o => o.Compare(5, 6) && !o.FlagDel))
                    continue;

                var mkf = mklist.First();

                /*List<InStat.Marker> sibl = Game.GetSiblings(mkf.Half, mkf.TimeVideo);
                if (!sibl.Any(o => o.ActionId == 14 && o.player2_id == mkf.player1_id))
                    continue;*/

                var ft = mklist.Sum(o => Game.GetFineTime(o));
                var mkf_act_time = mkf.TimeActualTotal;

                lock (Game.Markers)
                {
                    var mks = Game.Markers.Where(o => o.TimeActualTotal >= mkf.TimeActualTotal && !o.FlagDel)
                        .OrderBy(o => o.TimeActualTotal)
                        .FirstOrDefault(o => Game.IsStopTimeMarker(o));

                    if (mks != null)
                    {
                        mkf_act_time = mks.TimeActualTotal;
                    }
                }

                var fine_end = mkf_act_time + ft;

                if (mkct != null && mkct.Compare(8, 1)
                    && Game.GetSiblings(mkct).Any(o => o.Compare(5, 1)
                    && o.team1_id == mklist[0].team1_id))
                {
                    first_small_fine = null;
                }

                //Выход, если был гол
                var goal = first_small_fine != null && first_small_fine == mklist[0] && mkct != null && mkct.Compare(8, 1)
                    && mkct.Team2 == team && mklist.Count == 1 && mklist[0].Compare(5, 2);

                //Проверка на обоюдный фол
                var sibl2 = Game.GetSiblings(mklist[0]);
                if (goal)
                {
                    if (sibl2.Exists(o => o.ActionId == 14 && o.Player2 != mklist[0].Player1))
                        goal = false;
                }

                if ((actual_time_total > fine_end && actual_time_total < fine_end + 10000) || goal)
                {
                    lock (Game.Markers)
                        if (Game.Markers
                            .Exists(o
                            => (o.TimeActualTotal < fine_end + 1000 && o.TimeActualTotal > fine_end - 200 && !o.FlagDel)
                            && (o.Compare(5, 1))
                            && (o.Player1 != null && o.Player1.Id == player.Id)))
                            continue;

                    if (sibl2.Any(o => o.ActionId == 14 && o.player2_id == mklist[0].player1_id && o.user_id == HockeyIce.User.Id))
                    {
                        var mksub = sibl2.First(o => o.ActionId == 14 && o.player2_id == mklist[0].player1_id);
                        RestorePlayer(player, mklist);
                    }
                }
            }
        }

        private int changedPlayerTime = 0;

        private void hockeyField1_ChangedPlayers(object sender, EventArgs e)
        {
            if (HockeyGui.ChangedPlayersList.Count > 0 && changedPlayerTime == 0)
            {
                changedPlayerTime = Second;
            }

            if (HockeyGui.ChangedPlayersList.Count == 0)
            {
                changedPlayerTime = 0;
            }

            if (HockeyGui.ChangedPlayersList.Count > 0 && !HockeyGui.ChangedPlayersList.Exists(o => o.Place1 == null || o.Place2 == null))
            {
                try
                {
                    lockUpdateTactics = true;

                    var tm = HockeyGui.ChangedPlayersList[0].Place1.Player.Team;
                    var ami = new List<int>();

                    for (var i = 0; i < tm.Tactics.Count; i++)
                    {
                        foreach (var cpp in HockeyGui.ChangedPlayersList)
                        {
                            if (tm.Tactics[i].Places.Contains(cpp.Place1) || tm.Tactics[i].Places.Contains(cpp.Place2))
                            {
                                ami.Add(i);
                                break;
                            }
                        }
                    }

                    Game.Marker mk = null;
                    foreach (var cpp in HockeyGui.ChangedPlayersList)
                    {
                        if (cpp.Place1.Player == cpp.Place2.Player || cpp.Tactics1 == null || cpp.Tactics2 == null)
                            continue;

                        if (cpp.Tactics1.NameActionType != 0 && cpp.Tactics2.NameActionType != 0)
                        {
                            if (cpp.Place1.Amplua.Id == 1 || cpp.Place2.Amplua.Id == 1)
                                continue;

                            var _p = cpp.Place1.Player;
                            cpp.Place1.Player = cpp.Place2.Player;
                            cpp.Place2.Player = _p;
                        }
                        else
                        {
                            lock (Game.Markers)
                            {
                                try
                                {
                                    IEnumerable<Marker> amich = Game.Markers.Where(o =>
                                            !o.FlagDel
                                            && o.Half.Index == Half.Index && o.TimeVideo > Second
                                            && o.ActionId == 14 && o.Player1 != null).OrderBy(o => o.TimeVideo);

                                    var player1 = false;
                                    var player2 = false;

                                    foreach (var mki in amich)
                                    {
                                        if (mki.Player2 != null && mki.Player2.Id == cpp.Place1.Player.Id)
                                            player1 = true;

                                        if (cpp.Place2.Player != null && mki.Player1 != null && mki.Player1.Id == cpp.Place2.Player.Id)
                                            player2 = true;

                                        /*if (!player1 && mki.Player1 != null && mki.Player1.Id == cpp.Place1.Player.Id)
                                        {
                                            MessageBox.Show(String.Format("Невозможно вставить замену {0} <- {1},\nт.к. в маркере ID={2} {3} {0} выходит снова на замену", cpp.Place1.Player, cpp.Place2.Player, mki.Id, Utils.TimeFormat(mki.TimeVideo)),
                                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        if (!player2 && cpp.Place2.Player != null && mki.Player2 != null && mki.Player2.Id == cpp.Place2.Player.Id)
                                        {
                                            MessageBox.Show(String.Format("Невозможно вставить замену {0} <- {1},\nт.к. в маркере ID={2} {3} {1} был заменен", cpp.Place1.Player, cpp.Place2.Player, mki.Id, Utils.TimeFormat(mki.TimeVideo)),
                                                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }*/
                                    }
                                }
                                catch
                                { }
                            }


                            if (cpp.Place1.Player != null && cpp.Place2.Player != null && cpp.Place1.Player.Team.Id != cpp.Place2.Player.Team.Id)
                            {
                                Log.Write(String.Format("CHANGE PLAYERS ERROR {2} {0} <- {1}", cpp.Place1.Player, cpp.Place2.Player, cpp.Place2.ToString()));
                                continue;
                            }

                            mk = new Game.Marker(Game, 14, cpp.Place2.GetCode()) { Player1 = cpp.Place1.Player, Player2 = cpp.Place2.Player, Half = Half, TimeVideo = Second };
                            Log.Write(String.Format("CHANGE PLAYERS {2} {0} <- {1}", cpp.Place1.Player, cpp.Place2.Player, cpp.Place2.ToString()));
                            Game.Insert(mk);
                        }
                    }

                    Game.SaveLocal();

                    HockeyGui.ChangedPlayersList.Clear();
                    changedPlayerTime = 0;
                    ReloadDataGridView();

                    SetEditMarker(null, StageEnum.CreateMarker);
                    //!!!hockeyField1.Visible = false;
                }
                finally
                {
                    lockUpdateTactics = false;

                    HockeyGui.InvalidateRect();
                    UpdateUI();
                }
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedMarkers();
            }
        }

        private void DeleteSelectedMarkers()
        {
            var lst = new List<ListViewItem>();
            foreach (ListViewItem item in dataGridView1.SelectedItems)
            {
                lst.Add(item);
            }

            if (lst.Count > 0 &&
                MessageBox.Show("Удалить маркеры (" + lst.Count + "шт.)?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Log.Write("DELETE MANUAL");

                var lstDel = new List<Marker>();
                dataGridView1.BeginUpdate();
                try
                {
                    lock (Game.Markers)
                    {
                        var add_tacktics = 0;
                        foreach (var row in lst)
                        {
                            if (row.Tag is Marker)
                            {
                                var mk = (Marker)row.Tag;
                                if (mk.Compare(14, 1))
                                {
                                    lstDel.AddRange(
                                        Game.Markers.Where(o =>
                                            (o.ActionId == 15 || o.ActionId == 16)
                                            && o.team1_id == mk.team1_id
                                            && o.Half == mk.Half
                                            && o.TimeVideo == mk.TimeVideo
                                            && !o.FlagDel).ToArray<Marker>());
                                }

                                if (lstDel.IndexOf(mk) < 0)
                                    lstDel.Add(mk);
                            }
                        }

                        if (add_tacktics > 0 &&
                            MessageBox.Show("Дополнительно будут удалены еще " + add_tacktics + " маркеров тактики. Продолжить?",
                            "Confirmation",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            return;


                        var mkd = lstDel.Count > 0 ? lstDel[0] : null;
                        foreach (var mk in lstDel)
                        {
                            Log.Write("DELETE MANUAL " + mk);

                            if (!mk.FlagDel)
                            {
                                if (mk.FlagSaved)
                                    mk.FlagDel = true;
                                else
                                    Game.Markers.Remove(mk);
                            }
                            else
                            {
                                mk.FlagDel = false;
                            }
                        }

                        ReloadDataGridView();
                        dataGridView1.SelectedItems.Clear();

                        Game.SaveLocal();
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex);
                }
                finally
                {
                    dataGridView1.EndUpdate();
                    UpdateUI();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (vlcStreamPlayer1 != null)
                    vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Stop;
            }
            catch
            { }

            HockeyGui.Mode = HockeyGui.ModeEnum.EditTactics;

            foreach (var place in Game.Match.Team1.Tactics[0].Places)
                place.Player = null;
            foreach (var place in Game.Match.Team2.Tactics[0].Places)
                place.Player = null;

            timer2.Stop();
            timer1.Stop();
            if (vlcStreamPlayer1 != null)
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Stop;

            CaptureStream.Stop();
        }

        
        /// <summary>
        /// Отобразить контрол HockeyField для выбора в зависимости от режима.
        /// </summary>
        private void RefreshHockeyField()
        { 
            if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
            {
                hockeyField1.Visible = true;
                hockeyField1.Left = 5;
                hockeyField1.Top = 60;

                if (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPoint || HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest)
                {
                    hockeyField1.Width = 504;
                    hockeyField1.Height = 247;
                }
                else
                {
                    hockeyField1.Width = 220;
                    hockeyField1.Height = 154;
                }
            }
            else
            {
                if (Half != null && Half.Index == 255)
                {
                    hockeyField1.Visible = false;
                }
                else
                {
                    hockeyField1.Top = 90;
                    hockeyField1.Left = 115;
                    hockeyField1.Width = 497;
                    hockeyField1.Height = 301;

                    lock (Game.editMarker)
                    {
                        hockeyField1.Visible =
                           (Game.editMarker.G != null && HockeyGui.Mode == HockeyGui.ModeEnum.SelectPlayer && (Game.editMarker.G.Compare(1, 1) || Game.editMarker.G.Compare(6, 1)) && HockeyIce.Role == HockeyIce.RoleEnum.Substitutions)
                        || (HockeyGui.Mode == HockeyGui.ModeEnum.SelectPlayer && HockeyIce.Role != HockeyIce.RoleEnum.Substitutions)
                        || HockeyGui.Mode == HockeyGui.ModeEnum.SelectPoint
                        || HockeyGui.Mode == HockeyGui.ModeEnum.SelectPointAndDest;
                    }
                }
            }
        }

        private void SetEditMarker(Game.Marker mk, StageEnum stage)
        {
            RefreshHockeyField();

            lock (Game.editMarker)
            {
                Game.editMarker.G = mk;
                HockeyGui.Marker = mk;

                if (Game.editMarker.G != null)
                {
                    if (Game.editMarker.G.Compare(0, 0))
                    {
                        label2.Text = "Выбор действия";
                    }
                    else
                    {
                        var stageName = mk.GetNameStage(stage);
                        var action = mk.Action;
                        label2.Text = String.Format("{2} {0}: {1}",
                            Convert.ToString(HockeyIce.convAction.ConvertTo(action, typeof(string))),
                            stageName,
                            Game.TimeToString(mk.TimeVideo));

                        //label2.Text = string.Format("{0} {1}: {2}",
                        //    Game.TimeToString(mk.TimeVideo),
                        //    Convert.ToString(HockeyIce.convAction.ConvertTo(action, typeof(string))),
                        //    stageName);
                    }
                    label2.Visible = true;
                }
                else
                {
                    fixedTime = 0;
                    label2.Text = String.Empty;
                    label2.Visible = false;
                }
            }
        }

        private void hockeyField1_SelectedManyPlayer(object sender, HockeyGui.SelectedManyPlayerEventArgs e)
        {
            lock (Game.editMarker)
            {
                if (Game.editMarker.G != null && Game.editMarker.G.Compare(12, 0))
                {
                    if (Game.editMarker.G.Team1 == null || Game.editMarker.G.Team2 == null)
                    {
                        MessageBox.Show("Не выбрано ни одного игрока", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var count1 = e.Players.Count(o => o.Team == Game.editMarker.G.Team1);
                    var count2 = e.Players.Count(o => o.Team == Game.editMarker.G.Team2);

                    if (count1 < 1)
                    {
                        MessageBox.Show(String.Format("В команде атакующих {0} не выбрано ни одного игрока", Game.editMarker.G.Team1), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (count1 > 3 || count2 > 3)
                    {
                        MessageBox.Show("Выбрано больше игроков, чем максимально возможное число", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Game.editMarker.G.ActionType = count1 * 10 + count2;
                    Game.editMarker.G.ExtraOptionsExists = true;
                }

                if (Game.editMarker.G != null && Game.editMarker.G.Compare(8, 1))
                {
                    foreach (var p in e.Players)
                    {
                        var mk = new Game.Marker(Game, 1, 2) { Half = Game.editMarker.G.Half, TimeVideo = Game.editMarker.G.TimeVideo, Player1 = p };
                        Game.Insert(mk);
                    }

                    Game.editMarker.G.ExtraOptionsExists = true;
                }

                if (Game.editMarker.G != null && Game.editMarker.G.Compare(6, 2))
                {
                    foreach (var p in e.Players)
                    {
                        var mk = new Game.Marker(Game, 6, 2)
                        {
                            Half = Game.editMarker.G.Half,
                            TimeVideo = Game.editMarker.G.TimeVideo,
                            Player1 = p
                        };
                        Game.Insert(mk);
                    }

                    Game.editMarker.G.ExtraOptionsExists = true;
                }

                if (Game.editMarker.G != null && Game.editMarker.G.Compare(2, 11))
                {
                    Team team = null;
                    foreach (var p in e.Players)
                    {
                        if (team == null)
                            team = p.Team;

                        var mk = new Game.Marker(Game, 2, 11)
                        {
                            Half = Game.editMarker.G.Half,
                            TimeVideo = Game.editMarker.G.TimeVideo,
                            Player1 = p,
                            Point1 = Game.editMarker.G.Point1,
                            Win = team == p.Team ? 1 : 2,
                        };
                        Game.Insert(mk);
                    }

                    Game.editMarker.G.ExtraOptionsExists = true;
                }

                ProcessingMarker(Game.editMarker.G);
            }
        }

        private void hockeyField1_SelectedPlayer(object sender, HockeyGui.SelectedPlayerEventArgs e)
        {
            lock (Game.editMarker)
            {
                if (Game.editMarker.G != null)
                {
                    if (Game.editMarker.G.Player1 == e.Player || Game.editMarker.G.Player2 == e.Player)
                        return;

                    if ((Game.editMarker.G.ActionId == 4 || Game.editMarker.G.ActionId == 6 || Game.editMarker.G.ActionId == 8)
                        && Game.editMarker.G.Player1 != null && Game.editMarker.G.Team1 == e.Player.Team)
                        return;

                    if ((Game.editMarker.G.Compare(1, 1) || Game.editMarker.G.Compare(3, new int[] { 1, 2 }))
                        && Game.editMarker.G.Player1 != null && Game.editMarker.G.Team1 == e.Player.Team)
                        return;

                    if (e.Player.Team.FinePlayers.Exists(o => o.Id == e.Player.Id && ((bool)o.Tag)))
                        return;

                    //Штраф
                    if (Game.editMarker.G.ActionId == 5)
                    {
                        if (Game.editMarker.G.Player1 != null && Game.editMarker.G.Player1.Team != e.Player.Team)
                            return;

                        if (Game.editMarker.G.Compare(5, new int[] { 2, 3, 4, 6 }) && e.Player.IsGk)
                            return;
                    }

                    if (Game.editMarker.G.Player1 != null)
                        Game.editMarker.G.Player2 = e.Player;
                    else
                        Game.editMarker.G.Player1 = e.Player;

                    ProcessingMarker(Game.editMarker.G);
                }
            }
        }


        private void hockeyField1_SelectedPoint(object sender, HockeyGui.SelectedPointEventArgs e)
        {
            lock (Game.editMarker)
            {
                // Тута нулл
                if (Game.editMarker.G != null)
                {
                    Game.editMarker.G.Point1 = e.Point;

                    hockeyField1.SetLastClickPointF(e.Point);

                    ProcessingMarker(Game.editMarker.G);
                }
            }
        }

        private void hockeyField1_SelectedPointAndDest(object sender, HockeyGui.SelectedPointAndDestEventArgs e)
        {
            lock (Game.editMarker)
            {
                if (Game.editMarker.G != null)
                {
                    Game.editMarker.G.Point1 = e.Point1;
                    Game.editMarker.G.Point2 = e.Point2;

                    ProcessingMarker(Game.editMarker.G);
                }
            }
        }

        private void tabControl1_SizeChanged(object sender, EventArgs e)
        {
            
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            ShowAddPanel(false);

            var mif = new MediaInputForm(Game.Match.Id, Half.Index);
            if (mif.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Stop;
                
                Marker mk11 = null;
                lock (Game.Markers)
                    mk11 = Game.Markers.Where(o => !o.FlagDel && o.Half.Index == Half.Index && o.Compare(1, 1))
                        .OrderBy(o => o.TimeVideo)
                        .FirstOrDefault();              

                switch (mif.SourceType)
                {
                    case 0:
                        vlcStreamPlayer1.OpenUrl(mif.VideoName);
                        vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
                        break;

                    case 1:
                        vlcStreamPlayer1.OpenUrl(mif.VideoName);
                        vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
                        break;
                }
            }

            UpdateUI();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            vlcStreamPlayer1.Volume = trackBar2.Value;
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

        private bool saving = false;
        private void button19_Click(object sender, EventArgs e)
        {
            ShowAddPanel(false);
            SaveAsync();
        }

        private void SaveAsync()
        {
            Log.Write("Save QUERY=" + saving);

            if (saving)
                return;

            saving = true;                    
            var changed = false;

            Task.Factory.StartNew(() =>
            {
                sync.Execute(() => timer2.Enabled = false);

                var nums = new Random(1000).Next();

                Log.Write("Save BEGIN=" + nums);
                                
                var rems = new List<Game.Marker>();

                var copy = new List<Marker>();
                Marker edit = Game.editMarker.G;

                try
                {
                    UpdateUI();

                    lock (Game.Markers)
                    {
                        foreach (var mki in Game.Markers)
                            copy.Add(mki);
                    }

                    if (edit != null && HockeyIce.Role != HockeyIce.RoleEnum.Online)
                        copy.Remove(edit);                    

                    var random = new Random(DateTime.Now.Millisecond);

                    //линковка
                    /*if (HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                    {
                        ShowStatus("Линковка...", 0);
                        List<Marker> used = new List<Marker>();
                        foreach (Marker mk in copy.Where(o => !o.FlagDel).OrderBy(o => o.Half.Index).ThenBy(o => o.TimeVideo))
                        {
                            if (used.IndexOf(mk) >= 0)
                                continue;

                            List<Marker> sibl = Game.GetSiblings(mk);
                            used.AddRange(sibl);

                            if (sibl.Count == 1)
                            {
                                if (mk.Link != 0)
                                {
                                    mk.Link = 0;
                                    mk.FlagUpdate = true;
                                }
                            }
                            else
                            {
                                int max = sibl.Max(o => o.Link);
                                if ((max == 0) || (max != sibl.Min(o => o.Link)))
                                {
                                    int link = random.Next(Int32.MaxValue);
                                    foreach (Marker mki in sibl)
                                    {
                                        mki.Link = link;
                                        mki.FlagUpdate = true;
                                    }
                                }
                            }
                        }
                    }*/

                    //Проверка маркеров
                    if (HockeyIce.Role != HockeyIce.RoleEnum.Online)
                    {
                        ShowStatus("Проверка маркеров...", 0);
                        var msg_err = String.Empty;
                        var ami2 = copy.Where(o => !o.FlagDel).ToList<Marker>();
                        for (var i = 0; i < ami2.Count; i++)
                        {
                            var mk = (Game.Marker)ami2[i];
                            try
                            {
                                Game.CheckValid(mk);
                            }
                            catch (HockeyIce.CheckValidMarkerException ex)
                            {
                                if (ex.CheckValidLevel == HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.CRITICAL)
                                    throw new Exception(String.Format("Обнаружена критическая ошибка в маркере {0} {1} {2}. Сохранение невозможно.", mk.Id, mk.Half, Utils.TimeFormat(mk.TimeVideo)));
                            }
                        }
                    }

                    var dellocal = copy.Where(o => o.Id == 0 && o.FlagDel).ToList<Marker>();
                    lock (Game.Markers)
                    {
                        foreach (var mk in dellocal)
                            Game.Markers.Remove(mk);
                    }         

                    var ami = copy.Where(o => !o.FlagSaved || o.FlagUpdate || o.FlagDel).ToList<Marker>();
                                        
                    var count = (float)ami.Count(o => o != null);                    
                    
                    Exception exception = null;
                    var tasks = new List<Task>();

                    ShowStatus("Сохранение...", 0);
                    for (var i = 0; i < ami.Count; i++)
                    {
                        var mk = (Game.Marker)ami[i];

                        if (exception != null)
                            break;

                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                Web.SaveMarker(Game, mk);
                                mk.FlagGuiUpdate = true;

                                if (mk.FlagDel)
                                    rems.Add(mk);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteException(ex);
                                Log.Write("ERROR SAVE " + mk + " num=" + nums);
                                if (ex != null)
                                    exception = ex;
                            }
                        }));
                    }

                    if (ami.Count > 0)
                    {
                        var time_out = ami.Count * 1000 + 45000;
                        var sw = new Stopwatch();
                        sw.Start();

                        while (sw.ElapsedMilliseconds < time_out)
                        {
                            var f = (float)tasks.Count(o => o.IsCompleted) / count;
                            if (f > 0.0f)
                                changed = true;
                            
                            if (f == 1.0f)
                                break;

                            ShowStatus("Сохранение " + f.ToString("0%"), 0);
                            Thread.Sleep(100);
                        }
                    }

                    for (var i = 0; i < ami.Count; i++)
                    {
                        Log.Write("SAVED " + ami[i].ToString() + " num=" + nums);
                    }

                    if (exception != null)
                        throw exception;

                    ShowStatus("Сохранено", 0);
                }
                catch (Exception ex)
                {
                    ShowStatus(ex.Message, 1);
                    Log.WriteException(ex);
                }
                finally
                {
                    try
                    {
                        if (HockeyIce.Role != HockeyIce.RoleEnum.Online)
                        {
                            var mkget = Web.GetMarkers(Game, Game.Match.Id);
                            var players = new List<Player>();
                            players.AddRange(Game.Match.Team1.Players);
                            players.AddRange(Game.Match.Team2.Players);

                            var add = new List<Marker>();

                            foreach (var mk in mkget)
                            {
                                if (edit != null && edit.Id > 0 && edit.Id == mk.Id)
                                    continue;

                                //Новый маркер
                                if (!copy.Any(o => o.Id == mk.Id))
                                {
                                    if (mk.player1_id > 0 && players.Exists(o => o.Id == mk.player1_id))
                                    {
                                        mk.Player1 = players.First(o => o.Id == mk.player1_id);
                                    }
                                    if (mk.player2_id > 0 && players.Exists(o => o.Id == mk.player2_id))
                                    {
                                        mk.Player2 = players.First(o => o.Id == mk.player2_id);
                                    }

                                    Log.Write("SYNC FROM DB " + mk.ToString() + " num=" + nums);

                                    add.Add(mk);
                                    changed = true;
                                }
                                else
                                {
                                    //Имеющийся
                                    var mkloc = copy.First(o => o.Id == mk.Id);

                                    if (mkloc == edit || mkloc.FlagUpdate)
                                        continue;

                                    if ((mk.TimeVideo != mkloc.TimeVideo
                                        || mk.ActionId != mkloc.ActionId
                                        || mk.ActionType != mkloc.ActionType
                                        || mk.Num != mkloc.Num) && !mkloc.FlagUpdate)
                                    {
                                        Log.Write("UPDATE FROM DB " + mk.ToString() + " num=" + nums);

                                        mkloc.TimeVideo = mk.TimeVideo;
                                        mkloc.ActionId = mk.ActionId;
                                        mkloc.ActionType = mk.ActionType;
                                        mkloc.Num = mk.Num;
                                        mkloc.FlagGuiUpdate = true;
                                        changed = true;
                                    }

                                    mkloc.Sync = mk.Sync;

                                    if (mk.player1_id == 0)
                                    {
                                        if (mkloc.player1_id != 0)
                                        {
                                            Log.Write("UPDATE DEL PLAYER FROM DB " + mk.ToString() + " num=" + nums);

                                            mkloc.Player1 = null;
                                            mkloc.FlagGuiUpdate = true;
                                            changed = true;
                                        }
                                    }
                                    else
                                        if (mk.player1_id != mkloc.player1_id && players.Exists(o => o.Id == mk.player1_id))
                                        {
                                            Log.Write("UPDATE PLAYER FROM DB " + mk.ToString() + " num=" + nums);

                                            mkloc.Player1 = players.First(o => o.Id == mk.player1_id);
                                            mkloc.FlagGuiUpdate = true;
                                            changed = true;
                                        }

                                    if (mk.player2_id == 0)
                                    {
                                        if (mkloc.player2_id != 0)
                                        {
                                            Log.Write("UPDATE DEL PLAYER FROM DB " + mk.ToString() + " num=" + nums);

                                            mkloc.Player2 = null;
                                            mkloc.FlagGuiUpdate = true;
                                            changed = true;
                                        }
                                    }
                                    else
                                        if (mk.player2_id != mkloc.player2_id && players.Exists(o => o.Id == mk.player2_id))
                                        {
                                            Log.Write("UPDATE PLAYER FROM DB " + mk.ToString() + " num=" + nums);

                                            mkloc.Player2 = players.First(o => o.Id == mk.player2_id);
                                            mkloc.FlagGuiUpdate = true;
                                            changed = true;
                                        }

                                }
                            }

                            var sync_ch = Game.UpdateSync(Half);
                            Log.Write("sync_ch=" + sync_ch);

                            lock (Game.Markers)
                            {
                                if (add.Count > 0)
                                {
                                    Log.Write("ADD NEW FORM DB count=" + add.Count);
                                    Game.Markers.AddRange(add);
                                }

                                if (changed)
                                {
                                    Log.Write("=>RecalcActualTime");
                                    Game.RecalcActualTime(Game.Markers, Half);
                                }

                                var rem_mkget = new List<Marker>();
                                foreach (var mk in copy)
                                {
                                    if (!mkget.Exists(o => o.Id == mk.Id) && mk.Id > 0)
                                    {
                                        rem_mkget.Add(mk);
                                        changed = true;
                                    }
                                }

                                foreach (var mk in rem_mkget)
                                {
                                    Log.Write("REMOVE MARKER LOCAL " + mk.ToString() + " num=" + nums);
                                    Game.Markers.Remove(mk);
                                }

                                Log.Write("=>sync_ch => mk.FlagGuiUpdate" + " num=" + nums);

                                if (sync_ch)
                                {
                                    foreach (var mk in Game.Markers.Where(o => !o.FlagDel && o.Half.Index == Half.Index && o.Sync == 1))
                                            mk.FlagGuiUpdate = true;
                                }

                                foreach (var mk in rems)
                                {
                                    if (mk != null)
                                    {
                                        Log.Write("REMOVE FROM DB " + mk.ToString() + " num=" + nums);
                                        Game.Markers.Remove(mk);
                                    }
                                }
                            }

                            Log.Write("=>Game.SaveLocal" + " num=" + nums);
                            Game.SaveLocal();

                            Log.Write("=>ReloadDataGridView 1" + " num=" + nums);
                            if (changed || sync_ch)
                                ReloadDataGridView(rems.Count > 0 || sync_ch);

                            Log.Write("=>ReloadDataGridView 2" + " num=" + nums);
                        }
                        else
                        {
                            foreach (var mk in rems)
                            {
                                if (mk != null)
                                {
                                    Log.Write("REMOVE FROM DB " + mk.ToString() + " num=" + nums);
                                    lock (Game.Markers)
                                        Game.Markers.Remove(mk);
                                }
                            }

                            Game.SaveLocal();
                            ReloadDataGridView(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowStatus(ex.Message, 1);
                        Log.WriteException(ex);
                    }
                    finally
                    {
                        Log.Write("Save FIN=" + nums);
                        saving = false;
                        UpdateUI();
                        UpdateAutoSaveTimer();
                        Log.Write("Save END=" + nums);
                    }
                }
            });
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReloadDataGridView(true);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            //hide_subs_markers = checkBox5.Checked;
            ReloadDataGridView(true);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            only_current_time = checkBox4.Checked;
            ReloadDataGridView(true);
        }
        
        private Marker lastmk = null;

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Stop();

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    var list = new List<Marker>();
                    lock (Game.Markers)
                    {
                        list.AddRange(Game.Markers);
                    }

                    var update = false;
                    var ami = list.Where(o => !o.FlagDel && !o.Compare(16)).OrderBy(o => o.Half.Index).ThenBy(o => o.TimeVideo).ToList<Marker>();
                    var uplist = new List<Game.Marker>();
                    for (var i = 0; i < ami.Count; i++)
                    {
                        var mk = (Game.Marker)ami[i];

                        update = false;
                        try
                        {
                            Game.CheckValid(mk);
                            if (mk.Exception != null)
                            {
                                uplist.Add(mk);
                                mk.FlagGuiUpdate = true;
                                //update = true;
                            }

                            mk.Exception = null;

                            if (update)
                                UpdateGridView(mk);
                        }
                        catch (Uniso.InStat.Game.HockeyIce.CheckValidMarkerException ex)
                        {
                            if (mk.Exception == null || (mk.Exception != null && !mk.Exception.Message.Equals(ex.Message)))
                            {
                                uplist.Add(mk);
                                mk.FlagGuiUpdate = true;
                                //update = true;
                            }

                            mk.Exception = ex;

                            if (update)
                                UpdateGridView(mk);
                        }
                    }

                    if (uplist.Count == 1)
                        UpdateGridView(uplist[0]);

                    if (uplist.Count > 1)
                        ReloadDataGridView(true);

                    if (only_current_time && Game != null && Game.Half != null)
                    {
                        var prev = Game.GetPrevousMarkersHalf(Game.Half, Game.Time, true);
                        if (prev.Count > 0)
                        {
                            var lastmk_0 = prev.Last();
                            if (lastmk == null || lastmk_0.TimeVideo != lastmk.TimeVideo)
                            {
                                lastmk = lastmk_0;
                                ReloadDataGridView(true);
                            }
                        }
                        else
                            lastmk = null;
                    }
                }
                finally
                {
                    sync.Execute(() => timer3.Start());
                }
            });
        }

        private void EditPropertiesMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;
                var mke = new Game.Marker(Game);
                mke.Assign(mk);
                var mef = new MarkerEditForm();
                mef.Edit(mke);
                if (mef.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mk.Assign(mke);
                    mk.FlagUpdate = true;
                    UpdateGridView((Game.Marker)mk);

                    try
                    {
                        Game.SaveLocal();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private void CorrectMarkerChangedPlayers(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                && dataGridView1.SelectedItems[0].Tag is Game.Marker
                && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).ActionId == 14
                && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Player1 != null
                && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Player2 != null)
            {
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;
                Marker mki = new Game.Marker(Game);
                mki.Assign(mk);

                Marker mk1 = null;
                lock (Game.Markers)
                {
                    mk1 = Game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == mk.Half.Index && o.TimeVideo < mk.TimeVideo
                        && o.ActionId == 14 && o.player1_id == mk.player2_id)
                        .OrderByDescending(o => o.TimeVideo).FirstOrDefault();
                }

                Marker mk2 = null;
                lock (Game.Markers)
                {
                    mk2 = Game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == mk.Half.Index && o.TimeVideo > mk.TimeVideo
                        && o.ActionId == 14 && o.player2_id == mk.player1_id)
                        .OrderBy(o => o.TimeVideo).FirstOrDefault();
                }

                var form = new CorrectChangedPlayersMarkerForm(Game, mki, mk1, mk2);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Log.Write("MANUAL CORRECT BEFORE " + mk.ToString());

                    var time_up = mki.TimeVideo != mk.TimeVideo;

                    var team = mk.Player1 != null
                        ? mk.Team1 : mk.Team2; ;

                    if (mk1 != null)
                    {
                        if (mk1.Player1 != mki.Player2)
                        {
                            mk1.Player1 = mki.Player2;
                            mk1.FlagUpdate = true;
                            mk1.FlagGuiUpdate = true;
                            UpdateGridView((Game.Marker)mk1);
                        }
                    }

                    if (mk2 != null)
                    {
                        if (mk2.Player2 != mki.Player1)
                        {
                            mk2.Player2 = mki.Player1;
                            mk2.FlagUpdate = true;
                            mk2.FlagGuiUpdate = true;
                            UpdateGridView((Game.Marker)mk2);
                        }
                    }

                    mk.Assign(mki);
                    mk.FlagUpdate = true;
                    mk.FlagGuiUpdate = true;
                    UpdateGridView((Game.Marker)mk);

                    Log.Write("MANUAL CORRECT AFTER " + mk.ToString());

                    if (time_up)
                        ReloadDataGridView(true);

                    Game.SaveLocal();
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1 && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mk1 = (Game.Marker)dataGridView1.SelectedItems[0].Tag;

                if (mk1.Half.Index != Game.Half.Index)
                {
                    MessageBox.Show("Этот маркер не относится к текущему периоду", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (mk1.Compare(1, 1))
                {
                    Marker mk2 = null;
                    lock (Game.Markers)
                        mk2 = Game.Markers
                            .Where(o => !o.FlagDel && o.Half.Index == mk1.Half.Index && o.TimeActual > mk1.TimeActual && Game.IsStopTimeMarker(o))
                            .OrderBy(o => o.TimeActual)
                            .FirstOrDefault();

                    List<Marker> ami = null;
                    lock (Game.Markers)
                        ami = Game.Markers.Where(o => !o.FlagDel && o.TimeVideo == -1 && o.Half.Index == mk1.Half.Index && o.TimeActual >= mk1.TimeActual && (mk2 == null || (mk2 != null && o.TimeActual <= mk2.TimeActual)))
                            .OrderBy(o => o.TimeActual)
                            .ToList<Marker>();

                    var time0 = mk1.TimeActualSrv;

                    foreach (var mk in ami)
                    {
                        if (mk.Compare(1, 1) && mk != mk1)
                            continue;

                        if (mk.ActionId == 16)
                            continue;

                        mk.TimeVideo = Game.Time + (mk.TimeActual - time0);
                        mk.FlagUpdate = true;
                    }

                    lock (Game.Markers)
                        Game.RecalcActualTime(Game.Markers, mk1.Half);

                    ReloadDataGridView(true);
                }
                else
                {
                    mk1.TimeVideo = Game.Time;
                    mk1.FlagUpdate = true;

                    lock (Game.Markers)
                        Game.RecalcActualTime(Game.Markers, mk1.Half);

                    ReloadDataGridView(true);

                    if (mk1.Compare(8, 1) && mk1.Point1.IsEmpty)
                    {
                        ProcessingMarker(mk1);
                    }
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip3.Items.Clear();

                contextMenuStrip3.Items.Add(new ToolStripMenuItem("Свойства...", null, EditPropertiesMarker));

                Game.Marker mk = null;
                if (dataGridView1.SelectedItems.Count == 1
                    && dataGridView1.SelectedItems[0].Tag is Game.Marker)
                    mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;

                //Вбрасывание
                /*if (mk != null && mk.Compare(1, 1))
                { 
                    Marker mk1 = null;
                    lock (game.Markers)
                        mk1 = game.Markers.Where(o => !o.FlagDel && o.Compare(1, 1) && o.Half.Index == game.Half.Index)
                            .OrderBy(o => o.TimeVideo).FirstOrDefault();

                    if (mk1 == mk)
                    {
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Установить текущее время для первого вбрасывания", null, SetupCurrentTime) { Tag = mk });
                    }
                }*/

                if (mk != null && mk.ActionId == 14)
                {
                    contextMenuStrip3.Items.Add(new ToolStripMenuItem("Коррекция маркера замены...", null, CorrectMarkerChangedPlayers));

                    if (mk.Num == 1)
                    {
                        contextMenuStrip3.Items.Add(new ToolStripSeparator());
                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Завершить эпизод замен", null, FinallySubsEpisode) { Tag = mk });
                    }
                }

                if (dataGridView1.SelectedItems.Count > 0)
                {
                    //if (mk.ActionId != 16)
                    {
                        if (contextMenuStrip3.Items.Count > 0)
                            contextMenuStrip3.Items.Add(new ToolStripSeparator());

                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время +1s", null, ShiftTimeMarker) { Tag = +1000 });
                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время +0.1s", null, ShiftTimeMarker) { Tag = +100 });
                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время -1s", null, ShiftTimeMarker) { Tag = -1000 });
                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время -0.1s", null, ShiftTimeMarker) { Tag = -100 });
                        contextMenuStrip3.Items.Add(new ToolStripSeparator());

                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время +2s", null, ShiftTimeMarker) { Tag = +2000 });
                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время -2s", null, ShiftTimeMarker) { Tag = -2000 });

                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время +0.5s", null, ShiftTimeMarker) { Tag = +500 });
                        contextMenuStrip3.Items.Add(new ToolStripMenuItem("Время -0.5s", null, ShiftTimeMarker) { Tag = -500 });

                        if (mk != null && mk.ActionId == 14 && mk.player1_id > 0)
                        {
                            contextMenuStrip3.Items.Add(new ToolStripSeparator());
                            contextMenuStrip3.Items.Add(new ToolStripMenuItem("Выделить все замены игрока", null, SelectSubsMarker) { Tag = mk });
                            contextMenuStrip3.Items.Add(new ToolStripMenuItem("Выделить все замены позиции", null, SelectPosMarker) { Tag = mk });
                        }
                    }
                }

                if (dataGridView1.SelectedItems.Count > 0)
                {
                    if (contextMenuStrip3.Items.Count > 0)
                        contextMenuStrip3.Items.Add(new ToolStripSeparator());

                    contextMenuStrip3.Items.Add(new ToolStripMenuItem("Удалить", null, DeleteSelectedMarkers));
                }

                if (dataGridView1.SelectedItems.Count == 1
                    && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Exception != null
                    && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Exception.Code == 1)
                {
                    if (contextMenuStrip3.Items.Count > 0)
                        contextMenuStrip3.Items.Add(new ToolStripSeparator());

                    contextMenuStrip3.Items.Add(new ToolStripMenuItem("Вставить подбор ?", null, InsertPickUp));
                }

                if (dataGridView1.SelectedItems.Count == 1
                    && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Exception != null
                    && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Exception.Code == 2)
                {
                    if (contextMenuStrip3.Items.Count > 0)
                        contextMenuStrip3.Items.Add(new ToolStripSeparator());

                    contextMenuStrip3.Items.Add(new ToolStripMenuItem("Заменить на прием ?", null, ChangeToPickUp2));
                }

                if (dataGridView1.SelectedItems.Count == 1
                    && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Exception != null
                    && ((Game.Marker)dataGridView1.SelectedItems[0].Tag).Exception.Code == 3)
                {
                    if (contextMenuStrip3.Items.Count > 0)
                        contextMenuStrip3.Items.Add(new ToolStripSeparator());

                    contextMenuStrip3.Items.Add(new ToolStripMenuItem("Заменить на подбор ?", null, ChangeToPickUp));
                }
            }
        }

        private void SetupCurrentTime(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            var mk = (Marker)mi.Tag;
            if (mk != null && mk.Compare(1, 1))
            {


                if (mk.TimeVideo > 1000
                    && MessageBox.Show("Маркер уже пересчитан. Продолжить?", "Предупрежедние", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                lock (Game.Markers)
                {
                    var ami = Game.Markers.Where(o => o.Sync == 1
                        && !o.FlagDel
                        && o.Half.Index == Game.Half.Index
                        && o.TimeVideo >= mk.TimeVideo
                        && !o.Compare(new int[] { 16, 18 })).ToList<Marker>();

                    var time_offset = Game.Time - mk.TimeVideo;
                    foreach (var mki in ami)
                    {
                        mki.TimeVideo += time_offset;
                        mki.FlagUpdate = true;
                        mki.Sync = 0;
                        UpdateGridView((Game.Marker)mki);
                    }
                }

                mk.Sync = 2;
            }
        }

        private void FinallySubsEpisode(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            var mk = (Marker)mi.Tag;
            if (mk != null)
            {
                var sibl0 = Game.GetSiblings(mk);

                var err_msg = String.Empty;

                foreach (Game.Marker mk0 in sibl0)
                {
                    Marker mk2 = null;
                    lock (Game.Markers)
                        mk2 = Game.Markers.Where(o => !o.FlagDel && o.Half.Index == mk0.Half.Index && o.TimeVideo > mk0.TimeVideo && o.Compare(14, mk0.ActionType)).OrderBy(o => o.TimeVideo).FirstOrDefault();
                    Marker mk1 = null;
                    lock (Game.Markers)
                        mk1 = Game.Markers.Where(o => !o.FlagDel && o.Half.Index == mk0.Half.Index && o.TimeVideo < mk0.TimeVideo && o.Compare(14, mk0.ActionType)).OrderByDescending(o => o.TimeVideo).FirstOrDefault();

                    if (mk1 != null && mk2 != null)
                    {
                        mk2.Player2 = mk1.Player1;
                        if (mk2.player2_id == mk2.player1_id)
                            Game.Remove(mk0);
                        else
                            mk2.FlagUpdate = true;

                        Game.Remove(mk0);

                        UpdateGridView((Game.Marker)mk2);
                        UpdateGridView((Game.Marker)mk0);
                    }
                    else
                        err_msg += String.Format("Не найдены маркеры замен для маркера {0}\n", mk0.ToString());
                }

                if (!String.IsNullOrEmpty(err_msg))
                    MessageBox.Show(err_msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectSubsMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;

                List<Marker> ami = null;
                lock (Game.Markers)
                    ami = Game.Markers.Where(o => !o.FlagDel && o.ActionId == 14 && o.Half == mk.Half
                        && (o.player1_id == mk.player1_id || o.player2_id == mk.player1_id)).ToList<Marker>();

                foreach (Game.Marker mki in ami)
                    mki.row.Selected = true;
            }
        }

        private void SelectPosMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;

                List<Marker> ami = null;
                lock (Game.Markers)
                    ami = Game.Markers.Where(o => !o.FlagDel && o.ActionId == 14 && o.ActionType == mk.ActionType && o.Half == mk.Half
                        && (o.team1_id == mk.team1_id || o.team2_id == mk.team1_id)).ToList<Marker>();

                foreach (Game.Marker mki in ami)
                    mki.row.Selected = true;
            }
        }

        private void ChangeToPickUp(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;
                mk.ActionId = 2;
                mk.ActionType = 5;
                mk.FlagUpdate = true;

                Game.SaveLocal();

                UpdateGridView((Game.Marker)mk);
            }
        }

        private void ChangeToPickUp2(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;
                mk.ActionId = 2;
                mk.ActionType = 4;
                mk.FlagUpdate = true;

                Game.SaveLocal();

                UpdateGridView((Game.Marker)mk);
            }
        }

        private void InsertPickUp(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;
                Game.Insert(new Game.Marker(Game, 2, 5, mk.Half, mk.TimeVideo) { Player1 = mk.Player1 });

                Game.SaveLocal();

                ReloadDataGridView(true);
            }
        }

        private void ShiftTimeMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count > 0)
            {
                var mi = (ToolStripMenuItem)sender;
                var shift = Convert.ToInt32(mi.Tag);

                foreach (ListViewItem row in dataGridView1.SelectedItems)
                {
                    var mki = (Game.Marker)row.Tag;
                    Log.Write("MANUAL SHIFT TIME BEFORE " + mki.ToString());
                    mki.TimeVideo += shift;
                    mki.FlagUpdate = true;
                    mki.row = null;
                    Log.Write("MANUAL SHIFT TIME AFTER " + mki.ToString());
                }

                Game.SaveLocal();

                lock (Game.Markers)
                    Game.RecalcActualTime(Game.Markers, Game.Half);

                ReloadDataGridView(true);
            }
        }

        private void dataGridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1 && dataGridView1.SelectedItems[0].Tag is Game.Marker)
            {
                var mk = (Game.Marker)dataGridView1.SelectedItems[0].Tag;

                linkLabel7.Visible = mk.TimeVideo == -1 || Options.G.Game_EnableApplyTimeEx;

                try
                {
                    Game.CheckValid(mk);
                    label11.Visible = false;
                }
                catch (HockeyIce.CheckValidMarkerException ex)
                {
                    switch (ex.CheckValidLevel)
                    {
                        case HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.WARNING:
                            label11.ForeColor = Color.Black;
                            label11.BackColor = Color.Yellow;
                            break;

                        case HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.CRITICAL:
                            label11.ForeColor = Color.White;
                            label11.BackColor = Color.Red;
                            break;
                    }

                    label11.Text = ex.Message;
                    label11.Visible = true;
                }
            }
            else
                linkLabel7.Visible = false;
        }

        private void statusStrip1_Resize(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new OptionsForm();
            form.ShowDialog();
            UpdateAutoSaveTimer();
        }

        private void RegisterCancel()
        {
            fixedTime = 0;
            ShowStatus("", 0);
            HockeyGui.Mode = HockeyGui.ModeEnum.View;
            SetEditMarker(null, StageEnum.ScreenPosition);
            vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
            HockeyGui.ChangedPlayersList.Clear();
        }

        private int fixedTime = 0;

        public void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedMarkers(sender, EventArgs.Empty);
            }

            if (e.KeyData == Keys.Escape)
            {
                RegisterCancel();
                return;
            }

            if (e.KeyData == Options.G.Hotkey_PauseResume)
            {
                if (vlcStreamPlayer1.Mode != StreamPlayer.PlayerMode.Stop)
                {
                    vlcStreamPlayer1.Mode =
                        vlcStreamPlayer1.Mode == StreamPlayer.PlayerMode.Play
                        ? StreamPlayer.PlayerMode.Pause
                        : StreamPlayer.PlayerMode.Play;
                }

                return;
            }

            if (e.KeyData == Options.G.Hotkey_RegisterBeginNoFix)
            {
                if (Game.editMarker.G != null)
                {
                    RegisterCancel();
                }
                else
                {
                    RegisterBegin(0, 0);
                }
                return;
            }

            if (e.KeyData == Options.G.Hotkey_RegisterBeginFix)
            {
                if (Game.editMarker.G != null)
                {
                    RegisterCancel();
                }
                else
                {
                    fixedTime = Second; // Game.Time;
                    RegisterBegin(0, 0);
                }
                return;
            }

            if (e.KeyData == Options.G.Hotkey_StepNext)
            {
                vlcStreamPlayer1.Position += Options.G.StepNextValue;
                return;
            }

            if (e.KeyData == Options.G.Hotkey_StepPrev)
            {
                vlcStreamPlayer1.Position -= Options.G.StepPrevValue;
                return;
            }

            if (e.KeyData == Options.G.HotKey_1_1_1_2)
            {
                var mklct = Game.GetLastControlTimeMarker(Half, Second);
                var rest_time1 = mklct != null && Game.IsRestoreTimeMarker(mklct);
                var stop_time1 = mklct == null || Game.IsStopTimeMarker(mklct);

                if (!stop_time1)
                    RegisterBegin(3, 8);
                else
                    if (!rest_time1)
                        RegisterBegin(1, 1);

                return;
            }

            if (Half != null)
            {
                var type = Options.G.GetType();
                var ps = type.GetProperties();
                foreach (var p in ps)
                {
                    if (p.Name.Contains("HotKeyAction_") && (Keys)p.GetValue(Options.G, null) == e.KeyData)
                    {
                        var tag = p.Name.Replace("HotKeyAction_", "");
                        Marker mk = new Game.Marker(Game, tag);
                        var code = mk.ActionId * 100000 + mk.ActionType * 100 + mk.Win;

                        if (mk.Compare(2, 9))
                        {
                            RegisterAddons(code);
                            return;
                        }

                        RegisterBegin(code);
                    }
                }
            }

            var lastmk = Game.GetSiblings(Half, Second);
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);

            var proc_and_clear = false;
            lock (Game.editMarker)
                proc_and_clear = Game.editMarker.G != null && Game.editMarker.G.Compare(0, 0);

            var video_load = vlcStreamPlayer1 != null && vlcStreamPlayer1.Mode != StreamPlayer.PlayerMode.Stop;
            var rest_time = prevmk.Exists(o => Game.IsRestoreTimeMarker(o));
            var stop_time = prevmk.Exists(o => Game.IsStopTimeMarker(o));
            var stop_game = prevmk.Exists(o => o.Compare(3, 8));
            var bullet_period = Half != null && Half.Index == 255;

            if (e.KeyData == Options.G.Hotkey_Action_DumpIn)
            {
                if (Half != null
                    && video_load && !rest_time
                    && (stop_time || prevmk.Count == 0)
                    && !bullet_period)
                {
                    RegisterBegin(1, 1);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                timer2.Enabled = false;
                SaveAsync();
            }
            finally
            {
                timer2.Enabled = true;
            }
        }

        void vlcStreamPlayer1_DownloadSceneResult(object sender, StreamPlayer.DownloadSceneResultEventAgrs e)
        {
            try
            {
                sync.Execute(() =>
                {
                    var player = (StreamPlayer)sender;
                    label16.Text = String.Format("{2}[kbps] {0}/{1}",
                        Utils.TimeFormat(player.DurationUpload),
                        Utils.TimeFormat(player.DurationTotal),
                        Convert.ToInt32(e.Scene.Rate / 1024.0f));

                    if (e.ErrorCode == null)
                    {
                        label14.BackColor = SystemColors.ActiveCaption;
                        label14.ForeColor = Color.Black;
                        label14.Text = vlcStreamPlayer1.Uri.ToString();

                        macTrackBar1.Maximum = vlcStreamPlayer1.DurationTotal;
                        macTrackBar1.Buffered = vlcStreamPlayer1.DurationUpload;
                        label8.Text = Utils.TimeFormat(vlcStreamPlayer1.DurationUpload);
                    }
                    else
                    {
                        label14.BackColor = Color.Red;
                        label14.ForeColor = Color.White;
                        label14.Text = e.ErrorCode.Message;

                        Log.WriteException(e.ErrorCode);
                    }
                });
            }
            catch
            { }
        }

        void vlcStreamPlayer1_DownloadPlaylistResult(object sender, StreamPlayer.DownloadPlaylistResultEventAgrs e)
        {
            try
            {
                sync.Execute(() =>
                {
                    if (e.ErrorCode == null)
                    {
                        label15.BackColor = SystemColors.ActiveCaption;
                        label15.ForeColor = Color.Black;
                        label15.Text = vlcStreamPlayer1.Uri.ToString();
                    }
                    else
                    {
                        label15.BackColor = Color.Red;
                        label15.ForeColor = Color.White;
                        label15.Text = e.ErrorCode.Message;

                        //Log.WriteException(e.ErrorCode);
                        if (e.ErrorCode != null && e.ErrorCode.InnerException != null)
                        {
                            Log.WriteException(e.ErrorCode.InnerException);
                        }
                    }
                });
            }
            catch
            { }
        }

        private void vlcStreamPlayer1_MediaBuffering(object sender, EventArgs e)
        {
            sync.Execute(() => panel8.Visible = vlcStreamPlayer1.Buffering );
        }

        private void vlcStreamPlayer1_MediaModeChanged(object sender, EventArgs e)
        {

        }

        private Marker last_shot_stop = null;

        private void vlcStreamPlayer1_PositionChanged(object sender, StreamPlayer.PositionEventArgs e)
        {
            sync.Execute(() =>
                {
                    macTrackBar1.Maximum = vlcStreamPlayer1.DurationTotal;
                    macTrackBar1.Buffered = vlcStreamPlayer1.DurationUpload;
                    macTrackBar1.Value = Convert.ToInt32(vlcStreamPlayer1.Position);

                    label5.Text = Utils.TimeFormat(Second);
                    label8.Text = Utils.TimeFormat(vlcStreamPlayer1.DurationUpload);

                    if (hockeyField1.Visible && HockeyIce.Role != HockeyIce.RoleEnum.AdvTtd)
                        hockeyField1.RefreshRgn();

                    if (Options.G.IsStopPlayingOnShot && vlcStreamPlayer1.Mode == StreamPlayer.PlayerMode.Play)
                    {
                        var prev = Game.GetPrevousMarkersHalf(Game.Half, Game.Time, true);
                        if (prev.Any(o => o.Compare(4, new int[] {1, 2, 3, 4, 5}) && o != last_shot_stop))
                        {
                            last_shot_stop = prev.First(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }));
                            vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
                        }
                    }
                });

            Game.Time = Second; 
            UpdateUI();
        }

        private void macTrackBar1_Scroll(object sender, EventArgs e)
        {
            sync.Execute(() =>
                {
                    var pos = macTrackBar1.Value >= 0 ? macTrackBar1.Value : 0;
                    vlcStreamPlayer1.Position = pos;
                });
        }

        private void ShowTimer()
        {
            sync.Execute(() =>
            {
                var s = 0;

                if (Options.G.Game_ShowActualTime)
                {
                    s = Game.GetActuialTime(Half, fixedTime > 0 ? fixedTime : Second);
                }
                else
                {
                    s = fixedTime > 0 ? fixedTime : Second;
                }

                if (s > Half.Length && Half.Length > 0)
                {
                    s = Convert.ToInt32(Half.Length);
                    label1.BackColor = Color.Red;
                    label1.ForeColor = Color.White;
                }
                else
                {
                    label1.BackColor = SystemColors.ButtonFace;
                    label1.ForeColor = Color.Black;
                }

                if (fixedTime > 0)
                {
                    label4.BackColor = Color.Yellow;
                    label4.ForeColor = Color.Black;
                }
                else
                {
                    label4.BackColor = SystemColors.ButtonFace;
                    label4.ForeColor = Color.Black;
                }

                labelTimeP.Text = Utils.TimeFormat(Half.Length - s);
                label4.Text = Utils.TimeFormat(s);

                int sc1, sc2;
                Game.GetScore(Half, Second, out sc1, out sc2);
                label13.Text = String.Format("{0}-{1}", sc1, sc2);

                Game.GetScoreBullet(Second, out sc1, out sc2);
                label14.Text = String.Format("{0}-{1}", sc1, sc2);
            });
        }

        private void hockeyField1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void vlcStreamPlayer1_MouseEnter(object sender, EventArgs e)
        {
            
        }

        private void transparentPanel1_MouseEnter(object sender, EventArgs e)
        {
            hockeyField1.ClearMousePosition();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAddPanel(false);
            panel4.Visible = !panel4.Visible;
        }

        public Rectangle LastDataGridRectangle { get; set; }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAddPanel(false);

            /*if (!dataGridView1.Visible)
                SlideDatagridToCurrentTime();

            dataGridView1.Visible = !dataGridView1.Visible;
            linkLabel7.Visible = !linkLabel7.Visible;*/

            /*if (dataGridForm1 == null)
            {
                if (LastDataGridRectangle.IsEmpty)
                {
                    LastDataGridRectangle = new Rectangle(Left + panel2.Left + 14, Bottom - 210, panel2.Width - 10, 200);
                }

                dataGridForm1 = new DataGridForm(this, Game);
                dataGridForm1.Show();
                dataGridForm1.Location = LastDataGridRectangle.Location;
                dataGridForm1.Size = LastDataGridRectangle.Size;
                dataGridForm1.ReloadDataGridView(true);
                dataGridForm1.SlideDatagridToCurrentTime(true);
            }
            else
            {
                dataGridForm1.BringToFront();
            }*/
        }

        private void labelTimeP_Click(object sender, EventArgs e)
        {
            var mk = Game.GetLastControlTimeMarker(Game.Half, Game.Time);
            if (mk != null && Game.IsStopTimeMarker(mk))
            {
                var actual_time_0 = Half.Length - mk.TimeActual;
                var cf = new CorrectTimeForm(actual_time_0);
                if (cf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var offset = actual_time_0 - cf.CorrectTime;
                    mk.TimeVideo += offset;
                    mk.FlagUpdate = true;

                    lock (Game.Markers)
                        Game.RecalcActualTime(Game.Markers, Game.Half);

                    ReloadDataGridView(true);
                    UpdateUI();
                }
            }
        }

        private void hockeyField1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void spareField2_ChangedPlayersBegin(object sender, EventArgs e)
        {
            vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
            if (Half != null && Half.Index != 255)
                hockeyField1.Visible = true;
        }

        private void spareField2_ChangedPlayersEnd(object sender, EventArgs e)
        {
            if (HockeyGui.ChangedPlayersList.Count == 0)
            {
                if (HockeyIce.Role != HockeyIce.RoleEnum.AdvTtd)
                    hockeyField1.Visible = false;
            }
        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Game.Match.IsChangedTeamPlaces = !Game.Match.IsChangedTeamPlaces;

            HockeyGui.SF1.Team = Game.Match.Team1;
            HockeyGui.SF2.Team = Game.Match.Team2;

            UpdateUI();
            HockeyGui.InvalidateRect();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(toolStripStatusLabel1.Text.Replace("&&", "&"));
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (hockeyField1 != null)
                hockeyField1.CompleteEnter();
        }

        private void ReturnAllPlayers(Team team, int time, int num)
        {
            HockeyGui.ChangedPlayersList.Clear();
            foreach (var p in team.Tactics[0].Places)
            {
                if (p.Player == null)
                    continue;

                if (team.FinePlayers.Any(o => o.Id == p.Player.Id))
                    continue;

                var mk = new Game.Marker(Game, 14, p.GetCode(), Half, time) { Player1 = null, Player2 = p.Player, Num = num };
                Log.Write(String.Format("RETURN PLAYER {0}", p.Player));
                Game.Insert(mk);
            }

            ReloadDataGridView();
            UpdateUI();
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Вернуть всех игроков с площадки?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
                ReturnAllPlayers(Game.Match.Team1, Second, 0);
            }
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Вернуть всех игроков с площадки?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
                ReturnAllPlayers(Game.Match.Team2, Second, 0);
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            //vlcStreamPlayer1.Rate = (float)trackBar3.Value / 10.0f;
            //label6.Text = "x" + vlcStreamPlayer1.Rate.ToString("0.0");
        }

        private void vlcStreamPlayer1_MediaOpened(object sender, EventArgs e)
        {
            sync.Execute(() =>
            {
                if (vlcStreamPlayer1.Mode == StreamPlayer.PlayerMode.Play)
                {
                    //trackBar2.Value = vlcStreamPlayer1.Volume;
                }
            });

            UpdateUI();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel14_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Marker mk189 = null;
            lock (Game.Markers)
                mk189 = Game.Markers.FirstOrDefault(o => !o.FlagDel && o.Half.Index == Half.Index && o.Compare(18, 9));

            if (mk189 != null && MessageBox.Show("Маркер синхронизации уже есть. Сформировать новый?", "Предупрежедние",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            List<Marker> del = null;
            lock (Game.Markers)
                del = Game.Markers.Where(o => !o.FlagDel && o.Half.Index == Half.Index && o.Compare(18, 9)).ToList<Marker>();

            foreach (var mki in del)
                Game.Remove(mki);

            Game.Insert(new Game.Marker(Game, 18, 9, Game.Half, Game.Time));
            UpdateUI();

            var sync_ch = Game.UpdateSync(Half);

            if (sync_ch)
            {
                lock (Game.Markers)
                    foreach (var mk in Game.Markers.Where(o => !o.FlagDel && o.Half.Index == Half.Index && o.Sync == 1))
                        mk.FlagGuiUpdate = true;
            }

            ReloadDataGridView(sync_ch);
        }
        
        private void checkBox401000_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (Button)sender;

            var update = false;
            var code = Convert.ToInt32(rb.Tag);

            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            if (prevmk.Any(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
            {
                var mk4 = prevmk.Where(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();
                var lastmk = Game.GetSiblings(mk4.Half, mk4.TimeVideo);
                var mk_new = new Game.Marker(Game) { ActionCode = code, Half = mk4.Half, TimeVideo = mk4.TimeVideo, Player1 = mk4.Player1, Player2 = mk4.Player2 };

                if (lastmk.Any(o => o.Compare(4, new int[] { 10, 11 })))
                {
                    foreach (var mk4p in lastmk.Where(o => o.Compare(4, new int[] { 10, 11 })))
                    {
                        if (mk4p.ActionType == mk_new.ActionType)
                            continue;

                        Game.Remove(mk4p);

                        update = true;
                    }
                }

                if (code > 0 && !Game.GetSiblings(mk4.Half, mk4.TimeVideo).Any(o => o.Compare(4, mk_new.ActionType)))
                {
                    update = true;
                    Game.Insert(mk_new);
                }

                if (update)
                {
                    Game.SaveLocal();
                }

                UpdateUI();
                ReloadDataGridView(true);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Options.G.IsStopPlayingOnShot = checkBox2.Checked;
            Options.G.SaveXml();
        }

        //Точка в воротах
        private void button1_Click_1(object sender, EventArgs e)
        {
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            if (prevmk.Any(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
            {
                var mk4 = prevmk.Where(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();
                var lastmk = Game.GetSiblings(mk4.Half, mk4.TimeVideo);

                var new_up = false;
                Marker mk_new = null;
                if (lastmk.Any(o => o.Compare(11, 4) && o.Win == 1))
                {
                    mk_new = lastmk.First(o => o.Compare(11, 4) && o.Win == 1);
                }
                else
                {
                    new_up = true;
                    mk_new = new Game.Marker(Game, 11, 4, mk4.Half, mk4.TimeVideo) { Win = 1, Player1 = mk4.Player1, Player2 = mk4.Player2 };
                }

                Marker copy = new Game.Marker(Game);
                copy.Assign(mk_new);
                
                var form = new GoalPointForm(Game, copy);
                form.ShowDialog();
                //if (form.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    if (!mk_new.Point1.Equals(copy.Point1))
                    {
                        mk_new.Assign(copy);
                        mk_new.FlagUpdate = true;

                        if (new_up)
                            Game.Insert(mk_new);

                        Game.SaveLocal();

                        UpdateUI();
                        ReloadDataGridView(true);
                    }
                }
            }
        }

        private void button1100101_Click(object sender, EventArgs e)
        {
            var rb = (Button)sender;

            var exists = false;
            var update = false;
            var code = Convert.ToInt32(rb.Tag);

            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            if (prevmk.Any(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
            {
                var mk4 = prevmk.Where(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();
                var lastmk = Game.GetSiblings(mk4.Half, mk4.TimeVideo);
                var mk_new = new Game.Marker(Game) { ActionCode = code, Half = mk4.Half, TimeVideo = mk4.TimeVideo, Player1 = mk4.Player1, Player2 = mk4.Player2 };
                /*if (mk_new.ActionType == 3)
                {
                    mk_new.Player1 = mk4.Player2;
                    mk_new.Player2 = mk4.Player1;
                }*/

                if (lastmk.Any(o => o.Compare(11, mk_new.ActionType)))
                {
                    foreach (var mk4p in lastmk.Where(o => o.Compare(11, mk_new.ActionType)))
                    {
                        if (mk4p.Compare(11, 1) && mk4p.Win == mk_new.Win)
                        {
                            exists = true;
                        }

                        Game.Remove(mk4p);

                        update = true;
                    }
                }

                if (code > 0 && !exists && !Game.GetSiblings(mk4.Half, mk4.TimeVideo).Any(o => o.Compare(11, mk_new.ActionType) && o.Win == mk_new.Win))
                {
                    update = true;
                    Game.Insert(mk_new);
                }

                if (update)
                {
                    Game.SaveLocal();
                }

                UpdateUI();
                ReloadDataGridView(true);
            }
        }

        //Отмена атаки
        private void linkLabel_a12_0_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var attack = Game.GetLastMarkerAttack(Half, Second);
            if (attack != null && attack.ActionType > 1)
            {
                if (attack.FlagSaved)
                {
                    attack.FlagDel = true;
                    attack.FlagUpdate = true;                    
                }
                else
                {
                    lock (Game.Markers)
                        Game.Markers.Remove(attack);
                }

                Game.SaveLocal();

                UpdateUI();
                ReloadDataGridView();
                SetEditMarker(null, StageEnum.CreateMarker);
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
            }
        }

        //Завершение атаки
        private void linkLabel_a12_1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var attack = Game.GetLastMarkerAttack(Half, Second);
            if (attack != null && attack.ActionType > 1)
            {
                Game.Insert(new Game.Marker(Game, 12, 1, Game.Half, Game.Time));

                Game.SaveLocal();

                UpdateUI();
                ReloadDataGridView();
                SetEditMarker(null, StageEnum.CreateMarker);
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Play;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel && comboBox2.SelectedItem is Player)
            {
                var player = comboBox2.SelectedItem as Player;
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
                {
                    var mk4 = prevmk.Where(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();
                    
                    var sibl = Game.GetSiblings(mk4);
                    foreach (var mki in sibl)
                    {
                        lock (Game.editMarker)
                        {
                            Game.editMarker.G = (Game.Marker)mki;
                            if (mki.Compare(2, 9))
                                mki.Player2 = player;
                            else
                                mki.Player1 = player;

                            mki.FlagUpdate = true;

                            Game.editMarker.G = null;
                        }

                        UpdateGridView((Game.Marker)mki);
                    }

                    UpdateGridView((Game.Marker)mk4);
                    UpdateCombo29(mk4);
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel && comboBox3.SelectedItem is Player)
            {
                var player = comboBox3.SelectedItem as Player;
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
                {
                    var mk4 = prevmk.Where(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();
                    
                    var sibl = Game.GetSiblings(mk4);
                    foreach (var mki in sibl)
                    {
                        if (!mki.Compare(2, 9))
                        {
                            lock (Game.editMarker)
                            {
                                Game.editMarker.G = (Game.Marker)mki;
                                mki.Player2 = player;
                                mki.FlagUpdate = true;
                                Game.editMarker.G = null;
                            }

                            UpdateGridView((Game.Marker)mki);
                        }
                    }

                    UpdateGridView((Game.Marker)mk4);
                }
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel && comboBox5.SelectedItem is Player)
            {
                var player = comboBox5.SelectedItem as Player;
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(1, 1)))
                {
                    var mk11 = prevmk.Where(o => o.Compare(1, 1)).OrderByDescending(o => o.TimeVideo).First();

                    lock (Game.editMarker)
                    {
                        Game.editMarker.G = (Game.Marker)mk11;
                        mk11.Player1 = player;
                        mk11.FlagUpdate = true;
                        Game.editMarker.G = null;
                    }

                    UpdateGridView((Game.Marker)mk11);
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel && comboBox4.SelectedItem is Player)
            {
                var player = comboBox4.SelectedItem as Player;
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(1, 1)))
                {
                    var mk11 = prevmk.Where(o => o.Compare(1, 1)).OrderByDescending(o => o.TimeVideo).First();
                    lock (Game.editMarker)
                    {
                        Game.editMarker.G = (Game.Marker)mk11;
                        mk11.Player2 = player;
                        mk11.FlagUpdate = true;
                        Game.editMarker.G = null;
                    }

                    UpdateGridView((Game.Marker)mk11);
                }
            }
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel && comboBox9.SelectedItem is Player)
            {
                var player = comboBox9.SelectedItem as Player;
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(6, 1)))
                {
                    var mk11 = prevmk.Where(o => o.Compare(6, 1)).OrderByDescending(o => o.TimeVideo).First();

                    lock (Game.editMarker)
                    {
                        Game.editMarker.G = (Game.Marker)mk11;
                        mk11.Player1 = player;
                        mk11.FlagUpdate = true;
                        Game.editMarker.G = null;
                    }

                    UpdateGridView((Game.Marker)mk11);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel && comboBox1.SelectedItem is Player)
            {
                var player = comboBox1.SelectedItem as Player;
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(6, 1)))
                {
                    var mk11 = prevmk.Where(o => o.Compare(6, 1)).OrderByDescending(o => o.TimeVideo).First();
                    lock (Game.editMarker)
                    {
                        Game.editMarker.G = (Game.Marker)mk11;
                        mk11.Player2 = player;
                        mk11.FlagUpdate = true;
                        Game.editMarker.G = null;
                    }

                    UpdateGridView((Game.Marker)mk11);
                }
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel)
            {
                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(8, 1)))
                {
                    var mk4 = prevmk.Where(o => o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();

                    var sibl = Game.GetSiblings(mk4);

                    var assist = new List<Player>();
                    if (comboBox7.SelectedItem is Player)
                        assist.Add(comboBox7.SelectedItem as Player);
                    if (comboBox8.SelectedItem is Player)
                        assist.Add(comboBox8.SelectedItem as Player);

                    foreach (var mka in sibl.Where(o => o.Compare(1, 2)))
                        Game.Remove(mka);

                    foreach (var p in assist)
                    {
                        Game.Insert(new Game.Marker(Game, 1, 2, mk4.Half, mk4.TimeVideo) { Player1 = p });
                    }

                    ReloadDataGridView();
                }
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lock_change_players_in_panel)
            {
                Player player = null;
                if (comboBox6.SelectedItem is Player)
                    player = comboBox6.SelectedItem as Player;

                var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
                if (prevmk.Any(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
                {
                    var mk4 = prevmk.Where(o => o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)).OrderByDescending(o => o.TimeVideo).First();

                    var sibl = Game.GetSiblings(mk4);

                    if (player == null && sibl.Any(o => o.Compare(2, 9)))
                    {
                        var mki = sibl.First(o => o.Compare(2, 9));

                        Game.Remove(mki);

                        ReloadDataGridView();
                    }

                    if (player != null)
                    {
                        var pt = new PointF(Game.FieldSize.Width - mk4.Point1.X, Game.FieldSize.Height - mk4.Point1.Y);

                        var mki = sibl.FirstOrDefault(o => o.Compare(2, 9));
                        if (mki != null)
                        {
                            lock (Game.editMarker)
                            {
                                Game.editMarker.G = (Game.Marker)mki;
                                mki.Player1 = player;
                                mki.FlagUpdate = true;
                                Game.editMarker.G = null;
                            }

                            UpdateGridView((Game.Marker)mki);
                        }
                        else
                        {
                            mki = new Game.Marker(Game, 2, 9, mk4.Half, mk4.TimeVideo) { Player1 = player, Player2 = mk4.Player1, Point1 = pt };
                            Game.Insert(mki);
                            ReloadDataGridView();
                        }
                    }
                }
            }
        }

        private void button1400000_Click(object sender, EventArgs e)
        {
            var form = new InsertSubstitutionForm(Game);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {



                lock (Game.Markers)
                {
                    //Опорный - удаляем
                    var mk1 = Game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == Game.Half.Index && o.TimeVideo > Game.Time && o.ActionId == 14 && o.player1_id == form.Player1.Id)
                        .OrderByDescending(o => o.TimeVideo)
                        .FirstOrDefault();

                    //Заменяем
                    var mk2 = Game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == Game.Half.Index && o.TimeVideo > Game.Time && o.ActionId == 14 && o.player2_id == form.Player2.Id)
                        .OrderBy(o => o.TimeVideo)
                        .FirstOrDefault();

                    if (mk1 != null)
                    {
                        Game.Remove(mk1);

                        if (mk2 != null)
                        {
                            mk2.Player2 = mk1.Player2;
                            mk2.FlagUpdate = true;
                        }
                    }

                    Marker mk = new Game.Marker(Game, 14, mk1.ActionType, Game.Half, Game.Time)
                    {
                        Player1 = form.Player1,
                        Player2 = form.Player2
                    };

                    Game.Insert(mk);
                    Game.SaveLocal(); 

                    ReloadDataGridView();

                    /*Marker mk1 = Game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == Game.Half.Index && o.TimeVideo < Game.Time && o.ActionId == 14 && o.player1_id == form.Player2.Id)
                        .OrderByDescending(o => o.TimeVideo)
                        .FirstOrDefault();

                    Marker mk2 = Game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == Game.Half.Index && o.TimeVideo > Game.Time && o.ActionId == 14 && o.player2_id == form.Player2.Id)
                        .OrderBy(o => o.TimeVideo)
                        .FirstOrDefault();

                    if (mk1 != null)
                    {
                        Marker mk = new Game.Marker(14, mk1.ActionType, Game.Half, Game.Time) 
                        { 
                            Player1 = form.Player1, 
                            Player2 = form.Player2 
                        };

                        if (mk2 != null)
                        {
                            mk2.Player2 = form.Player1;
                            mk2.FlagUpdate = true;
                        }

                        Game.Insert(mk);

                        ReloadDataGridView();
                    }*/
                }
            }
        }

        private bool dataGrid_Mode = false;

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private int offset = 0;

        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*CorrectTimeForm cf = new CorrectTimeForm(0);
            if (cf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                offset = cf.CorrectTime - Second;
                UpdateUI();
            }*/
        }

        private void button_11_Point_Click(object sender, EventArgs e)
        {
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            var mk11 = (Game.Marker)prevmk
                .Where(o => o.TimeVideo > -1 && o.Compare(1, 1))
                .OrderByDescending(o => o.TimeVideo).FirstOrDefault();

            if (mk11 != null)
            {
                mk11.Point1 = Point.Empty;
                Second = mk11.TimeVideo;// -100;
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
                UpdateUI();
                ProcessingMarker(mk11);
            }
        }

        private void button_4_Point_Click(object sender, EventArgs e)
        {
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            var mk11 = (Game.Marker)prevmk
                    .Where(o => o.TimeVideo > -1 && (o.Compare(4, new int[] { 1, 2, 3, 4, 5 }) || o.Compare(8, 1)))
                    .OrderByDescending(o => o.TimeVideo).FirstOrDefault();

            if (mk11 != null)
            {
                mk11.Point1 = Point.Empty;
                mk11.Point2 = Point.Empty;
                Second = mk11.TimeVideo - 100;
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
                UpdateUI();
                ProcessingMarker(mk11);
            }
        }

        private void button_61_Point_Click(object sender, EventArgs e)
        {
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            var mk11 = (Game.Marker)prevmk
                .Where(o => o.TimeVideo > -1 && o.Compare(6, 1))
                .OrderByDescending(o => o.TimeVideo).FirstOrDefault();

            if (mk11 != null)
            {
                mk11.Point1 = Point.Empty;
                Second = mk11.TimeVideo - 100;
                vlcStreamPlayer1.Mode = StreamPlayer.PlayerMode.Pause;
                UpdateUI();
                ProcessingMarker(mk11);
            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowAddPanel(false);
            HockeyIce.Role = (HockeyIce.RoleEnum)(comboBox10.SelectedIndex + 1);

            RefreshHockeyField();
            RefreshRole();
            UpdateUI();
        }

        private SelectedMarker selectedMarker = null;

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMarker = (SelectedMarker)comboBoxEx1.SelectedItem;
            ReloadDataGridView();
        }

        private void button300600_100900_Click(object sender, EventArgs e)
        {
            // TODO:
            var prevmk = Game.GetPrevousMarkersHalf(Half, Second, true);
            // 4.2.0
            if (prevmk == null || prevmk.Count == 0)
            {
                return;
            }

            var markerk400200 = (Game.Marker)prevmk.FirstOrDefault(o => ((Game.Marker) o).Compare(4, 2, 0));

            if (markerk400200 == null)
            {
                return;
            }

            var tag = (sender as ButtonEx).Tag.ToString();

            var tagid = 0;

            if (int.TryParse(tag, out tagid) == false)
            {
                return;
            }

            var newMarker = new Game.Marker(Game)
            {
                ActionCode = tagid,
                Half = markerk400200.Half,
                TimeVideo = markerk400200.TimeVideo,
                Player1 = markerk400200.Player2,
                Point1 = markerk400200.Point2,
                FlagGuiUpdate = true,
            };

            if (MarkersWomboCombo.CheckPrevMarkerNeedsExtraMarker(markerk400200, newMarker))
            {
                MarkersWomboCombo.XorAddSingleNewExtraMarker(markerk400200, newMarker);
                

                var markersToDelete = Game.Markers.Where(x =>
                                ((Game.Marker)x).Half.Index == newMarker.Half.Index &&
                                ((Game.Marker)x).TimeVideo == newMarker.TimeVideo &&
                                (((Game.Marker)x).Compare(3, 6, 0) || ((Game.Marker)x).Compare(1, 9, 0))
                    );

                var toDelete = markersToDelete.ToList<Marker>(); //markersToDelete as Marker[] ?? markersToDelete.ToArray()
                if (toDelete.Any())
                {
                    foreach (var marker in toDelete)
                    {
                        Game.Remove(marker);
                    }
                }
                else
                {
                    Game.Insert(newMarker);
                }
            }

            ReloadDataGridView(true);

            UpdateUI();
        }

    }

    

    
}
