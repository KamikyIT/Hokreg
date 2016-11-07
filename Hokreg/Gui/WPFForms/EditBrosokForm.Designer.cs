namespace Uniso.InStat.Gui.WPFForms
{
    partial class EditBrosokForm
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
            this.components = new System.ComponentModel.Container();
            this.lblStartFlyTime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblEndFlyTime = new System.Windows.Forms.Label();
            this.markersListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnBrosokType6 = new System.Windows.Forms.Button();
            this.btnBrosokType5 = new System.Windows.Forms.Button();
            this.btnBrosokType4 = new System.Windows.Forms.Button();
            this.btnBrosokType3 = new System.Windows.Forms.Button();
            this.btnBrosokType2 = new System.Windows.Forms.Button();
            this.btnBrosokType1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnGoalkeeperView3 = new System.Windows.Forms.Button();
            this.btnGoalkeeperView2 = new System.Windows.Forms.Button();
            this.btnGoalkeeperView1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGoalKeeperStanding4 = new System.Windows.Forms.Button();
            this.btnGoalKeeperStanding3 = new System.Windows.Forms.Button();
            this.btnGoalKeeperStanding2 = new System.Windows.Forms.Button();
            this.btnGoalKeeperStanding1 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnGoalFixation4 = new System.Windows.Forms.Button();
            this.btnGoalFixation2 = new System.Windows.Forms.Button();
            this.btnGoalFixation3 = new System.Windows.Forms.Button();
            this.btnGoalFixation1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnEndFlyTime = new System.Windows.Forms.Button();
            this.btnStartFlyTime = new System.Windows.Forms.Button();
            this.elementHost3 = new System.Windows.Forms.Integration.ElementHost();
            this.streamPlayer1 = new Uniso.InStat.PlayerTypes.StreamPlayer(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStartFlyTime
            // 
            this.lblStartFlyTime.AutoSize = true;
            this.lblStartFlyTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblStartFlyTime.Location = new System.Drawing.Point(191, 25);
            this.lblStartFlyTime.Name = "lblStartFlyTime";
            this.lblStartFlyTime.Size = new System.Drawing.Size(84, 25);
            this.lblStartFlyTime.TabIndex = 1;
            this.lblStartFlyTime.Text = "33:25.2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(12, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Момент броска:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(311, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Момент отскока:";
            // 
            // lblEndFlyTime
            // 
            this.lblEndFlyTime.AutoSize = true;
            this.lblEndFlyTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblEndFlyTime.Location = new System.Drawing.Point(490, 25);
            this.lblEndFlyTime.Name = "lblEndFlyTime";
            this.lblEndFlyTime.Size = new System.Drawing.Size(84, 25);
            this.lblEndFlyTime.TabIndex = 3;
            this.lblEndFlyTime.Text = "33:25.2";
            // 
            // markersListBox
            // 
            this.markersListBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.markersListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.markersListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.markersListBox.FormattingEnabled = true;
            this.markersListBox.HorizontalScrollbar = true;
            this.markersListBox.Location = new System.Drawing.Point(861, 53);
            this.markersListBox.Name = "markersListBox";
            this.markersListBox.Size = new System.Drawing.Size(151, 524);
            this.markersListBox.TabIndex = 5;
            this.markersListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.markersListBox_DrawItem);
            this.markersListBox.SelectedIndexChanged += new System.EventHandler(this.markersListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnBrosokType6);
            this.panel1.Controls.Add(this.btnBrosokType5);
            this.panel1.Controls.Add(this.btnBrosokType4);
            this.panel1.Controls.Add(this.btnBrosokType3);
            this.panel1.Controls.Add(this.btnBrosokType2);
            this.panel1.Controls.Add(this.btnBrosokType1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(1018, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(371, 150);
            this.panel1.TabIndex = 6;
            // 
            // btnBrosokType6
            // 
            this.btnBrosokType6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBrosokType6.Location = new System.Drawing.Point(201, 106);
            this.btnBrosokType6.Name = "btnBrosokType6";
            this.btnBrosokType6.Size = new System.Drawing.Size(167, 33);
            this.btnBrosokType6.TabIndex = 6;
            this.btnBrosokType6.Text = "БРОСОК С БЛИЗКОЙ ДИСТАНЦИИ (ДОБИВАНИЕ)";
            this.btnBrosokType6.UseVisualStyleBackColor = true;
            this.btnBrosokType6.Click += new System.EventHandler(this.btnBrosokType_Click);
            // 
            // btnBrosokType5
            // 
            this.btnBrosokType5.Location = new System.Drawing.Point(8, 106);
            this.btnBrosokType5.Name = "btnBrosokType5";
            this.btnBrosokType5.Size = new System.Drawing.Size(175, 33);
            this.btnBrosokType5.TabIndex = 5;
            this.btnBrosokType5.Text = "ПОДСТАВЛЕННАЯ КЛЮШКА";
            this.btnBrosokType5.UseVisualStyleBackColor = true;
            this.btnBrosokType5.Click += new System.EventHandler(this.btnBrosokType_Click);
            // 
            // btnBrosokType4
            // 
            this.btnBrosokType4.Location = new System.Drawing.Point(201, 67);
            this.btnBrosokType4.Name = "btnBrosokType4";
            this.btnBrosokType4.Size = new System.Drawing.Size(167, 33);
            this.btnBrosokType4.TabIndex = 4;
            this.btnBrosokType4.Text = "С НЕУДОБНОЙ РУКИ";
            this.btnBrosokType4.UseVisualStyleBackColor = true;
            this.btnBrosokType4.Click += new System.EventHandler(this.btnBrosokType_Click);
            // 
            // btnBrosokType3
            // 
            this.btnBrosokType3.Location = new System.Drawing.Point(8, 67);
            this.btnBrosokType3.Name = "btnBrosokType3";
            this.btnBrosokType3.Size = new System.Drawing.Size(175, 33);
            this.btnBrosokType3.TabIndex = 3;
            this.btnBrosokType3.Text = "КИСТЕВОЙ";
            this.btnBrosokType3.UseVisualStyleBackColor = true;
            this.btnBrosokType3.Click += new System.EventHandler(this.btnBrosokType_Click);
            // 
            // btnBrosokType2
            // 
            this.btnBrosokType2.Location = new System.Drawing.Point(201, 28);
            this.btnBrosokType2.Name = "btnBrosokType2";
            this.btnBrosokType2.Size = new System.Drawing.Size(167, 33);
            this.btnBrosokType2.TabIndex = 2;
            this.btnBrosokType2.Text = "ЩЕЛЧОК С РАЗМАХОМ";
            this.btnBrosokType2.UseVisualStyleBackColor = true;
            this.btnBrosokType2.Click += new System.EventHandler(this.btnBrosokType_Click);
            // 
            // btnBrosokType1
            // 
            this.btnBrosokType1.Location = new System.Drawing.Point(8, 28);
            this.btnBrosokType1.Name = "btnBrosokType1";
            this.btnBrosokType1.Size = new System.Drawing.Size(175, 33);
            this.btnBrosokType1.TabIndex = 1;
            this.btnBrosokType1.Text = "ЩЕЛЧОК";
            this.btnBrosokType1.UseVisualStyleBackColor = true;
            this.btnBrosokType1.Click += new System.EventHandler(this.btnBrosokType_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "ТИП БРОСКА";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnGoalkeeperView3);
            this.panel2.Controls.Add(this.btnGoalkeeperView2);
            this.panel2.Controls.Add(this.btnGoalkeeperView1);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(1018, 209);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(371, 107);
            this.panel2.TabIndex = 7;
            // 
            // btnGoalkeeperView3
            // 
            this.btnGoalkeeperView3.Location = new System.Drawing.Point(8, 67);
            this.btnGoalkeeperView3.Name = "btnGoalkeeperView3";
            this.btnGoalkeeperView3.Size = new System.Drawing.Size(175, 33);
            this.btnGoalkeeperView3.TabIndex = 3;
            this.btnGoalkeeperView3.Text = "БРОСОК С ОБМАННЫМ ДВИЖЕНИЕМ";
            this.btnGoalkeeperView3.UseVisualStyleBackColor = true;
            this.btnGoalkeeperView3.Click += new System.EventHandler(this.btnGoalkeeperView_Click);
            // 
            // btnGoalkeeperView2
            // 
            this.btnGoalkeeperView2.Location = new System.Drawing.Point(201, 28);
            this.btnGoalkeeperView2.Name = "btnGoalkeeperView2";
            this.btnGoalkeeperView2.Size = new System.Drawing.Size(167, 33);
            this.btnGoalkeeperView2.TabIndex = 2;
            this.btnGoalkeeperView2.Text = "ЗАКРЫТЫЙ";
            this.btnGoalkeeperView2.UseVisualStyleBackColor = true;
            this.btnGoalkeeperView2.Click += new System.EventHandler(this.btnGoalkeeperView_Click);
            // 
            // btnGoalkeeperView1
            // 
            this.btnGoalkeeperView1.Location = new System.Drawing.Point(8, 28);
            this.btnGoalkeeperView1.Name = "btnGoalkeeperView1";
            this.btnGoalkeeperView1.Size = new System.Drawing.Size(175, 33);
            this.btnGoalkeeperView1.TabIndex = 1;
            this.btnGoalkeeperView1.Text = "ОТКРЫТЫЙ";
            this.btnGoalkeeperView1.UseVisualStyleBackColor = true;
            this.btnGoalkeeperView1.Click += new System.EventHandler(this.btnGoalkeeperView_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(202, 25);
            this.label6.TabIndex = 0;
            this.label6.Text = "ОБЗОР ВРАТАРЯ";
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(12, 588);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(263, 224);
            this.elementHost1.TabIndex = 8;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(1018, 322);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(224, 221);
            this.elementHost2.TabIndex = 9;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGoalKeeperStanding4);
            this.groupBox1.Controls.Add(this.btnGoalKeeperStanding3);
            this.groupBox1.Controls.Add(this.btnGoalKeeperStanding2);
            this.groupBox1.Controls.Add(this.btnGoalKeeperStanding1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(1248, 322);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 221);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Стойка";
            // 
            // btnGoalKeeperStanding4
            // 
            this.btnGoalKeeperStanding4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGoalKeeperStanding4.Location = new System.Drawing.Point(6, 178);
            this.btnGoalKeeperStanding4.Name = "btnGoalKeeperStanding4";
            this.btnGoalKeeperStanding4.Size = new System.Drawing.Size(129, 37);
            this.btnGoalKeeperStanding4.TabIndex = 3;
            this.btnGoalKeeperStanding4.Text = "ЛЕЖИТ";
            this.btnGoalKeeperStanding4.UseVisualStyleBackColor = true;
            this.btnGoalKeeperStanding4.Click += new System.EventHandler(this.btnGoalKeeperStanding_Click);
            // 
            // btnGoalKeeperStanding3
            // 
            this.btnGoalKeeperStanding3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGoalKeeperStanding3.Location = new System.Drawing.Point(6, 127);
            this.btnGoalKeeperStanding3.Name = "btnGoalKeeperStanding3";
            this.btnGoalKeeperStanding3.Size = new System.Drawing.Size(129, 37);
            this.btnGoalKeeperStanding3.TabIndex = 2;
            this.btnGoalKeeperStanding3.Text = "В СПЛИТЕ";
            this.btnGoalKeeperStanding3.UseVisualStyleBackColor = true;
            this.btnGoalKeeperStanding3.Click += new System.EventHandler(this.btnGoalKeeperStanding_Click);
            // 
            // btnGoalKeeperStanding2
            // 
            this.btnGoalKeeperStanding2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGoalKeeperStanding2.Location = new System.Drawing.Point(6, 78);
            this.btnGoalKeeperStanding2.Name = "btnGoalKeeperStanding2";
            this.btnGoalKeeperStanding2.Size = new System.Drawing.Size(129, 37);
            this.btnGoalKeeperStanding2.TabIndex = 1;
            this.btnGoalKeeperStanding2.Text = "НИЗКАЯ СТОЙКА";
            this.btnGoalKeeperStanding2.UseVisualStyleBackColor = true;
            this.btnGoalKeeperStanding2.Click += new System.EventHandler(this.btnGoalKeeperStanding_Click);
            // 
            // btnGoalKeeperStanding1
            // 
            this.btnGoalKeeperStanding1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGoalKeeperStanding1.Location = new System.Drawing.Point(6, 27);
            this.btnGoalKeeperStanding1.Name = "btnGoalKeeperStanding1";
            this.btnGoalKeeperStanding1.Size = new System.Drawing.Size(129, 37);
            this.btnGoalKeeperStanding1.TabIndex = 0;
            this.btnGoalKeeperStanding1.Text = "ВЫСОКАЯ СТОЙКА";
            this.btnGoalKeeperStanding1.UseVisualStyleBackColor = true;
            this.btnGoalKeeperStanding1.Click += new System.EventHandler(this.btnGoalKeeperStanding_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnGoalFixation4);
            this.panel3.Controls.Add(this.btnGoalFixation2);
            this.panel3.Controls.Add(this.btnGoalFixation3);
            this.panel3.Controls.Add(this.btnGoalFixation1);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(1018, 549);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(371, 114);
            this.panel3.TabIndex = 11;
            // 
            // btnGoalFixation4
            // 
            this.btnGoalFixation4.Location = new System.Drawing.Point(193, 69);
            this.btnGoalFixation4.Name = "btnGoalFixation4";
            this.btnGoalFixation4.Size = new System.Drawing.Size(167, 35);
            this.btnGoalFixation4.TabIndex = 4;
            this.btnGoalFixation4.Text = "НЕКОНТРОЛИРУЕМЫЙ ОТСКОК";
            this.btnGoalFixation4.UseVisualStyleBackColor = true;
            this.btnGoalFixation4.Click += new System.EventHandler(this.btnGoalFixation_Click);
            // 
            // btnGoalFixation2
            // 
            this.btnGoalFixation2.Location = new System.Drawing.Point(8, 69);
            this.btnGoalFixation2.Name = "btnGoalFixation2";
            this.btnGoalFixation2.Size = new System.Drawing.Size(167, 35);
            this.btnGoalFixation2.TabIndex = 3;
            this.btnGoalFixation2.Text = "ЗАФИКСИРОВАЛ С ОТСКОКА";
            this.btnGoalFixation2.UseVisualStyleBackColor = true;
            this.btnGoalFixation2.Click += new System.EventHandler(this.btnGoalFixation_Click);
            // 
            // btnGoalFixation3
            // 
            this.btnGoalFixation3.Location = new System.Drawing.Point(193, 28);
            this.btnGoalFixation3.Name = "btnGoalFixation3";
            this.btnGoalFixation3.Size = new System.Drawing.Size(167, 35);
            this.btnGoalFixation3.TabIndex = 2;
            this.btnGoalFixation3.Text = "КОНТРОЛИРУЕМЫЙ ОТСКОК";
            this.btnGoalFixation3.UseVisualStyleBackColor = true;
            this.btnGoalFixation3.Click += new System.EventHandler(this.btnGoalFixation_Click);
            // 
            // btnGoalFixation1
            // 
            this.btnGoalFixation1.Location = new System.Drawing.Point(8, 28);
            this.btnGoalFixation1.Name = "btnGoalFixation1";
            this.btnGoalFixation1.Size = new System.Drawing.Size(167, 35);
            this.btnGoalFixation1.TabIndex = 1;
            this.btnGoalFixation1.Text = "ЗАФИКСИРОВАЛ СРАЗУ";
            this.btnGoalFixation1.UseVisualStyleBackColor = true;
            this.btnGoalFixation1.Click += new System.EventHandler(this.btnGoalFixation_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 25);
            this.label7.TabIndex = 0;
            this.label7.Text = "ФИКСАЦИЯ";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnEndFlyTime);
            this.panel4.Controls.Add(this.btnStartFlyTime);
            this.panel4.Location = new System.Drawing.Point(1018, 669);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(371, 68);
            this.panel4.TabIndex = 12;
            // 
            // btnEndFlyTime
            // 
            this.btnEndFlyTime.Location = new System.Drawing.Point(193, 23);
            this.btnEndFlyTime.Name = "btnEndFlyTime";
            this.btnEndFlyTime.Size = new System.Drawing.Size(167, 35);
            this.btnEndFlyTime.TabIndex = 1;
            this.btnEndFlyTime.Text = "КОНЕЦ ПОЛЕТА ШАЙБЫ";
            this.btnEndFlyTime.UseVisualStyleBackColor = true;
            // 
            // btnStartFlyTime
            // 
            this.btnStartFlyTime.Location = new System.Drawing.Point(8, 23);
            this.btnStartFlyTime.Name = "btnStartFlyTime";
            this.btnStartFlyTime.Size = new System.Drawing.Size(167, 35);
            this.btnStartFlyTime.TabIndex = 0;
            this.btnStartFlyTime.Text = "НАЧАЛО ПОЛЕТА ШАЙБЫ";
            this.btnStartFlyTime.UseVisualStyleBackColor = true;
            // 
            // elementHost3
            // 
            this.elementHost3.Location = new System.Drawing.Point(281, 588);
            this.elementHost3.Name = "elementHost3";
            this.elementHost3.Size = new System.Drawing.Size(283, 224);
            this.elementHost3.TabIndex = 13;
            this.elementHost3.Text = "elementHost3";
            this.elementHost3.Child = null;
            // 
            // streamPlayer1
            // 
            this.streamPlayer1.BackColor = System.Drawing.Color.Black;
            this.streamPlayer1.BufferTime = 1000;
            this.streamPlayer1.CacheDirectory = ".\\cache\\";
            this.streamPlayer1.CurrentConnectorSecond = ((long)(0));
            this.streamPlayer1.CurrentSceneFileName = null;
            this.streamPlayer1.DurationTotal = 0;
            this.streamPlayer1.DurationUpload = 0;
            this.streamPlayer1.Location = new System.Drawing.Point(12, 53);
            this.streamPlayer1.MediaType = Uniso.InStat.PlayerTypes.StreamPlayer.MediaTypeEnum.FLAT_FILE;
            this.streamPlayer1.Mode = Uniso.InStat.PlayerTypes.StreamPlayer.PlayerMode.Stop;
            this.streamPlayer1.Name = "streamPlayer1";
            this.streamPlayer1.NumScene1 = 0;
            this.streamPlayer1.NumScene2 = 0;
            this.streamPlayer1.Position = 0;
            this.streamPlayer1.Rate = 1D;
            this.streamPlayer1.Size = new System.Drawing.Size(843, 529);
            this.streamPlayer1.TabIndex = 0;
            this.streamPlayer1.Text = "streamPlayer1";
            this.streamPlayer1.VideoSize = new System.Drawing.Size(0, 0);
            this.streamPlayer1.Volume = 0;
            // 
            // EditBrosokForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1401, 815);
            this.Controls.Add(this.elementHost3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.elementHost2);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.markersListBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblEndFlyTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblStartFlyTime);
            this.Controls.Add(this.streamPlayer1);
            this.Name = "EditBrosokForm";
            this.Text = "EditBrosokForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PlayerTypes.StreamPlayer streamPlayer1;
        private System.Windows.Forms.Label lblStartFlyTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblEndFlyTime;
        private System.Windows.Forms.ListBox markersListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrosokType6;
        private System.Windows.Forms.Button btnBrosokType5;
        private System.Windows.Forms.Button btnBrosokType4;
        private System.Windows.Forms.Button btnBrosokType3;
        private System.Windows.Forms.Button btnBrosokType2;
        private System.Windows.Forms.Button btnBrosokType1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnGoalkeeperView3;
        private System.Windows.Forms.Button btnGoalkeeperView2;
        private System.Windows.Forms.Button btnGoalkeeperView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnGoalKeeperStanding4;
        private System.Windows.Forms.Button btnGoalKeeperStanding3;
        private System.Windows.Forms.Button btnGoalKeeperStanding2;
        private System.Windows.Forms.Button btnGoalKeeperStanding1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnGoalFixation4;
        private System.Windows.Forms.Button btnGoalFixation2;
        private System.Windows.Forms.Button btnGoalFixation3;
        private System.Windows.Forms.Button btnGoalFixation1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnEndFlyTime;
        private System.Windows.Forms.Button btnStartFlyTime;
        private System.Windows.Forms.Integration.ElementHost elementHost3;
    }
}
