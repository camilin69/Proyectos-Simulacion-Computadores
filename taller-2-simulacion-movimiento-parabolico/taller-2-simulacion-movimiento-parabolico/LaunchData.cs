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
        public double TotalTime { get; set; }
        public double MaxHeight { get; set; }
        public double MaxVelocity { get; set; }
        public double LaunchAngle { get; set; }

        // Nuevas propiedades para los cálculos
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double VIX { get; set; }
        public double VIY { get; set; }
        public double VIMagnitude { get; set; }
        public double VIAngle { get; set; }
        public double VFX { get; set; }
        public double VFY { get; set; }
        public double VFMagnitude { get; set; }
        public double VFAngle { get; set; }
        public double VelocityAngleAtMaxHeight { get; set; }

        public List<double> BounceTimes { get; set; }
        public List<Point> TrajectoryPoints { get; set; }
        public int DeltaX { get; set; }
        public int DeltaY { get; set; }

        public LaunchData()
        {
            BounceTimes = new List<double>();
            TrajectoryPoints = new List<Point>();
        }
    }
}
