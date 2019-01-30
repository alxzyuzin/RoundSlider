using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VolumeControl
{
    public sealed partial class VolumeKnob : UserControl, INotifyPropertyChanged
    {
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, (double)value); }
        }
        public static DependencyProperty MinValueProperty =
                      DependencyProperty.Register("MinValue", typeof(double), typeof(VolumeKnob), null);

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, (double)value); }
        }
        public static DependencyProperty MaxValueProperty =
                      DependencyProperty.Register("MaxValue", typeof(double), typeof(VolumeKnob), null);

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, (double)value); }
        }
        public static DependencyProperty ValueProperty =
                      DependencyProperty.Register("Value", typeof(double), typeof(VolumeKnob), null);

        public double SliderThickness
        {
            get { return (double)GetValue(SliderThicknessProperty); }
            set { SetValue(SliderThicknessProperty, (double)value); }
        }
        public static DependencyProperty SliderThicknessProperty =
                      DependencyProperty.Register(nameof(SliderThickness), typeof(double), typeof(VolumeKnob), null);

        public Brush SliderBrush
        {
            get { return (Brush)GetValue(SliderBrushProperty); }
            set { SetValue(SliderBrushProperty, (Brush)value); }
        }
        public static DependencyProperty SliderBrushProperty =
                      DependencyProperty.Register(nameof(SliderBrush), typeof(Brush), typeof(VolumeKnob), null);

        public Brush SliderBackgroundBrush
        {
            get { return (Brush)GetValue(SliderBackgroundBrushProperty); }
            set { SetValue(SliderBackgroundBrushProperty, (Brush)value); }
        }
        public static DependencyProperty SliderBackgroundBrushProperty =
                      DependencyProperty.Register(nameof(SliderBackgroundBrush), typeof(Brush), typeof(VolumeKnob), null);


        //public bool SliderThickness
        //{
        //    get { return (bool)GetValue(SliderThicknessProperty); }
        //    set { SetValue(SliderThicknessProperty, (bool)value); }

        //}
        //public static DependencyProperty SliderThicknessProperty =
        //              DependencyProperty.Register("SliderThickness", typeof(bool), typeof(VolumeKnob), null);

        private string _mousePosition;
        public string MousePosition
        {
            get { return _mousePosition; }
            set
            {
                if (_mousePosition!= value)
                {
                    _mousePosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MousePosition)));
                }
            }

        }

        private string _angle;
        public string Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Angle)));
                }
            }

        }


        private double _startAngle = 5 * Math.PI / 8;
        private double _endAngle = 3 * Math.PI / 8;

        private double _radius;
        private Point  _center;
        private Size   _size;
        private double _valueAngle;

        public VolumeKnob()
        {
            this.InitializeComponent();

            var color = new Color();
            color.R = 0xFF;
            color.A = 0xFF;
            var brash = new SolidColorBrush();
            brash.Color = color;

         }


        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            _center = new Point() { X = Width / 2, Y = Height / 2 - 4 };
            _radius = (Math.Min(Width, Height) / 2) - SliderThickness - 2;
            _size = new Size(_radius, _radius);
            _valueAngle = 7 * Math.PI / 4 / (MaxValue - MinValue) * Value;

            VolumePath.Data = VolumeGeometry;
            VolumeBackGroundPath.Data = VolumeBackgroundGeometry;
        }

        public void SetVolume()
        {
            Value = Value < MinValue ? MinValue : Value;
            Value = Value > MaxValue ? MaxValue : Value;

        }

        public Geometry VolumeGeometry
        {
            get
            {
                double volumeAngle = _startAngle + _valueAngle;
                bool largeAngle1 = _valueAngle > Math.PI ? true : false;

                Point p0 = new Point(_center.X + _radius * Math.Cos(_startAngle), _center.Y + _radius * Math.Sin(_startAngle));
                Point p1 = new Point(_center.X + _radius * Math.Cos(volumeAngle), _center.Y + _radius * Math.Sin(volumeAngle));

                ArcSegment arcSegment = new ArcSegment()
                {
                    IsLargeArc = largeAngle1,
                    Point = p1,
                    Size = _size,
                    SweepDirection = SweepDirection.Clockwise,
                    RotationAngle = 0.0
                };

                PathFigure pathFigure = new PathFigure()
                {
                    StartPoint = p0,
                    IsClosed = false,
                    IsFilled = false,
                };
                pathFigure.Segments.Add(arcSegment);

                PathGeometry pathGeometry = new PathGeometry() { FillRule = FillRule.Nonzero };
                pathGeometry.Figures.Add(pathFigure);

                return pathGeometry;
            }
        }

        public Geometry VolumeBackgroundGeometry
        {
            get
            {
                double volumeAngle = _startAngle + _valueAngle;
                bool largeAngle = 7 * Math.PI / 4 - _valueAngle > Math.PI ? true : false;

                double radius = (Math.Min(ActualWidth, ActualHeight) / 2) - SliderThickness - 2;
                Size size = new Size(radius, radius);

                Point p0 = new Point(_center.X + radius * Math.Cos(volumeAngle), _center.Y + radius * Math.Sin(volumeAngle));
                Point p1 = new Point(_center.X + radius * Math.Cos(_endAngle), _center.Y + radius * Math.Sin(_endAngle));

                ArcSegment arcSegment = new ArcSegment()
                {
                    IsLargeArc = largeAngle,
                    Point = p1,
                    Size = size,
                    SweepDirection = SweepDirection.Clockwise,
                    RotationAngle = 0.0
                };

                PathFigure pathFigure = new PathFigure()
                {
                    StartPoint = p0,
                    IsClosed = false,
                    IsFilled = false,
                };
                pathFigure.Segments.Add(arcSegment);

                PathGeometry pathGeometry = new PathGeometry() { FillRule = FillRule.Nonzero };
                pathGeometry.Figures.Add(pathFigure);

                return pathGeometry;
            }
        }

        private Point _lastPointerPosition = new Point(-1, -1);

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {

            double aaa = Math.Atan(0.125);
            double bbb = Math.Atan(0.25);
            double ccc = Math.Atan(0.5);
            double ddd = Math.Atan(1);
            Point PointerPosition = e.GetPosition(this);

            _valueAngle =  PointToAngle(PointerPosition);
           // _valueAngle = Math.Atan(_center.Y - PointerPosition.Y  / _center.X - PointerPosition.X );

            VolumePath.Data = VolumeGeometry;
            VolumeBackGroundPath.Data = VolumeBackgroundGeometry;
            // PointerPoint PointerPoint = e.GetCurrentPoint(this);
            // if (Math.Abs(_lastPointerPosition.X - PointerPoint.Position.X) > 10 ||
        }

        private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
