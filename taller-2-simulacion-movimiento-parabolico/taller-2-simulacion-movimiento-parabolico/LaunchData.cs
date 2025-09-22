using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taller_2_simulacion_movimiento_parabolico
{
    public class LaunchData
    {
        public int LaunchNumber { get; set; }
        public double MaxHeight { get; set; }
        public double MaxVelocity { get; set; }
        public double TotalTime { get; set; }
        public double LaunchAngle { get; set; }
        public List<double> BounceTimes { get; set; }
        public List<Point> TrajectoryPoints { get; set; }
        public double DeltaX { get; set; }
        public double DeltaY { get; set; }

        public LaunchData()
        {
            BounceTimes = new List<double>();
            TrajectoryPoints = new List<Point>();
        }
    }
}
