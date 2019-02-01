using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VolumeControl
{
    public sealed partial class VolumeKnob : UserControl //, INotifyPropertyChanged
    {

        #region Properties
        //        public event PropertyChangedEventHandler PropertyChanged;
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, (double)value); }
        }
        public static DependencyProperty MinimumProperty =
                      DependencyProperty.Register("Minimum", typeof(double), typeof(VolumeKnob), null);

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, (double)value); }
        }
        public static DependencyProperty MaximumProperty =
                      DependencyProperty.Register("Maximum", typeof(double), typeof(VolumeKnob), null);

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, (double)value);
                UpdateSlider(ConvertValueToAngle(value));
            }
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

        public SliderSnapsTo SnapsTo
        {
            get; set;
        }

        public double StepFrequency { get; set; }

        public double TickFrequency { get; set; }

        public TickPlacement TickPlacement { get; set; }


        //public bool SliderThickness
        //{
        //    get { return (bool)GetValue(SliderThicknessProperty); }
        //    set { SetValue(SliderThicknessProperty, (bool)value); }

        //}
        //public static DependencyProperty SliderThicknessProperty =
        //              DependencyProperty.Register("SliderThickness", typeof(bool), typeof(VolumeKnob), null);

        //private string _mousePosition;
        //public string MousePosition
        //{
        //    get { return _mousePosition; }
        //    set
        //    {
        //        if (_mousePosition!= value)
        //        {
        //            _mousePosition = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MousePosition)));
        //        }
        //    }

        //}

        //private string _angle;
        //public string Angle
        //{
        //    get { return _angle; }
        //    set
        //    {
        //        if (_angle != value)
        //        {
        //            _angle = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Angle)));
        //        }
        //    }

        //}

        #endregion

        private const double _startAngle = 5 * Math.PI / 8;
        private const double _endAngle = 3 * Math.PI / 8 +2 * Math.PI;
        private const double _workingRangeAngle = 7 * Math.PI / 4;

        private double _radius = 150 - 4;
        private Point  _center = new Point(150,150);
        private Size   _size = new Size(150,150);


        public VolumeKnob()
        {
            this.InitializeComponent();
            //_center.X = Width / 2; _center.Y = Height / 2;

            //_center.X = Width / 2; _center.Y = Height / 2;
            //_radius = (Math.Min(ActualWidth, ActualHeight) / 2) - SliderThickness - 2;
            //_size.Width = _radius; _size.Height = _radius;
            UpdateSlider(ConvertValueToAngle(Value));

            //var color = new Color();
            //color.R = 0xFF;
            //color.A = 0xFF;
            //var brash = new SolidColorBrush();
            //brash.Color = color;

        }


        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            _center.X = (Width + Margin.Left - Margin.Right) / 2;
            _center.Y = (Height + Margin.Top - Margin.Bottom) / 2;

             _radius = Math.Min(_center.X - (Margin.Left + Margin.Right) / 2,
                               _center.Y - (Margin.Top + Margin.Bottom) / 2
                              ) - SliderThickness - 2;
             _size.Width = _size.Height = _radius;

            double valueAngle = ConvertValueToAngle(Value);
            UpdateSlider(valueAngle);

        }

        public void SetVolume()
        {
            Value = Value < Minimum ? Minimum : Value;
            Value = Value > Maximum ? Maximum : Value;

        }

        private double ConvertValueToAngle(double value)
        {
            return _workingRangeAngle / (Maximum - Minimum) * value;
        }
        
        private double ConvertAngleToValue(double angle)
        {
            return (Maximum - Minimum) / _workingRangeAngle * angle;
        }


        private void UpdateSlider(double valueAngle)
        {
            VolumePath.Data = GetSliderGeometry(_startAngle, _startAngle + valueAngle);
            VolumeBackGroundPath.Data = GetSliderGeometry(_startAngle + valueAngle, _endAngle);
        }

        private Geometry GetSliderGeometry(double startAngle, double endAngle)
        {
                
                bool largeAngle1 = (endAngle -  startAngle) > Math.PI ? true : false;

                Point p0 = new Point(_center.X + _radius * Math.Cos(startAngle), _center.Y + _radius * Math.Sin(startAngle));
                Point p1 = new Point(_center.X + _radius * Math.Cos(endAngle), _center.Y + _radius * Math.Sin(endAngle));

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

        public Geometry VolumeGeometry
        {
            get { return GetSliderGeometry(_startAngle, _startAngle + Math.PI/2); }
        }

        //public Geometry VolumeGeometry
        //{
        //    get
        //    {
        //        double volumeAngle = _startAngle + _valueAngle;
        //        bool largeAngle1 = _valueAngle > Math.PI ? true : false;

        //        Point p0 = new Point(_center.X + _radius * Math.Cos(_startAngle), _center.Y + _radius * Math.Sin(_startAngle));
        //        Point p1 = new Point(_center.X + _radius * Math.Cos(volumeAngle), _center.Y + _radius * Math.Sin(volumeAngle));

        //        ArcSegment arcSegment = new ArcSegment()
        //        {
        //            IsLargeArc = largeAngle1,
        //            Point = p1,
        //            Size = _size,
        //            SweepDirection = SweepDirection.Clockwise,
        //            RotationAngle = 0.0
        //        };

        //        PathFigure pathFigure = new PathFigure()
        //        {
        //            StartPoint = p0,
        //            IsClosed = false,
        //            IsFilled = false,
        //        };
        //        pathFigure.Segments.Add(arcSegment);

        //        PathGeometry pathGeometry = new PathGeometry() { FillRule = FillRule.Nonzero };
        //        pathGeometry.Figures.Add(pathFigure);

        //        return pathGeometry;
        //    }
        //}

        //public Geometry VolumeBackgroundGeometry
        //{
        //    get
        //    {
        //        double volumeAngle = _startAngle + _valueAngle;
        //        bool largeAngle = 7 * Math.PI / 4 - _valueAngle > Math.PI ? true : false;

        //        double radius = (Math.Min(ActualWidth, ActualHeight) / 2) - SliderThickness - 2;
        //        Size size = new Size(radius, radius);

        //        Point p0 = new Point(_center.X + radius * Math.Cos(volumeAngle), _center.Y + radius * Math.Sin(volumeAngle));
        //        Point p1 = new Point(_center.X + radius * Math.Cos(_endAngle), _center.Y + radius * Math.Sin(_endAngle));

        //        ArcSegment arcSegment = new ArcSegment()
        //        {
        //            IsLargeArc = largeAngle,
        //            Point = p1,
        //            Size = size,
        //            SweepDirection = SweepDirection.Clockwise,
        //            RotationAngle = 0.0
        //        };

        //        PathFigure pathFigure = new PathFigure()
        //        {
        //            StartPoint = p0,
        //            IsClosed = false,
        //            IsFilled = false,
        //        };
        //        pathFigure.Segments.Add(arcSegment);

        //        PathGeometry pathGeometry = new PathGeometry() { FillRule = FillRule.Nonzero };
        //        pathGeometry.Figures.Add(pathFigure);

        //        return pathGeometry;
        //    }
        //}

        private Point _lastPointerPosition = new Point(-1, -1);

        private double PointerDeviation(Point pointerPosition)
        {
            return Math.Abs(
                            Math.Sqrt(
                                        Math.Pow(pointerPosition.X - _center.X, 2) +
                                        Math.Pow(pointerPosition.Y - _center.Y, 2)
                                      ) - _radius
                           );
        }

        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point PointerPosition = e.GetPosition(this);

            if (PointerDeviation(PointerPosition) < 40)
            {
                 double valueAngle = PointToAngle(PointerPosition);

                UpdateSlider(valueAngle);
                Value = ConvertAngleToValue(valueAngle);
            }
        }


        private void Border_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(this);

            if (pointerPoint.IsInContact)
            {
                if (PointerDeviation(pointerPoint.Position) < 40)
                {
                    double valueAngle = PointToAngle(pointerPoint.Position);

                    UpdateSlider(valueAngle);
                    Value = ConvertAngleToValue(valueAngle);
                }
            }
        }

        private double PointToAngle(Point p)
        {
            double angle = Math.Atan((p.Y - _center.Y) / (p.X - _center.X));
            if (p.X < _center.X)
                angle += Math.PI;
            if (p.X > _center.X)
                angle += 2 * Math.PI;

            angle -= _startAngle;
            angle = Math.Max(angle,0);
            angle = Math.Min(angle, 7 * Math.PI / 4);
            return angle;
        }



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
