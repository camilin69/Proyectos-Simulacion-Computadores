namespace taller_2_simulacion_movimiento_parabolico
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.RestartButton = new System.Windows.Forms.Button();
            this.DataButton = new System.Windows.Forms.Button();
            this.InfoTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.InfoDataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GridPictureBox = new System.Windows.Forms.PictureBox();
            this.GroundPictureBox = new System.Windows.Forms.PictureBox();
            this.RubyPictureBox = new System.Windows.Forms.PictureBox();
            this.Slingshot1PictureBox = new System.Windows.Forms.PictureBox();
            this.Tree2PictureBox = new System.Windows.Forms.PictureBox();
            this.Tree1PictureBox = new System.Windows.Forms.PictureBox();
            this.Slingshot2PictureBox = new System.Windows.Forms.PictureBox();
            this.DeltaXYLabel = new System.Windows.Forms.Label();
            this.BounceCountLabel = new System.Windows.Forms.Label();
            this.AreaSpawnPanel = new System.Windows.Forms.Panel();
            this.InfoTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InfoDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroundPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RubyPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slingshot1PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tree2PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tree1PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slingshot2PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // RestartButton
            // 
            this.RestartButton.Location = new System.Drawing.Point(1112, 12);
            this.RestartButton.Name = "RestartButton";
            this.RestartButton.Size = new System.Drawing.Size(75, 23);
            this.RestartButton.TabIndex = 12;
            this.RestartButton.Text = "Reiniciar";
            this.RestartButton.UseVisualStyleBackColor = true;
            this.RestartButton.Click += new System.EventHandler(this.RestartButton_Click);
            // 
            // DataButton
            // 
            this.DataButton.Location = new System.Drawing.Point(1031, 12);
            this.DataButton.Name = "DataButton";
            this.DataButton.Size = new System.Drawing.Size(75, 23);
            this.DataButton.TabIndex = 13;
            this.DataButton.Text = "Datos";
            this.DataButton.UseVisualStyleBackColor = true;
            this.DataButton.Click += new System.EventHandler(this.DataButton_Click);
            // 
            // InfoTabControl
            // 
            this.InfoTabControl.Controls.Add(this.tabPage1);
            this.InfoTabControl.Location = new System.Drawing.Point(724, 41);
            this.InfoTabControl.Name = "InfoTabControl";
            this.InfoTabControl.SelectedIndex = 0;
            this.InfoTabControl.Size = new System.Drawing.Size(448, 570);
            this.InfoTabControl.TabIndex = 14;
            this.InfoTabControl.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.InfoDataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(440, 544);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Información";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // InfoDataGridView
            // 
            this.InfoDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InfoDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.InfoDataGridView.Location = new System.Drawing.Point(6, 6);
            this.InfoDataGridView.Name = "InfoDataGridView";
            this.InfoDataGridView.Size = new System.Drawing.Size(431, 532);
            this.InfoDataGridView.TabIndex = 0;
            this.InfoDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.InfoDataGridView_CellClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Launch";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "MaxHeight";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "MaxVelocity";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "TotalTime";
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Angle";
            this.Column5.Name = "Column5";
            // 
            // GridPictureBox
            // 
            this.GridPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.GridPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("GridPictureBox.Image")));
            this.GridPictureBox.Location = new System.Drawing.Point(1113, -5);
            this.GridPictureBox.Name = "GridPictureBox";
            this.GridPictureBox.Size = new System.Drawing.Size(73, 642);
            this.GridPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GridPictureBox.TabIndex = 22;
            this.GridPictureBox.TabStop = false;
            // 
            // GroundPictureBox
            // 
            this.GroundPictureBox.Location = new System.Drawing.Point(-2, 643);
            this.GroundPictureBox.Name = "GroundPictureBox";
            this.GroundPictureBox.Size = new System.Drawing.Size(1188, 22);
            this.GroundPictureBox.TabIndex = 19;
            this.GroundPictureBox.TabStop = false;
            this.GroundPictureBox.Visible = false;
            // 
            // RubyPictureBox
            // 
            this.RubyPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.RubyPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("RubyPictureBox.Image")));
            this.RubyPictureBox.Location = new System.Drawing.Point(116, 466);
            this.RubyPictureBox.Name = "RubyPictureBox";
            this.RubyPictureBox.Size = new System.Drawing.Size(74, 70);
            this.RubyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RubyPictureBox.TabIndex = 18;
            this.RubyPictureBox.TabStop = false;
            this.RubyPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RubyPictureBox_MouseDown);
            this.RubyPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RubyPictureBox_MouseMove);
            this.RubyPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RubyPictureBox_MouseUp);
            // 
            // Slingshot1PictureBox
            // 
            this.Slingshot1PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.Slingshot1PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("Slingshot1PictureBox.Image")));
            this.Slingshot1PictureBox.Location = new System.Drawing.Point(126, 373);
            this.Slingshot1PictureBox.Name = "Slingshot1PictureBox";
            this.Slingshot1PictureBox.Size = new System.Drawing.Size(145, 217);
            this.Slingshot1PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Slingshot1PictureBox.TabIndex = 17;
            this.Slingshot1PictureBox.TabStop = false;
            // 
            // Tree2PictureBox
            // 
            this.Tree2PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.Tree2PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("Tree2PictureBox.Image")));
            this.Tree2PictureBox.Location = new System.Drawing.Point(552, 303);
            this.Tree2PictureBox.Name = "Tree2PictureBox";
            this.Tree2PictureBox.Size = new System.Drawing.Size(166, 306);
            this.Tree2PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Tree2PictureBox.TabIndex = 21;
            this.Tree2PictureBox.TabStop = false;
            // 
            // Tree1PictureBox
            // 
            this.Tree1PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.Tree1PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("Tree1PictureBox.Image")));
            this.Tree1PictureBox.Location = new System.Drawing.Point(424, 466);
            this.Tree1PictureBox.Name = "Tree1PictureBox";
            this.Tree1PictureBox.Size = new System.Drawing.Size(146, 124);
            this.Tree1PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Tree1PictureBox.TabIndex = 20;
            this.Tree1PictureBox.TabStop = false;
            // 
            // Slingshot2PictureBox
            // 
            this.Slingshot2PictureBox.BackColor = System.Drawing.Color.Transparent;
            this.Slingshot2PictureBox.Image = ((System.Drawing.Image)(resources.GetObject("Slingshot2PictureBox.Image")));
            this.Slingshot2PictureBox.Location = new System.Drawing.Point(126, 373);
            this.Slingshot2PictureBox.Name = "Slingshot2PictureBox";
            this.Slingshot2PictureBox.Size = new System.Drawing.Size(145, 217);
            this.Slingshot2PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Slingshot2PictureBox.TabIndex = 23;
            this.Slingshot2PictureBox.TabStop = false;
            this.Slingshot2PictureBox.Visible = false;
            // 
            // DeltaXYLabel
            // 
            this.DeltaXYLabel.AutoSize = true;
            this.DeltaXYLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeltaXYLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.DeltaXYLabel.Location = new System.Drawing.Point(115, 152);
            this.DeltaXYLabel.Name = "DeltaXYLabel";
            this.DeltaXYLabel.Size = new System.Drawing.Size(226, 37);
            this.DeltaXYLabel.TabIndex = 16;
            this.DeltaXYLabel.Text = "DeltaX: DeltaY";
            // 
            // BounceCountLabel
            // 
            this.BounceCountLabel.AutoSize = true;
            this.BounceCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BounceCountLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.BounceCountLabel.Location = new System.Drawing.Point(119, 215);
            this.BounceCountLabel.Name = "BounceCountLabel";
            this.BounceCountLabel.Size = new System.Drawing.Size(229, 37);
            this.BounceCountLabel.TabIndex = 24;
            this.BounceCountLabel.Text = "Bounce Count:";
            // 
            // AreaSpawnPanel
            // 
            this.AreaSpawnPanel.Location = new System.Drawing.Point(727, 59);
            this.AreaSpawnPanel.Name = "AreaSpawnPanel";
            this.AreaSpawnPanel.Size = new System.Drawing.Size(380, 367);
            this.AreaSpawnPanel.TabIndex = 25;
            this.AreaSpawnPanel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.AreaSpawnPanel);
            this.Controls.Add(this.BounceCountLabel);
            this.Controls.Add(this.RubyPictureBox);
            this.Controls.Add(this.GroundPictureBox);
            this.Controls.Add(this.Slingshot1PictureBox);
            this.Controls.Add(this.Tree2PictureBox);
            this.Controls.Add(this.Tree1PictureBox);
            this.Controls.Add(this.Slingshot2PictureBox);
            this.Controls.Add(this.DeltaXYLabel);
            this.Controls.Add(this.InfoTabControl);
            this.Controls.Add(this.DataButton);
            this.Controls.Add(this.RestartButton);
            this.Controls.Add(this.GridPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.InfoTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.InfoDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroundPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RubyPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slingshot1PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tree2PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tree1PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slingshot2PictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button RestartButton;
        private System.Windows.Forms.Button DataButton;
        private System.Windows.Forms.TabControl InfoTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView InfoDataGridView;
        private System.Windows.Forms.PictureBox GridPictureBox;
        private System.Windows.Forms.PictureBox GroundPictureBox;
        private System.Windows.Forms.PictureBox RubyPictureBox;
        private System.Windows.Forms.PictureBox Slingshot1PictureBox;
        private System.Windows.Forms.PictureBox Tree2PictureBox;
        private System.Windows.Forms.PictureBox Tree1PictureBox;
        private System.Windows.Forms.PictureBox Slingshot2PictureBox;
        private System.Windows.Forms.Label DeltaXYLabel;
        private System.Windows.Forms.Label BounceCountLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.Panel AreaSpawnPanel;
    }
}

