using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AssetManager.AssetHelpers
{
    internal class ColorDetector
    {
        public static string GetColor(string imagePath)
        {
            Bitmap bitmap = new Bitmap(imagePath);

            Color backgroundColor = GetDominantEdgeColor(bitmap);

            double threshold = 10.0;

            Dictionary<int, int> colorCounts = new Dictionary<int, int>();

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(ptr, rgbValues, 0, bytes);

            bitmap.UnlockBits(bmpData);

            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            int stride = bmpData.Stride;

            int width = bitmap.Width;
            int height = bitmap.Height;

            int xStart = (int)(width * 0.335);
            int xEnd = (int)(width * 0.665);
            int yStart = (int)(height * 0.335);
            int yEnd = (int)(height * 0.665);

            for (int y = yStart; y < yEnd; y++)
            {
                int yPos = y * stride;
                for (int x = xStart; x < xEnd; x++)
                {
                    int position = yPos + x * bytesPerPixel;

                    byte b = rgbValues[position];
                    byte g = rgbValues[position + 1];
                    byte r = rgbValues[position + 2];
                    byte a = bytesPerPixel == 4 ? rgbValues[position + 3] : (byte)255;

                    Color pixelColor = Color.FromArgb(a, r, g, b);

                    if (IsSimilarColor(pixelColor, backgroundColor, threshold) || pixelColor.A == 0)
                    {
                        continue;
                    }

                    int argb = pixelColor.ToArgb();

                    if (colorCounts.ContainsKey(argb))
                    {
                        colorCounts[argb]++;
                    }
                    else
                    {
                        colorCounts[argb] = 1;
                    }
                }
            }

            if (colorCounts.Count > 0)
            {
                var mostUsedColorArgb = colorCounts.OrderByDescending(c => c.Value).First().Key;
                Color mostUsedColor = Color.FromArgb(mostUsedColorArgb);

                return $"{mostUsedColor.R},{mostUsedColor.G},{mostUsedColor.B}";
            }

            return String.Empty;
    }
       static Color GetDominantEdgeColor(Bitmap bitmap)
        {
            List<Color> edgeColors = new List<Color>();

            int width = bitmap.Width;
            int height = bitmap.Height;
            int edgeThickness = Math.Max(1, Math.Min(width, height) / 10);

            for (int y = 0; y < edgeThickness; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    edgeColors.Add(bitmap.GetPixel(x, y));
                    edgeColors.Add(bitmap.GetPixel(x, height - y - 1));
                }
            }

            for (int x = 0; x < edgeThickness; x++)
            {
                for (int y = edgeThickness; y < height - edgeThickness; y++)
                {
                    edgeColors.Add(bitmap.GetPixel(x, y));
                    edgeColors.Add(bitmap.GetPixel(width - x - 1, y));
                }
            }

            int dominantArgb = edgeColors
                .GroupBy(color => color.ToArgb())
                .OrderByDescending(group => group.Count())
                .First().Key;

            return Color.FromArgb(dominantArgb);
        }


       static bool IsSimilarColor(Color color1, Color color2, double threshold)
        {
            int deltaR = color1.R - color2.R;
            int deltaG = color1.G - color2.G;
            int deltaB = color1.B - color2.B;
            int deltaA = color1.A - color2.A;

            double distance = Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB + deltaA * deltaA);

            return distance < threshold;
        }
    }

}