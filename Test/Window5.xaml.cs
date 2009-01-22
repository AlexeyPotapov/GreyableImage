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
  /// Interaction logic for Window5.xaml
  /// </summary>
  public partial class Window5 : Window
  {
    public Window5()
    {
      InitializeComponent();
    }

    private void ApplyOpacityMask(object sender, RoutedEventArgs e)
    {
      if (null == img || null == g_img)
        return;

      Color c1 = new Color();
      c1.A = 0;
      Color c2 = new Color();
      c2.A = 0xAA;

      img.OpacityMask = g_img.OpacityMask = new LinearGradientBrush(c1, c2, 90);
    }

    private void RemoveOpacityMask(object sender, RoutedEventArgs e)
    {
      if (null == img || null == g_img)
        return;

      img.OpacityMask = g_img.OpacityMask = null;
    }

  }
}
