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
using System.Drawing;
using System.Windows.Interop;

namespace Test
{
  /// <summary>
  /// Interaction logic for Window9.xaml
  /// </summary>
  public partial class Window9 : Window
  {

    #region ImgSrc

    /// <summary>
    /// DependencyProperty used as a backing store for ImgSrc property.
    /// </summary>
    public static readonly DependencyProperty ImgSrcProperty =
      DependencyProperty.Register("ImgSrc", typeof(BitmapSource), typeof(Window9));

    /// <summary>
    /// Gets or Sets ImgSrc value.
    /// </summary>
    public BitmapSource ImgSrc
    {
      get { return (BitmapSource)GetValue(ImgSrcProperty); }
      set { SetValue(ImgSrcProperty, value); }
    }

    #endregion // ImgSrc

    public Window9()
    {
      DataContext = this;
      int width = SystemIcons.Question.Width;
      int height = SystemIcons.Question.Height;
      ImgSrc = Imaging.CreateBitmapSourceFromHBitmap(SystemIcons.Question.ToBitmap().GetHbitmap(),
                                                     IntPtr.Zero, 
                                                     Int32Rect.Empty, 
                                                     BitmapSizeOptions.FromWidthAndHeight(width, height));

      InitializeComponent();
    }
  }
}
