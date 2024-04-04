using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Lab1;
using System.Security.Permissions;
using System.Runtime.Intrinsics.X86;

namespace Lab1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceimage, int x, int y);
        public Bitmap processImage(Bitmap sourceimage, BackgroundWorker worker)
        {
            Bitmap resultimage = new Bitmap(sourceimage.Width, sourceimage.Height);

            for (int i = 0; i < sourceimage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultimage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceimage.Height; j++)
                {
                    resultimage.SetPixel(i, j, calculateNewPixelColor(sourceimage, i, j));
                }
            }
            return resultimage;

        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            Color sourceColor = sourceimage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            Color sourceColor = sourceimage.GetPixel(x, y);
            int intensity = (int)((float)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B));
            Color resultColor = Color.FromArgb(intensity,
                intensity, intensity);
            return resultColor;
        }
    }

    class Sepiya : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            Color sourceColor = sourceimage.GetPixel(x, y);
            int intensity = (int)((float)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B));
            double resultR = intensity + 2 * 15;
            double resultG = intensity + (0.5 * 15);
            double resultB = intensity - 1 * 15;
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255));
        }
    }

    class AddBrightness : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            Color sourceColor = sourceimage.GetPixel(x, y);
            int resultR = sourceColor.R + 30;
            int resultG = sourceColor.G + 30;
            int resultB = sourceColor.B + 30;
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }
    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceimage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceimage.Width - 1);
                    int idY = Clamp(y + 1, 0, sourceimage.Height - 1);
                    Color neighboColor = sourceimage.GetPixel(idX, idY);
                    resultR += neighboColor.R * kernel[k + radiusX, 1 + radiusY];
                    resultG += neighboColor.G * kernel[k + radiusX, 1 + radiusY];
                    resultB += neighboColor.B * kernel[k + radiusX, 1 + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }
    }

    class GrayWorld : Filters
    {
        int NewR;
        int NewG;
        int NewB;
        int Avg;
        public GrayWorld(Bitmap sourceImage)
        {
            NewR = 0;
            NewG = 0;
            NewB = 0;
            for (int i = 0; i < sourceImage.Height; i++)
            {
                for (int j = 0; j < sourceImage.Width; j++)
                {
                    Color c = sourceImage.GetPixel(j, i);
                    NewR += c.R;
                    NewG += c.G;
                    NewB += c.B;
                }
            }
            NewR = NewR / (sourceImage.Width * sourceImage.Height);
            NewG = NewG / (sourceImage.Height * sourceImage.Width);
            NewB = NewB / (sourceImage.Width * sourceImage.Height);
            Avg = (NewR + NewB + NewG) / 3;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color c = sourceImage.GetPixel(i, j);
            return Color.FromArgb(
                Clamp(c.R / NewR * Avg, 0, 255),
                Clamp(c.G / NewG * Avg, 0, 255),
                Clamp(c.B / NewB * Avg, 0, 255)
                );
        }
    }

    class PerfectReflector : Filters
    {
        int MaxR;
        int MaxG;
        int MaxB;
        public PerfectReflector(Bitmap sourceImage) {
            MaxR = -1;
            MaxG = -1;
            MaxB = -1;
            for (int i = 0; i < sourceImage.Height; i++)
            {
                for (int j = 0; j < sourceImage.Width; j++)
                {
                    Color c = sourceImage.GetPixel(j, i);
                    if (c.R > MaxR)
                    {
                        MaxR = c.R;
                    }
                    if (c.G > MaxG)
                    {
                        MaxG = c.G;
                    }
                    if (c.B > MaxB)
                    {
                        MaxB = c.B;
                    }
                }
            }
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            Color c = sourceImage.GetPixel(i, j);
            return Color.FromArgb(
                Clamp(c.R / MaxR * 255, 0, 255),
                Clamp(c.G / MaxG * 255 , 0, 255),
                Clamp(c.B / MaxB * 255, 0, 255)
                );
        }
    }

    class Sobel : MatrixFilter
    {
        private float[,] kernelX = {
        { -1, 0, 1 },
        { -2, 0, 2 },
        { -1, 0, 1 }
    };

        private float[,] kernelY = {
        { -1, -2, -1 },
        { 0, 0, 0 },
        { 1, 2, 1 }
    };

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float gradientX = CalculateGradient(sourceImage, x, y, kernelX);
            float gradientY = CalculateGradient(sourceImage, x, y, kernelY);

            // Calculate gradient magnitude
            float magnitude = (float)Math.Sqrt(gradientX * gradientX + gradientY * gradientY);

            // Clamp and return the color based on magnitude
            int intensity = Clamp((int)magnitude, 0, 255);
            return Color.FromArgb(intensity, intensity, intensity);
        }

        private float CalculateGradient(Bitmap sourceImage, int x, int y, float[,] kernel)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float result = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    float grayValue = (float)(0.299 * neighborColor.R + 0.587 * neighborColor.G + 0.114 * neighborColor.B);
                    result += grayValue * kernel[k + radiusX, l + radiusY];
                }
            }

            return result;
        }
    }

    class Sharpness : MatrixFilter
    {
        public Sharpness()
        {
            kernel = new float[,]
            {
                { 0, -1, 0 },
                { -1, 5, -1 },
                { 0, -1, 0 }
            };
        }
    }

    class MotionBlur : MatrixFilter
    {
        public MotionBlur()
        {
            kernel = new float[,]
            {
                { 0.2f, 0, 0, 0, 0},
                { 0, 0.2f, 0, 0, 0 },
                { 0, 0, 0.2f, 0, 0 },
                 { 0, 0, 0, 0.2f, 0},
                { 0, 0, 0, 0, 0.2f }
            };
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }

        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }
    }

    class Transfer : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int idi = Clamp(i, 0, sourceImage.Width - 1);
            int idj = Clamp(j + 50, 0, sourceImage.Height - 1);
            if (idj == sourceImage.Height - 1) { return Color.FromArgb(0, 0, 0); }
            Color resultColor = sourceImage.GetPixel(idi, idj);
            return resultColor;
        }
    }

    class Turn : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int x0 = sourceImage.Width / 2;
            int y0 = sourceImage.Height / 2;
            double corner = 1;
            int idI = Clamp((int)((i - x0) * Math.Cos(corner) - (j - y0) * Math.Sin(corner)) + x0, 0, sourceImage.Width - 1);
            int idJ = Clamp((int)((i - x0) * Math.Sin(corner) + (j - y0) * Math.Cos(corner)) + y0, 0, sourceImage.Height - 1);
            if (idJ == sourceImage.Height - 1) { return Color.FromArgb(0, 0, 0); }
            Color resultColor = sourceImage.GetPixel(idI, idJ);
            return resultColor;
        }
    }
    class WaveVertical : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int idI = Clamp(i + (int)(20 * Math.Sin(Math.PI * j / 30)), 0, sourceImage.Width - 1);
            int idJ = Clamp(j, 0, sourceImage.Height - 1);
            if (idJ == sourceImage.Height - 1) { return Color.FromArgb(0, 0, 0); }
            Color resultColor = sourceImage.GetPixel(idI, idJ);
            return resultColor;
        }
    }

    class WaveHorizontal : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int i, int j)
        {
            int idI = Clamp(i + (int)(20 * Math.Sin(Math.PI * i / 15)), 0, sourceImage.Width - 1);
            int idJ = Clamp(j, 0, sourceImage.Height - 1);
            if (idJ == sourceImage.Height - 1) { return Color.FromArgb(0, 0, 0); }
            Color resultColor = sourceImage.GetPixel(idI, idJ);
            return resultColor;
        }
    }
}


