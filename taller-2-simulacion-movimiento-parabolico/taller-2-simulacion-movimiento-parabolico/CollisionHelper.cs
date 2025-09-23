using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace taller_2_simulacion_movimiento_parabolico
{
    public static class CollisionHelper
    {
        public static bool PixelCollision(PictureBox pic1, PictureBox pic2)
        {
            // Obtener los rectángulos de colisión
            Rectangle rect1 = pic1.Bounds;
            Rectangle rect2 = pic2.Bounds;

            // Verificar si los rectángulos se intersectan primero (optimización)
            if (!rect1.IntersectsWith(rect2))
                return false;

            // Encontrar el área de intersección
            Rectangle intersection = Rectangle.Intersect(rect1, rect2);

            // Obtener los bitmaps de las imágenes
            Bitmap bmp1 = new Bitmap(pic1.Image);
            Bitmap bmp2 = new Bitmap(pic2.Image);

            try
            {
                // Verificar colisión píxel por píxel en el área de intersección
                for (int x = intersection.Left; x < intersection.Right; x++)
                {
                    for (int y = intersection.Top; y < intersection.Bottom; y++)
                    {
                        // Convertir coordenadas globales a coordenadas locales de cada imagen
                        Point pointInPic1 = new Point(x - rect1.Left, y - rect1.Top);
                        Point pointInPic2 = new Point(x - rect2.Left, y - rect2.Top);

                        // Verificar si ambos píxeles son opacos (Alpha > 0)
                        if (pointInPic1.X >= 0 && pointInPic1.X < bmp1.Width &&
                            pointInPic1.Y >= 0 && pointInPic1.Y < bmp1.Height &&
                            pointInPic2.X >= 0 && pointInPic2.X < bmp2.Width &&
                            pointInPic2.Y >= 0 && pointInPic2.Y < bmp2.Height)
                        {
                            Color pixel1 = bmp1.GetPixel(pointInPic1.X, pointInPic1.Y);
                            Color pixel2 = bmp2.GetPixel(pointInPic2.X, pointInPic2.Y);

                            if (pixel1.A > 0 && pixel2.A > 0) // Ambos píxeles son visibles
                                return true;
                        }
                    }
                }
            }
            finally
            {
                bmp1.Dispose();
                bmp2.Dispose();
            }

            return false;
        }

        public static bool PixelCollisionWithControl(PictureBox pic, Control control)
        {
            Rectangle rect1 = pic.Bounds;
            Rectangle rect2 = control.Bounds;

            if (!rect1.IntersectsWith(rect2))
                return false;

            Rectangle intersection = Rectangle.Intersect(rect1, rect2);
            Bitmap bmp1 = new Bitmap(pic.Image);

            try
            {
                using (Bitmap bmp2 = new Bitmap(control.Width, control.Height))
                {
                    // Dibujar el control en un bitmap
                    control.DrawToBitmap(bmp2, new Rectangle(0, 0, control.Width, control.Height));

                    for (int x = intersection.Left; x < intersection.Right; x++)
                    {
                        for (int y = intersection.Top; y < intersection.Bottom; y++)
                        {
                            Point pointInPic1 = new Point(x - rect1.Left, y - rect1.Top);
                            Point pointInControl = new Point(x - rect2.Left, y - rect2.Top);

                            if (pointInPic1.X >= 0 && pointInPic1.X < bmp1.Width &&
                                pointInPic1.Y >= 0 && pointInPic1.Y < bmp1.Height &&
                                pointInControl.X >= 0 && pointInControl.X < bmp2.Width &&
                                pointInControl.Y >= 0 && pointInControl.Y < bmp2.Height)
                            {
                                Color pixel1 = bmp1.GetPixel(pointInPic1.X, pointInPic1.Y);
                                Color pixel2 = bmp2.GetPixel(pointInControl.X, pointInControl.Y);

                                // Para controles sin transparencia, considerar cualquier píxel no transparente
                                if (pixel1.A > 0 && pixel2.A > 0)
                                    return true;
                            }
                        }
                    }
                }
            }
            finally
            {
                bmp1.Dispose();
            }

            return false;
        }
    }
}