//            PointerPoint PointerPoint = e.GetCurrentPoint(this);
//            if (Math.Abs(_lastPointerPosition.X - PointerPoint.Position.X) > 10 ||
//                (_lastPointerPosition.Y - PointerPoint.Position.Y) > 10)
//            {
//                _lastPointerPosition = PointerPoint.Position;

//                double x = PointerPoint.Position.X - _center.X;
//                double y = _center.Y - PointerPoint.Position.Y;
////                MousePosition = $"X:{x.ToString()} Y:{y.ToString()}";
//            }
        }

        private double PointToAngle(Point p)
        {
            double angle =0;
            // Точка лежит в нижнем левом квадранте
            if (p.X < _center.X && p.Y > _center.Y)
            {
                double x = _center.X - p.X;
                double y = p.Y -_center.Y ;
                MousePosition = $"X:{x.ToString()} Y:{y.ToString()}";
                angle = Math.Max(Math.Atan(x / y) - Math.PI/8, Math.PI/8);
                Angle = $"Angle: {angle.ToString()}";
            }


            if (p.X < _center.X && p.Y < _center.Y)
            {
                double x = _center.X - p.X;
                double y = _center.Y - p.Y;
                MousePosition = $"X:{x.ToString()} Y:{y.ToString()}";
                angle = Math.Atan(y / x) + Math.PI - _startAngle;
                Angle = $"Angle: {angle.ToString()}";
            }

            // Точка лежит в верхнем левом квадранте
            //if (n.X < 0 && n.Y >= 0)
            //{
            //    double aaaaa = Math.Atan(Math.Abs(n.Y / n.X));

            //    angle = Math.PI / 4 + Math.Atan(n.Y / n.X);
            //}




            return angle;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Border_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //PointerPoint PointerPoint = e.GetCurrentPoint(this);
            //if (Math.Abs(_lastPointerPosition.X - PointerPoint.Position.X)>10 ||
            //    (_lastPointerPosition.Y - PointerPoint.Position.Y)> 10)
            //{
            //    _lastPointerPosition = PointerPoint.Position;
            //    MousePosition = $"X:{PointerPoint.Position.X.ToString()} Y:{PointerPoint.Position.Y.ToString()}";
            //}
            

           
        }

    
        
    }
}
