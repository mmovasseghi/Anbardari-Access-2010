using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anbarban.Controls
{
    public partial class IconLabel : UserControl
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(IconLabel),
                new PropertyMetadata(null, OnIconChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(IconLabel),
                new PropertyMetadata("", OnLabelChanged));

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

        public IconLabel() => InitializeComponent();

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconLabel c) c.ImgIcon.Source = e.NewValue as ImageSource;
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconLabel c) c.TxtLabel.Text = e.NewValue as string ?? "";
        }
    }
}
