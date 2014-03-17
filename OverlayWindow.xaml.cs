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
using System.Windows.Shapes;

namespace WpfRenderPerformance
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public OverlayWindow()
        {
            InitializeComponent();
        }

        WriteableBitmap wb_source_out_;
        public void OnNewWriteableImage(Byte[] pixel)
        {
            App app = App.Current as App;

            Dispatcher.BeginInvoke(new Action(delegate() {
                if (app.MainWindow != null && (app.MainWindow as MainWindow).RenderChk.IsChecked.GetValueOrDefault(false)) {
                    if (wb_source_out_ == null) {
                        wb_source_out_ = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgra32, null);
                        foo.Source = wb_source_out_;
                    }
                    else if (foo.Source != wb_source_out_) foo.Source = wb_source_out_;

                    (wb_source_out_ as WriteableBitmap).WritePixels(new System.Windows.Int32Rect(0, 0, 640, 480), pixel, 640 * 4, 0);
                }
            }));
        }

        private Thread worker_thread_;
        public WpfRenderHelper.WpfRenderHelper helper_;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            helper_ = new WpfRenderHelper.WpfRenderHelper();
            helper_.NewWriteableImage += OnNewWriteableImage;
            worker_thread_ = new Thread(new ThreadStart(helper_.Worker));
            worker_thread_.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            worker_thread_.Abort();
        }
    }
}
