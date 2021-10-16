# Calibre Plugin Notes
Add this plugin to Calibre to automatically set the "time to read" when adding an ebook. Requires adding
a custom "read_time" column.

This plugin will also move any tags to the description to avoid cluttering the existing tag system and
will add a "Fanfiction" tag if the publisher is "Archive of our Own"

```

- update changes to your plugin by using the following command line from the \CalibrePlugins\AddBookPlugin folder:
```
calibre-customize -b .; calibre-debug -g
```

- use `calibre-debug -g` to `print` debug statements to the command line 
```
calibre-customize -b .; calibre-debug -g