using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WebcamView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // URLs containing the current webcam frames
        private string url1 = "http://viewfinder1.case.edu/image.jpg";
        private string url2 = "http://viewfinder2.case.edu/image.jpg";
        private string url3 = "http://viewfinder3.case.edu/image.jpg";
        CoreDispatcher dispatcher;

        public MainPage()
        {
            this.InitializeComponent();

            // Don't cache the page - we want it to update
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick; 
            // Update the UI every half second
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Start();

        }

        /// <summary>
        /// Update the camera current image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void timer_Tick(object sender, object e)
        {
            await updateImage(cam1, url1);
            await updateImage(cam2, url2);
            await updateImage(cam3, url3);
        }

        private async Task updateImage(Image cameraImage, string url)
        {
            // Download the image into memory as a stream
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage imageResponse = await client.GetAsync(url);

            InMemoryRandomAccessStream randomAccess =
                 new Windows.Storage.Streams.InMemoryRandomAccessStream();

            DataWriter writer =
                new Windows.Storage.Streams.DataWriter(randomAccess.GetOutputStreamAt(0));

            writer.WriteBytes(await imageResponse.Content.ReadAsByteArrayAsync());
            await writer.StoreAsync();

            BitmapImage bit = new BitmapImage();
            await bit.SetSourceAsync(randomAccess);
            cameraImage.Source = bit;   
        }
    }
}
