
using Microsoft.Win32;
using System.Windows;
using System.IO;
using System;
using System.Windows.Media.Imaging;

namespace Test
{
  /// <summary>
  /// Interaction logic for Window6.xaml
  /// </summary>
  public partial class Window6 : Window
  {

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register("ImageSource", typeof(string), typeof(Window6));

    public string ImageSource
    {
      get { return (string)GetValue(ImageSourceProperty); }
      set { SetValue(ImageSourceProperty, value); }
    }

    public Window6()
    {
      DataContext = this;
      InitializeComponent();
    }

    private void OnClickBrowse(object sender, RoutedEventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
      if (null != ImageSource && ImageSource.Length > 0)
        ofd.InitialDirectory = Path.GetDirectoryName(ImageSource);

      ofd.RestoreDirectory = true;
      ofd.Multiselect = false;
      ofd.Filter = "Image files (*.jpg;*.png;*.gif;*.bmp;)|*.jpg;*.png;*.gif;*.bmp;";
      ofd.CheckFileExists = true;

      if (true == ofd.ShowDialog())
        ImageSource = ofd.FileName;
    }
  }
}
