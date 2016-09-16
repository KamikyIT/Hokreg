using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Uniso.InStat.Game;
using Uniso.InStat.Gui;

namespace Uniso.InStat
{
    public partial class DataGridForm : Form
    {
        private UISync sync = null;
        private MainForm mainForm = null;
        private HockeyIce game = null;

        public DataGridForm(MainForm mainForm, HockeyIce game)
        {
            InitializeComponent();

            this.mainForm = mainForm;
            this.game = game;

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
        }

        public ListView GetDataGrid()
        {
            return dataGridView1;
        }

        private void DataGridForm_Load(object sender, EventArgs e)
        {
            sync = new UISync(this);
        }

        private void DataGridForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //mainForm.dataGridForm1 = null;
            lock (game.Markers)
                foreach (Game.Marker mk in game.Markers)
                    mk.row = null;
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
            foreach (ListViewItem item in GetDataGrid().SelectedItems)
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
                GetDataGrid().BeginUpdate();
                try
                {
                    lock (game.Markers)
                    {
                        var lstDel1 = new List<Marker>();
                        foreach (var row in lst)
                        {
                            if (row.Tag is Marker)
                            {
                                var mk = (Marker)row.Tag;
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
                        /*foreach (Marker mk in lstDel)
                        {
                            Log.Write("DELETE MANUAL " + mk);

                            if (mk.FlagSaved)
                                mk.FlagDel = true;
                            else
                                game.Markers.Remove(mk);
                        }*/

                        for (var i = 0; i < lstDel.Count; i++)
                        {
                            var mk = lstDel[i];
                            game.Remove(mk);
                        }

                            if (mkd != null && HockeyIce.Role == HockeyIce.RoleEnum.AdvTtd)
                                game.RecalcMarkers(mkd.Half);

                        ReloadDataGridView(true);
                        GetDataGrid().SelectedItems.Clear();

                        game.SaveLocal();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.WriteException(ex);
                }
                finally
                {
                    GetDataGrid().EndUpdate();
                    //UpdateUI();
                }
            }
        }

        private Object locker = new object();
        private bool hide_subs_markers = false;
        private bool only_current_time = false;
        private Font font_italic = null;

        private void FillRow(Game.Marker mk)
        {
            if (font_italic == null)
                font_italic = new System.Drawing.Font(Font, FontStyle.Italic);

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

            if (action == Game.Hockey.ActionEnum._18_05)
                name += String.Format(" #{0}", mk.Half.Index - 3);

            if (game.IsRestoreTimeMarker(mk) || game.IsStopTimeMarker(mk))
                mk.row.Font = font_italic;
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

        public void ReloadDataGridView(bool force_update)
        {
            if (!auto_update_datagrid && !force_update)
                return;

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
                    if (mk.ActionId == 14 && hide_subs_markers)
                        continue;
                    if (only_current_time && (mk.Half.Index != game.Half.Index || mk.TimeVideo > game.Time))
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

                locker = new object();
            }
            //});
        }

        public void SlideDatagridToCurrentTime(bool force_update)
        {
            if (!auto_update_datagrid && !force_update)
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
                        mainForm.ProcessingMarker(mk);
                        return;
                    }

                    if (mk.Compare(6, 1) && mk.Player1 == null && mk.Player2 == null)
                    {
                        mainForm.ProcessingMarker(mk);
                        return;
                    }

                    if (mk.Compare(8, 1) && mk.Point1.IsEmpty)
                    {
                        mk.Point2 = Point.Empty;
                        mainForm.ProcessingMarker(mk);
                        return;
                    }

                    if (mk.ActionId == 5 && mk.ActionType > 1 && mk.Num == 0 && HockeyIce.Role != HockeyIce.RoleEnum.Ttd)
                    {
                        lock (game.Markers)
                        {
                            if (!game.Markers.Any(o =>
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

                                    var mkc = new Game.Marker(game, 14, placemk.GetCode(), mk.Half, mk.TimeVideo) { Player1 = null, Player2 = mk.Player1 };
                                    Log.Write("INSERT FINE " + mkc.Player1);
                                    game.Insert(mkc);
                                    ReloadDataGridView(false);
                                    return;
                                }
                            }
                        }

                        return;
                    }

                    mainForm.Second = mk.TimeVideo - 1500;
                }
            }
        }

        private void DataGridForm_KeyDown(object sender, KeyEventArgs e)
        {
            //mainForm.MainForm_KeyDown(sender, e);

            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedMarkers(sender, EventArgs.Empty);
            }
        }

        private void DataGridForm_Move(object sender, EventArgs e)
        {
            mainForm.LastDataGridRectangle = new Rectangle(
                Left, Top, mainForm.LastDataGridRectangle.Width, mainForm.LastDataGridRectangle.Height);
        }

        private void DataGridForm_SizeChanged(object sender, EventArgs e)
        {
            mainForm.LastDataGridRectangle = new Rectangle(
                mainForm.LastDataGridRectangle.Left, mainForm.LastDataGridRectangle.Top, Width, Height);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReloadDataGridView(true);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void EditPropertiesMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;
                var mke = new Game.Marker(game);
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
                        game.SaveLocal();
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
                && GetDataGrid().SelectedItems[0].Tag is Game.Marker
                && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).ActionId == 14
                && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Player1 != null
                && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Player2 != null)
            {
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;
                Marker mki = new Game.Marker(game);
                mki.Assign(mk);

                Marker mk1 = null;
                if (mk.player2_id > 0 && game.Markers.Any(o =>
                        !o.FlagDel && o.Half.Index == mk.Half.Index && o.TimeVideo < mk.TimeVideo
                        && o.ActionId == 14 && o.player1_id == mk.player2_id))
                {
                    mk1 = game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == mk.Half.Index && o.TimeVideo < mk.TimeVideo
                        && o.ActionId == 14 && o.player1_id == mk.player2_id)
                        .OrderByDescending(o => o.TimeVideo).First();
                }

                Marker mk2 = null;
                if (mk.player1_id > 0 && game.Markers.Any(o =>
                        !o.FlagDel && o.Half.Index == mk.Half.Index && o.TimeVideo > mk.TimeVideo
                        && o.ActionId == 14 && o.player2_id == mk.player1_id))
                {
                    mk2 = game.Markers.Where(o =>
                        !o.FlagDel && o.Half.Index == mk.Half.Index && o.TimeVideo > mk.TimeVideo
                        && o.ActionId == 14 && o.player2_id == mk.player1_id)
                        .OrderBy(o => o.TimeVideo).First();
                }

                var form = new CorrectChangedPlayersMarkerForm(game, mki, mk1, mk2);
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

                    lock (game.Markers)
                        MarkerList.SaveToFile(game.Match.Id, game.Markers);
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
                contextMenuStrip1.Items.Clear();

                contextMenuStrip1.Items.Add(new ToolStripMenuItem("Свойства...", null, EditPropertiesMarker));
                
                Game.Marker mk = null;
                if (dataGridView1.SelectedItems.Count == 1
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
                    mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;

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
                    contextMenuStrip1.Items.Add(new ToolStripMenuItem("Коррекция маркера замены...", null, CorrectMarkerChangedPlayers));

                    if (mk.Num == 1)
                    {
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Завершить эпизод замен", null, FinallySubsEpisode) { Tag = mk });
                    }
                }

                if (mk != null)
                {                    
                    //if (mk.ActionId != 16)
                    {
                        if (contextMenuStrip1.Items.Count > 0)
                            contextMenuStrip1.Items.Add(new ToolStripSeparator());

                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время +1s", null, ShiftTimeMarker) { Tag = +1000 });
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время +0.1s", null, ShiftTimeMarker) { Tag = +100 });
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время -1s", null, ShiftTimeMarker) { Tag = -1000 });
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время -0.1s", null, ShiftTimeMarker) { Tag = -100 });
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());

                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время +2s", null, ShiftTimeMarker) { Tag = +2000 });
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время -2s", null, ShiftTimeMarker) { Tag = -2000 });

                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время +0.5s", null, ShiftTimeMarker) { Tag = +500 });
                        contextMenuStrip1.Items.Add(new ToolStripMenuItem("Время -0.5s", null, ShiftTimeMarker) { Tag = -500 });

                        if (mk.ActionId == 14 && mk.player1_id > 0)
                        {
                            contextMenuStrip1.Items.Add(new ToolStripSeparator());
                            contextMenuStrip1.Items.Add(new ToolStripMenuItem("Выделить все замены игрока", null, SelectSubsMarker) { Tag = mk });
                            contextMenuStrip1.Items.Add(new ToolStripMenuItem("Выделить все замены позиции", null, SelectPosMarker) { Tag = mk });
                        }
                    }
                }

                if (dataGridView1.SelectedItems.Count > 0)
                {
                    if (contextMenuStrip1.Items.Count > 0)
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());

                    contextMenuStrip1.Items.Add(new ToolStripMenuItem("Удалить", null, DeleteSelectedMarkers));
                }

                if (dataGridView1.SelectedItems.Count == 1 
                    && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Exception != null
                    && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Exception.Code == 1)
                {
                    if (contextMenuStrip1.Items.Count > 0)
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());

                    contextMenuStrip1.Items.Add(new ToolStripMenuItem("Вставить подбор ?", null, InsertPickUp));
                }

                if (dataGridView1.SelectedItems.Count == 1
                    && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Exception != null
                    && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Exception.Code == 2)
                {
                    if (contextMenuStrip1.Items.Count > 0)
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());

                    contextMenuStrip1.Items.Add(new ToolStripMenuItem("Заменить на прием ?", null, ChangeToPickUp2));
                }

                if (dataGridView1.SelectedItems.Count == 1
                    && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Exception != null
                    && ((Game.Marker)GetDataGrid().SelectedItems[0].Tag).Exception.Code == 3)
                {
                    if (contextMenuStrip1.Items.Count > 0)
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());

                    contextMenuStrip1.Items.Add(new ToolStripMenuItem("Заменить на подбор ?", null, ChangeToPickUp));
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

                lock (game.Markers)
                {
                    var ami = game.Markers.Where(o => o.Sync == 1 
                        && !o.FlagDel 
                        && o.Half.Index == game.Half.Index 
                        && o.TimeVideo >= mk.TimeVideo 
                        && !o.Compare(new int[] { 16, 18 })).ToList<Marker>();

                    var time_offset = game.Time - mk.TimeVideo;
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
                var sibl0 = game.GetSiblings(mk);

                var err_msg = String.Empty;

                foreach (Game.Marker mk0 in sibl0)
                {
                    Marker mk2 = null;
                    lock (game.Markers)
                        mk2 = game.Markers.Where(o => !o.FlagDel && o.Half.Index == mk0.Half.Index && o.TimeVideo > mk0.TimeVideo && o.Compare(14, mk0.ActionType)).OrderBy(o => o.TimeVideo).FirstOrDefault();
                    Marker mk1 = null;
                    lock (game.Markers)
                        mk1 = game.Markers.Where(o => !o.FlagDel && o.Half.Index == mk0.Half.Index && o.TimeVideo < mk0.TimeVideo && o.Compare(14, mk0.ActionType)).OrderByDescending(o => o.TimeVideo).FirstOrDefault();

                    if (mk1 != null && mk2 != null)
                    {
                        mk2.Player2 = mk1.Player1;
                        if (mk2.player2_id == mk2.player1_id)
                            game.Remove(mk0);
                        else
                            mk2.FlagUpdate = true;

                        game.Remove(mk0);

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
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;

                List<Marker> ami = null;
                lock (game.Markers)
                    ami = game.Markers.Where(o => !o.FlagDel && o.ActionId == 14 && o.Half == mk.Half 
                        && (o.player1_id == mk.player1_id || o.player2_id == mk.player1_id)).ToList<Marker>();

                foreach (Game.Marker mki in ami)
                    mki.row.Selected = true;
            }
        }

        private void SelectPosMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;

                List<Marker> ami = null;
                lock (game.Markers)
                    ami = game.Markers.Where(o => !o.FlagDel && o.ActionId == 14 && o.ActionType == mk.ActionType && o.Half == mk.Half
                        && (o.team1_id == mk.team1_id || o.team2_id == mk.team1_id)).ToList<Marker>();

                foreach (Game.Marker mki in ami)
                    mki.row.Selected = true;
            }
        }

        private void ChangeToPickUp(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;
                mk.ActionId = 2;
                mk.ActionType = 5;
                mk.FlagUpdate = true;

                lock (game.Markers)
                {
                    MarkerList.SaveToFile(game.Match.Id, game.Markers);
                }

                UpdateGridView((Game.Marker)mk);
            }
        }

        private void ChangeToPickUp2(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;
                mk.ActionId = 2;
                mk.ActionType = 4;
                mk.FlagUpdate = true;

                lock (game.Markers)
                {
                    MarkerList.SaveToFile(game.Match.Id, game.Markers);
                }

                UpdateGridView((Game.Marker)mk);
            }
        }

        private void InsertPickUp(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;
                game.Insert(new Game.Marker(game, 2, 5, mk.Half, mk.TimeVideo) { Player1 = mk.Player1 });

                lock (game.Markers)
                {
                    MarkerList.SaveToFile(game.Match.Id, game.Markers);
                }

                ReloadDataGridView(true);
            }
        }

        private void ShiftTimeMarker(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1
                    && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mi = (ToolStripMenuItem)sender;
                var shift = Convert.ToInt32(mi.Tag);

                foreach (ListViewItem row in GetDataGrid().SelectedItems)
                {
                    var mki = (Game.Marker)row.Tag;
                    Log.Write("MANUAL SHIFT TIME BEFORE " + mki.ToString());
                    mki.TimeVideo += shift;
                    mki.FlagUpdate = true;
                    mki.row = null;
                    Log.Write("MANUAL SHIFT TIME AFTER " + mki.ToString());
                }

                /*Marker mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;
                List<Marker> sibl = new List<Marker>();
                if (mk.ActionId == 14)
                    sibl.Add(mk);
                else
                    sibl = game.GetSiblings(mk);

                int shift = Convert.ToInt32(mi.Tag);

                foreach (Game.Marker mki in sibl)
                {
                    Log.Write("MANUAL SHIFT TIME BEFORE " + mki.ToString());                                       
                    mki.TimeVideo += shift;
                    mki.FlagUpdate = true;
                    mki.row = null;
                    Log.Write("MANUAL SHIFT TIME AFTER " + mk.ToString());
                }*/

                lock (game.Markers)
                {
                    MarkerList.SaveToFile(game.Match.Id, game.Markers);
                }

                lock (game.Markers)
                    game.RecalcActualTime(game.Markers, game.Half);

                ReloadDataGridView(true);
            }
        }

        private void dataGridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1 && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mk = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;

                linkLabel2.Visible = mk.TimeVideo == -1;

                try
                {
                    game.CheckValid(mk);
                    label1.Visible = false;
                }
                catch (HockeyIce.CheckValidMarkerException ex)
                {
                    switch (ex.CheckValidLevel)
                    {
                        case HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.WARNING:
                            label1.ForeColor = Color.Black;
                            label1.BackColor = Color.Yellow;
                            break;

                        case HockeyIce.CheckValidMarkerException.CheckValidLevelEnum.CRITICAL:
                            label1.ForeColor = Color.White;
                            label1.BackColor = Color.Red;
                            break;
                    }

                    label1.Text = ex.Message;
                    label1.Visible = true;
                }
            }
            else
                linkLabel2.Visible = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            hide_subs_markers = checkBox1.Checked;
            ReloadDataGridView(true);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            only_current_time = checkBox2.Checked;
            ReloadDataGridView(true);
        }

        private Marker lastmk = null;

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    var list = new List<Marker>();
                    lock (game.Markers)
                    {
                        list.AddRange(game.Markers);
                    }

                    var update = false;
                    var ami = list.Where(o => !o.FlagDel && !o.Compare(16)).OrderBy(o => o.Half.Index).ThenBy(o => o.TimeVideo).ToList<Marker>();
                    for (var i = 0; i < ami.Count; i++)
                    {
                        var mk = (Game.Marker)ami[i];

                        update = false;
                        try
                        {
                            game.CheckValid(mk);
                            if (mk.Exception != null)
                            {
                                mk.FlagGuiUpdate = true;
                                update = true;
                            }

                            mk.Exception = null;

                            if (update)
                                UpdateGridView(mk);
                        }
                        catch (Uniso.InStat.Game.HockeyIce.CheckValidMarkerException ex)
                        {
                            if (mk.Exception == null || (mk.Exception != null && !mk.Exception.Message.Equals(ex.Message)))
                            {
                                mk.FlagGuiUpdate = true;
                                update = true;
                            }

                            mk.Exception = ex;

                            if (update)
                                UpdateGridView(mk);
                        }
                    }

                    if (only_current_time && game != null && game.Half != null)
                    {
                        var prev = game.GetPrevousMarkersHalf(game.Half, game.Time, true);
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
                    sync.Execute(() => timer1.Start());
                }
            });
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBox3.Checked;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (dataGridView1.SelectedItems.Count == 1 && GetDataGrid().SelectedItems[0].Tag is Game.Marker)
            {
                var mk1 = (Game.Marker)GetDataGrid().SelectedItems[0].Tag;

                if (mk1.Half.Index != game.Half.Index)
                {
                    MessageBox.Show("Этот маркер не относится к текущему периоду", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (mk1.Compare(1, 1))
                {
                    Marker mk2 = null;
                    lock (game.Markers)
                        mk2 = game.Markers
                            .Where(o => !o.FlagDel && o.Half.Index == mk1.Half.Index && o.TimeActual > mk1.TimeActual && game.IsStopTimeMarker(o))
                            .OrderBy(o => o.TimeActual)
                            .FirstOrDefault();

                    List<Marker> ami = null;
                    lock (game.Markers)
                        ami = game.Markers.Where(o => !o.FlagDel && o.TimeVideo == -1 && o.Half.Index == mk1.Half.Index && o.TimeActual >= mk1.TimeActual && (mk2 == null || (mk2 != null && o.TimeActual <= mk2.TimeActual)))
                            .OrderBy(o => o.TimeActual)
                            .ToList<Marker>();

                    var time0 = mk1.TimeActualSrv;

                    foreach (var mk in ami)
                    {
                        if (mk.Compare(1, 1) && mk != mk1)
                            continue;

                        if (mk.ActionId == 16)
                            continue;

                        mk.TimeVideo = game.Time + (mk.TimeActual - time0);
                        mk.FlagUpdate = true;
                    }

                    lock (game.Markers)
                        game.RecalcActualTime(game.Markers, mk1.Half);
                    ReloadDataGridView(true);
                }
                else
                {
                    mk1.TimeVideo = game.Time;
                    mk1.FlagUpdate = true;

                    lock (game.Markers)
                        game.RecalcActualTime(game.Markers, mk1.Half);

                    ReloadDataGridView(true);

                    if (mk1.Compare(8, 1) && mk1.Point1.IsEmpty)
                    {
                        mainForm.ProcessingMarker(mk1);
                    }
                }
            }
        }
    }
}
