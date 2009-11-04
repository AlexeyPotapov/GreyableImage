using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControl
{
  /// <summary>
  /// </summary>
  public class CustomControl : Control
  {
    static CustomControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl), new FrameworkPropertyMetadata(typeof(CustomControl)));
    }

    public static readonly DependencyProperty IconProperty =
      DependencyProperty.Register("Icon", typeof(object), typeof(CustomControl),
                                  new FrameworkPropertyMetadata(null, OnCoerce_Icon));

    public object Icon
    {
      get { return (object)GetValue(IconProperty); }
      set { SetValue(IconProperty, value); }
    }


    private static object OnCoerce_Icon(DependencyObject o, object value)
    {
      if (value is string)
      {
        Uri iconUri;

        // try to resolve given value as an absolute URI
        if (Uri.TryCreate(value as String, UriKind.RelativeOrAbsolute, out iconUri))
        {
          ImageSource img = new BitmapImage(iconUri);
          if (null != img)
          {
            GreyableImage.GreyableImage icon = (o as CustomControl).Icon as GreyableImage.GreyableImage;
            if (null == icon)
              icon = new GreyableImage.GreyableImage();

            icon.Source = img;
            icon.Stretch = Stretch.None;
            icon.SnapsToDevicePixels = true;

            return icon;
          }
        }
      }
      return value;
    } 

  }
}
