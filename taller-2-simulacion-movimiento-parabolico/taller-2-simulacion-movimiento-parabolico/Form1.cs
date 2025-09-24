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

        private List<double> timePoints = new List<double>();
        private List<double> xPoints = new List<double>();
        private List<double> yPoints = new List<double>();
        private List<double> vxPoints = new List<double>();
        private List<double> vyPoints = new List<double>();

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
            if (timer1.Enabled) timer1.Stop();
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
            ClearDataLists();
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
                    DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px";
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
                DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px";

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

            double currentTime = totalTime + t;
            Point newPosition = new Point(initialRubyX + (int)xt, initialRubyY - (int)yt);

            // Agregar datos a las listas
            timePoints.Add(currentTime);
            xPoints.Add(newPosition.X);
            yPoints.Add(this.Height - newPosition.Y);

            // Calcular velocidades instantáneas usando las ecuaciones del movimiento
            double currentVX = velocityX; // En movimiento parabólico sin resistencia, Vx es constante
            double currentVY = velocityY - gravity * t; // Vy = Voy - g*t

            vxPoints.Add(currentVX);
            vyPoints.Add(currentVY);

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
                bounceCount > 4)
            {
                timer1.Stop();
                CalculateFlightData();
                SaveFlightData();
                bounceCount = 0;
                bouncingLeft = false;
            }
            else if (CheckCollision())
            {
                bounceTimes.Add(totalTime + t);
                trajectoryX0 = 0;
                trajectoryY0 = 0;
                HandleBounce();
            }
            else if (BulletPictureBox.Bounds.IntersectsWith(TargetPictureBox.Bounds))
            {
                StartExplosion();
            }

            t += 0.1; // Solo incrementamos t, totalTime se mantiene igual hasta que haya rebote
        }

        private void StartExplosion()
        {
            if (isExploding) return;
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
                    velocityX = (bouncingLeft) ? Math.Abs(velocityX) * -coefficientOfRestitution : Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityX) * coefficientOfRestitution;
                    break;
            }

            // Reiniciar el tiempo para el nuevo segmento de trayectoria
            t = 0;
            trajectoryX0 = 0;
            trajectoryY0 = 0;

            // Actualizar el tiempo total (sumar el tiempo transcurrido hasta el rebote)
            totalTime = timePoints.Count > 0 ? timePoints[timePoints.Count - 1] : 0;
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

                // EN LA ALTURA MÁXIMA, CALCULAR LA VELOCIDAD CORRECTA
                // En el punto de máxima altura, Vy = 0, por lo que la velocidad total = Vx
                double velocityAtMaxHeight = Math.Abs(velocityX); // Solo la componente horizontal
                if (velocityAtMaxHeight > maxVelocity)
                {
                    maxVelocity = velocityAtMaxHeight;
                }
            }

            // Calcular velocidad actual
            double currentTotalVelocity = Math.Sqrt(velocityX * velocityX + velocityY * velocityY);

            // Guardar velocidad final para el último cálculo
            finalVelocityX = velocityX;
            finalVelocityY = velocityY;

            // Actualizar velocidad máxima SOLO si es mayor que la velocidad en altura máxima
            // Pero mantener el enfoque principal en la velocidad en altura máxima
            if (currentTotalVelocity > maxVelocity && currentHeight >= maxHeight * 0.9)
            {
                maxVelocity = currentTotalVelocity;
            }
        }

        private void CalculateAllLaunchData(LaunchData launch)
        {
            // Velocidad inicial (VI)
            launch.VIX = deltaX;
            launch.VIY = deltaY;
            launch.VIMagnitude = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            launch.VIAngle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);

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

            // Velocidad en la altura máxima (magnitud)
            double velocityMagnitudeAtMaxHeight = Math.Abs(velocityX); // = √(Vx² + 0²) = |Vx|
            launch.MaxVelocity = velocityMagnitudeAtMaxHeight;
        }

        private void SaveFlightData()
        {
            // Calcular todos los datos antes de guardar
            CalculateAllLaunchData(new LaunchData());

            // Guardar el lanzamiento actual
            LaunchData currentLaunch = new LaunchData
            {
                LaunchNumber = currentLaunchNumber,
                TotalTime = totalTime + t, // Tiempo total hasta el final
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
                VIX = deltaX,
                VIY = deltaY,
                VIMagnitude = Math.Sqrt(deltaX * deltaX + deltaY * deltaY),
                VIAngle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI),
                VFX = finalVelocityX,
                VFY = finalVelocityY,
                VFMagnitude = Math.Sqrt(finalVelocityX * finalVelocityX + finalVelocityY * finalVelocityY),
                VFAngle = Math.Atan2(finalVelocityY, finalVelocityX) * (180 / Math.PI),

                // Datos para los charts
                TimePoints = new List<double>(timePoints),
                VelocityXPoints = new List<double>(vxPoints),
                VelocityYPoints = new List<double>(vyPoints)
            };

            savedLaunches.Add(currentLaunch);

            // Mostrar datos en el DataGridView
            InfoDataGridView.Rows.Add(
                currentLaunchNumber.ToString(),
                $"{currentLaunch.TotalTime:F2} s",
                $"{maxX:F2} px",
                $"{maxY:F2} px",
                $"{maxVelocity:F2} px/s",
                $"{currentLaunch.VelocityAngleAtMaxHeight:F1}°",
                $"{deltaX:F2} px/s",
                $"{deltaY:F2} px/s",
                $"{currentLaunch.VIMagnitude:F2} px/s",
                $"{currentLaunch.VIAngle:F1}°",
                $"{finalVelocityX:F2} px/s",
                $"{finalVelocityY:F2} px/s",
                $"{currentLaunch.VFMagnitude:F2} px/s",
                $"{currentLaunch.VFAngle:F1}°"
            );

            currentLaunchNumber++;

            // Resetear variables para el próximo lanzamiento
            ResetLaunchVariables();
            ClearDataLists();

            // Resetear el tiempo total para el próximo lanzamiento
            totalTime = 0;
            t = 0;
        }

        private void ClearDataLists()
        {
            timePoints.Clear();
            xPoints.Clear();
            yPoints.Clear();
            vxPoints.Clear();
            vyPoints.Clear();
        }

        private void InfoTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabPage2 || e.TabPage == tabPage3 || e.TabPage == tabPage4 || e.TabPage == tabPage5 || e.TabPage == tabPage6) // Cambia "ChartsTabPage" por el nombre real de tu pestaña
            {
                // Verificar si no hay ningún lanzamiento seleccionado en el DataGridView
                if (InfoDataGridView.SelectedRows.Count == 0 && !showingSavedTrajectory)
                {
                    // Mostrar mensaje y cancelar el cambio de pestaña
                    MessageBox.Show("Por favor, seleccione primero un lanzamiento en la tabla de datos para ver las gráficas.",
                                  "Selección Requerida",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                    e.Cancel = true; // Cancela el cambio de pestaña
                }
            }
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
            UpdateChartsWithLaunchData(launch);
            // Forzar redibujado
            this.Invalidate();
        }

        private void UpdateChartsWithLaunchData(LaunchData launch)
        {
            // Limpiar todos los charts primero
            ClearAllCharts();

            // Verificar que hay datos
            if (launch.TimePoints == null || launch.TimePoints.Count == 0)
                return;

            // Calcular datos derivados
            List<double> velocityPoints = new List<double>();
            List<double> anglePoints = new List<double>(); // Ángulo vs Tiempo

            for (int i = 0; i < Math.Min(launch.VelocityXPoints.Count, launch.VelocityYPoints.Count); i++)
            {
                // Velocidad total (magnitud)
                double velocity = Math.Sqrt(
                    launch.VelocityXPoints[i] * launch.VelocityXPoints[i] +
                    launch.VelocityYPoints[i] * launch.VelocityYPoints[i]
                );
                velocityPoints.Add(velocity);

                // Ángulo de la velocidad en grados (respecto al eje X positivo)
                double angleRad = Math.Atan2(launch.VelocityYPoints[i], launch.VelocityXPoints[i]);
                double angleDeg = angleRad * (180 / Math.PI);
                anglePoints.Add(angleDeg);
            }

            // Asegurarnos de que todas las listas tengan la misma longitud
            int minCount = Math.Min(launch.TimePoints.Count, launch.TrajectoryPoints.Count);
            minCount = Math.Min(minCount, velocityPoints.Count);

            List<double> adjustedTimePoints = launch.TimePoints.Take(minCount).ToList();
            List<double> adjustedXPoints = launch.TrajectoryPoints.Take(minCount).Select(p => (double)p.X).ToList();
            List<double> adjustedYPoints = launch.TrajectoryPoints.Take(minCount).Select(p => (this.Height - (double)p.Y)).ToList();
            List<double> adjustedVX = launch.VelocityXPoints.Take(minCount).ToList();
            List<double> adjustedVY = launch.VelocityYPoints.Take(minCount).ToList();
            List<double> adjustedVelocity = velocityPoints.Take(minCount).ToList();
            List<double> adjustedAngle = anglePoints.Take(minCount).ToList();

            LaunchChart2Label.Text = $"Lanzamiento #{launch.LaunchNumber}";
            LaunchChart3Label.Text = $"Lanzamiento #{launch.LaunchNumber}";
            LaunchChart4Label.Text = $"Lanzamiento #{launch.LaunchNumber}";
            LaunchChart5Label.Text = $"Lanzamiento #{launch.LaunchNumber}";
            LaunchChart6Label.Text = $"Lanzamiento #{launch.LaunchNumber}";

            // Actualizar charts
            UpdateChart(XvsTChart, adjustedTimePoints, adjustedXPoints, "Tiempo (s)", "Posición X (px)", "X vs T");
            UpdateChart(YvsTChart, adjustedTimePoints, adjustedYPoints, "Tiempo (s)", "Posición Y (px)", "Y vs T");
            UpdateChart(XvsYChart, adjustedXPoints, adjustedYPoints, "Posición X (px)", "Posición Y (px)", "Trayectoria (X vs Y)");
            UpdateChart(VXvsTChart, adjustedTimePoints, adjustedVX, "Tiempo (s)", "Velocidad X (px/s)", "Vx vs T");
            UpdateChart(VYvsTChart, adjustedTimePoints, adjustedVY, "Tiempo (s)", "Velocidad Y (px/s)", "Vy vs T");
            UpdateChart(VvsTChart, adjustedTimePoints, adjustedVelocity, "Tiempo (s)", "Velocidad (px/s)", "Velocidad vs T");
            UpdateChart(AvsTChart, adjustedTimePoints, adjustedAngle, "Tiempo (s)", "Ángulo (grados)", "Ángulo vs T");
        }

        private void UpdateChart(System.Windows.Forms.DataVisualization.Charting.Chart chart, List<double> xValues, List<double> yValues, string xTitle, string yTitle, string seriesName)
        {
            if (chart == null || xValues == null || yValues == null || xValues.Count == 0 || yValues.Count == 0)
                return;

            chart.Series.Clear();
            chart.ChartAreas.Clear();

            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartArea.AxisX.Title = xTitle;
            chartArea.AxisY.Title = yTitle;
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;

            // Configurar los ejes para que se ajusten automáticamente
            chartArea.AxisX.IsStartedFromZero = false;
            chartArea.AxisY.IsStartedFromZero = false;

            chart.ChartAreas.Add(chartArea);

            var series = new System.Windows.Forms.DataVisualization.Charting.Series();
            series.Name = seriesName;
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series.Color = Color.Blue;
            series.BorderWidth = 2;

            int pointCount = Math.Min(xValues.Count, yValues.Count);
            for (int i = 0; i < pointCount; i++)
            {
                series.Points.AddXY(xValues[i], yValues[i]);
            }

            chart.Series.Add(series);

            // Ajustar la vista del chart para que muestre todos los puntos
            chart.ChartAreas[0].RecalculateAxesScale();
            chart.Invalidate();
        }

        private void ClearAllCharts()
        {
            var charts = new System.Windows.Forms.DataVisualization.Charting.Chart[]
            {
        XvsTChart, YvsTChart, XvsYChart, VXvsTChart, VYvsTChart, VvsTChart, AvsTChart
            };

            foreach (var chart in charts)
            {
                if (chart != null)
                {
                    chart.Series.Clear();
                    chart.ChartAreas.Clear();
                    chart.Invalidate();
                }
            }
        }

        private void HitTargetButton_Click(object sender, EventArgs e)
        {
            if (isExploding || timer1.Enabled) return;

            // Asegurarse de que la bala esté visible y en posición inicial
            if (!BulletPictureBox.Visible)
            {
                BulletPictureBox.Visible = true;
                BulletPictureBox.Location = backForm.BulletLocation;
            }

            // Obtener posiciones clave
            Point targetCenter = new Point(
                TargetPictureBox.Location.X + TargetPictureBox.Width / 2,
                TargetPictureBox.Location.Y + TargetPictureBox.Height / 2
            );

            Point bulletStart = backForm.BulletLocation;
            Point bulletCenter = new Point(
                bulletStart.X + BulletPictureBox.Width / 2,
                bulletStart.Y + BulletPictureBox.Height / 2
            );

            // Calcular distancia al objetivo
            double distanceX = targetCenter.X - bulletCenter.X;
            double distanceY = bulletCenter.Y - targetCenter.Y;

            // Si la distancia es muy pequeña o el objetivo está en posición inválida
            if (Math.Abs(distanceX) < 10 || distanceY <= 0)
            {
                // Usar valores por defecto
                deltaX = 70;
                deltaY = 70;
            }
            else
            {
                // Buscar la mejor trayectoria considerando obstáculos
                LaunchSolution bestSolution = FindOptimalTrajectory(bulletCenter, targetCenter);

                if (bestSolution.IsValid)
                {
                    deltaX = bestSolution.DeltaX;
                    deltaY = bestSolution.DeltaY;
                }
                else
                {
                    // Fallback a valores por defecto
                    deltaX = 70;
                    deltaY = 70;
                }
            }

            // Limitar valores como en el lanzamiento manual
            if (deltaX > 100) deltaX = 100;
            if (deltaY > 100) deltaY = 100;
            if (deltaX < 10) deltaX = 10;
            if (deltaY < 10) deltaY = 10;

            // Configurar parámetros de lanzamiento (IGUAL que en el lanzamiento manual)
            trajectoryX0 = deltaX * -1;
            trajectoryY0 = deltaY * -1;
            velocityX = deltaX;
            velocityY = deltaY;
            t = 0;

            // Resetear variables de la misma manera que en MouseUp
            bounceCount = 0;
            coefficientOfRestitution = 0.7;
            bouncingLeft = false;

            maxHeight = 0;
            maxX = 0;
            maxY = 0;
            finalVelocityX = 0;
            finalVelocityY = 0;
            maxVelocity = 0;
            totalTime = 0;
            bounceTimes.Clear();
            trajectoryPoints.Clear();
            showingSavedTrajectory = false;

            // Limpiar datos anteriores
            ClearDataLists();

            // Posicionar la bala en el punto inicial correctamente
            BulletPictureBox.Location = backForm.BulletLocation;
            initialRubyX = BulletPictureBox.Location.X;
            initialRubyY = BulletPictureBox.Location.Y;

            // Asegurar que la honda esté en estado correcto
            Slingshot1PictureBox.Visible = true;
            Slingshot2PictureBox.Visible = false;

            // Mostrar información
            DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px (Auto)";

            // DEBUG: Verificar que todo esté correcto antes de iniciar
            Console.WriteLine($"Bala visible: {BulletPictureBox.Visible}");
            Console.WriteLine($"Posición bala: {BulletPictureBox.Location}");
            Console.WriteLine($"Timer activo: {timer1.Enabled}");

            // Iniciar el lanzamiento
            timer1.Start();
        }

        private LaunchSolution FindOptimalTrajectory(Point start, Point target)
        {
            List<LaunchSolution> solutions = new List<LaunchSolution>();

            // 1. Intentar trayectoria directa (si es posible)
            LaunchSolution directSolution = CalculateDirectTrajectory(start, target);
            if (directSolution.IsValid && !HasObstacleCollision(directSolution, start))
            {
                directSolution.Score = CalculateTrajectoryScore(directSolution, 0);
                solutions.Add(directSolution);
            }

            // 2. Intentar trayectoria alta (para saltar obstáculos)
            LaunchSolution highSolution = CalculateHighTrajectory(start, target);
            if (highSolution.IsValid && !HasObstacleCollision(highSolution, start))
            {
                highSolution.Score = CalculateTrajectoryScore(highSolution, 0);
                solutions.Add(highSolution);
            }

            // 3. Intentar trayectoria con rebote en el suelo
            LaunchSolution bounceSolution = CalculateBounceTrajectory(start, target);
            if (bounceSolution.IsValid)
            {
                bounceSolution.Score = CalculateTrajectoryScore(bounceSolution, 1); // Penalizar por rebote
                solutions.Add(bounceSolution);
            }

            // 4. Intentar trayectoria con rebote en pared
            LaunchSolution wallBounceSolution = CalculateWallBounceTrajectory(start, target);
            if (wallBounceSolution.IsValid)
            {
                wallBounceSolution.Score = CalculateTrajectoryScore(wallBounceSolution, 2); // Mayor penalización
                solutions.Add(wallBounceSolution);
            }

            // 5. Si no hay buenas soluciones, usar fuerza bruta con ángulos alternativos
            if (solutions.Count == 0)
            {
                LaunchSolution bruteForceSolution = BruteForceTrajectory(start, target);
                if (bruteForceSolution.IsValid)
                {
                    bruteForceSolution.Score = CalculateTrajectoryScore(bruteForceSolution, 3); // Máxima penalización
                    solutions.Add(bruteForceSolution);
                }
            }

            // Elegir la mejor solución (puntuación más alta)
            if (solutions.Count > 0)
            {
                return solutions.OrderByDescending(s => s.Score).First();
            }

            // Fallback: lanzamiento predeterminado
            return new LaunchSolution
            {
                DeltaX = 70,
                DeltaY = 70,
                Type = "Default (Fallback)",
                IsValid = true
            };
        }

        private bool HasObstacleCollision(LaunchSolution solution, Point start)
        {
            // Simular la trayectoria rápidamente para detectar colisiones
            double simulatedT = 0;
            double simulatedGravity = 9.8;

            // Usar las mismas ecuaciones que en timer1_Tick
            double trajectoryX0 = solution.DeltaX * -1;
            double trajectoryY0 = solution.DeltaY * -1;

            Point lastPosition = start;

            // Simular por segmentos de tiempo (más preciso)
            while (simulatedT < 3.0) // Máximo 3 segundos de simulación
            {
                double xt = solution.DeltaX * simulatedT + trajectoryX0;
                double yt = (-0.5 * simulatedGravity * simulatedT * simulatedT) + (solution.DeltaY * simulatedT) + trajectoryY0;

                Point currentPosition = new Point(
                    start.X + (int)xt,
                    start.Y - (int)yt
                );

                // Crear un Rectangle para la posición actual (usar el tamaño real de la bala)
                Rectangle currentBounds = new Rectangle(
                    currentPosition.X,
                    currentPosition.Y,
                    BulletPictureBox.Width,
                    BulletPictureBox.Height
                );

                // Verificar colisión con obstáculos (usar los mismos métodos que en el juego real)
                if (CheckCollisionInSimulation(currentBounds))
                {
                    return true; // Hay colisión con obstáculo
                }

                // Verificar si hemos salido de la pantalla
                if (currentPosition.X < -BulletPictureBox.Width ||
                    currentPosition.X > this.ClientSize.Width ||
                    currentPosition.Y < -BulletPictureBox.Height ||
                    currentPosition.Y > this.ClientSize.Height)
                {
                    break;
                }

                // Verificar si hemos alcanzado el área del objetivo
                if (Math.Abs(currentPosition.X - targetCenter.X) < 50 &&
                    Math.Abs(currentPosition.Y - targetCenter.Y) < 50)
                {
                    break; // Éxito - llegamos cerca del objetivo
                }

                simulatedT += 0.05; // Incremento más pequeño para mayor precisión
                lastPosition = currentPosition;
            }

            return false; // No hay colisión
        }
        private bool CheckCollisionInSimulation(Rectangle bounds)
        {
            // Verificar colisión con los mismos obstáculos que en el juego real
            return bounds.IntersectsWith(GroundPictureBox.Bounds) ||
                   bounds.IntersectsWith(Tree1PictureBox.Bounds) ||
                   bounds.IntersectsWith(Tree2PictureBox.Bounds) ||
                   bounds.IntersectsWith(GridPictureBox.Bounds);
        }

        private LaunchSolution CalculateDirectTrajectory(Point start, Point target)
        {
            double distanceX = target.X - start.X;
            double distanceY = start.Y - target.Y; // Invertir Y

            if (distanceY <= 0)
                return new LaunchSolution { IsValid = false };

            // Calcular ángulo y velocidad óptimos
            double g = gravity;
            double optimalAngle = 45.0 * Math.PI / 180.0;

            double cosTheta = Math.Cos(optimalAngle);
            double tanTheta = Math.Tan(optimalAngle);
            double cosSquared = cosTheta * cosTheta;

            double numerator = g * distanceX * distanceX;
            double denominator = 2 * cosSquared * (distanceY + distanceX * tanTheta);

            if (denominator <= 0)
                return new LaunchSolution { IsValid = false };

            double requiredVelocity = Math.Sqrt(numerator / denominator);

            // Limitar velocidad
            requiredVelocity = Math.Max(20, Math.Min(100, requiredVelocity));

            int deltaX = (int)(requiredVelocity * Math.Cos(optimalAngle));
            int deltaY = (int)(requiredVelocity * Math.Sin(optimalAngle));

            return new LaunchSolution
            {
                DeltaX = Math.Abs(deltaX),
                DeltaY = Math.Abs(deltaY),
                Type = "Direct Shot",
                IsValid = true
            };
        }

        private LaunchSolution CalculateHighTrajectory(Point start, Point target)
        {
            double distanceX = target.X - start.X;
            double distanceY = start.Y - target.Y;

            // Usar un ángulo más alto (60-75 grados) para trayectoria alta
            double highAngle = 75.0 * Math.PI / 180.0;

            // Aumentar la componente Y para ganar altura
            int baseDelta = 70;
            int deltaX = (int)(baseDelta * Math.Cos(highAngle));
            int deltaY = (int)(baseDelta * Math.Sin(highAngle) * 1.5); // Más altura

            // Ajustar según la distancia
            double distanceFactor = Math.Sqrt(distanceX * distanceX + distanceY * distanceY) / 300.0;
            distanceFactor = Math.Max(0.5, Math.Min(2.0, distanceFactor));

            deltaX = (int)(deltaX * distanceFactor);
            deltaY = (int)(deltaY * distanceFactor);

            return new LaunchSolution
            {
                DeltaX = Math.Abs(deltaX),
                DeltaY = Math.Abs(deltaY),
                Type = "High Arc",
                IsValid = true
            };
        }

        private LaunchSolution CalculateBounceTrajectory(Point start, Point target)
        {
            // Calcular un rebote estratégico en el suelo
            double distanceX = target.X - start.X;
            double distanceY = start.Y - target.Y;

            // Para rebote, usar ángulo más bajo y confiar en el rebote
            double lowAngle = 30.0 * Math.PI / 180.0;
            int baseDelta = 80;

            int deltaX = (int)(baseDelta * Math.Cos(lowAngle));
            int deltaY = (int)(baseDelta * Math.Sin(lowAngle) * 0.7); // Menos altura

            return new LaunchSolution
            {
                DeltaX = Math.Abs(deltaX),
                DeltaY = Math.Abs(deltaY),
                Type = "Bounce Shot",
                IsValid = true
            };
        }

        private LaunchSolution CalculateWallBounceTrajectory(Point start, Point target)
        {
            // Para objetivos detrás de obstáculos, intentar rebote en pared
            double distanceX = target.X - start.X;

            // Si el objetivo está a la izquierda, rebotar en pared izquierda
            if (distanceX < 0)
            {
                return new LaunchSolution
                {
                    DeltaX = 60,
                    DeltaY = 50,
                    Type = "Left Wall Bounce",
                    IsValid = true
                };
            }

            return new LaunchSolution
            {
                DeltaX = 60,
                DeltaY = 50,
                Type = "Right Wall Bounce",
                IsValid = true
            };
        }

        private LaunchSolution BruteForceTrajectory(Point start, Point target)
        {
            // Probar varios ángulos y velocidades
            double bestScore = double.MinValue;
            LaunchSolution bestSolution = new LaunchSolution { IsValid = false };

            for (int angle = 30; angle <= 80; angle += 10)
            {
                for (int velocity = 40; velocity <= 100; velocity += 15)
                {
                    double angleRad = angle * Math.PI / 180.0;
                    int deltaX = (int)(velocity * Math.Cos(angleRad));
                    int deltaY = (int)(velocity * Math.Sin(angleRad));

                    var solution = new LaunchSolution
                    {
                        DeltaX = deltaX,
                        DeltaY = deltaY,
                        Type = $"BruteForce {angle}°",
                        IsValid = true
                    };

                    if (!HasObstacleCollision(solution, start))
                    {
                        double score = CalculateTrajectoryScore(solution, 0);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestSolution = solution;
                        }
                    }
                }
            }

            return bestSolution.IsValid ? bestSolution : new LaunchSolution { IsValid = false };
        }

        private double CalculateTrajectoryScore(LaunchSolution solution, int penalty)
        {
            double score = 100.0;

            // Penalizar por número de rebotes necesarios
            score -= penalty * 20;

            // Penalizar por ángulos extremos
            double angle = Math.Atan2(solution.DeltaY, solution.DeltaX) * (180 / Math.PI);
            if (angle < 20 || angle > 80)
                score -= 15;

            // Bonificar por ángulo cercano a 45° (óptimo)
            if (Math.Abs(angle - 45) < 10)
                score += 10;

            return Math.Max(0, score);
        }

        // Clase auxiliar para representar soluciones de lanzamiento
        public class LaunchSolution
        {
            public int DeltaX { get; set; }
            public int DeltaY { get; set; }
            public string Type { get; set; }
            public bool IsValid { get; set; }
            public double Score { get; set; }
        }



    }


}
