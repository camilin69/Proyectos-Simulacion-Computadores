using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace taller_2_simulacion_movimiento_parabolico
{
    

    public partial class Form1 : Form
    {
        private Form2 backForm;

        int startMouseX, startMouseY;
        bool isDragging = false;
        int deltaX, deltaY;
        int initialRubyX, initialRubyY, newX, newY;
        double trajectoryX0, trajectoryY0;
        double velocityX, velocityY;
        double gravity = 9.8;
        double t = 0;

        private int bounceCount = 0;
        private double coefficientOfRestitution = 0.7;
        private bool bouncingLeft = false;

        private double maxHeight = 0;
        private double maxVelocity = 0;
        private double totalTime = 0;
        private List<double> bounceTimes = new List<double>();
        private List<Point> trajectoryPoints = new List<Point>();
        private Pen trajectoryPen = new Pen(Color.Red, 2);

        private List<LaunchData> savedLaunches = new List<LaunchData>();
        private int currentLaunchNumber = 1;
        private List<Point> displayedTrajectory = new List<Point>();
        private bool showingSavedTrajectory = false;
        private Pen savedTrajectoryPen = new Pen(Color.Blue, 3);

        private PictureBox TargetPictureBox;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (backForm != null)
            {
                backForm.Location = this.Location;
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            RubyPictureBox.Location = backForm.RubyLocation;
            initialRubyX = RubyPictureBox.Location.X;
            initialRubyY = RubyPictureBox.Location.Y;

            // Solo ocultar la trayectoria actual, no borrar las guardadas
            trajectoryPoints.Clear();
            showingSavedTrajectory = false; // ← Esto oculta la trayectoria mostrada
            this.Invalidate();
            SpawnTargetRandomly();
        }

        private void DataButton_Click(object sender, EventArgs e)
        {
            if (InfoTabControl.Visible)
            {
                InfoTabControl.Visible = false;
            }
            else
            {
                InfoTabControl.Visible = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backForm = new Form2()
            {
                StartPosition = FormStartPosition.Manual,
                Size = this.Size,
                Location = this.Location,
                ShowInTaskbar = false
            };
            backForm.Show();
            this.BringToFront();
            AreaSpawnPanel.Location = backForm.AreaSpawnLocation;
            RubyPictureBox.Location = backForm.RubyLocation;
            Slingshot1PictureBox.Location = backForm.Slingshot1Location;
            Slingshot2PictureBox.Location = backForm.Slingshot2Location;
            GroundPictureBox.Location = backForm.GroundLocation;
            Tree1PictureBox.Location = backForm.Tree1Location;
            Tree2PictureBox.Location = backForm.Tree2Location;
            GridPictureBox.Location = backForm.GridLocation;
            AreaSpawnPanel.Size = backForm.AreaSpawnSize;
            RubyPictureBox.Size = backForm.RubySize;
            Slingshot1PictureBox.Size = backForm.Slingshot1Size;
            Slingshot2PictureBox.Size = backForm.Slingshot2Size;
            GroundPictureBox.Size = backForm.GroundSize;
            Tree1PictureBox.Size = backForm.Tree1Size;
            Tree2PictureBox.Size = backForm.Tree2Size;
            GridPictureBox.Size = backForm.GridSize;
            initialRubyX = RubyPictureBox.Location.X;
            initialRubyY = RubyPictureBox.Location.Y;
            timer1.Interval = 1;
            CreateTarget();
        }

        private void CreateTarget()
        {
            // Si ya existe, removerlo
            if (TargetPictureBox != null)
            {
                this.Controls.Remove(TargetPictureBox);
                TargetPictureBox.Dispose();
            }

            // Crear nueva PictureBox para el Target
            TargetPictureBox = new PictureBox();
            TargetPictureBox.Name = "Target";
            TargetPictureBox.Size = new Size(30, 30); // Tamaño del target

            // Asignar una imagen o color (puedes cambiar esto por una imagen real)
            TargetPictureBox.BackColor = Color.Gold;
            TargetPictureBox.BorderStyle = BorderStyle.FixedSingle;

            // Hacerlo circular
            TargetPictureBox.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, TargetPictureBox.Width, TargetPictureBox.Height, 15, 15));

            // Posicionar aleatoriamente dentro del AreaSpawnPanel
            SpawnTargetRandomly();

            // Agregar al formulario
            this.Controls.Add(TargetPictureBox);
            TargetPictureBox.BringToFront();
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void SpawnTargetRandomly()
        {
            if (TargetPictureBox != null && AreaSpawnPanel != null)
            {
                // Calcular posición aleatoria dentro del AreaSpawnPanel
                int maxX = AreaSpawnPanel.Width - TargetPictureBox.Width;
                int maxY = AreaSpawnPanel.Height - TargetPictureBox.Height;

                // Asegurar que no salga de los límites
                if (maxX > 0 && maxY > 0)
                {
                    int randomX = random.Next(0, maxX);
                    int randomY = random.Next(0, maxY);

                    // Posicionar relative al AreaSpawnPanel
                    TargetPictureBox.Location = new Point(
                        AreaSpawnPanel.Location.X + randomX,
                        AreaSpawnPanel.Location.Y + randomY
                    );
                }
            }
        }


        private void RubyPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !isDragging) 
            {
                Slingshot1PictureBox.Visible = false;
                Slingshot2PictureBox.Visible = true;
                startMouseX = e.X;
                startMouseY = e.Y;
                isDragging = true;
            }
        }

        private void RubyPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point formPoint = this.PointToClient(Control.MousePosition);

                if (formPoint.Y > (initialRubyY + startMouseY) && formPoint.X < (initialRubyX + startMouseX))
                {
                    newY = formPoint.Y - startMouseY;
                    newX = formPoint.X - startMouseX;
                    deltaX =  initialRubyX - newX;
                    deltaY =  newY - initialRubyY;
                    DeltaXYLabel.Text = $"Delta X: {deltaX}, Delta Y: {deltaY}";
                    if (deltaX <= 100 &&  deltaY <= 100)
                    {
                        RubyPictureBox.Location = new Point(newX, newY);
                    }
                }
                
            }
        }

        private void RubyPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Slingshot1PictureBox.Visible = true;
                Slingshot2PictureBox.Visible = false;
                if (deltaX >= 100 && deltaY >= 100)
                {
                    deltaX = 100;
                    deltaY = 100;
                }
                trajectoryX0 = deltaX * -1;
                trajectoryY0 = deltaY * -1;
                velocityX = deltaX;
                velocityY = deltaY;
                t = 0;

                // Resetear variables de rebote
                bounceCount = 0;
                coefficientOfRestitution = 0.7;
                BounceCountLabel.Text = "Rebotes: 0/3";

                maxHeight = 0;
                maxVelocity = 0;
                totalTime = 0;
                bounceTimes.Clear();
                trajectoryPoints.Clear();

                showingSavedTrajectory = false;
                timer1.Start();
            }
            isDragging = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            // Movimiento parabólico normal
            double xt = velocityX * t + trajectoryX0;
            double yt = (-0.5 * gravity * t * t) + (velocityY * t) + trajectoryY0;
            Point newPosition = new Point(initialRubyX + (int)xt, initialRubyY - (int)yt);
            RubyPictureBox.Location = newPosition;

            Point centerPoint = new Point(
                newPosition.X + RubyPictureBox.Width / 2,
                newPosition.Y + RubyPictureBox.Height / 2
            );
            trajectoryPoints.Add(centerPoint);

            // Calcular datos en tiempo real
            CalculateFlightData();

            // Detectar colisión
            if (RubyPictureBox.Location.Y < -RubyPictureBox.Height ||
                RubyPictureBox.Location.X < -RubyPictureBox.Width ||
                RubyPictureBox.Location.Y > this.ClientSize.Height ||
                bounceCount > 3)
            {
                timer1.Stop();
                SaveFlightData();
                bounceCount = 0;
                bouncingLeft = false;
            } else if (CheckCollision())
            {
                bounceTimes.Add(totalTime + t);
                trajectoryX0 = 0;
                trajectoryY0 = 0;
                HandleBounce();
            } else if (RubyPictureBox.Bounds.IntersectsWith(TargetPictureBox.Bounds))
            {
                timer1.Stop();
                MessageBox.Show("¡Has alcanzado el objetivo!", "Éxito");
                SaveFlightData();
            }
                totalTime += 0.1;
            t += 0.1;
        }

        private bool CheckCollision()
        {
            return RubyPictureBox.Bounds.IntersectsWith(GroundPictureBox.Bounds) ||
                   RubyPictureBox.Bounds.IntersectsWith(Tree1PictureBox.Bounds) ||
                   RubyPictureBox.Bounds.IntersectsWith(Tree2PictureBox.Bounds) ||
                   RubyPictureBox.Bounds.IntersectsWith(GridPictureBox.Bounds);
        }

        private void HandleBounce()
        {
            bounceCount++;

            // Determinar el lado de colisión
            string collisionSide = DetectCollisionSide();

            // Aplicar rebote según el lado de colisión
            switch (collisionSide)
            {
                case "left":
                    initialRubyX = RubyPictureBox.Location.X - 5;
                    initialRubyY = RubyPictureBox.Location.Y;
                    velocityX = Math.Abs(velocityX) * -coefficientOfRestitution;
                    velocityY = Math.Abs(velocityY) * -coefficientOfRestitution;
                    bouncingLeft = true;
                    break;
                case "right":
                    initialRubyX = RubyPictureBox.Location.X + 5;
                    initialRubyY = RubyPictureBox.Location.Y;
                    velocityX = Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityY) * -coefficientOfRestitution;
                    bouncingLeft = false;
                    break;
                case "top":
                    initialRubyX = RubyPictureBox.Location.X;
                    initialRubyY = RubyPictureBox.Location.Y - 5;
                    velocityX = (bouncingLeft) ? Math.Abs(velocityX) * -coefficientOfRestitution: Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityX) * coefficientOfRestitution;
                    break;
            }

            BounceCountLabel.Text = $"Rebotes: {bounceCount}/3 - Lado: {collisionSide}";

            t = 0;
        }

        private string DetectCollisionSide()
        {
            Rectangle ruby = RubyPictureBox.Bounds;

            if (ruby.IntersectsWith(GroundPictureBox.Bounds))
                return GetCollisionSide(ruby, GroundPictureBox.Bounds);
            if (ruby.IntersectsWith(Tree1PictureBox.Bounds))
                return GetCollisionSide(ruby, Tree1PictureBox.Bounds);
            if (ruby.IntersectsWith(Tree2PictureBox.Bounds))
                return GetCollisionSide(ruby, Tree2PictureBox.Bounds);
            if (ruby.IntersectsWith(GridPictureBox.Bounds))
                return GetCollisionSide(ruby, GridPictureBox.Bounds);

            return "screen_edge";
        }

        private string GetCollisionSide(Rectangle rubyBounds, Rectangle obstacleBounds)
        {

            // Calcular la superposición en X e Y
            int overlapLeft = rubyBounds.Right - obstacleBounds.Left;
            int overlapRight = obstacleBounds.Right - rubyBounds.Left;
            int overlapTop = rubyBounds.Bottom - obstacleBounds.Top;
            int overlapBottom = obstacleBounds.Bottom - rubyBounds.Top;

            // Encontrar la dirección de menor superposición
            int minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));

            if (minOverlap == overlapLeft) return "left";
            if (minOverlap == overlapRight) return "right";
            if (minOverlap == overlapTop) return "top";
            if (minOverlap == overlapBottom) return "bottom";

            return "unknown";
        }

        private void CalculateFlightData()
        {
            // Calcular altura actual DESDE EL CENTRO del Ruby
            double rubyCenterY = RubyPictureBox.Location.Y + RubyPictureBox.Height / 2;
            double currentHeight = initialRubyY - rubyCenterY;

            // Actualizar altura máxima
            if (currentHeight > maxHeight)
            {
                maxHeight = currentHeight;
            }

            // Calcular velocidad actual
            double currentVelocityX = velocityX;
            double currentVelocityY = velocityY - (gravity * t);
            double currentTotalVelocity = Math.Sqrt(currentVelocityX * currentVelocityX + currentVelocityY * currentVelocityY);

            // Actualizar velocidad máxima
            if (currentTotalVelocity > maxVelocity)
            {
                maxVelocity = currentTotalVelocity;
            }
        }

        private void SaveFlightData()
        {
            // Guardar el lanzamiento actual
            LaunchData currentLaunch = new LaunchData
            {
                LaunchNumber = currentLaunchNumber,
                MaxHeight = maxHeight,
                MaxVelocity = maxVelocity,
                TotalTime = totalTime,
                LaunchAngle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI),
                BounceTimes = new List<double>(bounceTimes),
                TrajectoryPoints = new List<Point>(trajectoryPoints),
                DeltaX = deltaX,
                DeltaY = deltaY
            };

            savedLaunches.Add(currentLaunch);

            // Mostrar datos en el DataGridView
            InfoDataGridView.Rows.Add(
                currentLaunchNumber.ToString(),
                $"{maxHeight:F2} px",
                $"{maxVelocity:F2} px/s",
                $"{totalTime:F2} s",
                $"{currentLaunch.LaunchAngle:F1}°"
            );

            currentLaunchNumber++;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Dibujar trayectoria actual (roja) si está activa y no estamos mostrando una guardada
            if (!showingSavedTrajectory && trajectoryPoints.Count > 1)
            {
                for (int i = 0; i < trajectoryPoints.Count - 1; i++)
                {
                    int alpha = 255 - (int)((i / (float)trajectoryPoints.Count) * 200);
                    trajectoryPen.Color = Color.FromArgb(alpha, Color.Red);
                    e.Graphics.DrawLine(trajectoryPen, trajectoryPoints[i], trajectoryPoints[i + 1]);
                }
            }

            // Dibujar trayectoria guardada (azul) si está seleccionada
            if (showingSavedTrajectory && displayedTrajectory.Count > 1)
            {
                for (int i = 0; i < displayedTrajectory.Count - 1; i++)
                {
                    int alpha = 200 - (int)((i / (float)displayedTrajectory.Count) * 150);
                    savedTrajectoryPen.Color = Color.FromArgb(alpha, Color.Blue);
                    e.Graphics.DrawLine(savedTrajectoryPen, displayedTrajectory[i], displayedTrajectory[i + 1]);
                }

                // Dibujar punto final
                if (displayedTrajectory.Count > 0)
                {
                    Point lastPoint = displayedTrajectory[displayedTrajectory.Count - 1];
                    e.Graphics.FillEllipse(Brushes.DarkBlue, lastPoint.X - 4, lastPoint.Y - 4, 8, 8);
                }
            }
        }

        private void InfoDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= savedLaunches.Count) return;

            // Obtener el lanzamiento seleccionado
            LaunchData selectedLaunch = savedLaunches[e.RowIndex];

            // Mostrar la trayectoria guardada
            ShowSavedTrajectory(selectedLaunch);
        }
        private void ShowSavedTrajectory(LaunchData launch)
        {
            // Guardar la trayectoria para dibujar
            displayedTrajectory = new List<Point>(launch.TrajectoryPoints);
            showingSavedTrajectory = true;

            // Forzar redibujado
            this.Invalidate();
        }


    }


}
