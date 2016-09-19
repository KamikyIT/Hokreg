using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Uniso.InStat.Server;

namespace Uniso.InStat.Gui.Forms
{
    public partial class Login : Form
    {
        private List<User> usersList = new List<User>();
        private Uniso.InStat.Gui.UISync sync = null;
        private User selectedUser = null;
        private String enterPassw = String.Empty;

        public User User { get; set; }

        public Login()
        {
            InitializeComponent();
        }

        private void UpdateUI()
        {
            sync.Execute(new Action(() =>
                {
                    button1.Enabled = !String.IsNullOrEmpty(enterPassw) && selectedUser != null && !auth_proc && !loading_users;
                    radioButton1.Enabled = false;// !auth_proc && !loading_users;
                    radioButton2.Enabled = false;// !auth_proc && !loading_users;
                    textBox2.Enabled = !auth_proc && !loading_users;
                    comboBox1.Enabled = !auth_proc && !loading_users;
                }));
        }

        public void FillUserList()
        {
            sync.Execute(() =>
            {
                comboBox1.Items.Clear();
                comboBox1.Items.Add("-ВЫБЕРИТЕ ПОЛЬЗОВАТЕЛЯ-");
                comboBox1.SelectedIndex = 0;
                if (usersList != null)
                {
                    foreach (var u in usersList)
                    {
                        comboBox1.Items.Add(u);
                    }

                    var lastUser = User.Load();

                    if (lastUser != null)
                    {
                        var ami = usersList.Where(o => o.Id == lastUser.Id).ToList<User>();
                        if (ami.Count > 0)
                        {
                            comboBox1.SelectedItem = ami[0];
                        }
                    }
                }
            });
        }

        public void ShowStatus(String text, int code)
        {
            sync.Execute(() =>
            {
                if (text.Length > 50)
                    text = text.Substring(0, 50);
                toolStripStatusLabel1.Text = text;

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
            });
        }

        public void ClearPassword()
        {
            sync.Execute(() =>
            {
                textBox2.Text = String.Empty;
            });
        }

        public DialogResult ShowDialog(List<User> users, User lastUser)
        {
            comboBox1.Items.AddRange(users.ToArray());
            if (lastUser != null)
            { 
                var ami = users.Where(o => o.Id == lastUser.Id).ToList<User>();
                if (ami.Count == 1)
                    comboBox1.SelectedItem = ami[0];
                else
                    comboBox1.SelectedIndex = 0;
            }
            else
                comboBox1.SelectedIndex = 0;

            return ShowDialog();
        }

        public User Nickname
        {
            get 
            {
                if (comboBox1.SelectedItem is User)
                    return (User)comboBox1.SelectedItem;

                return null;
            }
        }

        private void Login_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    var t = new Thread(DoLoginUser);
                    t.IsBackground = true;
                    t.Start();
                    break;

                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    break;
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            sync = new UISync(this);

            var t = new Thread(DoLoadUsers);
            t.IsBackground = true;
            t.Start();

            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            label2.Text = v.ToString();
            Uniso.Log.Write("VERSION: " + v.ToString());
        }

        private bool loading_users = false;

        private void DoLoadUsers()
        {
            ShowStatus("Loading users...", 0);
            loading_users = true;
            UpdateUI();

            try
            {
                usersList = Web.GetUserList();
                FillUserList();
                ShowStatus("OK", 0);
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message, 1);
            }
            finally
            {                
                loading_users = false;
                UpdateUI();
            }
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (!rb.Checked)
                return;

            UpdateUI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is User)
            {
                selectedUser = (User)comboBox1.SelectedItem;
                textBox2.Focus();
                textBox2.Text = String.Empty;
            }
            else
                selectedUser = null;

            UpdateUI();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            enterPassw = textBox2.Text;
            UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var t = new Thread(DoLoginUser);
            t.IsBackground = true;
            t.Start();
        }

        private bool auth_proc = false;

        private void DoLoginUser()
        {
            try
            {
                auth_proc = true;
                UpdateUI();

                if (selectedUser == null)
                    return;

                ShowStatus("Authorization...", 0);
                Thread.Sleep(1000);

                var res = Web.Login(selectedUser, enterPassw);

                ShowStatus(String.Empty, 0);

                if (!res)
                    throw new Exception("ACCESS DENIED");

                User = selectedUser;
                User.Save();

                sync.Execute(() => Close());
            }
            catch (Exception ex)
            {
                ClearPassword();
                ShowStatus(ex.Message, 1);
                Thread.Sleep(1000);
            }
            finally
            {
                ShowStatus(String.Empty, 0);
                auth_proc = false;
                UpdateUI();
            }
        }
    }
}
