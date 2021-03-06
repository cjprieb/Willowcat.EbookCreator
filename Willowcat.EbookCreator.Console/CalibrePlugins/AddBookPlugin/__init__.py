from calibre.customize import FileTypePlugin
from calibre_plugins.willowcat_add_book.config import prefs

class TimeToReadAddedFile(FileTypePlugin):

    name                = 'Time To Read Added File' # Name of the plugin
    description         = 'Sets how long it takes to read a book based on a file'
    supported_platforms = ['windows'] # Platforms this plugin will run on
    author              = 'Willowcat' # The author of this plugin
    version             = (1, 2, 0)   # The version number of this plugin
    file_types          = set(['epub']) # The file types that this plugin will be applied to
    on_postprocess      = True # Run this plugin after conversion is complete
    # on_import           = True # Run this plugin when books are added to the database
    on_postimport       = True # Run after books added to the database; postimport/postadd methods are called
    minimum_calibre_version = (5, 30, 0)

    # _wordsPerMinute = "471"
    # _ebookConsoleAppPath = 'D:\\Users\\Crystal\\Programming\\repos\\Willowcat.EbookCreator\\Willowcat.EbookCreator.Console\\bin\\Debug\\netcoreapp3.1\\Willowcat.EbookCreator.Console.exe'

    def run(self, path_to_ebook):
        self.log("running for book {0}".format(path_to_ebook))
        timeToRead = self.getTimeToRead(path_to_ebook)
        self.log("time to read: {0}".format(timeToRead))
        return path_to_ebook

    def postadd(self, book_id, fmt_map, db):
        description = db.get_field(book_id, "comments", index_is_id=True)
        publisher = db.get_field(book_id, "publisher", index_is_id=True)
        if description.find("<p><b>Tags: </b>") < 0:
            tags = db.get_field(book_id, "tags", index_is_id=True)

            tagDescription = self.getTagDescription(tags)
            newDescription = description + "\n" + tagDescription
            db.new_api.set_field("comments", {book_id: newDescription})
            self.log("updated description: " + newDescription)
            
            tags = []
            format_custom_field = prefs['format_custom_field_name']
            if publisher == "Archive of Our Own" and format_custom_field != "": 
                db.set_custom(book_id, "Fan Fiction", label=format_custom_field)
                tags = [u"Fan Fiction"]
            db.set_tags(book_id, tags)

        self.log(fmt_map)
        read_time_custom_field = prefs['time_to_read_custom_field_name']
        if "epub" in fmt_map and read_time_custom_field != "":
            path_to_ebook = fmt_map["epub"]
            timeToRead = self.getTimeToRead(path_to_ebook)
            db.set_custom(book_id, timeToRead, label=read_time_custom_field)

    def getTimeToRead(self, path_to_ebook):
        import subprocess
        self.log("computed time to read for " + path_to_ebook)

        ebook_console_app_path = prefs['ebook_console_app_path']
        words_per_minute = prefs['words_per_minute']
        self.log("console path " + ebook_console_app_path)
        self.log("words per minute " + words_per_minute)

        text = ""
        if (words_per_minute != "") and (ebook_console_app_path != ""):
            command = [ebook_console_app_path, 'readtime', "-f", path_to_ebook, "-w", words_per_minute]
            p = subprocess.Popen(command, stdout=subprocess.PIPE)
            text = p.stdout.read().decode("utf-8").strip()
            self.log("result: " + text)
            
        return text

    def log(self, message):
        print("Willowcat ", self.version, ": ", message)

    def getTagDescription(self, tags):
        return "<p><b>Tags: </b>" + ", ".join(tags) + "</p>"

    def is_customizable(self):
        '''
        This method must return True to enable customization via
        Preferences->Plugins
        '''
        return True

    def config_widget(self):
        '''
        Implement this method and :meth:`save_settings` in your plugin to
        use a custom configuration dialog.

        This method, if implemented, must return a QWidget. The widget can have
        an optional method validate() that takes no arguments and is called
        immediately after the user clicks OK. Changes are applied if and only
        if the method returns True.

        If for some reason you cannot perform the configuration at this time,
        return a tuple of two strings (message, details), these will be
        displayed as a warning dialog to the user and the process will be
        aborted.

        The base class implementation of this method raises NotImplementedError
        so by default no user configuration is possible.
        '''
        # It is important to put this import statement here rather than at the
        # top of the module as importing the config class will also cause the
        # GUI libraries to be loaded, which we do not want when using calibre
        # from the command line
        from calibre_plugins.willowcat_add_book.config import ConfigWidget
        return ConfigWidget()


    def save_settings(self, config_widget):
        '''
        Save the settings specified by the user with config_widget.

        :param config_widget: The widget returned by :meth:`config_widget`.
        '''
        config_widget.save_settings()