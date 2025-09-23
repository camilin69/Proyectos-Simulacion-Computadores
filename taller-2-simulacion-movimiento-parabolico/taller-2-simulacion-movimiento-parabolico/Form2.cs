using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace taller_2_simulacion_movimiento_parabolico
{
    public partial class Form2 : Form
    {
        public Point AreaSpawnLocation
        {
            get { return AreaSpawnPanel.Location; }
            set { AreaSpawnPanel.Location = value; }
        }

        public Size AreaSpawnSize
        {
            get { return AreaSpawnPanel.Size; }
            set { AreaSpawnPanel.Size = value; }
        }

        public Point BulletLocation
        {
            get { return BulletPictureBox.Location; }
            set { BulletPictureBox.Location = value; }
        }

        public Size BulletSize
        {
            get { return BulletPictureBox.Size; }
            set { BulletPictureBox.Size = value; }
        }

        public Point Slingshot1Location
        {
            get { return Slingshot1PictureBox.Location; }
            set { Slingshot1PictureBox.Location = value; }
        }

        public Size Slingshot1Size
        {
            get { return Slingshot1PictureBox.Size; }
            set { Slingshot1PictureBox.Size = value; }
        }

        public Point Slingshot2Location
        {
            get { return Slingshot2PictureBox.Location; }
            set { Slingshot2PictureBox.Location = value; }
        }
        public Size Slingshot2Size
        {
            get { return Slingshot2PictureBox.Size; }
            set { Slingshot2PictureBox.Size = value; }
        }

        public Point GroundLocation
        {
            get { return GroundPictureBox.Location; }
            set { GroundPictureBox.Location = value; }
        }

        public Size GroundSize
        {
            get { return GroundPictureBox.Size; }
            set { GroundPictureBox.Size = value; }
        }

        public Point Tree1Location
        {
            get { return Tree1PictureBox.Location; }
            set { Tree1PictureBox.Location = value; }
        }
        public Size Tree1Size
        {
            get { return Tree1PictureBox.Size; }
            set { Tree1PictureBox.Size = value; }
        }
        public Point Tree2Location
        {
            get { return Tree2PictureBox.Location; }
            set { Tree2PictureBox.Location = value; }
        }
        public Size Tree2Size
        {
            get { return Tree2PictureBox.Size; }
            set { Tree2PictureBox.Size = value; }
        }
        public Point GridLocation
        {
            get { return GridPictureBox.Location; }
            set { GridPictureBox.Location = value; }
        }
        public Size GridSize
        {
            get { return GridPictureBox.Size; }
            set { GridPictureBox.Size = value; }
        }
        public Form2()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }


        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            var form1 = Application.OpenForms["Form1"];
            form1?.BringToFront();
        }

    }
}
