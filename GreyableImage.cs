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

using System.ComponentModel;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace GreyableImage
{
  /// <summary>
  /// Image control that get's greyed out when disabled.
  /// This control is intended to be used for toolbar, menu or button icons where ability of an icon to 
  /// grey itself out when disabled is essential.
  /// <remarks>
  /// 1) Greyscale image is created using FormatConvertedBitmap class. Unfortunately when converting the
  ///    image to greyscale this class does not preserve transparency information. To overcome that, there is 
  ///    an opacity mask created from original image that is applied to greyscale image in order to preserve
  ///    transparency information. Because of that if an OpacityMask is applied to original image that mask 
  ///    has to be combined with that special opacity mask of greyscale image in order to make a proper 
  ///    greyscale image look. If you know how to combine two opacity masks please let me know.
  /// 2) When specifying source Uri from XAML try to use Absolute Uri otherwise the greyscale image
  ///    may not be created in some scenarious. There is GetAbsoluteUri() method aiming to improve the situation 
  ///    by trying to generate an absolute Uri from given source, but I cannot guarantee it will work in all 
  ///    possible scenarious.
  /// 3) In case the greyscaled version cannot be created for whatever reason the original image with 
  ///    60% opacity (i.e. dull colours) will be used instead.
  /// 4) Changing Source from code will take precedence over Style triggers. Source set through triggers 
  ///    will be ignored once it was set from code. This is not the fault of the control, but is the way 
  ///    WPF works: http://msdn.microsoft.com/en-us/library/ms743230%28classic%29.aspx
  /// 5) SnapsToDevicePixels is set to true by default
  /// 6) Stretch is set to none by default
  /// 7) Supports DrawingImage as a source, thanks to Morten Schou.
  /// </remarks>
  /// </summary>
  public class GreyableImage : Image
  {
    #region Fields

    // these are holding references to original and greyscale ImageSources
    private ImageSource _sourceColour, _sourceGreyscale;

    // these are holding original and greyscale opacity masks
    private Brush _opacityMaskColour, _opacityMaskGreyscale;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Default constructor. Creates an empty image.
    /// </summary>
    public GreyableImage()
    {
      // since this class was intended for use in toolbars and menus it is most often the case that the image should
      // not be stretched and should be snapped to pixels so those properties are set this way by default
      SnapsToDevicePixels = true;
      Stretch = Stretch.None;
    }

    /// <summary>
    /// This constructor simplifies creating GreyableImage from code
    /// </summary>
    /// <param name="sourceUri">string Uri used to set Source property</param>
    public GreyableImage(String sourceUri) : this()
    {
      try
      {
        Source = new BitmapImage(GetAbsoluteUri(sourceUri));
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.Fail("Source URI is invalid.",
                                      "The string URI used to construct the image is invalid. The image Source is not set. Make sure a correct absolute Uri is used as relative Uri may sometimes resolve incorrectly.\n\nException: " + e.Message);
      }
    }

    #endregion // Constructors

    #region Overrides

    /// <summary>
    /// Overwritten to handle changes of IsEnabled, Source and OpacityMask properties
    /// </summary>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.Property.Name.Equals("IsEnabled"))
      {
        UpdateImage();
      }
      else if (e.Property.Name.Equals("Source") &&
               !object.ReferenceEquals(Source, _sourceColour) &&
               !object.ReferenceEquals(Source, _sourceGreyscale))  // only recache Source if it's the new one from outside
      {
        SetSources();

        SetGreyscaleOpacityMask();

        // have to asynchronously invoke UpdateImage because it changes the Source property 
        // of an image, but we cannot change it from within its change notification handler.
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(UpdateImage));
      }
      else if (e.Property.Name.Equals("OpacityMask") &&
               !object.ReferenceEquals(OpacityMask, _opacityMaskColour) &&
               !object.ReferenceEquals(OpacityMask, _opacityMaskGreyscale)) // only recache opacityMask if it's the new one from outside
      {
        _opacityMaskColour = OpacityMask;
      }

      base.OnPropertyChanged(e);
    }

    #endregion // Overrides

    #region Private Helpers

    /// <summary>
    /// Cashes original ImageSource, creates and caches greyscale ImageSource and greyscale opacity mask
    /// </summary>
    private void SetSources()
    {
      // in case greyscale image cannot be created set greyscale source to original Source first
      _sourceGreyscale = _sourceColour = Source;

      try
      {
        BitmapSource colourBitmap;
              
        if (_sourceColour is DrawingImage)
        {
          // support for DrawingImage as a source - thanks to Morten Schou who provided this code
          colourBitmap = new RenderTargetBitmap((int)_sourceColour.Width,
                                                (int)_sourceColour.Height,
                                                96, 96,
                                                PixelFormats.Default);
          DrawingVisual drawingVisual = new DrawingVisual();
          DrawingContext drawingDC = drawingVisual.RenderOpen();
          drawingDC.DrawImage(_sourceColour,
                              new Rect(new Size(_sourceColour.Height,
                                                _sourceColour.Width)));
          drawingDC.Close();
          (colourBitmap as RenderTargetBitmap).Render(drawingVisual);
          _sourceGreyscale = new FormatConvertedBitmap(colourBitmap, PixelFormats.Gray8, null, 0);
        }
        else
        {
          // get the string Uri for the original image source first
          String stringUri = TypeDescriptor.GetConverter(_sourceColour).ConvertTo(_sourceColour, typeof(string)) as string;

          // generate an absolute Uri for the string Uri
          colourBitmap = new BitmapImage(GetAbsoluteUri(stringUri));
        }

        // create and cache greyscale ImageSource
        _sourceGreyscale = new FormatConvertedBitmap(colourBitmap, PixelFormats.Gray8, null, 0);
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.Fail("The Image used cannot be greyed out.",
                                      "Make sure absolute Uri is used, relative Uri may sometimes resolve incorrectly.\n\nException: " + e.Message);
      }
    }

    /// <summary>
    /// Сreates and caches greyscale Image opacity mask.
    /// </summary>
    private void SetGreyscaleOpacityMask()
    {
      // create Opacity Mask for greyscale image as FormatConvertedBitmap used to 
      // create greyscale image does not preserve transparency info.
      _opacityMaskGreyscale = new ImageBrush(_sourceColour);
      _opacityMaskGreyscale.Opacity = 0.6;
    }

    /// <summary>
    /// Sets image source and opacity mask from cache.
    /// </summary>
    public void UpdateImage()
    {
      if (IsEnabled)
      {
        // change Source and OpacityMask of an image back to original values
        Source = _sourceColour;
        OpacityMask = _opacityMaskColour;
      }
      else
      {
        // change Source and OpacityMask of an image to values generated for greyscale version
        Source = _sourceGreyscale;
        OpacityMask = _opacityMaskGreyscale;
      }
    }

    /// <summary>
    /// Creates and returns an absolute Uri using the path provided.
    /// Throws UriFormatException if an absolute URI cannot be created.
    /// </summary>
    /// <param name="stringUri">string uri</param>
    /// <returns>an absolute URI based on string URI provided</returns>
    /// <exception cref="UriFormatException - thrown when absolute Uri cannot be created from provided stringUri."/>
    /// <exception cref="ArgumentNullException - thrown when stringUri is null."/>
    private Uri GetAbsoluteUri(String stringUri)
    {
      Uri uri = null;

      // try to resolve it as an absolute Uri 
      // if uri is relative its likely to point in a wrong direction
      if (!Uri.TryCreate(stringUri, UriKind.Absolute, out uri))
      {
        // it seems that the Uri is relative, at this stage we can only assume that
        // the image requested is in the same assembly as this oblect,
        // so we modify the string Uri to make it absolute ...
        stringUri = "pack://application:,,,/" + stringUri.TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

        // ... and try to resolve again
        // at this stage if it doesn't resolve the UriFormatException is thrown
        uri = new Uri(stringUri);
      }

      return uri;
    }

    #endregion // Private Helpers
  }
}
