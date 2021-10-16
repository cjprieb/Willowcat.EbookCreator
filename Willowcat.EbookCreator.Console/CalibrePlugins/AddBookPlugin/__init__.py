from calibre.customize import FileTypePlugin

class Willowcat(FileTypePlugin):

    name                = 'Time To Read Plugin' # Name of the plugin
    description         = 'Sets how long it takes to read a book based on a file'
    supported_platforms = ['windows'] # Platforms this plugin will run on
    author              = 'Willowcat' # The author of this plugin
    version             = (1, 0, 3)   # The version number of this plugin
    file_types          = set(['epub']) # The file types that this plugin will be applied to
    # on_postprocess      = True # Run this plugin after conversion is complete
    on_import           = True # Run this plugin when books are added to the database
    on_postimport       = True # Run after books added to the database; postimport/postadd methods are called
    minimum_calibre_version = (0, 7, 53)

    _wordsPerMinute = "471"
    _ebookConsoleAppPath = 'D:\\Users\\Crystal\\Programming\\repos\\Willowcat.EbookCreator\\Willowcat.EbookCreator.Console\\bin\\Debug\\netcoreapp3.1\\Willowcat.EbookCreator.Console.exe'

    def run(self, path_to_ebook):
        self.log("running for book " + path_to_ebook)
        return path_to_ebook

    def postadd(self, book_id, fmt_map, db):
        import string

        description = db.get_field(book_id, "comments", index_is_id=True)
        publisher = db.get_field(book_id, "publisher", index_is_id=True)
        if string.find(description, "<p><b>Tags: </b>") < 0:
            tags = db.get_field(book_id, "tags", index_is_id=True)

            tagDescription = self.getTagDescription(tags)
            newDescription = description + "\n" + tagDescription
            db.new_api.set_field("comments", {book_id: newDescription})
            self.log("updated description: " + newDescription)
            
            tags = []
            if publisher == "Archive of Our Own": 
                db.set_custom(book_id, "Fan Fiction", label="format", num=5)
                tags = [u"Fan Fiction"]
            db.set_tags(book_id, tags)

        self.log(fmt_map)
        if "epub" in fmt_map:
            path_to_ebook = fmt_map["epub"]
            timeToRead = self.getTimeToRead(path_to_ebook)
            db.set_custom(book_id, timeToRead, label="read_time", num=9)

    def getTimeToRead(self, path_to_ebook):
        import subprocess
        self.log("computed time to read for " + path_to_ebook)
        command = [self._ebookConsoleAppPath, 'readtime', "-f", path_to_ebook, "-w", self._wordsPerMinute]
        p = subprocess.Popen(command, stdout=subprocess.PIPE)
        text = p.stdout.read()
        self.log("result: " + text)
        return text

    def log(self, message):
        print("Willowcat ", self.version, ": ", message)

    def getTagDescription(self, tags):
        import string
        return "<p><b>Tags: </b>" + string.join(tags, ', ') + "</p>"