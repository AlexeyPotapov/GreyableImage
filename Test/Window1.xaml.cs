
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Test
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
      if ((sender as CheckBox).IsChecked == true)
      {
        Color c1 = new Color();
        c1.A = 0;
        Color c2 = new Color();
        c2.A = 0xAA;

        img1.OpacityMask = img2.OpacityMask = img3.OpacityMask = img4.OpacityMask = new LinearGradientBrush(c1, c2, 90);
      }
      else
        img1.OpacityMask = img2.OpacityMask = img3.OpacityMask = img4.OpacityMask = null;
    }
  }
}
