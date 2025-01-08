using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;


public class MapDrawer
{
    private Graphics _graphics;
    private Brush _brush;
    private Pen _pen;
    public void DrawMap(string pathDest, int width, int height)
    {
        
        using (Bitmap src = new Bitmap(width, height))
        using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppPArgb)) 
        using (_graphics = Graphics.FromImage(bmp)) {
            _graphics.Clear(Color.Black);
            _brush = new SolidBrush(Color.White);
            _pen = new Pen(Color.Green, 2);
            DrawCircle(width/2, height/2, 30);
            bmp.Save(pathDest, ImageFormat.Bmp);
        }
    }

    private void DrawCircle(int centerX, int centerY, int radius)
    {
        _graphics.FillEllipse(_brush, new Rectangle(centerX-radius, centerY-radius, 2*radius, 2*radius));
    }
}
