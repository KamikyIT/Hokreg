namespace Uniso.InStat.Gui.WPF_Forms
{
    partial class WpfMainWindow
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
            this.fullWPFelement = new System.Windows.Forms.Integration.ElementHost();
            this.wpfMainFormControl1 = new WPFForms.WPFMainFormControl();
            this.SuspendLayout();
            // 
            // fullWPFelement
            // 
            this.fullWPFelement.Location = new System.Drawing.Point(0, 0);
            this.fullWPFelement.Name = "fullWPFelement";
            this.fullWPFelement.Size = new System.Drawing.Size(1311, 777);
            this.fullWPFelement.TabIndex = 0;
            this.fullWPFelement.Text = "elementHost1";
            this.fullWPFelement.Child = this.wpfMainFormControl1;
            // 
            // WpfMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 776);
            this.Controls.Add(this.fullWPFelement);
            this.Name = "WpfMainWindow";
            this.Text = "WpfMainWindow";
            this.Resize += new System.EventHandler(this.WpfMainWindow_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost fullWPFelement;
        private WPFForms.WPFMainFormControl wpfMainFormControl1;
    }
}