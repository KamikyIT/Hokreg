namespace Uniso.InStat.Gui.Forms
{
    partial class MyViolationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyViolationForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb0P = new System.Windows.Forms.RadioButton();
            this.rb1P = new System.Windows.Forms.RadioButton();
            this.rb2P = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rb1 = new System.Windows.Forms.RadioButton();
            this.rb14 = new System.Windows.Forms.RadioButton();
            this.rb13 = new System.Windows.Forms.RadioButton();
            this.rb12 = new System.Windows.Forms.RadioButton();
            this.rb9 = new System.Windows.Forms.RadioButton();
            this.rb8 = new System.Windows.Forms.RadioButton();
            this.rb11 = new System.Windows.Forms.RadioButton();
            this.rb5 = new System.Windows.Forms.RadioButton();
            this.rb7 = new System.Windows.Forms.RadioButton();
            this.rb10 = new System.Windows.Forms.RadioButton();
            this.rb4 = new System.Windows.Forms.RadioButton();
            this.rb6 = new System.Windows.Forms.RadioButton();
            this.rb3 = new System.Windows.Forms.RadioButton();
            this.rb2 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox1.Controls.Add(this.rb0P);
            this.groupBox1.Controls.Add(this.rb1P);
            this.groupBox1.Controls.Add(this.rb2P);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(648, 62);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Тип Фола";
            // 
            // rb0P
            // 
            this.rb0P.AutoSize = true;
            this.rb0P.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.rb0P.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb0P.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rb0P.Location = new System.Drawing.Point(343, 27);
            this.rb0P.Name = "rb0P";
            this.rb0P.Size = new System.Drawing.Size(141, 24);
            this.rb0P.TabIndex = 5;
            this.rb0P.TabStop = true;
            this.rb0P.Tag = "0";
            this.rb0P.Text = "Командный фол";
            this.rb0P.UseVisualStyleBackColor = false;
            this.rb0P.CheckedChanged += new System.EventHandler(this.radioButtonFoulPlayersCount_CheckedChanged);
            // 
            // rb1P
            // 
            this.rb1P.AutoSize = true;
            this.rb1P.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.rb1P.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb1P.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rb1P.Location = new System.Drawing.Point(162, 27);
            this.rb1P.Name = "rb1P";
            this.rb1P.Size = new System.Drawing.Size(79, 24);
            this.rb1P.TabIndex = 4;
            this.rb1P.TabStop = true;
            this.rb1P.Tag = "1";
            this.rb1P.Text = "1 игрок";
            this.rb1P.UseVisualStyleBackColor = false;
            this.rb1P.CheckedChanged += new System.EventHandler(this.radioButtonFoulPlayersCount_CheckedChanged);
            // 
            // rb2P
            // 
            this.rb2P.AutoSize = true;
            this.rb2P.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.rb2P.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb2P.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rb2P.Location = new System.Drawing.Point(6, 26);
            this.rb2P.Name = "rb2P";
            this.rb2P.Size = new System.Drawing.Size(87, 24);
            this.rb2P.TabIndex = 3;
            this.rb2P.TabStop = true;
            this.rb2P.Tag = "2";
            this.rb2P.Text = "2 игрока";
            this.rb2P.UseVisualStyleBackColor = false;
            this.rb2P.CheckedChanged += new System.EventHandler(this.radioButtonFoulPlayersCount_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox2.Controls.Add(this.rb1);
            this.groupBox2.Controls.Add(this.rb14);
            this.groupBox2.Controls.Add(this.rb13);
            this.groupBox2.Controls.Add(this.rb12);
            this.groupBox2.Controls.Add(this.rb9);
            this.groupBox2.Controls.Add(this.rb8);
            this.groupBox2.Controls.Add(this.rb11);
            this.groupBox2.Controls.Add(this.rb5);
            this.groupBox2.Controls.Add(this.rb7);
            this.groupBox2.Controls.Add(this.rb10);
            this.groupBox2.Controls.Add(this.rb4);
            this.groupBox2.Controls.Add(this.rb6);
            this.groupBox2.Controls.Add(this.rb3);
            this.groupBox2.Controls.Add(this.rb2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(12, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(648, 259);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Нарушение";
            // 
            // rb1
            // 
            this.rb1.AutoSize = true;
            this.rb1.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb1.Location = new System.Drawing.Point(338, 214);
            this.rb1.Name = "rb1";
            this.rb1.Size = new System.Drawing.Size(79, 24);
            this.rb1.TabIndex = 0;
            this.rb1.Tag = "1";
            this.rb1.Text = "Прочее";
            this.rb1.UseVisualStyleBackColor = true;
            this.rb1.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb14
            // 
            this.rb14.AutoSize = true;
            this.rb14.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb14.Location = new System.Drawing.Point(338, 184);
            this.rb14.Name = "rb14";
            this.rb14.Size = new System.Drawing.Size(133, 24);
            this.rb14.TabIndex = 0;
            this.rb14.Tag = "14";
            this.rb14.Text = "Выброс шайбы";
            this.rb14.UseVisualStyleBackColor = true;
            this.rb14.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb13
            // 
            this.rb13.AutoSize = true;
            this.rb13.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb13.Location = new System.Drawing.Point(338, 154);
            this.rb13.Name = "rb13";
            this.rb13.Size = new System.Drawing.Size(71, 24);
            this.rb13.TabIndex = 0;
            this.rb13.Tag = "13";
            this.rb13.Text = "Драка";
            this.rb13.UseVisualStyleBackColor = true;
            this.rb13.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb12
            // 
            this.rb12.AutoSize = true;
            this.rb12.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb12.Location = new System.Drawing.Point(338, 124);
            this.rb12.Name = "rb12";
            this.rb12.Size = new System.Drawing.Size(205, 24);
            this.rb12.TabIndex = 0;
            this.rb12.Tag = "12";
            this.rb12.Text = "Неспортивное поведение";
            this.rb12.UseVisualStyleBackColor = true;
            this.rb12.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb9
            // 
            this.rb9.AutoSize = true;
            this.rb9.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb9.Location = new System.Drawing.Point(338, 34);
            this.rb9.Name = "rb9";
            this.rb9.Size = new System.Drawing.Size(308, 24);
            this.rb9.TabIndex = 0;
            this.rb9.Tag = "9";
            this.rb9.Text = "Удар (клюшкой, локтем, ногой головой)";
            this.rb9.UseVisualStyleBackColor = true;
            this.rb9.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb8
            // 
            this.rb8.AutoSize = true;
            this.rb8.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb8.Location = new System.Drawing.Point(22, 214);
            this.rb8.Name = "rb8";
            this.rb8.Size = new System.Drawing.Size(249, 24);
            this.rb8.TabIndex = 0;
            this.rb8.Tag = "8";
            this.rb8.Text = "Нарушение численного состава";
            this.rb8.UseVisualStyleBackColor = true;
            this.rb8.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb11
            // 
            this.rb11.AutoSize = true;
            this.rb11.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb11.Location = new System.Drawing.Point(338, 94);
            this.rb11.Name = "rb11";
            this.rb11.Size = new System.Drawing.Size(114, 24);
            this.rb11.TabIndex = 0;
            this.rb11.Tag = "11";
            this.rb11.Text = "Сдвиг ворот";
            this.rb11.UseVisualStyleBackColor = true;
            this.rb11.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb5
            // 
            this.rb5.AutoSize = true;
            this.rb5.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb5.Location = new System.Drawing.Point(22, 124);
            this.rb5.Name = "rb5";
            this.rb5.Size = new System.Drawing.Size(255, 24);
            this.rb5.TabIndex = 0;
            this.rb5.Tag = "5";
            this.rb5.Text = "Игра высоко поднятой клюшкой";
            this.rb5.UseVisualStyleBackColor = true;
            this.rb5.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb7
            // 
            this.rb7.AutoSize = true;
            this.rb7.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb7.Location = new System.Drawing.Point(22, 184);
            this.rb7.Name = "rb7";
            this.rb7.Size = new System.Drawing.Size(292, 24);
            this.rb7.TabIndex = 0;
            this.rb7.Tag = "7";
            this.rb7.Text = "Атака Игрока, не владеющего шайбой";
            this.rb7.UseVisualStyleBackColor = true;
            this.rb7.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb10
            // 
            this.rb10.AutoSize = true;
            this.rb10.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb10.Location = new System.Drawing.Point(338, 64);
            this.rb10.Name = "rb10";
            this.rb10.Size = new System.Drawing.Size(100, 24);
            this.rb10.TabIndex = 0;
            this.rb10.Tag = "10";
            this.rb10.Text = "Подножка";
            this.rb10.UseVisualStyleBackColor = true;
            this.rb10.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb4
            // 
            this.rb4.AutoSize = true;
            this.rb4.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb4.Location = new System.Drawing.Point(22, 94);
            this.rb4.Name = "rb4";
            this.rb4.Size = new System.Drawing.Size(92, 24);
            this.rb4.TabIndex = 0;
            this.rb4.Tag = "4";
            this.rb4.Text = "Грубость";
            this.rb4.UseVisualStyleBackColor = true;
            this.rb4.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb6
            // 
            this.rb6.AutoSize = true;
            this.rb6.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb6.Location = new System.Drawing.Point(22, 154);
            this.rb6.Name = "rb6";
            this.rb6.Size = new System.Drawing.Size(221, 24);
            this.rb6.TabIndex = 0;
            this.rb6.Tag = "6";
            this.rb6.Text = "Задержка руками/клюшкой";
            this.rb6.UseVisualStyleBackColor = true;
            this.rb6.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb3
            // 
            this.rb3.AutoSize = true;
            this.rb3.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb3.Location = new System.Drawing.Point(22, 64);
            this.rb3.Name = "rb3";
            this.rb3.Size = new System.Drawing.Size(170, 24);
            this.rb3.TabIndex = 0;
            this.rb3.Tag = "3";
            this.rb3.Text = "Неправильная атака";
            this.rb3.UseVisualStyleBackColor = true;
            this.rb3.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // rb2
            // 
            this.rb2.AutoSize = true;
            this.rb2.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.rb2.Location = new System.Drawing.Point(22, 34);
            this.rb2.Name = "rb2";
            this.rb2.Size = new System.Drawing.Size(213, 24);
            this.rb2.TabIndex = 0;
            this.rb2.Tag = "2";
            this.rb2.Text = "Толчок соперника на борт";
            this.rb2.UseVisualStyleBackColor = true;
            this.rb2.CheckedChanged += new System.EventHandler(this.radioButtonFoulType_CheckedChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(560, 342);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 36);
            this.button1.TabIndex = 7;
            this.button1.Text = "O K";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F);
            this.checkBox1.Location = new System.Drawing.Point(415, 347);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(140, 24);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Обоюдный фол";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // MyViolationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(672, 382);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MyViolationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Причина фола";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rb0P;
        private System.Windows.Forms.RadioButton rb1P;
        private System.Windows.Forms.RadioButton rb2P;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rb1;
        private System.Windows.Forms.RadioButton rb14;
        private System.Windows.Forms.RadioButton rb13;
        private System.Windows.Forms.RadioButton rb12;
        private System.Windows.Forms.RadioButton rb9;
        private System.Windows.Forms.RadioButton rb8;
        private System.Windows.Forms.RadioButton rb11;
        private System.Windows.Forms.RadioButton rb5;
        private System.Windows.Forms.RadioButton rb7;
        private System.Windows.Forms.RadioButton rb10;
        private System.Windows.Forms.RadioButton rb4;
        private System.Windows.Forms.RadioButton rb6;
        private System.Windows.Forms.RadioButton rb3;
        private System.Windows.Forms.RadioButton rb2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}