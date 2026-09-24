using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anbarban.Controls
{
    public partial class NavMenuRow : UserControl
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(NavMenuRow));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(NavMenuRow));

        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public NavMenuRow()
        {
            InitializeComponent();
        }
    }
}
