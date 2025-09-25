using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace taller_2_simulacion_movimiento_parabolico
{
    public partial class Form1 : Form
    {
        private Form2 backForm;

        // Movement variables
        int startMouseX, startMouseY;
        bool isDragging = false;
        int deltaX, deltaY;
        int initialBulletX, initialBulletY, newX, newY;
        double trajectoryX0, trajectoryY0;
        double velocityX, velocityY;
        double gravity = 9.8;
        double t = 0;
        bool stopped = false;

        // Bounce variables
        private int bounceCount = 0;
        private double coefficientOfRestitution = 0.7;
        private bool bouncingLeft = false;

        // Information DAta Grid Variables
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

        // Information Data Grid
        private List<LaunchData> savedLaunches = new List<LaunchData>();
        private int currentLaunchNumber = 1;
        private List<Point> displayedTrajectory = new List<Point>();
        private bool showingSavedTrajectory = false;
        private Pen savedTrajectoryPen = new Pen(Color.Blue, 3);

        // Spawn Target
        private Random random = new Random();

        // Target shotted
        private bool isExploding = false;
        private int explosionFrameCount = 0;

        // Charts
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
        private void DataButton_Click(object sender, EventArgs e)
        {
            if (InfoTabControl.Visible) { 
                InfoTabControl.Visible = false;
                DataButton.BackColor = Color.Gainsboro;
            }
            else { 
                InfoTabControl.Visible = true;
                DataButton.BackColor = Color.LawnGreen;
            }
        }
        private void LaunchManuallyButton_Click(object sender, EventArgs e)
        {
            if (stopped || isExploding || timer1.Enabled) { 
                MessageBox.Show("Presione reinciar para ejecutar otro lanzamiento",
                               "Lanzamiento en espera",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                return;
            }
            if (IsValidDeltaValue(LaunchManualDXTextBox.Text) && IsValidDeltaValue(LaunchManualDYTextBox.Text))
            {
                int dx = int.Parse(LaunchManualDXTextBox.Text);
                int dy = int.Parse(LaunchManualDYTextBox.Text);

                LaunchWithManualValues(dx, dy);
            }
            else
            {
                MessageBox.Show("Por favor ingrese valores numéricos entre 1 y 110 en ambos campos.",
                               "Valores Inválidos",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning);

                if (!IsValidDeltaValue(LaunchManualDXTextBox.Text))
                    LaunchManualDXTextBox.Focus();
                else
                    LaunchManualDYTextBox.Focus();
            }
        }
        private bool IsValidDeltaValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (!int.TryParse(text, out int value))
                return false;

            return value >= 1 && value <= 110;
        }
        private void LaunchWithManualValues(int dx, int dy)
        {

            BulletPictureBox.Visible = true;
            BulletPictureBox.Location = backForm.BulletLocation;
            initialBulletX = backForm.BulletLocation.X;
            initialBulletY = backForm.BulletLocation.Y;

            deltaX = dx;
            deltaY = dy;
            trajectoryX0 = deltaX * -1;
            trajectoryY0 = deltaY * -1;
            velocityX = deltaX;
            velocityY = deltaY;
            t = 0;

            bounceCount = 0;
            bouncingLeft = false;
            maxHeight = 0;
            totalTime = 0;
            bounceTimes.Clear();
            trajectoryPoints.Clear();
            ClearDataLists();
            ResetLaunchVariables();

            DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px";

            timer1.Start();

            Console.WriteLine($"🎯 LANZAMIENTO MANUAL: DeltaX={dx}, DeltaY={dy}");
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
            initialBulletX = BulletPictureBox.Location.X;
            initialBulletY = BulletPictureBox.Location.Y;
            timer1.Interval = 1;
            SpawnTargetRandomly();

        }
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void SpawnTargetRandomly()
        {
            if (TargetPictureBox != null && AreaSpawnPanel != null)
            {
                int maxX = AreaSpawnPanel.Width - TargetPictureBox.Width;
                int maxY = AreaSpawnPanel.Height - TargetPictureBox.Height;

                if (maxX > 0 && maxY > 0)
                {
                    int randomX = random.Next(0, maxX);
                    int randomY = random.Next(0, maxY);

                    TargetPictureBox.Location = new Point(
                        AreaSpawnPanel.Location.X + randomX,
                        AreaSpawnPanel.Location.Y + randomY
                    );
                }
            }
        }
        private void RubyPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (stopped) return;
            if (e.Button == MouseButtons.Left && !isDragging)
            {
                startMouseX = e.X;
                startMouseY = e.Y;
                isDragging = true;
            }
        }
        private void RubyPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (stopped) return;
            if (e.Button == MouseButtons.Left)
            {
                Point formPoint = this.PointToClient(Control.MousePosition);

                if (formPoint.Y > (initialBulletY + startMouseY) && formPoint.X < (initialBulletX + startMouseX))
                {
                    newY = formPoint.Y - startMouseY;
                    newX = formPoint.X - startMouseX;
                    deltaX = initialBulletX - newX;
                    deltaY = newY - initialBulletY;
                    if (deltaX <= 110 && deltaY <= 110)
                    {
                        BulletPictureBox.Location = new Point(newX, newY);
                    }
                    DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px";

                }

            }
        }
        private void RubyPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (stopped) return;
            if (e.Button == MouseButtons.Left)
            {
                Slingshot1PictureBox.Visible = false;
                Slingshot2PictureBox.Visible = true;
                if (deltaX > 110) deltaX = 110;
                if (deltaY > 110) deltaY = 110;
                trajectoryX0 = deltaX * -1;
                trajectoryY0 = deltaY * -1;
                velocityX = deltaX;
                velocityY = deltaY;
                t = 0;
                DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px";

                bounceCount = 0;
                coefficientOfRestitution = 0.7;

                bouncingLeft = false;
                maxHeight = 0;
                maxVelocity = 0;
                totalTime = 0;
                bounceTimes.Clear();
                trajectoryPoints.Clear();
                ClearDataLists();
                ResetLaunchVariables();
                showingSavedTrajectory = false;
                timer1.Start();
            }
            isDragging = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (t > 2 && bounceCount == 0)
            {
                Slingshot1PictureBox.Visible = true;
                Slingshot2PictureBox.Visible = false;
            }
            
            double xt = velocityX * t + trajectoryX0;
            double yt = (-0.5 * gravity * t * t) + (velocityY * t) + trajectoryY0;

            double currentTime = totalTime + t;
            Point newPosition = new Point(initialBulletX + (int)xt, initialBulletY - (int)yt);

            timePoints.Add(currentTime * 2 / 10);
            xPoints.Add(newPosition.X);
            yPoints.Add(this.Height - newPosition.Y);

            double currentVX = velocityX; 
            double currentVY = velocityY - gravity * t;

            vxPoints.Add(currentVX);
            vyPoints.Add(currentVY);

            BulletPictureBox.Location = newPosition;

            Point centerPoint = new Point(
                newPosition.X + BulletPictureBox.Width / 2,
                newPosition.Y + BulletPictureBox.Height / 2
            );
            trajectoryPoints.Add(centerPoint);

            CalculateFlightData();

            if (
                BulletPictureBox.Location.X < -BulletPictureBox.Width ||
                BulletPictureBox.Location.Y > this.ClientSize.Height ||
                bounceCount > 4)
            {
                timer1.Stop();
                CalculateFlightData();
                SaveFlightData();
                bounceCount = 0;
                bouncingLeft = false;
                stopped = true;
            }
            else if (CheckCollision(BulletPictureBox.Bounds))
            {
                bounceTimes.Add(currentTime);
                trajectoryX0 = 0;
                trajectoryY0 = 0;
                HandleBounce(BulletPictureBox.Bounds, ref initialBulletX, ref initialBulletY, ref velocityX, ref velocityY);
            }
            else if (BulletPictureBox.Bounds.IntersectsWith(TargetPictureBox.Bounds))
            {
                StartExplosion();
                stopped = true;

            }

            t += 0.1;
        }
        private bool CheckCollision(Rectangle bullet)
        {
            return bullet.IntersectsWith(GroundPictureBox.Bounds) ||
                   bullet.IntersectsWith(Tree1PictureBox.Bounds) ||
                   bullet.IntersectsWith(Tree2PictureBox.Bounds) ||
                   bullet.IntersectsWith(GridPictureBox.Bounds);
        }
        private void HandleBounce(Rectangle bullet, ref int initialBulletX, ref int initialBulletY, ref double velocityX, ref double velocityY)
        {
            bounceCount++;

            string collisionSide = DetectCollisionSide(bullet);
            switch (collisionSide)
            {
                case "left":
                    initialBulletX = bullet.X - 4;
                    initialBulletY = bullet.Y;
                    velocityX = Math.Abs(velocityX) * -coefficientOfRestitution;
                    velocityY = Math.Abs(velocityY) * -coefficientOfRestitution;
                    bouncingLeft = true;
                    break;
                case "right":
                    initialBulletX = bullet.X + 4;
                    initialBulletY = bullet.Y;
                    velocityX = Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityY) * -coefficientOfRestitution;
                    bouncingLeft = false;
                    break;
                case "top":
                    initialBulletX = bullet.X;
                    initialBulletY = bullet.Y - 4;
                    velocityX = (bouncingLeft) ? Math.Abs(velocityX) * -coefficientOfRestitution : Math.Abs(velocityX) * coefficientOfRestitution;
                    velocityY = Math.Abs(velocityX) * coefficientOfRestitution;
                    break;
            }

            totalTime += t;
            t = 0;
            trajectoryX0 = 0;
            trajectoryY0 = 0;

        }
        private string DetectCollisionSide(Rectangle bullet)
        {

            if (bullet.IntersectsWith(GroundPictureBox.Bounds))
                return GetCollisionSide(bullet, GroundPictureBox.Bounds);
            if (bullet.IntersectsWith(Tree1PictureBox.Bounds))
                return GetCollisionSide(bullet, Tree1PictureBox.Bounds);
            if (bullet.IntersectsWith(Tree2PictureBox.Bounds))
                return GetCollisionSide(bullet, Tree2PictureBox.Bounds);
            if (bullet.IntersectsWith(GridPictureBox.Bounds))
                return GetCollisionSide(bullet, GridPictureBox.Bounds);

            return "screen_edge";
        }
        private string GetCollisionSide(Rectangle bulletBounds, Rectangle obstacleBounds)
        {

            int overlapLeft = bulletBounds.Right - obstacleBounds.Left;
            int overlapRight = obstacleBounds.Right - bulletBounds.Left;
            int overlapTop = bulletBounds.Bottom - obstacleBounds.Top;
            int overlapBottom = obstacleBounds.Bottom - bulletBounds.Top;

            int minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));

            if (minOverlap == overlapLeft) return "left";
            if (minOverlap == overlapRight) return "right";
            if (minOverlap == overlapTop) return "top";
            if (minOverlap == overlapBottom) return "bottom";

            return "unknown";
        }
        private void CalculateFlightData()
        {
            double currentX = BulletPictureBox.Location.X;
            double currentY = BulletPictureBox.Location.Y;

            if (currentX > maxX) maxX = currentX;
            if (currentY > maxY) maxY = currentY;

            double currentHeight = initialBulletY - BulletPictureBox.Location.Y;
            if (currentHeight > maxHeight)
            {
                maxHeight = currentHeight;
            }

            double currentVX = velocityX; 
            double currentVY = velocityY - gravity * t;
            double currentTotalVelocity = Math.Sqrt(currentVX * currentVX + currentVY * currentVY);

            if (currentTotalVelocity > maxVelocity)
            {
                maxVelocity = currentTotalVelocity;
            }

            finalVelocityX = currentVX;
            finalVelocityY = currentVY;
        }
        private void CalculateAllLaunchData(LaunchData launch)
        {
            totalTime = timePoints.Count > 0 ? timePoints[timePoints.Count - 1] : 0;

            if (vxPoints.Count > 0 && vyPoints.Count > 0)
            {
                launch.VIX = vxPoints[0];
                launch.VIY = vyPoints[0];
                launch.VIMagnitude = Math.Sqrt(launch.VIX * launch.VIX + launch.VIY * launch.VIY);
                launch.VIAngle = Math.Atan2(launch.VIY, launch.VIX) * (180 / Math.PI);
            }

            if (vxPoints.Count > 0 && vyPoints.Count > 0)
            {
                launch.VFX = vxPoints[vxPoints.Count - 1];
                launch.VFY = vyPoints[vyPoints.Count - 1];
                launch.VFMagnitude = Math.Sqrt(launch.VFX * launch.VFX + launch.VFY * launch.VFY);
                launch.VFAngle = Math.Atan2(launch.VFY, launch.VFX) * (180 / Math.PI);
            }

            launch.MaxVelocity = FindVelocityAtMaxHeight();

            launch.VelocityAngleAtMaxHeight = FindAngleAtMaxHeight();

            launch.MaxX = (xPoints.Count > 0) ? xPoints.Max() : 0;
            launch.MaxY = (yPoints.Count > 0) ? yPoints.Max() : 0;
            launch.MaxHeight = maxHeight;

            launch.LaunchAngle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);
        }
        private double FindVelocityAtMaxHeight()
        {
            if (vyPoints.Count == 0) return 0;

            double minVy = double.MaxValue;
            int maxHeightIndex = 0;

            for (int i = 0; i < vyPoints.Count; i++)
            {
                if (Math.Abs(vyPoints[i]) < Math.Abs(minVy))
                {
                    minVy = vyPoints[i];
                    maxHeightIndex = i;
                }
            }

            if (maxHeightIndex < vxPoints.Count)
            {
                double vxAtMaxHeight = vxPoints[maxHeightIndex];
                double vyAtMaxHeight = vyPoints[maxHeightIndex];
                return Math.Sqrt(vxAtMaxHeight * vxAtMaxHeight + vyAtMaxHeight * vyAtMaxHeight);
            }

            return 0;
        }
        private double FindAngleAtMaxHeight()
        {
            if (vyPoints.Count == 0) return 0;

            double minVy = double.MaxValue;
            int maxHeightIndex = 0;

            for (int i = 0; i < vyPoints.Count; i++)
            {
                if (Math.Abs(vyPoints[i]) < Math.Abs(minVy))
                {
                    minVy = vyPoints[i];
                    maxHeightIndex = i;
                }
            }

            if (maxHeightIndex < vxPoints.Count && maxHeightIndex < vyPoints.Count)
            {
                double vx = vxPoints[maxHeightIndex];
                double vy = vyPoints[maxHeightIndex];
                return Math.Atan2(vy, vx) * (180 / Math.PI);
            }

            return 0;
        }
        private void SaveFlightData()
        {
            CalculateAllLaunchData(new LaunchData());

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

                MaxX = (xPoints.Count > 0) ? xPoints.Max() : 0,
                MaxY = (yPoints.Count > 0) ? yPoints.Max() : 0,

                VIX = 0,
                VIY = 0,
                VIMagnitude = 0,
                VIAngle = 0,
                VFX = 0,
                VFY = 0,
                VFMagnitude = 0,
                VFAngle = 0,
                VelocityAngleAtMaxHeight = 0,

                TimePoints = new List<double>(timePoints),
                VelocityXPoints = new List<double>(vxPoints),
                VelocityYPoints = new List<double>(vyPoints)
            };

            CalculateAllLaunchData(currentLaunch);

            savedLaunches.Add(currentLaunch);

            InfoDataGridView.Rows.Add(
                currentLaunchNumber.ToString(),
                $"{currentLaunch.TotalTime:F2} s",
                $"{currentLaunch.MaxX:F2} px",
                $"{currentLaunch.MaxY:F2} px",
                $"{currentLaunch.MaxVelocity:F2} px/s",
                $"{currentLaunch.VelocityAngleAtMaxHeight:F1}°",
                $"{currentLaunch.VIX:F2} px/s",
                $"{currentLaunch.VIY:F2} px/s",
                $"{currentLaunch.VIMagnitude:F2} px/s",
                $"{currentLaunch.VIAngle:F1}°",
                $"{currentLaunch.VFX:F2} px/s",
                $"{currentLaunch.VFY:F2} px/s",
                $"{currentLaunch.VFMagnitude:F2} px/s",
                $"{currentLaunch.VFAngle:F1}°"
            );

            currentLaunchNumber++;
            ResetLaunchVariables();
            ClearDataLists();
            totalTime = 0;
            t = 0;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!showingSavedTrajectory && trajectoryPoints.Count > 1)
            {
                for (int i = 0; i < trajectoryPoints.Count - 1; i++)
                {
                    int alpha = 255 - (int)((i / (float)trajectoryPoints.Count) * 200);
                    trajectoryPen.Color = Color.FromArgb(alpha, Color.Yellow);
                    e.Graphics.DrawLine(trajectoryPen, trajectoryPoints[i], trajectoryPoints[i + 1]);
                }
            }

            if (showingSavedTrajectory && displayedTrajectory.Count > 1)
            {
                for (int i = 0; i < displayedTrajectory.Count - 1; i++)
                {
                    int alpha = 200 - (int)((i / (float)displayedTrajectory.Count) * 150);
                    savedTrajectoryPen.Color = Color.FromArgb(alpha, Color.LawnGreen);
                    e.Graphics.DrawLine(savedTrajectoryPen, displayedTrajectory[i], displayedTrajectory[i + 1]);
                }

                if (displayedTrajectory.Count > 0)
                {
                    Point lastPoint = displayedTrajectory[displayedTrajectory.Count - 1];
                    e.Graphics.FillEllipse(Brushes.DarkBlue, lastPoint.X - 4, lastPoint.Y - 4, 8, 8);
                }
            }
        }
        private void ShowSavedTrajectory(LaunchData launch)
        {
            displayedTrajectory = new List<Point>(launch.TrajectoryPoints);
            showingSavedTrajectory = true;
            DeltaXYLabel.Text = $"ΔX: {launch.DeltaX} px, ΔY: {launch.DeltaY} px";
            UpdateChartsWithLaunchData(launch);
            this.Invalidate();
        }
        private void StartExplosion()
        {
            if (isExploding) return;
            timer1.Stop(); 

            BulletPictureBox.Visible = false;
            TargetPictureBox.Visible = false;

            Point explosionCenter = new Point(
                TargetPictureBox.Location.X + TargetPictureBox.Width / 2 - ExplosionPictureBox.Width / 2,
                TargetPictureBox.Location.Y + TargetPictureBox.Height / 2 - ExplosionPictureBox.Height / 2 + 30
            );
            ExplosionPictureBox.Location = explosionCenter;

            ExplosionPictureBox.Visible = true;
            isExploding = true;
            explosionFrameCount = 0;

            Point targetHitCenter = new Point(
                TargetPictureBox.Location.X + TargetPictureBox.Width / 2 - TargetHitPictureBox.Width / 2,
                TargetPictureBox.Location.Y + TargetPictureBox.Height / 2 - TargetHitPictureBox.Height / 2
            );
            TargetHitPictureBox.Location = targetHitCenter;

            TargetHitPictureBox.Visible = true;

            if (!timer2.Enabled)
            {
                timer2.Start();
            }

            ScoreLabel.Text = $"Puntuación: {++score}";
            SaveFlightData();
        }
        private void EndExplosion()
        {
            isExploding = false;
            explosionFrameCount = 0;
            ExplosionPictureBox.Visible = false;
            TargetPictureBox.Visible = true;
            TargetHitPictureBox.Visible = false;
            timer2.Stop();

            BulletPictureBox.Visible = true;
            BulletPictureBox.Location = backForm.BulletLocation;
            initialBulletX = BulletPictureBox.Location.X;
            initialBulletY = BulletPictureBox.Location.Y;

            SpawnTargetRandomly();

            trajectoryPoints.Clear();
            showingSavedTrajectory = false;
            this.Invalidate();
            stopped = false;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (isExploding)
            {
                explosionFrameCount++;

                if (explosionFrameCount >= 30) EndExplosion();
            }
        }
        private void InfoTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabPage2 || e.TabPage == tabPage3 || e.TabPage == tabPage4 || e.TabPage == tabPage5 || e.TabPage == tabPage6) // Cambia "ChartsTabPage" por el nombre real de tu pestaña
            {
                if (InfoDataGridView.SelectedRows.Count == 0 && !showingSavedTrajectory)
                {
                    MessageBox.Show("Por favor, seleccione primero un lanzamiento en la tabla de datos para ver las gráficas.",
                                  "Selección Requerida",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                    e.Cancel = true; 
                }
            }
        }
        private void InfoDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= savedLaunches.Count) return;

            LaunchData selectedLaunch = savedLaunches[e.RowIndex];

            ShowSavedTrajectory(selectedLaunch);
        }
        private void UpdateChartsWithLaunchData(LaunchData launch)
        {
            ClearAllCharts();

            if (launch.TimePoints == null || launch.TimePoints.Count == 0)
                return;

            List<double> velocityPoints = new List<double>();
            List<double> anglePoints = new List<double>(); 

            for (int i = 0; i < Math.Min(launch.VelocityXPoints.Count, launch.VelocityYPoints.Count); i++)
            {
                double velocity = Math.Sqrt(
                    launch.VelocityXPoints[i] * launch.VelocityXPoints[i] +
                    launch.VelocityYPoints[i] * launch.VelocityYPoints[i]
                );
                velocityPoints.Add(velocity);

                double angleRad = Math.Atan2(launch.VelocityYPoints[i], launch.VelocityXPoints[i]);
                double angleDeg = angleRad * (180 / Math.PI);
                anglePoints.Add(angleDeg);
            }

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

            chart.ChartAreas[0].RecalculateAxesScale();
            chart.Invalidate();
        }
        private void HitTargetButton_Click(object sender, EventArgs e)
        {
            if (isExploding || timer1.Enabled || stopped)
            {
                MessageBox.Show("Presione reinciar para ejecutar otro lanzamiento",
                               "Lanzamiento en espera",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                return;
            }
            int solutions = 0;
            initialBulletX = backForm.BulletLocation.X;
            initialBulletY = backForm.BulletLocation.Y;
            bouncingLeft = false;
            Point bullet = new Point(
                initialBulletX,
                initialBulletY
            );

            Point target = new Point(
                TargetPictureBox.Location.X,
                TargetPictureBox.Location.Y
            );

            for (int dx = 1; dx <= 110; dx++) 
            {
                for (int dy = 1; dy <= 110; dy++)
                {
                    if (SimulateTrajectory(bullet, target, dx, dy)) solutions++;
                    if (SimulateTrajectory(bullet, target, dx, dy) && solutions > 20)
                    {
                        deltaX = dx;
                        deltaY = dy;
                        trajectoryX0 = deltaX * -1;
                        trajectoryY0 = deltaY * -1;
                        velocityX = deltaX;
                        velocityY = deltaY;
                        t = 0;

                        bounceCount = 0;
                        maxHeight = 0;
                        bouncingLeft = false;
                        maxHeight = 0;
                        totalTime = 0;
                        bounceTimes.Clear();
                        trajectoryPoints.Clear();
                        ClearDataLists();

                        DeltaXYLabel.Text = $"ΔX: {deltaX} px, ΔY: {deltaY} px";

                        timer1.Start();
                        return;
                    }
                }
            }
            Console.WriteLine("❌ No se encontró trayectoria válida en el rango 1-110");
        }
        private bool SimulateTrajectory(Point start, Point target, int deltaX, int deltaY)
        {
            double simT = 0;
            double simTotalTime = 0;
            int simBounceCount = 0;
            double simTrajectoryX0 = deltaX * -1;
            double simTrajectoryY0 = deltaY * -1;
            double simVelocityX = deltaX;
            double simVelocityY = deltaY;
            int simInitialBulletX = start.X;
            int simInitialBulletY = start.Y;

            Rectangle targetBounds = new Rectangle(
                    target.X, target.Y,
                    TargetPictureBox.Width, TargetPictureBox.Height
            );
            Rectangle currentBounds = new Rectangle(
                start.X, start.Y,
                BulletPictureBox.Width, BulletPictureBox.Height
            );
            

            while (simTotalTime < 30.0)
            {
                double xt = simVelocityX * simT + simTrajectoryX0;
                double yt = (-0.5 * gravity * simT * simT) + (simVelocityY * simT) + simTrajectoryY0;

                Point currentPosition = new Point(
                    simInitialBulletX + (int)xt,
                    simInitialBulletY - (int)yt
                );

                currentBounds.Location = new Point(currentPosition.X, currentPosition.Y);
                if (currentBounds.IntersectsWith(targetBounds))
                {
                    return true;
                }

                if (
                currentBounds.Location.X < -currentBounds.Width ||
                currentBounds.Location.Y > this.ClientSize.Height ||
                simBounceCount > 4)
                {
                    return false;

                } else if (CheckCollision(currentBounds))
                {
                    simBounceCount++;
                    simTrajectoryX0 = 0;
                    simTrajectoryY0 = 0;
                    simT = 0;
                    HandleBounce(currentBounds, ref simInitialBulletX, ref simInitialBulletY, ref simVelocityX, ref simVelocityY);

                }

                simTotalTime += 0.1;
                simT += 0.1; 
            }

            return false; 
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
                initialBulletX = BulletPictureBox.Location.X;
                initialBulletY = BulletPictureBox.Location.Y;
                BulletPictureBox.Visible = true;
            }

            ResetAllVariables();

            TargetPictureBox.Visible = true;
            trajectoryPoints.Clear();
            showingSavedTrajectory = false;
            this.Invalidate();
            SpawnTargetRandomly();
            stopped = false;
            timer2.Stop();
            DeltaXYLabel.Text = $"ΔX:, ΔY:";
        }

        private void ResetAllVariables()
        {
            t = 0;
            totalTime = 0;
            deltaX = 0;
            deltaY = 0;
            trajectoryX0 = 0;
            trajectoryY0 = 0;
            velocityX = 0;
            velocityY = 0;

            bounceCount = 0;
            bouncingLeft = false;
            coefficientOfRestitution = 0.7;
            bounceTimes.Clear();

            maxHeight = 0;
            maxX = 0;
            maxY = 0;
            finalVelocityX = 0;
            finalVelocityY = 0;
            maxVelocity = 0;

            ClearDataLists();
            trajectoryPoints.Clear();
        }
        private void ClearDataLists()
        {
            timePoints.Clear();
            xPoints.Clear();
            yPoints.Clear();
            vxPoints.Clear();
            vyPoints.Clear();
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
