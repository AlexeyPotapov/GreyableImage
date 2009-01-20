using System.Windows;

namespace Test
{
  /// <summary>
  /// Interaction logic for Window3.xaml
  /// </summary>
  public partial class Window3 : Window
  {
    private static DependencyProperty ImageProperty =
      DependencyProperty.Register("Image", typeof(GreyableImage.GreyableImage), typeof(Window3));

    public GreyableImage.GreyableImage Image
    {
      get { return (GreyableImage.GreyableImage)GetValue(ImageProperty); }
      set { SetValue(ImageProperty, value); }
    }

    public Window3()
    {
      InitializeComponent();

      Image = new GreyableImage.GreyableImage("image.png");
    }
  }
}
