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
        private double maxX = 0;
        private double maxY = 0;
        private double finalVelocityX = 0;
        private double finalVelocityY = 0;
        private double maxVelocity = 0;
        private double totalTime = 0;
        private int score = 0;
        private List<double> bounceTimes = new List<double>();
        private List<Point> trajectoryPoints = new List<Point>();
        private Pen trajectoryPen = new Pen(Color.Red, 2);

        private List<LaunchData> savedLaunches = new List<LaunchData>();
        private int currentLaunchNumber = 1;
        private List<Point> displayedTrajectory = new List<Point>();
        private bool showingSavedTrajectory = false;
        private Pen savedTrajectoryPen = new Pen(Color.Blue, 3);

        private Random random = new Random();

        private bool isExploding = false;
        private int explosionFrameCount = 0;

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
            if (isExploding)
            {
                EndExplosion();
            }
            else
            {
                BulletPictureBox.Location = backForm.BulletLocation;
                initialRubyX = BulletPictureBox.Location.X;
                initialRubyY = BulletPictureBox.Location.Y;
                BulletPictureBox.Visible = true;
            }

            // Asegurarse de que el target esté visible
            TargetPictureBox.Visible = true;

            // Solo ocultar la trayectoria actual, no borrar las guardadas
            trajectoryPoints.Clear();
            showingSavedTrajectory = false;
            this.Invalidate();
            SpawnTargetRandomly();

            timer2.Stop();
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
            BulletPictureBox.Location = backForm.BulletLocation;
            Slingshot1PictureBox.Location = backForm.Slingshot1Location;
            Slingshot2PictureBox.Location = backForm.Slingshot2Location;
            GroundPictureBox.Location = backForm.GroundLocation;
            Tree1PictureBox.Location = backForm.Tree1Location;
            Tree2PictureBox.Location = backForm.Tree2Location;
            GridPictureBox.Location = backForm.GridLocation;
            AreaSpawnPanel.Size = backForm.AreaSpawnSize;
            BulletPictureBox.Size = backForm.BulletSize;
            Slingshot1PictureBox.Size = backForm.Slingshot1Size;
            Slingshot2PictureBox.Size = backForm.Slingshot2Size;
            GroundPictureBox.Size = backForm.GroundSize;
            Tree1PictureBox.Size = backForm.Tree1Size;
            Tree2PictureBox.Size = backForm.Tree2Size;
            GridPictureBox.Size = backForm.GridSize;
            initialRubyX = BulletPictureBox.Location.X;
            initialRubyY = BulletPictureBox.Location.Y;
            timer1.Interval = 1;
            SpawnTargetRandomly();

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
                    if (deltaX <= 100 &&  deltaY <= 100)
                    {
                        BulletPictureBox.Location = new Point(newX, newY);
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
                if (deltaX > 100) deltaX = 100;
                if (deltaY > 100) deltaY = 100;
                trajectoryX0 = deltaX * -1;
                trajectoryY0 = deltaY * -1;
                velocityX = deltaX;
                velocityY = deltaY;
                t = 0;

                // Resetear variables de rebote
                bounceCount = 0;
                coefficientOfRestitution = 0.7;

                maxHeight = 0;
                maxVelocity = 0;
                totalTime = 0;
                bounceTimes.Clear();
                trajectoryPoints.Clear();
                ResetLaunchVariables();
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
            BulletPictureBox.Location = newPosition;

            Point centerPoint = new Point(
                newPosition.X + BulletPictureBox.Width / 2,
                newPosition.Y + BulletPictureBox.Height / 2
            );
            trajectoryPoints.Add(centerPoint);

            // Calcular datos en tiempo real
            CalculateFlightData();

            // Detectar colisión
            if (BulletPictureBox.Location.Y < -BulletPictureBox.Height ||
                BulletPictureBox.Location.X < -BulletPictureBox.Width ||
                BulletPictureBox.Location.Y > this.ClientSize.Height ||
                bounceCount > 3)
            {
                timer1.Stop();
                CalculateFlightData();
                SaveFlightData();
                bounceCount = 0;
                bouncingLeft = false;
            } else if (CheckCollision())
            {
                bounceTimes.Add(totalTime + t);
                trajectoryX0 = 0;
                trajectoryY0 = 0;
                HandleBounce();
            } else if (BulletPictureBox.Bounds.IntersectsWith(TargetPictureBox.Bounds))
            {
                StartExplosion();
            }
                totalTime += 0.1;
            t += 0.1;
        }

        private void StartExplosion()
        {
            timer1.Stop(); // Detener el movimiento del ruby

            // Ocultar ruby y target
            BulletPictureBox.Visible = false;
            TargetPictureBox.Visible = false;

            // Posicionar la explosión en el centro del target
            Point explosionCenter = new Point(
                TargetPictureBox.Location.X + TargetPictureBox.Width / 2 - ExplosionPictureBox.Width / 2,
                TargetPictureBox.Location.Y + TargetPictureBox.Height / 2 - ExplosionPictureBox.Height / 2
            );
            ExplosionPictureBox.Location = explosionCenter;

            // Mostrar y iniciar la explosión
            ExplosionPictureBox.Visible = true;
            ExplosionPictureBox.BringToFront();
            isExploding = true;
            explosionFrameCount = 0;

            // Iniciar timer de explosión si no existe
            if (!timer2.Enabled)
            {
                timer2.Start();
            }

            // Actualizar score
            ScoreLabel.Text = $"Score: {++score}";
            SaveFlightData();
        }

        private bool CheckCollision()
        {
            return BulletPictureBox.Bounds.IntersectsWith(GroundPictureBox.Bounds) ||
                   BulletPictureBox.Bounds.IntersectsWith(Tree1PictureBox.Bounds) ||
                   BulletPictureBox.Bounds.IntersectsWith(Tree2PictureBox.Bounds) ||
                   BulletPictureBox.Bounds.IntersectsWith(GridPictureBox.Bounds);
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
                    initialRubyX = BulletPictureBox.Location.X - 4;
                    initialRubyY = BulletPictureBox.Location.Y;
                    velocityX = Math.Abs(velocityX) * -coefficientOfRestitution;
                    velocityY = Math.Abs(velocityY) * -coefficientOfRestitution;
                    bouncingLeft = true;
                    break;
                case "right":
                    initialRubyX = BulletPictureBox.Location.X + 4;
                    initialRubyY = BulletPictureBox.Location.Y;
                    velocityX = Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityY) * -coefficientOfRestitution;
                    bouncingLeft = false;
                    break;
                case "top":
                    initialRubyX = BulletPictureBox.Location.X;
                    initialRubyY = BulletPictureBox.Location.Y - 4;
                    velocityX = (bouncingLeft) ? Math.Abs(velocityX) * -coefficientOfRestitution: Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityX) * coefficientOfRestitution;
                    break;
            }


            t = 0;
        }

        private void EndExplosion()
        {
            isExploding = false;
            explosionFrameCount = 0;
            ExplosionPictureBox.Visible = false;
            TargetPictureBox.Visible = true;
            timer2.Stop();

            // Restaurar ruby y crear nuevo target
            BulletPictureBox.Visible = true;
            BulletPictureBox.Location = backForm.BulletLocation;
            initialRubyX = BulletPictureBox.Location.X;
            initialRubyY = BulletPictureBox.Location.Y;

            // Crear nuevo target
            SpawnTargetRandomly();

            // Limpiar trayectoria
            trajectoryPoints.Clear();
            showingSavedTrajectory = false;
            this.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (isExploding)
            {
                explosionFrameCount++;

                if (explosionFrameCount >= 30) EndExplosion();
            }
        }

        private string DetectCollisionSide()
        {
            Rectangle ruby = BulletPictureBox.Bounds;

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
            // Calcular posición actual
            double currentX = BulletPictureBox.Location.X;
            double currentY = BulletPictureBox.Location.Y;

            // Actualizar máximos de posición
            if (currentX > maxX) maxX = currentX;
            if (currentY > maxY) maxY = currentY;

            // Calcular altura actual DESDE EL CENTRO del Ruby
            double rubyCenterY = BulletPictureBox.Location.Y + BulletPictureBox.Height / 2;
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

            // Guardar velocidad final para el último cálculo
            finalVelocityX = currentVelocityX;
            finalVelocityY = currentVelocityY;

            // Actualizar velocidad máxima
            if (currentTotalVelocity > maxVelocity)
            {
                maxVelocity = currentTotalVelocity;
            }
        }

        private void CalculateAllLaunchData(LaunchData launch)
        {
            // Velocidad inicial (VI)
            launch.VIX = deltaX;
            launch.VIY = deltaY;
            launch.VIMagnitude = Math.Sqrt(velocityX * velocityX + velocityY * velocityY);
            launch.VIAngle = Math.Atan2(velocityY, velocityX) * (180 / Math.PI);

            // Velocidad final (VF) - en el último momento antes de detenerse
            launch.VFX = finalVelocityX;
            launch.VFY = finalVelocityY;
            launch.VFMagnitude = Math.Sqrt(finalVelocityX * finalVelocityX + finalVelocityY * finalVelocityY);
            launch.VFAngle = Math.Atan2(finalVelocityY, finalVelocityX) * (180 / Math.PI);

            // Máximos de posición
            launch.MaxX = maxX;
            launch.MaxY = maxY;

            // Ángulo de la velocidad en el punto de máxima altura
            // En la máxima altura, la componente vertical de velocidad es 0
            double velocityAtMaxHeightX = velocityX; // Componente horizontal constante
            double velocityAtMaxHeightY = 0; // En la máxima altura, Vy = 0
            launch.VelocityAngleAtMaxHeight = Math.Atan2(velocityAtMaxHeightY, velocityAtMaxHeightX) * (180 / Math.PI);
        }

        private void SaveFlightData()
        {
            // Calcular todos los datos antes de guardar
            CalculateAllLaunchData(new LaunchData());

            // Guardar el lanzamiento actual
            LaunchData currentLaunch = new LaunchData
            {
                LaunchNumber = currentLaunchNumber,
                TotalTime = totalTime,
                MaxHeight = maxHeight,
                MaxVelocity = maxVelocity,
                LaunchAngle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI),
                BounceTimes = new List<double>(bounceTimes),
                TrajectoryPoints = new List<Point>(trajectoryPoints),
                DeltaX = deltaX,
                DeltaY = deltaY,

                // Nuevos datos calculados
                MaxX = maxX,
                MaxY = maxY,
                VIX = velocityX,
                VIY = velocityY,
                VIMagnitude = Math.Sqrt(velocityX * velocityX + velocityY * velocityY),
                VIAngle = Math.Atan2(velocityY, velocityX) * (180 / Math.PI),
                VFX = finalVelocityX,
                VFY = finalVelocityY,
                VFMagnitude = Math.Sqrt(finalVelocityX * finalVelocityX + finalVelocityY * finalVelocityY),
                VFAngle = Math.Atan2(finalVelocityY, finalVelocityX) * (180 / Math.PI)
            };

            savedLaunches.Add(currentLaunch);

            // Mostrar datos en el DataGridView (en el orden que especificaste)
            InfoDataGridView.Rows.Add(
                currentLaunchNumber.ToString(),                    // Lanzamiento
                $"{totalTime:F2} s",                              // Tiempo total
                $"{maxX:F2} px",                                  // max X
                $"{maxY:F2} px",                                  // max Y
                $"{maxVelocity:F2} px/s",                         // Velocidad (magnitud) máxima
                $"{currentLaunch.VelocityAngleAtMaxHeight:F1}°",  // Ángulo de la velocidad respecto al eje x (en máxima altura)
                $"{velocityX:F2} px/s",                           // VI X
                $"{velocityY:F2} px/s",                           // VI Y
                $"{currentLaunch.VIMagnitude:F2} px/s",           // VI magnitud
                $"{currentLaunch.VIAngle:F1}°",                   // VI Ángulo
                $"{finalVelocityX:F2} px/s",                      // VF X
                $"{finalVelocityY:F2} px/s",                      // VF Y
                $"{currentLaunch.VFMagnitude:F2} px/s",           // VF magnitud
                $"{currentLaunch.VFAngle:F1}°"                    // VF Ángulo
            );

            currentLaunchNumber++;

            // Resetear variables para el próximo lanzamiento
            ResetLaunchVariables();
        }

        private void ResetLaunchVariables()
        {
            maxX = 0;
            maxY = 0;
            maxHeight = 0;
            maxVelocity = 0;
            finalVelocityX = 0;
            finalVelocityY = 0;
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
