using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octave.NET;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TallerMasResorte
{
    public partial class SpringMassForm : Form
    {
        int j = 0;
        double[] mass1VectorX = new double[2];
        double[] mass1VectorT = new double[2];
        double[] mass2VectorX = new double[2];
        double[] mass2VectorT = new double[2];
        int mass1InitialX, mass2InitialX;
        int spring0InitialWidth, spring1InitialWidth, spring2InitialWidth;
        int spring0InitialX, spring2InitialX, absorber2InitialX;
        int absorber1InitialWidth, absorber2InitialWidth;

        int right0Reference;
        int factor;
        public SpringMassForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hideTextBoxes();
            j = 0;
            factor = Convert.ToInt32(FactorTextBox.Text);
            setupInitialVariables();
            right0Reference = (int)(Spring2PictureBox.Location.X + Spring2PictureBox.Width);

            Mass1TableDataGridView.Rows.Clear();
            Mass2TableDataGridView.Rows.Clear();
            Mass1ResponseChart.Series[0].Points.Clear();
            Mass2ResponseChart.Series[0].Points.Clear();
            MetricsLabel.Text = "";

            var Octave = new OctaveContext();
            string instructions = "clc;"
                    + "clear;"
                    + "pkg load control;"
                    + "s = tf('s');"
                    + "k0 = " + Spring0TextBox.Text + ";"
                    + "k1 = " + Spring1TextBox.Text + ";"
                    + "k2 = " + Spring2TextBox.Text + ";"
                    + "b1 = " + Absorber1TextBox.Text + ";"
                    + "b2 = " + Absorber2TextBox.Text + ";"
                    + "m1 = " + Mass1TextBox.Text + ";"
                    + "m2 = " + Mass2TextBox.Text + ";"
                    + "G1 = (m2*s^2 + k0 + k2 + b2*s) / ((m1*s^2 + k1 + b1*s + k0)*(m2*s^2 + k0 + k2 + b2*s) - k0^2);"
                    + "G2 = k0 / ((m1*s^2 + k1 + b1*s + k0)*(m2*s^2 + k0 + k2 + b2*s) - k0^2);"
                    + "[y1, t1] = step(G1);"
                    + "c1 = length(y1);"
                    + "tiempo1 = t1(c1);"
                    + "[y2, t2] = step(G2);"
                    + "c2 = length(y2);"
                    + "tiempo2 = t2(c2);"
                    + "tiempo_final = max(tiempo1, tiempo2);"
                    + "[x1, t1] = step(G1, tiempo_final, tiempo_final/" + ElementsTextBox.Text + ");"
                    + "[x2, t2] = step(G2, tiempo_final, tiempo_final/" + ElementsTextBox.Text + ");"
                    + "x1 = x1(:);"
                    + "t1 = t1(:);"
                    + "x2 = x2(:);"
                    + "t2 = t2(:);";
            Octave.Execute(instructions);

            Array.Resize(ref mass1VectorX, Convert.ToInt16(ElementsTextBox.Text));
            Array.Resize(ref mass2VectorX, Convert.ToInt16(ElementsTextBox.Text));
            Array.Resize(ref mass1VectorT, Convert.ToInt16(ElementsTextBox.Text));
            Array.Resize(ref mass2VectorT, Convert.ToInt16(ElementsTextBox.Text));

            mass1VectorX = Octave.Execute("x1").AsVector();
            mass2VectorX = Octave.Execute("x2").AsVector();
            mass1VectorT = Octave.Execute("t1").AsVector();
            mass2VectorT = Octave.Execute("t2").AsVector();

            

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            calculatePositionsForAnimation();

            if (j == mass1VectorX.Length)
            {
                timer1.Stop();
                showData();
                reset();
                showTextBoxes();
            }
        }

        private void calculatePositionsForAnimation()
        {
            Mass1PictureBox.Location = new Point((int)(mass1InitialX - (mass1VectorX[j] * factor)), Mass1PictureBox.Location.Y);
            Mass2PictureBox.Location = new Point((int)(mass2InitialX - (mass2VectorX[j] * factor)), Mass2PictureBox.Location.Y);

            Spring1PictureBox.Width = (int)(Mass1PictureBox.Location.X) - 20;
            Absorber1PictureBox.Width = (int)(Mass1PictureBox.Location.X) - 20;

            Spring0PictureBox.Location = new Point((int)(Mass1PictureBox.Location.X + Mass1PictureBox.Width - 20),Spring0PictureBox.Location.Y);
            Spring0PictureBox.Width = (int)(Mass2PictureBox.Location.X - (Mass1PictureBox.Location.X + Mass1PictureBox.Width) + 20);

            Spring2PictureBox.Location = new Point((int)(Mass2PictureBox.Location.X + Mass2PictureBox.Width), Spring2PictureBox.Location.Y);
            Spring2PictureBox.Width = (int)(right0Reference - (Mass2PictureBox.Location.X + Mass2PictureBox.Width));

            Absorber2PictureBox.Location = new Point((int)(Mass2PictureBox.Location.X + Mass2PictureBox.Width), Absorber2PictureBox.Location.Y);
            Absorber2PictureBox.Width = (int)(right0Reference - (Mass2PictureBox.Location.X + Mass2PictureBox.Width));

            j++;
        }

        private void setupInitialVariables ()
        {
            mass1InitialX = Mass1PictureBox.Location.X;
            mass2InitialX = Mass2PictureBox.Location.X;
            spring0InitialWidth = Spring0PictureBox.Width;
            spring0InitialX = Spring0PictureBox.Location.X;
            spring1InitialWidth = Spring1PictureBox.Width;
            spring2InitialWidth = Spring2PictureBox.Width;
            spring2InitialX = Spring2PictureBox.Location.X;
            absorber1InitialWidth = Absorber1PictureBox.Width;
            absorber2InitialWidth = Absorber2PictureBox.Width;
            absorber2InitialX = Absorber2PictureBox.Location.X;

        }

        private void reset ()
        {
            Mass1PictureBox.Location = new Point(mass1InitialX, Mass1PictureBox.Location.Y);
            Mass2PictureBox.Location = new Point(mass2InitialX, Mass2PictureBox.Location.Y);
            Spring0PictureBox.Location = new Point(spring0InitialX, Spring0PictureBox.Location.Y);
            Spring0PictureBox.Width = spring0InitialWidth;
            Spring1PictureBox.Width = spring1InitialWidth;
            Absorber1PictureBox.Width = absorber1InitialWidth;
            Spring2PictureBox.Location = new Point(spring2InitialX, Spring2PictureBox.Location.Y);
            Spring2PictureBox.Width = spring2InitialWidth;
            Absorber2PictureBox.Location = new Point(absorber2InitialX, Absorber2PictureBox.Location.Y);
            Absorber2PictureBox.Width = absorber2InitialWidth;

        }

        private void hideTextBoxes()
        {
            Spring0TextBox.Visible = false;
            Spring1TextBox.Visible = false;
            Spring2TextBox.Visible = false;
            Absorber1TextBox.Visible = false;
            Absorber2TextBox.Visible = false;
            Mass1TextBox.Visible = false;
            Mass2TextBox.Visible = false;
            button1.Visible = false;
        }

        private void showTextBoxes()
        {
            Spring0TextBox.Visible = true;
            Spring1TextBox.Visible = true;
            Spring2TextBox.Visible = true;
            Absorber1TextBox.Visible = true;
            Absorber2TextBox.Visible = true;
            Mass1TextBox.Visible = true;
            Mass2TextBox.Visible = true;
            button1 .Visible = true;
        }

        private void showData ()
        {
            for (int i = 0; i < Convert.ToInt16(ElementsTextBox.Text); i++)
            {
                Mass1ResponseChart.Series[0].Points.AddXY(mass1VectorT[i], mass1VectorX[i]);
                Mass2ResponseChart.Series[0].Points.AddXY(mass2VectorT[i], mass2VectorX[i]);
                Mass1TableDataGridView.Rows.Add(i + 1, mass1VectorT[i], mass1VectorX[i]);
                Mass2TableDataGridView.Rows.Add(i + 1, mass2VectorT[i], mass2VectorX[i]);
            }

            string mass1MaxDisplacement = mass1VectorX.Max().ToString();
            string mass1MinDisplacement = mass1VectorX.Min().ToString();
            string mass2MaxDisplacement = mass2VectorX.Max().ToString();
            string mass2MinDisplacement = mass2VectorX.Min().ToString();

            string totalSimulationtime = mass1VectorT.Max() == mass2VectorT.Max() ? mass1VectorT.Max().ToString() : "error";

            string mass1MaxDisplacementTime = mass1VectorT[Array.IndexOf(mass1VectorX, mass1VectorX.Max())].ToString();
            string mass2MaxDisplacementTime = mass2VectorT[Array.IndexOf(mass2VectorX, mass2VectorX.Max())].ToString();

            MetricsLabel.Text = "Mass 1 Max Displacement: " + mass1MaxDisplacement +
                " - Mass 1 Min Displacement: " + mass2MinDisplacement +
                "\nMass 2 Max Displacement: " + mass2MaxDisplacement +
                " - Mass 2 Min Displacement: " + mass2MinDisplacement +
                "\nTotal Simulation Time: " + totalSimulationtime + "s" +
                " - Mass 1 Maximum Displacement Time: " + mass1MaxDisplacementTime +
                "s - Mass 2 Maximum Displacement Time: " + mass2MaxDisplacementTime + "s";
        }

        private void SpringMassForm_Load(object sender, EventArgs e)
        {
            OctaveContext.OctaveSettings.OctaveCliPath = @"C:\Program Files\GNU Octave\Octave-10.3.0\mingw64\bin\octave-cli.exe";
        }
    }
}
