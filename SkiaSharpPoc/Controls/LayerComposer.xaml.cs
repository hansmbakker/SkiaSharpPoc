using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using SkiaSharpPoc.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SkiaSharpPoc.Controls
{
    public sealed partial class LayerComposer : UserControl
    {
        public LayerComposer()
        {
            this.InitializeComponent();
            DataContext = Ioc.Default.GetService<LaserLayoutViewModel>();
            ViewModel.Layers.CollectionChanged += Layers_CollectionChanged;

            // get the screen density for scaling
            var display = DisplayInformation.GetForCurrentView();
            _displayScale = display.LogicalDpi / 96.0f;
        }

        public LaserLayoutViewModel ViewModel => (LaserLayoutViewModel)DataContext;

        private float _displayScale;

        private void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _layers = ViewModel.Layers.ToList();
            skXamlCanvas.Invalidate();
            glview.Invalidate();
        }

        private void SKXamlCanvas_PaintSurface(object sender, SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void OnPaintSurface(SKCanvas canvas, int width, int height)
        {
            var scaledSize = new SKSize(width / _displayScale, height / _displayScale);

            // handle the device screen density
            canvas.Scale(_displayScale);

            var canvasWidth = scaledSize.Width;//width;
            var canvasHeight = scaledSize.Height;//height;

            using (var paint = new SKPaint())
            {
                // clear the canvas / fill with white
                canvas.DrawColor(SKColors.White);

                //var RulerWidth = 50;
                //var LargeSteps = 50;
                //var SmallSteps = 10;
                //var drawHorizontalRuler = true;
                //if (drawHorizontalRuler)
                //{
                //    DrawHorizontalRuler(canvas, canvasWidth, RulerWidth, LargeSteps, SmallSteps);
                //}

                //DrawGrid(canvas, canvasWidth, canvasHeight, paint);

                foreach (var layer in _layers)
                {
                    layer.RenderItem.Paint(canvas);
                }
            }
        }

        private void DrawHorizontalRuler(SKCanvas canvas, float canvasWidth, int RulerWidth, int LargeSteps, int SmallSteps)
        {
            using (var paint = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1 * _displayScale,
                TextAlign = SKTextAlign.Center,
            })
            {
                canvas.DrawLine(0, 0, canvasWidth, 0, paint);
                canvas.DrawLine(0, RulerWidth, canvasWidth, RulerWidth, paint);

                for (int x = 0; x < canvasWidth; x += LargeSteps)
                {
                    for (int x1 = x + SmallSteps; x1 < x + LargeSteps; x1 += SmallSteps)
                    {
                        canvas.DrawLine(x1, 0, x1, 10, paint);
                    }
                    canvas.DrawLine(x, 0, x, 20, paint);

                    var typeface = SKTypeface.FromFamilyName(FontFamily.Source);
                    var font = new SKFont(typeface, (float)FontSize * _displayScale);
                    var measurementText = SKTextBlob.Create($"{x}", font);
                    canvas.DrawText(measurementText, x, 30, paint);
                }
            }
        }

        private static void DrawGrid(SKCanvas canvas, float canvasWidth, float canvasHeight, SKPaint paint)
        {
            paint.PathEffect = SKPathEffect.CreateDash(new float[] { 20, 20 }, 0);
            for (int i = 0; i <= 8; i++)
            {
                var x = i * canvasWidth / 8;
                var y = i * canvasHeight / 8;

                int textOffset;
                if (i > 0 && i < 8)
                {
                    canvas.DrawLine(new SKPoint(x, 0), new SKPoint(x, canvasHeight), paint);
                    canvas.DrawLine(new SKPoint(0, y), new SKPoint(canvasWidth, y), paint);
                    textOffset = 0;
                }
                else if (i == 0)
                {
                    textOffset = 15;
                }
                else
                {
                    textOffset = -15;
                }

                var font = new SKFont(SKTypeface.Default, 12.0f);
                var measurementText = SKTextBlob.Create($"{i * 10}", font);
                canvas.DrawText(measurementText, x + textOffset, 10, paint);
                canvas.DrawText(measurementText, 5, y + textOffset, paint);
            }
        }

        private void glview_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.BackendRenderTarget.Width, e.BackendRenderTarget.Height);
        }

        List<long> touchIds = new List<long>();
        private List<RenderItemViewModel> _layers = new List<RenderItemViewModel>();

        private void skXamlCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            HandlePointerEvent(sender, e, TouchActionType.Pressed);
        }


        private void skXamlCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            HandlePointerEvent(sender, e, TouchActionType.Moved);
        }

        private void skXamlCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            HandlePointerEvent(sender, e, TouchActionType.Released);
        }
        private void skXamlCanvas_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            HandlePointerEvent(sender, e, TouchActionType.Cancelled);
        }

        private void HandlePointerEvent(object sender, PointerRoutedEventArgs e, TouchActionType actionType)
        {
            var skXamlCanvas = sender as SKSwapChainPanel;

            // Convert UWP point to pixels
            Point pt = e.GetCurrentPoint(skXamlCanvas).Position;
            SKPoint point =
                new SKPoint((float)(/*_displayScale * */  pt.X),
                            (float)(/*_displayScale * */  pt.Y));

            var selectedLayer = ViewModel.SelectedLayer;
            if (selectedLayer is null)
            {
                return;
            }

            switch (actionType)
            {
                case TouchActionType.Pressed:
                    touchIds.Add(e.Pointer.PointerId);
                    ViewModel.SelectedLayer.RenderItem.ProcessTouchEvent(e.Pointer.PointerId, actionType, point);
                    break;

                case TouchActionType.Moved:
                    if (touchIds.Contains(e.Pointer.PointerId))
                    {
                        ViewModel.SelectedLayer.RenderItem.ProcessTouchEvent(e.Pointer.PointerId, actionType, point);
                        skXamlCanvas.Invalidate();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (touchIds.Contains(e.Pointer.PointerId))
                    {
                        ViewModel.SelectedLayer.RenderItem.ProcessTouchEvent(e.Pointer.PointerId, actionType, point);
                        touchIds.Remove(e.Pointer.PointerId);
                        skXamlCanvas.Invalidate();
                    }
                    break;
            }
        }
    }
}
