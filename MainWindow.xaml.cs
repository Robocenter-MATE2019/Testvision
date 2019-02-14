using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Color = System.Drawing.Color;
using System.Text.RegularExpressions;

namespace Testvision
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCapture _capture = new VideoCapture();
        private Recognition Recognition = new Recognition();
        private FiguresView figuresView = new FiguresView(new FiguresModel());

        public MainWindow()
        {
            InitializeComponent();
            _capture.FlipHorizontal = true;
            GoupBox_Grid.DataContext = figuresView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Mat srcMat = new Mat();
            srcMat = CvInvoke.Imread("C:/Users/ASUS/source/repos/Testvision/Testvision/myimage.jpg", ImreadModes.AnyColor);
            if (srcMat.IsEmpty) return;

            Mat bin1Mat = new Mat();
            Mat binMat = new Mat();
            
            CvInvoke.CvtColor(srcMat, binMat, ColorConversion.Bgr2Gray);
            
            CvInvoke.Threshold(binMat, bin1Mat, 100, 255, ThresholdType.Binary);
            
            CvInvoke.Threshold(bin1Mat, bin1Mat, 25, 255, ThresholdType.BinaryInv);
            CvInvoke.Threshold(bin1Mat, bin1Mat, 0, 255, ThresholdType.Otsu);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfVectorOfPoint contours2 = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();

            CvInvoke.FindContours(bin1Mat, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bin1Mat, contours, -1, new MCvScalar(100,0,0), 4);
            CvInvoke.Imshow("source", bin1Mat);
            for (int i = 0; i < contours.Size; ++i)
            {

                if (CvInvoke.ContourArea(contours[i]) > 10)
                {
                    VectorOfPoint hull = new VectorOfPoint();
                    CvInvoke.ConvexHull(contours[i], hull, true);
                    CvInvoke.ApproxPolyDP(hull, hull, 15, true);
                    if (hull.Size == 4)
                    {
                        System.Drawing.Rectangle r = CvInvoke.BoundingRectangle(contours[i]);
                        double minr = Math.Min(r.Width, r.Height);
                        double maxr = Math.Max(r.Height, r.Width);
                        if (minr / maxr > 0.7)
                        {
                            CvInvoke.DrawContours(srcMat, contours, i, new MCvScalar(0, 0, 255), 3);
                            CvInvoke.PutText(srcMat, "Square", new System.Drawing.Point(r.X, r.Y), FontFace.HersheyComplex, 2.0, new MCvScalar(0, 255, 0));
                        }
                        else
                        {
                            CvInvoke.DrawContours(srcMat, contours, i, new MCvScalar(255, 0, 255), 3);
                            CvInvoke.PutText(srcMat, "Line", new System.Drawing.Point(r.X, r.Y), FontFace.HersheyComplex, 2.0, new MCvScalar(0, 255, 255));
                        }
                    }
                    if (hull.Size == 3)
                    {
                        System.Drawing.Rectangle r = CvInvoke.BoundingRectangle(contours[i]);
                        CvInvoke.DrawContours(srcMat, contours, i, new MCvScalar(0, 255, 255), 3);
                        CvInvoke.PutText(srcMat, "Triangle", new System.Drawing.Point(r.X, r.Y), FontFace.HersheyComplex, 2.0, new MCvScalar(255, 0, 255));
                    }
                    if (hull.Size > 5)
                    {
                        double area = CvInvoke.ContourArea(contours[i]);
                        System.Drawing.Rectangle r = CvInvoke.BoundingRectangle(contours[i]);
                        double radius = r.Width / 2;
                        if (Math.Abs(1.0 - ((double)r.Width / (double)r.Height)) <= 0.4 && Math.Abs(1.0 - (area / (Math.PI * Math.Pow(radius, 2.0)))) <= 0.4)
                        {
                            CvInvoke.DrawContours(srcMat, contours, i, new MCvScalar(255, 0, 0), 3);
                            CvInvoke.PutText(srcMat, "Circle", new System.Drawing.Point(r.X, r.Y), FontFace.HersheyComplex, 2.0, new MCvScalar(255, 255, 0));
                        }
                    }
                    contours2.Push(hull);


                    //System.Drawing.Rectangle rc = CvInvoke.BoundingRectangle(contours[i]);
                    //CvInvoke.Rectangle(srcMat, rc, new MCvScalar(0, 0, 255));
                }
            }

            Mat ctrMat = new Mat(binMat.Size, DepthType.Cv8U, 3);
            //CvInvoke.DrawContours(srcMat, contours2, -1, new MCvScalar(100, 0, 255), 4);
            CvInvoke.Imshow("222", srcMat);
            Image1.Source = BitmapSourceConvert.ToBitmapSource(ctrMat.ToImage<Bgr, Byte>());
            
           
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer VideoTimer = new DispatcherTimer();
            VideoTimer.Tick += new EventHandler(VTimer_Tick);
            VideoTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            VideoTimer.Start();
            
        }
        private void VTimer_Tick(object sender, EventArgs e)
        {
            Mat image = _capture.QueryFrame();
            Image1.Source = Recognition.FindFigures(image);
            figuresView.Circles = Recognition.Circles;
            figuresView.Lines = Recognition.Lines;
            figuresView.Triangles = Recognition.Triangles;
            figuresView.Squares = Recognition.Squares;
        }
    }
}
