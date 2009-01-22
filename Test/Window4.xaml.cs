using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Test
{
  /// <summary>
  /// Interaction logic for Window4.xaml
  /// </summary>
  public partial class Window4 : Window
  {
    public Window4()
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

        img1.OpacityMask = img2.OpacityMask = img3.OpacityMask = new LinearGradientBrush(c1, c2, 90);
      }
      else
        img1.OpacityMask = img2.OpacityMask = img3.OpacityMask = null;
    }

  }
}
