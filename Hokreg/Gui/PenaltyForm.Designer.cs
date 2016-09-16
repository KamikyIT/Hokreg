namespace Uniso.InStat
{
    partial class PenaltyForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(PenaltyForm));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton26 = new System.Windows.Forms.RadioButton();
            this.radioButton28 = new System.Windows.Forms.RadioButton();
            this.radioButton29 = new System.Windows.Forms.RadioButton();
            this.radioButton30 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton15 = new System.Windows.Forms.RadioButton();
            this.radioButton16 = new System.Windows.Forms.RadioButton();
            this.radioButton17 = new System.Windows.Forms.RadioButton();
            this.radioButton20 = new System.Windows.Forms.RadioButton();
            this.radioButton24 = new System.Windows.Forms.RadioButton();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(18, 51);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(288, 28);
            this.comboBox1.TabIndex = 9;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(18, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(607, 23);
            this.label1.TabIndex = 8;
            this.label1.Text = "label1";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(512, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 45);
            this.button1.TabIndex = 7;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton26);
            this.groupBox3.Controls.Add(this.radioButton28);
            this.groupBox3.Controls.Add(this.radioButton29);
            this.groupBox3.Controls.Add(this.radioButton30);
            this.groupBox3.Location = new System.Drawing.Point(18, 103);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 177);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "УДАЛЕНИЕ";
            // 
            // radioButton26
            // 
            this.radioButton26.AutoSize = true;
            this.radioButton26.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton26.Location = new System.Drawing.Point(22, 111);
            this.radioButton26.Name = "radioButton26";
            this.radioButton26.Size = new System.Drawing.Size(168, 24);
            this.radioButton26.TabIndex = 0;
            this.radioButton26.Tag = "5";
            this.radioButton26.Text = "Большой штраф (5`)";
            this.radioButton26.UseVisualStyleBackColor = true;
            this.radioButton26.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // radioButton28
            // 
            this.radioButton28.AutoSize = true;
            this.radioButton28.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton28.Location = new System.Drawing.Point(22, 55);
            this.radioButton28.Name = "radioButton28";
            this.radioButton28.Size = new System.Drawing.Size(153, 24);
            this.radioButton28.TabIndex = 0;
            this.radioButton28.Tag = "2";
            this.radioButton28.Text = "Малый штраф (2`)";
            this.radioButton28.UseVisualStyleBackColor = true;
            this.radioButton28.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // radioButton29
            // 
            this.radioButton29.AutoSize = true;
            this.radioButton29.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton29.Location = new System.Drawing.Point(22, 83);
            this.radioButton29.Name = "radioButton29";
            this.radioButton29.Size = new System.Drawing.Size(217, 24);
            this.radioButton29.TabIndex = 0;
            this.radioButton29.Tag = "4";
            this.radioButton29.Text = "Двойной малый штраф (4`)";
            this.radioButton29.UseVisualStyleBackColor = true;
            this.radioButton29.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // radioButton30
            // 
            this.radioButton30.AutoSize = true;
            this.radioButton30.Checked = true;
            this.radioButton30.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton30.Location = new System.Drawing.Point(22, 27);
            this.radioButton30.Name = "radioButton30";
            this.radioButton30.Size = new System.Drawing.Size(112, 24);
            this.radioButton30.TabIndex = 0;
            this.radioButton30.TabStop = true;
            this.radioButton30.Tag = "0";
            this.radioButton30.Text = "Без штрафа";
            this.radioButton30.UseVisualStyleBackColor = true;
            this.radioButton30.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton15);
            this.groupBox2.Controls.Add(this.radioButton16);
            this.groupBox2.Controls.Add(this.radioButton17);
            this.groupBox2.Controls.Add(this.radioButton20);
            this.groupBox2.Controls.Add(this.radioButton24);
            this.groupBox2.Location = new System.Drawing.Point(321, 102);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 178);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ДИСЦИПЛИНАРНЫЕ";
            // 
            // radioButton15
            // 
            this.radioButton15.AutoSize = true;
            this.radioButton15.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton15.Location = new System.Drawing.Point(22, 83);
            this.radioButton15.Name = "radioButton15";
            this.radioButton15.Size = new System.Drawing.Size(60, 24);
            this.radioButton15.TabIndex = 0;
            this.radioButton15.Tag = "7";
            this.radioButton15.Text = "ДКИ";
            this.radioButton15.UseVisualStyleBackColor = true;
            this.radioButton15.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton16
            // 
            this.radioButton16.AutoSize = true;
            this.radioButton16.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton16.Location = new System.Drawing.Point(22, 55);
            this.radioButton16.Name = "radioButton16";
            this.radioButton16.Size = new System.Drawing.Size(232, 24);
            this.radioButton16.TabIndex = 0;
            this.radioButton16.Tag = "6";
            this.radioButton16.Text = "Дисциплинарный штраф (10`)";
            this.radioButton16.UseVisualStyleBackColor = true;
            this.radioButton16.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton17
            // 
            this.radioButton17.AutoSize = true;
            this.radioButton17.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton17.Location = new System.Drawing.Point(22, 139);
            this.radioButton17.Name = "radioButton17";
            this.radioButton17.Size = new System.Drawing.Size(193, 24);
            this.radioButton17.TabIndex = 0;
            this.radioButton17.Tag = "9";
            this.radioButton17.Text = "Штрафной бросок (ШБ)";
            this.radioButton17.UseVisualStyleBackColor = true;
            this.radioButton17.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton20
            // 
            this.radioButton20.AutoSize = true;
            this.radioButton20.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton20.Location = new System.Drawing.Point(22, 111);
            this.radioButton20.Name = "radioButton20";
            this.radioButton20.Size = new System.Drawing.Size(156, 24);
            this.radioButton20.TabIndex = 0;
            this.radioButton20.Tag = "8";
            this.radioButton20.Text = "Матч-штраф (МШ)";
            this.radioButton20.UseVisualStyleBackColor = true;
            this.radioButton20.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // radioButton24
            // 
            this.radioButton24.AutoSize = true;
            this.radioButton24.Checked = true;
            this.radioButton24.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton24.Location = new System.Drawing.Point(22, 26);
            this.radioButton24.Name = "radioButton24";
            this.radioButton24.Size = new System.Drawing.Size(82, 24);
            this.radioButton24.TabIndex = 0;
            this.radioButton24.TabStop = true;
            this.radioButton24.Tag = "0";
            this.radioButton24.Text = "Без ДШ";
            this.radioButton24.UseVisualStyleBackColor = true;
            this.radioButton24.CheckedChanged += new System.EventHandler(this.radioButton15_CheckedChanged_1);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Font = new System.Drawing.Font("Segoe UI Emoji", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.Location = new System.Drawing.Point(40, 298);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(231, 24);
            this.checkBox2.TabIndex = 10;
            this.checkBox2.Text = "Игрок остается на площадке";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // PenaltyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 341);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PenaltyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Штраф";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton26;
        private System.Windows.Forms.RadioButton radioButton28;
        private System.Windows.Forms.RadioButton radioButton29;
        private System.Windows.Forms.RadioButton radioButton30;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton15;
        private System.Windows.Forms.RadioButton radioButton16;
        private System.Windows.Forms.RadioButton radioButton17;
        private System.Windows.Forms.RadioButton radioButton20;
        private System.Windows.Forms.RadioButton radioButton24;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}