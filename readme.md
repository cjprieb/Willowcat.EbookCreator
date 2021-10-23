# Willowcat.EbookCreator

.NET Standard Library for building epub files to use with e-readers. 

Includes a command line tool that can estimate how long it would take
to read an epub file and a couple [Calibre](https://calibre-ebook.com/) plugins to automatically include 
a "Time to Read" field when adding a book to Calibre. 

## Dependencies

* .NET Standard 2.0 or higher
* (For the console program) .NET Core 3.1 or higher
* (For the plugins) Calibre 3.47 or higher

## Configuration

I am working on adding configuration UI to the Calibre plugins. Currently, 
the "words per minute" and the path to the command line tool are hard-coded.

For the "words per minute" field to work with Calibre, [add "long text" custom column](https://blog.calibre-ebook.com/calibre-custom-columns/)
with the name `#read_time`.

## Using the Library



```csharp

// the data to use to build the epub file
var bookItemData = new BookModel() 
{
    Bibliography = new BibliographyModel() 
    {
        // all kinds of metadata useful for a book
    },

    // if set, will include a custom calibre metadata element for "#read_times"
    // in the generated epub file
    WordsReadPerMinute = 471
};

// The html files that will be read. 
// Add them to the list in the preferred reading order.
bookItemData.TableOfContents.ChapterFiles.AddRange(new List<FileItemModel>());

// Other files that are referenced in the epub, such as images or stylesheets
bookItemData.TableOfContents.OtherFiles.AddRange(new List<FileItemModel>());

// These entries are used to build the Table of Contents file and should reference
// the chapter files above. Usually one per chapter file.
bookItemData.TableOfContents.Entries.AddRange(new List<TableOfContentsLinkModel>());

// Used for as a staging ground to create the files that
// will eventually be zipped into the epub file
var stagingDirectory = @"D:\Users\user\Documents\eBooks\bookTitle\epub",

// The directory to create the create the merged epub file at.
var epubFilePath = @"D:\Users\user\Documents\eBooks\bookTitle"

var epubBuilder = new EpubBuilder();
epubBuilder.Create(bookItemData, stagingDirectory, epubFilePath)
```

## Merging epub files

The `MergeBooksPublicationEngine` merges works from an [AO3](https://archiveofourown.org) 
series list into a single epub document. A subset or custom order of works
can also be specified.


```csharp

var series = new SeriesModel()
{
    // optional, will automatically download the requested works 
    // from the url if included. otherwise will use the epubs already
    // in the ?? directory.
    // should be in the format https://archiveofourown.org/series/\d+
    SeriesUrl = "{seriesUrl}",

    // optional, the work indexes to include; 1-based, see the SeriesUrl for the value.
    // if null, then all the works in the series will be included in their 
    // default index order.
    WorkIndexes = new int[] { 3, 2, 6, 7 },

    // optional, use to specify the name of the series to include in the metadata
    SeriesName = "{seriesTitle}", 

    // optional, use to specify the series index to include in the metadata
    SeriesIndex = 2
};

var options = new EpubOptions()
{
    // if false, the engine won't re-download the files if they have 
    // been downloaded already
    OverwriteOriginalFiles = true, 

    // if set, will include a custom calibre metadata element for "#read_times"
    // in the generated epub file
    WordsReadPerMinute = 471
};

var filePaths = new EpubFilePaths()
{
    // The directory where the original book files are. 
    // In this case, the location where AO3 epub files are be 
    // downloaded to.
    // NOTE! the files will be added to the merged epub in 
    // alphabetical order of their file names
    SourceDirectory = @"D:\Users\user\Documents\eBooks\bookTitle\original",
    
    // Used for as a staging ground to create the files that
    // will eventually be zipped into the epub file
    StagingDirectory = @"D:\Users\user\Documents\eBooks\bookTitle\epub",

    // The directory to create the create the merged epub file at.
    EpubFilePath = @"D:\Users\user\Documents\eBooks\bookTitle"
};

var epubBuilder = new EpubBuilder();
var engine = new MergeBooksPublicationEngine(epubBuilder, options);

await publicationEngine.PublishAsync(series, filePaths);
```