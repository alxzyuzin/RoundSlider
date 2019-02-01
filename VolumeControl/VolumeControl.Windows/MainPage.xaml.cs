using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VolumeControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
       

        public event PropertyChangedEventHandler PropertyChanged;

        public VolumeKnob Knob => VK;

        private double _volume;
        public double Volume
        {
            get { return _volume; }
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Volume)));
                }
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            Volume = Knob.Value;


           
          

//            VK.PropertyChanged += VK_PropertyChanged;
        }

        private void VK_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
          //  Volume = Knob.Value;
        }

        private void VolumeIncrease(object sender, TappedRoutedEventArgs e)
        {
            Knob.Value += 10;
            Volume = Knob.Value;
        }

        private void VolumeDecrease(object sender, TappedRoutedEventArgs e)
        {
            Knob.Value -= 10;
            Volume = Knob.Value;
        }
    }
}
