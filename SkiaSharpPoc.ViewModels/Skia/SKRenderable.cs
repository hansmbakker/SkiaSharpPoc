using System;
using SkiaSharp;

namespace SkiaSharpPoc.ViewModels
{
    public readonly struct SKRenderable
    {
        private readonly object skiaObject;
        private readonly Action<SKRenderable, SKCanvas, float, float, SKPaint> renderImpl;

        public SKRenderable(SKBitmap bmp)
        {
            skiaObject = bmp;
            renderImpl = RenderBitmap;
        }

        public SKRenderable(SKImage img)
        {
            skiaObject = img;
            renderImpl = RenderImage;
        }

        public SKRenderable(SKTextBlob text)
        {
            skiaObject = text;
            renderImpl = RenderTextBlob;
        }

        public float Width =>
            skiaObject switch
            {
                SKBitmap bmp => bmp.Width,
                SKImage img => img.Width,
                SKTextBlob text => text.Bounds.Width,
                _ => 0
            };

        public float Height =>
            skiaObject switch
            {
                SKBitmap bmp => bmp.Height,
                SKImage img => img.Height,
                SKTextBlob text => text.Bounds.Height,
                _ => 0
            };

        public void Render(SKCanvas canvas, float x, float y, SKPaint paint = null) =>
            renderImpl(this, canvas, x, y, paint);

        public static implicit operator SKRenderable(SKBitmap bmp) =>
            new SKRenderable(bmp);

        public static implicit operator SKRenderable(SKImage img) =>
            new SKRenderable(img);

        public static implicit operator SKRenderable(SKTextBlob text) =>
            new SKRenderable(text);

        private static void RenderBitmap(SKRenderable renderable, SKCanvas canvas, float x, float y, SKPaint paint) =>
            canvas.DrawBitmap((SKBitmap)renderable.skiaObject, x, y, paint);

        private static void RenderImage(SKRenderable renderable, SKCanvas canvas, float x, float y, SKPaint paint) =>
            canvas.DrawImage((SKImage)renderable.skiaObject, x, y, paint);

        private static void RenderTextBlob(SKRenderable renderable, SKCanvas canvas, float x, float y, SKPaint paint) =>
            canvas.DrawText((SKTextBlob)renderable.skiaObject, x, y, paint);
    }
}
