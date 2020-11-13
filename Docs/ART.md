# Art Assets

## Basic Card Art

* Format: Png
* Size: 1024x1024
* Usable Size: 1024x790 - Bottom 234 rows should be black


    //ImageMagic Script
    using(var final = new MagickImage(MagickColors.Black, 1024, 1024))
    using(var source = new MagickImage(new FileInfo(filePath)))
    {
	    final.Format = MagickFormat.Png;
	
    	var size = new MagickGeometry(1024, 790);
    	size.FillArea = true;
    	source.Resize(size);
    	source.Crop(1024, 790);
    	source.RePage();
    	
	    final.Composite(source, CompositeOperator.Atop);
	    final.Write(new FileInfo(outputPath));
    }


## Full Card Art - Vertical

* Format: Png
* Size: 1024x1024
* Usable Size: 710x1024 - Right 334 columns should be black


Vertical aligned full card art
* Character Cards Art
* Incap. Cart Art
* Back of Deck Art


    //ImageMagic Script
    using(var final = new MagickImage(MagickColors.Black, 1024, 1024))
    using(var source = new MagickImage(new FileInfo(filePath)))
    {
	    final.Format = MagickFormat.Png;
	
    	var size = new MagickGeometry(710, 1024);
    	size.FillArea = true;
    	source.Resize(size);
    	source.Crop(710, 1024);
    	source.RePage();
    	
	    final.Composite(source, CompositeOperator.Atop);
	    final.Write(new FileInfo(outputPath));
    }

## Full Card Art - Hortizontal
* Format: Png
* Size: 1024x1024
* Usable Size: 710x1024 - bottom 334 columns should be black 


Hortizontally aligned full card art
* Mission Cards

No sample art currently available


    //ImageMagic Script
    using(var final = new MagickImage(MagickColors.Black, 1024, 1024))
    using(var source = new MagickImage(new FileInfo(filePath)))
    {
	    final.Format = MagickFormat.Png;
	
    	var size = new MagickGeometry(1024, 710);
    	size.FillArea = true;
    	source.Resize(size);
    	source.Crop(1024, 710);
    	source.RePage();
    	
	    final.Composite(source, CompositeOperator.Atop);
	    final.Write(new FileInfo(outputPath));
    }

