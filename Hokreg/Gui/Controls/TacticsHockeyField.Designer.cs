namespace Uniso.InStat.Gui.Controls
{
    partial class TacticsHockeyField
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSelectedPlayer = new System.Windows.Forms.Label();
            this.cbTeamOneReplacement = new System.Windows.Forms.CheckBox();
            this.lblWear1 = new System.Windows.Forms.Label();
            this.cbWearTeamOne = new System.Windows.Forms.ComboBox();
            this.lblTeamOne = new System.Windows.Forms.Label();
            this.pbHockeyField = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblTeamTwo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbHockeyField)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSelectedPlayer
            // 
            this.lblSelectedPlayer.AutoSize = true;
            this.lblSelectedPlayer.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedPlayer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelectedPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSelectedPlayer.Location = new System.Drawing.Point(286, 5);
            this.lblSelectedPlayer.Name = "lblSelectedPlayer";
            this.lblSelectedPlayer.Size = new System.Drawing.Size(216, 27);
            this.lblSelectedPlayer.TabIndex = 0;
            this.lblSelectedPlayer.Text = "label1123213123123";
            this.lblSelectedPlayer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSelectedPlayer.TextChanged += new System.EventHandler(this.lblSelectedPlayer_TextChanged);
            // 
            // cbTeamOneReplacement
            // 
            this.cbTeamOneReplacement.AutoSize = true;
            this.cbTeamOneReplacement.Location = new System.Drawing.Point(17, 310);
            this.cbTeamOneReplacement.Name = "cbTeamOneReplacement";
            this.cbTeamOneReplacement.Size = new System.Drawing.Size(171, 17);
            this.cbTeamOneReplacement.TabIndex = 1;
            this.cbTeamOneReplacement.Text = "Замена стартовых составов";
            this.cbTeamOneReplacement.UseVisualStyleBackColor = true;
            // 
            // lblWear1
            // 
            this.lblWear1.AutoSize = true;
            this.lblWear1.Location = new System.Drawing.Point(14, 336);
            this.lblWear1.Name = "lblWear1";
            this.lblWear1.Size = new System.Drawing.Size(44, 13);
            this.lblWear1.TabIndex = 2;
            this.lblWear1.Text = "Форма";
            // 
            // cbWearTeamOne
            // 
            this.cbWearTeamOne.FormattingEnabled = true;
            this.cbWearTeamOne.Location = new System.Drawing.Point(64, 333);
            this.cbWearTeamOne.Name = "cbWearTeamOne";
            this.cbWearTeamOne.Size = new System.Drawing.Size(121, 21);
            this.cbWearTeamOne.TabIndex = 3;
            // 
            // lblTeamOne
            // 
            this.lblTeamOne.AutoSize = true;
            this.lblTeamOne.Location = new System.Drawing.Point(14, 39);
            this.lblTeamOne.Name = "lblTeamOne";
            this.lblTeamOne.Size = new System.Drawing.Size(88, 13);
            this.lblTeamOne.TabIndex = 4;
            this.lblTeamOne.Text = "Team One Name";
            // 
            // pbHockeyField
            // 
            this.pbHockeyField.Location = new System.Drawing.Point(0, 55);
            this.pbHockeyField.Name = "pbHockeyField";
            this.pbHockeyField.Size = new System.Drawing.Size(769, 249);
            this.pbHockeyField.TabIndex = 5;
            this.pbHockeyField.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(642, 333);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(592, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Форма";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(595, 310);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(171, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Замена стартовых составов";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // lblTeamTwo
            // 
            this.lblTeamTwo.AutoSize = true;
            this.lblTeamTwo.Location = new System.Drawing.Point(667, 39);
            this.lblTeamTwo.Name = "lblTeamTwo";
            this.lblTeamTwo.Size = new System.Drawing.Size(89, 13);
            this.lblTeamTwo.TabIndex = 9;
            this.lblTeamTwo.Text = "Team Two Name";
            this.lblTeamTwo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TacticsHockeyField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTeamTwo);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.pbHockeyField);
            this.Controls.Add(this.lblTeamOne);
            this.Controls.Add(this.cbWearTeamOne);
            this.Controls.Add(this.lblWear1);
            this.Controls.Add(this.cbTeamOneReplacement);
            this.Controls.Add(this.lblSelectedPlayer);
            this.Name = "TacticsHockeyField";
            this.Size = new System.Drawing.Size(769, 367);
            ((System.ComponentModel.ISupportInitialize)(this.pbHockeyField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelectedPlayer;
        private System.Windows.Forms.CheckBox cbTeamOneReplacement;
        private System.Windows.Forms.Label lblWear1;
        private System.Windows.Forms.ComboBox cbWearTeamOne;
        private System.Windows.Forms.Label lblTeamOne;
        private System.Windows.Forms.PictureBox pbHockeyField;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblTeamTwo;
    }
}
