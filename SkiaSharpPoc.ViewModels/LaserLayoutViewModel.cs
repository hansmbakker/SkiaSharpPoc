using Microsoft.Toolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaSharpPoc.ViewModels
{
    public class LaserLayoutViewModel : ObservableObject
    {
        public ObservableCollection<RenderItemViewModel> Layers { get; } = new ObservableCollection<RenderItemViewModel>();

        private RenderItemViewModel _selectedLayer;

        public RenderItemViewModel SelectedLayer
        {
            get => _selectedLayer;
            set => SetProperty(ref _selectedLayer, value);
        }
    }

    public class RenderItemViewModel : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private float _x;

        public float X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private float _y;

        public float Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        private TouchManipulationBitmap _renderItem;

        public TouchManipulationBitmap RenderItem
        {
            get => _renderItem;
            set => SetProperty(ref _renderItem, value);
        }
    }
}
