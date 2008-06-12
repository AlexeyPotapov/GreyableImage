//Copyright (c) 2008 Alexey Potapov

//Permission is hereby granted, free of charge, to any person obtaining a copy 
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights to 
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
//of the Software, and to permit persons to whom the Software is furnished to do 
//so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all 
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;


namespace GreyableImage
{
  /// <summary>
  /// Image control that get's greyed out when disabled.
  /// This control is intended to be used for toolbar, menu or button icons where ability of an icon to 
  /// grey itself out when disabled is essential.
  /// <remarks>
  /// 1) Greyscale image is created using FormatConvertedBitmap class. Unfortunately when converting the
  /// image to greyscale this class does nt preserve transparency information. To overcome that, there is 
  /// an opacity mask created from original image that is applied to greyscale image in order to preserve
  /// transparency information. Because of that if an OpacityMask is applied to original image that mask 
  /// has to be combined with that special opacity mask of greyscale image in order to make a proper 
  /// greyscale image look. If you know how to combine two opacity masks feel free to fix that issue.
  /// 2) DrawingImage source is not supported at the moment.
  /// 3) Have not tried to use any BitmapSource derived sources accept for BitmapImage so it may not be 
  /// able to convert them to greyscale too.
  /// </remarks>
  /// </summary>
  public class GreyableImage : Image
  {
    // these are holding references to original and greyscale ImageSources
    private ImageSource _sourceC, _sourceG;
    // these are holding original and greyscale opacity masks
    private Brush _opacityMaskC, _opacityMaskG;

    /// <summary>
    /// Overwritten to handle changes of IsEnabled, Source and OpacityMask properties
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.Property.Name.Equals("IsEnabled"))
      {
        if ((e.NewValue as bool?) == false)
        {
          Source = _sourceG;
          OpacityMask = _opacityMaskG;
        }
        else if ((e.NewValue as bool?) == true)
        {
          Source = _sourceC;
          OpacityMask = _opacityMaskC;
        }
      }
      else if (e.Property.Name.Equals("Source") &&
               !object.ReferenceEquals(Source, _sourceC) &&
               !object.ReferenceEquals(Source, _sourceG))  // only recache Source if it's the new one from outside
      {
        SetSources();
      }
      else if (e.Property.Name.Equals("OpacityMask") &&
               !object.ReferenceEquals(OpacityMask, _opacityMaskC) &&
               !object.ReferenceEquals(OpacityMask, _opacityMaskG)) // only recache opacityMask if it's the new one from outside
      {
        _opacityMaskC = OpacityMask;
      }

      base.OnPropertyChanged(e);
    }

    /// <summary>
    /// Cashes original ImageSource, creates and caches greyscale ImageSource and greyscale opacity mask
    /// </summary>
    private void SetSources()
    {
      _sourceC = Source;

      try
      {
        // create and cache greyscale ImageSource
        _sourceG = new FormatConvertedBitmap(new BitmapImage(new Uri(TypeDescriptor.GetConverter(Source).ConvertTo(Source, typeof(string)) as string)),
                                             PixelFormats.Gray8, null, 0);

        // create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
        _opacityMaskG = new ImageBrush(_sourceC);
        _opacityMaskG.Opacity = 0.6;
      }
      catch
      {
#if DEBUG
        MessageBox.Show(String.Format("The ImageSource used cannot be greyed out.\nUse BitmapImage or URI as a Source in order to allow greyscaling.\nSource type used: {0}", Source.GetType().Name),
                        "Unsupported Source in GreyableImage", MessageBoxButton.OK, MessageBoxImage.Warning);
#endif // DEBUG

        // in case greyscale image cannot be created set greyscale source to original Source
        _sourceG = Source;
      }
    }
  }
}
