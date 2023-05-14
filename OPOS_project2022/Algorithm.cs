using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;

namespace OPOS_project2022
{
    internal class Algorithm
    {

        private static int ImageCount { get; set; } = 0;
        private static int PixelationFactor { get; set; } = 4;  

       
        private static Bitmap BitmapFromColorMatrix(Color[][] picture)
        {

            int height = picture[0].Length;
            int width = picture.Length;
            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bmp.SetPixel(i, j, picture[i][j]);
                }
            }
            return bmp;
        }

        private static Color[][] ColorMatrix(Bitmap b1)
        {
            int height = b1.Height; 
            int width = b1.Width;

            Color[][] colorMatrix = new Color[width][];

            for (int i = 0; i < width; i++)
            {
                colorMatrix[i] = new Color[height];
                for (int j = 0; j < height; j++)
                {
                    colorMatrix[i][j] = b1.GetPixel(i, j);
                }

            }
            
            return colorMatrix;
        }

      

        public static void PixelateImages(SemaphoreSlim semaphore, List<Bitmap> images, int parallelLevel = 1, bool SIMD = false, int id = 0)
        {
            //Semaphore semaphore = new Semaphore(parallelLevel, parallelLevel);
            MetaData.PixelatedImagesFolderPath2 = MetaData.PixelatedImagesFolderPath + Path.DirectorySeparatorChar + "task" + id;
            Directory.CreateDirectory(MetaData.PixelatedImagesFolderPath2);
            for (int i = 0; i < images.Count; i++)
            {
                semaphore.Wait();
                if (SIMD)
                {
                    var bmpPix = PixelatedBitmapParallel(images[i]);
                   // SaveImage(bmpPix, id);
                }
                else
                { 
                    var bmpPix= PixelatedBitmap(images[i],PixelationFactor);
                   // SaveImage(bmpPix, id);
                }
                semaphore.Release();
            }
        }

        //Optimisation SIMD and parallel

        private static Bitmap PixelatedBitmapParallel(Bitmap picture)
        {
            List<UInt16[][]> colorMatrices = ColorComponentMatrix(picture);
            Color[][] colorMatrix = PixelatedColorMatrixParallel(colorMatrices);
            var bitmap = BitmapFromColorMatrix(colorMatrix);
            return bitmap;

        }
        private static List<UInt16[][]> ColorComponentMatrix(Bitmap b1)
        {


            int height = b1.Height;
            int width = b1.Width;

            UInt16[][] alpha = new UInt16[width][];
            UInt16[][] red = new UInt16[width][];
            UInt16[][] green = new UInt16[width][];
            UInt16[][] blue = new UInt16[width][];

            for (int i = 0; i < width; i++)
            {
                alpha[i] = new UInt16[height];
                red[i] = new UInt16[height];
                green[i] = new UInt16[height];
                blue[i] = new UInt16[height];


                for (int j = 0; j < height; j++)
                {
                    Color color = b1.GetPixel(i, j);
                    alpha[i][j] = color.A;
                    red[i][j] = color.R;
                    green[i][j] = color.G;
                    blue[i][j] = color.B;
                }

            }

            var matrices = new List<UInt16[][]>(){
                alpha, red, green, blue
            };

            return matrices;

        }
        private static Color[][] PixelatedColorMatrixParallel(List<UInt16[][]> matrices)
        {
            UInt16[][] alpha = matrices[0];
            UInt16[][] red = matrices[1];
            UInt16[][] green = matrices[2];
            UInt16[][] blue = matrices[3];




            Color[][] colorMatrix = new Color[alpha.Length / Vector<UInt16>.Count][];

            Parallel.For(0, alpha.Length / Vector<UInt16>.Count, i =>
            {
                colorMatrix[i] = new Color[alpha[i].Length / Vector<UInt16>.Count];
            });

            Parallel.For(0, alpha.Length / Vector<UInt16>.Count, i =>
            {
                int x = i * Vector<UInt16>.Count;

                Vector<UInt16>[] vectorsAlpha = new Vector<UInt16>[alpha[x].Length / Vector<short>.Count];
                Vector<UInt16>[] vectorsRed = new Vector<UInt16>[alpha[x].Length / Vector<short>.Count];
                Vector<UInt16>[] vectorsGreen = new Vector<UInt16>[alpha[x].Length / Vector<short>.Count];
                Vector<UInt16>[] vectorsBlue = new Vector<UInt16>[alpha[x].Length / Vector<short>.Count];


                Parallel.For(0, alpha[x].Length / Vector<UInt16>.Count, j =>
                {
                    int y = j * Vector<UInt16>.Count;

                    vectorsAlpha[y / Vector<short>.Count] = new Vector<UInt16>(alpha[x], y);
                    vectorsRed[y / Vector<short>.Count] = new Vector<UInt16>(red[x], y);
                    vectorsGreen[y / Vector<short>.Count] = new Vector<UInt16>(green[x], y);
                    vectorsBlue[y / Vector<short>.Count] = new Vector<UInt16>(blue[x], y);


                    for (int k = 1; k < Vector<short>.Count; k++)
                    {
                        vectorsAlpha[y / Vector<short>.Count] += new Vector<UInt16>(alpha[x + k], y);
                        vectorsRed[y / Vector<short>.Count] += new Vector<UInt16>(red[x + k], y);
                        vectorsGreen[y / Vector<short>.Count] += new Vector<UInt16>(green[x + k], y);
                        vectorsBlue[y / Vector<short>.Count] += new Vector<UInt16>(blue[x + k], y);

                    }



                    int alphaAvg = Vector.Sum(vectorsAlpha[y / Vector<short>.Count]) / (Vector<short>.Count * Vector<short>.Count);
                    int redAvg = Vector.Sum(vectorsRed[y / Vector<short>.Count]) / (Vector<short>.Count * Vector<short>.Count);
                    int greenAvg = Vector.Sum(vectorsGreen[y / Vector<short>.Count]) / (Vector<short>.Count * Vector<short>.Count);
                    int blueAvg = Vector.Sum(vectorsBlue[y / Vector<short>.Count]) / (Vector<short>.Count * Vector<short>.Count);

                    colorMatrix[x / Vector<short>.Count][y / Vector<short>.Count] = Color.FromArgb(alphaAvg, redAvg, greenAvg, blueAvg);

                });
            });


            return colorMatrix;
        }

        private static void SaveImage(Bitmap picture,int taskId)
        {
            picture.Save(MetaData.PixelatedImagesFolderPath2 + Path.DirectorySeparatorChar+ "pixelatedPicture" + ImageCount + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            ImageCount++;

        }

        //Single thread using matrices
        private static Bitmap PixelatedBitmap(Bitmap picture,int factor)
        {
            Color[][] colorMatrix = ColorMatrix(picture);
            colorMatrix = PixelatedColorMatrix(colorMatrix, factor);
            var bitmap = BitmapFromColorMatrix(colorMatrix);
            return bitmap;
           
        }

        private static Color[][] PixelatedColorMatrix(Color[][] picture, int factor)
        {
            Color[][] colorMatrix = new Color[picture.Length / factor][];
              
           for(int i = 0; i < picture.Length/factor; i++)  
            {
                colorMatrix[i] = new Color[picture[i].Length / factor];
            }
           
            for(int i = 0; i < picture.Length / factor; i++)
            { 
               for(int j=0; j < picture[i].Length/factor; j++)
                { 
                    int alphaSum = 0;
                    int redSum = 0;
                    int greenSum = 0;
                    int blueSum = 0;

                    for(int k=0; k<factor;k++)
                    {
                        for (int l = 0; l < factor; l++)
                        {
                            alphaSum += picture[i * factor + k][j * factor + l].A;
                            redSum += picture[i * factor + k][j * factor + l].R;
                            greenSum += picture[i * factor + k][j * factor + l].G;
                            blueSum += picture[i * factor + k][j * factor + l].B;
                        }

                    }
                    int alphaAvg = alphaSum / (factor * factor);
                    int redAvg = redSum / (factor * factor);
                    int greenAvg = greenSum / (factor * factor);
                    int blueAvg = blueSum / (factor * factor);

                    colorMatrix[i][j] = Color.FromArgb(alphaAvg, redAvg, greenAvg, blueAvg);

                }
            }
           
            return colorMatrix;
        }


    }



   
}
