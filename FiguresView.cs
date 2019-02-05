using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Testvision
{
    public class FiguresView : INotifyPropertyChanged
    {
        FiguresModel figuresmodel = new FiguresModel();
        public int Circles
        {
            get
            {
                return figuresmodel.Circles;
            }
            set
            {
                figuresmodel.Circles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Circles"));
            }
        }
        public int Triangles
        {
            get
            {
                return figuresmodel.Triangles;
            }
            set
            {
                figuresmodel.Triangles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Triangles"));
            }
        }
        public int Lines
        {
            get
            {
                return figuresmodel.Lines;
            }
            set
            {
                figuresmodel.Lines = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lines"));
            }
        }
        public int Squares
        {
            get
            {
                return figuresmodel.Squares;
            }
            set
            {
                figuresmodel.Squares = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Squares"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public FiguresView(FiguresModel figuresmodel)
        {
            this.figuresmodel = figuresmodel;
        }
    }
}
