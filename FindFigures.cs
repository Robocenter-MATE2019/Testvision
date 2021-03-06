﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Testvision
{
    public class Recognition
    {
       private Mat bin1Mat = new Mat();
       private Mat binMat = new Mat();
       private VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

        public int Circles      { get; set; }
        public int Lines        { get; set; }
        public int Triangles    { get; set; }
        public int Squares      { get; set; }

        public BitmapSource FindFigures(Mat image)
        {
            Mat binImage = Binarization(image);
            VectorOfVectorOfPoint findfigures = Finding_Contours(binImage);
            Circles     = 0;
            Triangles   = 0;
            Squares     = 0;
            Lines       = 0;
            for (int i = 0; i < findfigures.Size; i++)
            {
                if (CvInvoke.ContourArea(findfigures[i]) > 10)
                {
                    if (Is_Circle(findfigures[i]))
                    {
                        CvInvoke.DrawContours(image, findfigures, i, new MCvScalar(0, 0, 255), 3);
                        Circles++;
                    }
                    else if (Is_Rectangle(findfigures[i]))
                    {
                        CvInvoke.DrawContours(image, findfigures, i, new MCvScalar(255, 0, 255), 3);
                        Lines++;
                    }
                    else if (Is_Square(findfigures[i]))
                    {
                        CvInvoke.DrawContours(image, findfigures, i, new MCvScalar(255, 0, 0), 3);
                        Squares++;
                    }
                    else if (Is_Triangle(findfigures[i]))
                    {
                        CvInvoke.DrawContours(image, findfigures, i, new MCvScalar(0, 255, 0), 3);
                        Triangles++;
                    }

                }
            }
            return BitmapSourceConvert.ToBitmapSource(image.ToImage<Bgr, Byte>()); 
        }
        public Mat Binarization(Mat source)
        {
            if (source.IsEmpty) return source;
            /// конвертация изображения в серое
            CvInvoke.CvtColor(source, binMat, ColorConversion.Bgr2Gray);
            /// бинаризация черно-белой картинки
            CvInvoke.Threshold(binMat, bin1Mat, 100, 255, ThresholdType.Binary);
            CvInvoke.Threshold(bin1Mat, bin1Mat, 25, 255, ThresholdType.BinaryInv);
            CvInvoke.Threshold(bin1Mat, bin1Mat, 0, 255, ThresholdType.Otsu);

            return bin1Mat;
        }
        public VectorOfVectorOfPoint Finding_Contours(Mat source)
        {
            if (source.IsEmpty) return null;

            Mat hierarchy = new Mat();
            /// поиск контуров и их отрисовка на изображениии
            CvInvoke.FindContours(bin1Mat, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bin1Mat, contours, -1, new MCvScalar(100, 0, 0), 4);

            return contours;
        }
        public bool Is_Rectangle(VectorOfPoint hull)
        {
            CvInvoke.ApproxPolyDP(hull, hull, 15, true);
            if (hull.Size == 4)
            {
                System.Drawing.Rectangle r = CvInvoke.BoundingRectangle(hull);
                double minr = Math.Min(r.Width, r.Height);
                double maxr = Math.Max(r.Height, r.Width);
                if (minr / maxr < 0.7)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Is_Square(VectorOfPoint hull)
        {
            CvInvoke.ApproxPolyDP(hull, hull, 15, true);
            if (hull.Size == 4)
            {
                System.Drawing.Rectangle r = CvInvoke.BoundingRectangle(hull);
                double minr = Math.Min(r.Width, r.Height);
                double maxr = Math.Max(r.Height, r.Width);
                if (minr / maxr > 0.7)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Is_Circle(VectorOfPoint hull)
        {
            CvInvoke.ApproxPolyDP(hull, hull, 10, true);
            if (hull.Size > 5)
            {
                double area = CvInvoke.ContourArea(hull);
                System.Drawing.Rectangle r = CvInvoke.BoundingRectangle(hull);
                double radius = r.Width / 2;
                if (Math.Abs(1.0 - ((double)r.Width / (double)r.Height)) <= 0.4 && Math.Abs(1.0 - (area / (Math.PI * Math.Pow(radius, 2.0)))) <= 0.4)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Is_Triangle(VectorOfPoint hull)
        {
            CvInvoke.ApproxPolyDP(hull, hull, 10, true);
            if (hull.Size == 3) return true;
            else return false;
        }
    }
}
