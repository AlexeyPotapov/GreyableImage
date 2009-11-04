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
  /// Interaction logic for Window7.xaml
  /// </summary>
  public partial class Window7 : Window
  {
    public Window7()
    {
      InitializeComponent();
    }

    private void sourceBtn2_Click(object sender, RoutedEventArgs e)
    {
      Uri uri = new Uri("img1.bmp", UriKind.Relative);

      if (sourceBtn2.IsChecked == false)
        uri = new Uri("img2.bmp", UriKind.Relative);

      img1.Source = new BitmapImage(uri);
      img2.Source = new BitmapImage(uri);
      img3.Source = new BitmapImage(uri);
      img4.Source = new BitmapImage(uri);
    }
  }
}
