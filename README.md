GreyableImage - subclasses WPF Image control, allowing it to grey itself out when disabled.

    <Button>
        <gi:GreyableImage Source="image.png"/>
    </Button>


ImageGreyer - exposes attachable dependency property IsGreyable. When IsGreyable is attached to WPF Image control it makes it greyable. This class allows for exactly the same thing as the one above, but does not require you to use nonstandard WPF controls.

    <Button>
        <Image Source="image.png" gi:ImageGreyer.IsGreyable="true"/>
    </Button>


In both cases when the button is disabled the Image inside it turns greyscale.
