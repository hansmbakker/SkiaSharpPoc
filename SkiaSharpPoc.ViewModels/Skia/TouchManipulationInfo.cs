using SkiaSharp;

namespace SkiaSharpPoc.ViewModels
{
    public class TouchManipulationInfo
    {
        public SKPoint PreviousPoint { set; get; }

        public SKPoint NewPoint { set; get; }
    }
}
