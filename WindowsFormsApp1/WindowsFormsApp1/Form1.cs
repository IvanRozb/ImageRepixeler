using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private List<Bitmap> bitmaps = new List<Bitmap>();
        private Random random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stopwatch sw = Stopwatch.StartNew();
                menuStrip1.Enabled = trackBar1.Enabled = false;
                pictureBox1.Image = null;
                bitmaps.Clear();
                await Task.Run(() => RunProcessing(new Bitmap(openFileDialog1.FileName)));
                menuStrip1.Enabled = trackBar1.Enabled = true;
                sw.Stop();
                Text = $"{sw.Elapsed} seconds";
            }

        }
        private void RunProcessing(Bitmap bitmap)
        {
            List<Pixel> pixels = GetPixels(bitmap);
            int pixelsInOnePercent = (bitmap.Width * bitmap.Height) / 100;
            List<Pixel> currentPixelSet = new List<Pixel>(pixels.Count - pixelsInOnePercent);
            Text = "0 %";
            for (int i = 1; i < trackBar1.Maximum; i++)
            {
                for (int j = 0; j < pixelsInOnePercent; j++)
                {
                    int index = random.Next(pixels.Count);
                    currentPixelSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }
                Bitmap currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                foreach (Pixel pixel in currentPixelSet)
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                bitmaps.Add(currentBitmap);
                this.Invoke(new Action(() =>
                {
                    Text = $"{i} %";
                }));
            }
            bitmaps.Add(bitmap);
        }

        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            List<Pixel> pixels = new List<Pixel>(bitmap.Width * bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point(x, y)
                    });
                }
            }
            return pixels;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (bitmaps == null || bitmaps.Count == 0)
                return;

            pictureBox1.Image = (trackBar1.Value != 0)?bitmaps[trackBar1.Value- 1]:null; 
        }
    }
}
