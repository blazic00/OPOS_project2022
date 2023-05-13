using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project2022
{
    internal class MetaData
    {
        public static string ProjectPath { get; } = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();
        public static string PixelatedImagesFolderPath { get; set; } = ProjectPath + Path.DirectorySeparatorChar + "pixelatedImages";

        public static string PixelatedImagesFolderPath2 { get; set; } = PixelatedImagesFolderPath;
    }
}
