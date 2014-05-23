using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfRenderPerformance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            App app = App.Current as App;

            if (app.ow_ == null) {
                app.ow_ = new OverlayWindow();
                app.ow_.Width = 1024.0; // (app.MainWindow as MainWindow).WindowSizeSlider.Value / 10.0 * System.Windows.SystemParameters.PrimaryScreenWidth;
                app.ow_.Height = 768.0; // app.ow_.Width * 480.0 / 640.0;

                app.ow2_ = new OverlayWindow();
                app.ow2_.Width = app.ow_.Width;
                app.ow2_.Height = app.ow_.Height;
                
                DesktopRes.Content = "Desktop Res: " + System.Windows.SystemParameters.PrimaryScreenWidth.ToString() + "x" + System.Windows.SystemParameters.PrimaryScreenHeight.ToString();
                WinWidth.Content = app.ow_.Width.ToString();
                WinHeight.Content = app.ow_.Height.ToString();
                app.ow_.Show();
                app.ow2_.Show();

                // And spin up a busy thread. This is our application computational load simulator.
                // Note: this takes CPU resources, but doesn't try to simulate the computations that we're doing, nor does it
                // create any real memory pressure. In the Real World, things would be worse.
                Thread busyThread = new Thread(delegate()
                    {
                        long i = 0;
                        DateTime last_one_hundred_mm = DateTime.Now;
                        while (true)
                        {
                            i++;
                            if (i == 100000000)
                            {
                                //System.Console.WriteLine("busy thread work units completed/sec " + (1000.0/DateTime.Now.Subtract(last_one_hundred_mm).TotalMilliseconds).ToString());
                                Dispatcher.Invoke(new Action(delegate() {
                                    (app.MainWindow as MainWindow).WorkerUnits.Content = (1000.0 / DateTime.Now.Subtract(last_one_hundred_mm).TotalMilliseconds).ToString();
                                }));
                                last_one_hundred_mm = DateTime.Now;
                                i = 0;
                            }
                        }
                        System.Console.WriteLine("the answer is " + i.ToString());
                    });
                busyThread.Start();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            App app = App.Current as App;

            if (app.ow_ == null) return;

            app.ow_.Close();
            app.ow2_.Close();

            app.ow_ = new OverlayWindow();
            app.ow2_ = new OverlayWindow();
            app.ow_.Width = app.ow2_.Width = 1024; // (app.MainWindow as MainWindow).WindowSizeSlider.Value / 10.0 * System.Windows.SystemParameters.PrimaryScreenWidth;
            app.ow_.Height = app.ow2_.Height = 768; // app.ow_.Width * 480.0 / 640.0;

            if (TransparentChk.IsChecked.GetValueOrDefault(false)) {
                app.ow_.AllowsTransparency = true;
                app.ow2_.AllowsTransparency = true;
            }
            else {
                app.ow_.AllowsTransparency = false;
                app.ow2_.AllowsTransparency = false;
            }

            app.ow_.Show();
            app.ow2_.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App app = App.Current as App;
            app.ow_.Close();
            app.ow2_.Close();
        }

        private void WindowSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App app = App.Current as App;
            if (app.ow_ != null)
            {
                app.ow_.Width = app.ow2_.Width = e.NewValue / 10.0 * System.Windows.SystemParameters.PrimaryScreenWidth;
                app.ow_.Height = app.ow2_.Height = app.ow_.Width * 480.0 / 640.0;
                WinWidth.Content = app.ow_.Width.ToString();
                WinHeight.Content = app.ow_.Height.ToString();
            }
        }

        private void SoftwareRenderChk_Checked(object sender, RoutedEventArgs e)
        {
            App app = App.Current as App;
            HwndSource hwndSource = PresentationSource.FromVisual(app.ow_) as HwndSource;
            HwndTarget hwndTarget = hwndSource.CompositionTarget;
            HwndSource hwndSource2 = PresentationSource.FromVisual(app.ow2_) as HwndSource;
            HwndTarget hwndTarget2 = hwndSource2.CompositionTarget;
            if (SoftwareRenderChk.IsChecked.GetValueOrDefault(false)) {
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;
                hwndTarget2.RenderMode = RenderMode.SoftwareOnly;
            } else {
                hwndTarget.RenderMode = RenderMode.Default;
                hwndTarget2.RenderMode = RenderMode.Default;
            }
        }
    }
}
