using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using SkiaSharp;
using SkiaSharpPoc.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkiaSharpPoc
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = Ioc.Default.GetService<LaserLayoutViewModel>();

            LoadImagesAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is LaserLayoutViewModel)
            {
                DataContext = e.Parameter;
            }
        }

        public LaserLayoutViewModel ViewModel { get; set; }

        async void LoadImagesAsync()
        {
            var images = new List<Uri>
            {
                new System.Uri("ms-appx:///Assets/LockScreenLogo.scale-200.png"),
                new System.Uri("ms-appx:///Assets/StoreLogo.png"),
            };
            foreach (var uri in images)
            {
                var storagefile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

                Stream fileStream = (await storagefile.OpenAsync(Windows.Storage.FileAccessMode.Read)).AsStreamForRead();

                // decode the bitmap from the stream
                using (var stream = new SKManagedStream(fileStream))
                {
                    var bitmap = SKBitmap.Decode(stream);
                    var renderItem = new RenderItemViewModel
                    {
                        Name = uri.ToString(),
                        RenderItem = new TouchManipulationBitmap(bitmap),
                        X = 40,
                        Y = 60
                    };
                    ViewModel.Layers.Add(renderItem);
                }
            }

            //using (var paint = new SKPaint())
            //{
            //    canvas.DrawBitmap(bitmap, SKRect.Create(Width, Height), paint);
            //}
        }
    }
}
