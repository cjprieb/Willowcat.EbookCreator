# Calibre Plugins

## AddBookPlugin

This plugin will set the Time To Read on newly created ebooks if they are in epub format and are not encrypted. It will also check if the books are from Archive of Our Own, and parse tags and set the format accordingly.

## UpdateBookPlugin

This plugin will update the Time To Read on selected books. This is useful if the epub document was edited (ie, html files removed or added), or if the Words Per Minute setting was adjusted.


## Installing

1. Navigate to the plug-in folders and run `calibre-customize`.
    ```
    > cd Willowcat.EbookCreator.Console\CalibrePlugins\AddBookPlugin
    > calibre-customize -b
    ```

2. Add custom fields for the plug-ins to use.
    - "time to read" - long text; stores the time to read as a number of asterisks (*) plus a formatted hour/minute string. Meant as a visual representation.
    - "time to read (minutes)" - integers; stores the time to read as minutes. Meant for for sorting and querying the read time.
    - "fandom tags" - used to store the tags parsed from Archive of Our Own ebooks. A separate configuration file is used to determine how to parse the tags.
    - "format" - if the ebook is identified as an Archive of Our Own ebook, this field will be set to "Fan Fiction"
    <img src="Willowcat.EbookCreator.Console/CalibrePlugins/images/calibre_add_columns.png"/>

3. (optional) Create a [Tag Conversion](#tag-configuration) file.

4. Customize the plug-in settings
    - "Path to Ebook Console App" - the path where the console app can be found.
    - "Words Read Per Minute" - how many words can be read in a minute. Many eReaders can provide this information; I use MoonReader
    - "Custom Field Names..." - set using the "lookup name" without `#` of the custom field.
    - "Tag Conversion Config Path" - the path where the configuration for parsing tags can be found.
    <img src="Willowcat.EbookCreator.Console/CalibrePlugins/images/calibre_customize_plugin.png"/>

5. Use the plugins! The AddBookPlugin will be used when adding a new ebook to Calibre. By default, the UpdateBookPlugin will be used on selected ebooks when pressing `CTRL+R`.

See [Creating Plugins](https://manual.calibre-ebook.com/creating_plugins.html#id13) for more details on debugging and customizing plugins for Calibre.

## Tag Configuration

Tags are defined by `parsedTag = newTag`. The `newTag` value is optional, but the `=` is required for the parser to recognize the line as a tag line.
Tags can be under the header `[REMOVE]` or `[CONVERT]`. Tags in `[REMOVE]` are ignored and not added to the "fandom tags" field. Tags in `[CONVERT]` are renamed to the `newTag` value. Tags that contain the pattern "alternative universe - {x}" are renamed to "AU.{x}" All other tags are added as is.
```
[REMOVE]
gen = 
choose not to use warnings = 

[CONVERT]
humor = Tone.Humor
mystery = Genre.Mystery
```