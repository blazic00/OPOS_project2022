using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OPOS_project2022
{
    internal class Algorithm
    {

        private static int ImageCount { get; set; } = 0;

       
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

      

        public static void PixelateImages(SemaphoreSlim semaphore, List<Bitmap> images, int parallelLevel = 1, bool SIMD = false)
        {
            //Semaphore semaphore = new Semaphore(parallelLevel, parallelLevel);
            for (int i = 0; i < images.Count; i++)
            {
                semaphore.Wait();
                PixelateImageAndSave(images[i]);
                semaphore.Release();
            }
        }

        //Optimisation SIMD and parallel

        private static Bitmap PixelatedBitmapParallel(Bitmap picture)
        {
            List<UInt16[][]> colorMatrices = ColorComponentMatrix(picture);
            Color[][] colorMatrix = PixelatedColorMatrixParallel(colorMatrices)
            var bitmap = BitmapFromColorMatrix(colorMatrix);
            return bitmap;

        }

        //Single thread using matrices
        private static void PixelateImageAndSave(Bitmap picture)
        {
            var pixelatedBitmap = PixelatedBitmap(picture,4);
            pixelatedBitmap.Save(MetaData.PixelatedImagesFolderPath + Path.DirectorySeparatorChar+ "pixelatedPicture" + ImageCount + ".jpg"
                , System.Drawing.Imaging.ImageFormat.Jpeg);
            ImageCount++;

        }

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


        //Single thread using bitmap

        public static Bitmap PixelatedBitmap2(Bitmap bitmap, int factor)
        {
            Bitmap pixelatedBitMap = new Bitmap(bitmap.Width/factor, bitmap.Height/factor);
          
            for (int i = 0; i < bitmap.Width / factor; i++)
            {

                for (int j = 0; j < bitmap.Height / factor; j++)
                {

                    int alphaSum = 0;
                    int redSum = 0;
                    int greenSum = 0;
                    int blueSum = 0;



                    for (int k = 0; k < factor; k++)
                    {
                        for (int l = 0; l < factor; l++)
                        {
                            alphaSum += bitmap.GetPixel(i * factor + k,j * factor + l).A;
                            redSum += bitmap.GetPixel(i * factor + k, j * factor + l).R;
                            greenSum += bitmap.GetPixel(i * factor + k, j * factor + l).G;
                            blueSum += bitmap.GetPixel(i * factor + k, j * factor + l).B;
                        }
                    }

                    int alphaAvg = alphaSum / (factor * factor);
                    int redAvg = redSum / (factor * factor);
                    int greenAvg = greenSum / (factor * factor);
                    int blueAvg = blueSum / (factor * factor);

                    pixelatedBitMap.SetPixel(i,j,Color.FromArgb(alphaAvg, redAvg, greenAvg, blueAvg));

                }

            }
            return pixelatedBitMap;
        }

    }



   
}
