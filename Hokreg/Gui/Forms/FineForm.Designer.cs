namespace Uniso.InStat.Gui.Forms
{
    partial class FineForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FineForm));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton15 = new System.Windows.Forms.RadioButton();
            this.radioButton16 = new System.Windows.Forms.RadioButton();
            this.radioButton17 = new System.Windows.Forms.RadioButton();
            this.radioButton20 = new System.Windows.Forms.RadioButton();
            this.radioButton24 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton26 = new System.Windows.Forms.RadioButton();
            this.radioButton28 = new System.Windows.Forms.RadioButton();
            this.radioButton29 = new System.Windows.Forms.RadioButton();
            this.radioButton30 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton12 = new System.Windows.Forms.RadioButton();
            this.radioButton14 = new System.Windows.Forms.RadioButton();
            this.radioButton13 = new System.Windows.Forms.RadioButton();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.radioButton8 = new System.Windows.Forms.RadioButton();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton15);
            this.groupBox2.Controls.Add(this.radioButton16);
            this.groupBox2.Controls.Add(this.radioButton17);
            this.groupBox2.Controls.Add(this.radioButton20);
            this.groupBox2.Controls.Add(this.radioButton24);
            this.groupBox2.Location = new System.Drawing.Point(386, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(330, 152);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Дисциплинарные";
            // 
            // radioButton15
            // 
            this.radioButton15.AutoSize = true;
            this.radioButton15.Location = new System.Drawing.Point(22, 73);
            this.radioButton15.Name = "radioButton15";
            this.radioButton15.Size = new System.Drawing.Size(265, 17);
            this.radioButton15.TabIndex = 0;
            this.radioButton15.Tag = "7";
            this.radioButton15.Text = "ДИСЦИПЛИНАРНЫЙ ДО КОНЦА ИГРЫ (ДКИ)";
            this.radioButton15.UseVisualStyleBackColor = true;
            this.radioButton15.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton16
            // 
            this.radioButton16.AutoSize = true;
            this.radioButton16.Location = new System.Drawing.Point(22, 50);
            this.radioButton16.Name = "radioButton16";
            this.radioButton16.Size = new System.Drawing.Size(205, 17);
            this.radioButton16.TabIndex = 0;
            this.radioButton16.Tag = "6";
            this.radioButton16.Text = "ДИСЦИПЛИНАРНЫЙ ШТРАФ (10`)";
            this.radioButton16.UseVisualStyleBackColor = true;
            this.radioButton16.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton17
            // 
            this.radioButton17.AutoSize = true;
            this.radioButton17.Location = new System.Drawing.Point(22, 119);
            this.radioButton17.Name = "radioButton17";
            this.radioButton17.Size = new System.Drawing.Size(162, 17);
            this.radioButton17.TabIndex = 0;
            this.radioButton17.Tag = "9";
            this.radioButton17.Text = "ШТРАФНОЙ БРОСОК (ШБ)";
            this.radioButton17.UseVisualStyleBackColor = true;
            this.radioButton17.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton20
            // 
            this.radioButton20.AutoSize = true;
            this.radioButton20.Location = new System.Drawing.Point(22, 96);
            this.radioButton20.Name = "radioButton20";
            this.radioButton20.Size = new System.Drawing.Size(127, 17);
            this.radioButton20.TabIndex = 0;
            this.radioButton20.Tag = "8";
            this.radioButton20.Text = "МАТЧ-ШТРАФ (МШ)";
            this.radioButton20.UseVisualStyleBackColor = true;
            this.radioButton20.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton24
            // 
            this.radioButton24.AutoSize = true;
            this.radioButton24.Checked = true;
            this.radioButton24.Location = new System.Drawing.Point(22, 26);
            this.radioButton24.Name = "radioButton24";
            this.radioButton24.Size = new System.Drawing.Size(178, 17);
            this.radioButton24.TabIndex = 0;
            this.radioButton24.TabStop = true;
            this.radioButton24.Tag = "0";
            this.radioButton24.Text = "Без дисциплиновного штрафа";
            this.radioButton24.UseVisualStyleBackColor = true;
            this.radioButton24.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(601, 360);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 45);
            this.button1.TabIndex = 1;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton26);
            this.groupBox3.Controls.Add(this.radioButton28);
            this.groupBox3.Controls.Add(this.radioButton29);
            this.groupBox3.Controls.Add(this.radioButton30);
            this.groupBox3.Location = new System.Drawing.Point(386, 46);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(330, 132);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Удаление";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // radioButton26
            // 
            this.radioButton26.AutoSize = true;
            this.radioButton26.Location = new System.Drawing.Point(22, 97);
            this.radioButton26.Name = "radioButton26";
            this.radioButton26.Size = new System.Drawing.Size(142, 17);
            this.radioButton26.TabIndex = 0;
            this.radioButton26.Tag = "5";
            this.radioButton26.Text = "БОЛЬШОЙ ШТРАФ (5`)";
            this.radioButton26.UseVisualStyleBackColor = true;
            this.radioButton26.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // radioButton28
            // 
            this.radioButton28.AutoSize = true;
            this.radioButton28.Location = new System.Drawing.Point(22, 50);
            this.radioButton28.Name = "radioButton28";
            this.radioButton28.Size = new System.Drawing.Size(129, 17);
            this.radioButton28.TabIndex = 0;
            this.radioButton28.Tag = "2";
            this.radioButton28.Text = "МАЛЫЙ ШТРАФ (2`)";
            this.radioButton28.UseVisualStyleBackColor = true;
            this.radioButton28.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // radioButton29
            // 
            this.radioButton29.AutoSize = true;
            this.radioButton29.Location = new System.Drawing.Point(22, 74);
            this.radioButton29.Name = "radioButton29";
            this.radioButton29.Size = new System.Drawing.Size(188, 17);
            this.radioButton29.TabIndex = 0;
            this.radioButton29.Tag = "4";
            this.radioButton29.Text = "ДВОЙНОЙ МАЛЫЙ ШТРАФ (4`)";
            this.radioButton29.UseVisualStyleBackColor = true;
            this.radioButton29.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // radioButton30
            // 
            this.radioButton30.AutoSize = true;
            this.radioButton30.Checked = true;
            this.radioButton30.Location = new System.Drawing.Point(22, 27);
            this.radioButton30.Name = "radioButton30";
            this.radioButton30.Size = new System.Drawing.Size(94, 17);
            this.radioButton30.TabIndex = 0;
            this.radioButton30.TabStop = true;
            this.radioButton30.Tag = "0";
            this.radioButton30.Text = "Без удаления";
            this.radioButton30.UseVisualStyleBackColor = true;
            this.radioButton30.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ControlText;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(345, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Location = new System.Drawing.Point(369, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 400);
            this.panel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton12);
            this.groupBox1.Controls.Add(this.radioButton14);
            this.groupBox1.Controls.Add(this.radioButton13);
            this.groupBox1.Controls.Add(this.radioButton11);
            this.groupBox1.Controls.Add(this.radioButton8);
            this.groupBox1.Controls.Add(this.radioButton7);
            this.groupBox1.Controls.Add(this.radioButton10);
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Controls.Add(this.radioButton6);
            this.groupBox1.Controls.Add(this.radioButton9);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton5);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(12, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 365);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Нарушение";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // radioButton12
            // 
            this.radioButton12.AutoSize = true;
            this.radioButton12.Location = new System.Drawing.Point(22, 334);
            this.radioButton12.Name = "radioButton12";
            this.radioButton12.Size = new System.Drawing.Size(62, 17);
            this.radioButton12.TabIndex = 0;
            this.radioButton12.Tag = "1";
            this.radioButton12.Text = "Прочее";
            this.radioButton12.UseVisualStyleBackColor = true;
            this.radioButton12.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton14
            // 
            this.radioButton14.AutoSize = true;
            this.radioButton14.Location = new System.Drawing.Point(22, 311);
            this.radioButton14.Name = "radioButton14";
            this.radioButton14.Size = new System.Drawing.Size(101, 17);
            this.radioButton14.TabIndex = 0;
            this.radioButton14.Tag = "14";
            this.radioButton14.Text = "Выброс шайбы";
            this.radioButton14.UseVisualStyleBackColor = true;
            this.radioButton14.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton13
            // 
            this.radioButton13.AutoSize = true;
            this.radioButton13.Location = new System.Drawing.Point(22, 288);
            this.radioButton13.Name = "radioButton13";
            this.radioButton13.Size = new System.Drawing.Size(58, 17);
            this.radioButton13.TabIndex = 0;
            this.radioButton13.Tag = "13";
            this.radioButton13.Text = "Драка";
            this.radioButton13.UseVisualStyleBackColor = true;
            this.radioButton13.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton11
            // 
            this.radioButton11.AutoSize = true;
            this.radioButton11.Location = new System.Drawing.Point(22, 265);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(155, 17);
            this.radioButton11.TabIndex = 0;
            this.radioButton11.Tag = "12";
            this.radioButton11.Text = "Неспортивное поведение";
            this.radioButton11.UseVisualStyleBackColor = true;
            this.radioButton11.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton8
            // 
            this.radioButton8.AutoSize = true;
            this.radioButton8.Location = new System.Drawing.Point(22, 196);
            this.radioButton8.Name = "radioButton8";
            this.radioButton8.Size = new System.Drawing.Size(228, 17);
            this.radioButton8.TabIndex = 0;
            this.radioButton8.Tag = "9";
            this.radioButton8.Text = "Удар (клюшкой, локтем, ногой головой)";
            this.radioButton8.UseVisualStyleBackColor = true;
            this.radioButton8.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton7
            // 
            this.radioButton7.AutoSize = true;
            this.radioButton7.Location = new System.Drawing.Point(22, 172);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(187, 17);
            this.radioButton7.TabIndex = 0;
            this.radioButton7.Tag = "8";
            this.radioButton7.Text = "Нарушение численного состава";
            this.radioButton7.UseVisualStyleBackColor = true;
            this.radioButton7.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton10
            // 
            this.radioButton10.AutoSize = true;
            this.radioButton10.Location = new System.Drawing.Point(22, 242);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(87, 17);
            this.radioButton10.TabIndex = 0;
            this.radioButton10.Tag = "11";
            this.radioButton10.Text = "Сдвиг ворот";
            this.radioButton10.UseVisualStyleBackColor = true;
            this.radioButton10.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(22, 103);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(190, 17);
            this.radioButton4.TabIndex = 0;
            this.radioButton4.Tag = "5";
            this.radioButton4.Text = "Игра высоко поднятой клюшкой";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(22, 149);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(290, 17);
            this.radioButton6.TabIndex = 0;
            this.radioButton6.Tag = "7";
            this.radioButton6.Text = "Атака Игрока, не владеющего шайбой (блокировка)";
            this.radioButton6.UseVisualStyleBackColor = true;
            this.radioButton6.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton9
            // 
            this.radioButton9.AutoSize = true;
            this.radioButton9.Location = new System.Drawing.Point(22, 219);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(77, 17);
            this.radioButton9.TabIndex = 0;
            this.radioButton9.Tag = "10";
            this.radioButton9.Text = "Подножка";
            this.radioButton9.UseVisualStyleBackColor = true;
            this.radioButton9.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(22, 80);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(71, 17);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.Tag = "4";
            this.radioButton3.Text = "Грубость";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(22, 126);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(167, 17);
            this.radioButton5.TabIndex = 0;
            this.radioButton5.Tag = "6";
            this.radioButton5.Text = "Задержка руками/клюшкой";
            this.radioButton5.UseVisualStyleBackColor = true;
            this.radioButton5.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(22, 57);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(131, 17);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.Tag = "3";
            this.radioButton2.Text = "Неправильная атака";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(22, 34);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(159, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "2";
            this.radioButton1.Text = "Толчок соперника на борт";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton13_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(381, 10);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(335, 21);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(399, 360);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(103, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Обоюдный фол";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(399, 383);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(95, 17);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "Без удаления";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // FineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 423);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FineForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Штраф";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton15;
        private System.Windows.Forms.RadioButton radioButton16;
        private System.Windows.Forms.RadioButton radioButton17;
        private System.Windows.Forms.RadioButton radioButton20;
        private System.Windows.Forms.RadioButton radioButton24;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton26;
        private System.Windows.Forms.RadioButton radioButton28;
        private System.Windows.Forms.RadioButton radioButton29;
        private System.Windows.Forms.RadioButton radioButton30;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton12;
        private System.Windows.Forms.RadioButton radioButton11;
        private System.Windows.Forms.RadioButton radioButton8;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton radioButton10;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.RadioButton radioButton9;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.RadioButton radioButton13;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.RadioButton radioButton14;
    }
}