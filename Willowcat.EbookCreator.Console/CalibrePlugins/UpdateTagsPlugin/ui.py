#!/usr/bin/env python
# vim:fileencoding=utf-8

from calibre.gui2.actions import InterfaceAction
from calibre_plugins.willowcat_add_book.config import prefs
from calibre_plugins.willowcat_add_book.read import run_command


class InterfacePlugin(InterfaceAction):

    #: Set this to a unique name it will be used as a key
    name = 'willowcat-update-tags-tool'

    # Declare the main action associated with this plugin
    # The keyboard shortcut can be None if you dont want to use a keyboard
    # shortcut. Remember that currently calibre has no central management for
    # keyboard shortcuts, so try to use an unusual/unused shortcut.
    action_spec = ('Update identifiers on book', None, 'Set Identifiers On Book', 'Ctrl+Shift+T')
    
    action_add_menu  = True # add a menu to self.qaction
    
    # def create_menu_action(self, menu, unique_name, text, icon=None, shortcut=None, description=None, triggered=None, shortcut_name=None, persist_shortcut=False):
    #     QAction("Compute Time To Read")

    def genesis(self):
        # This method is called once per plugin, do initial setup here

        # Set the icon for this interface action
        # The get_icons function is a builtin function defined for all your
        # plugin code. It loads icons from the plugin zip file. It returns
        # QIcon objects, if you want the actual data, use the analogous
        # get_resources builtin function.
        #
        # Note that if you are loading more than one icon, for performance, you
        # should pass a list of names to get_icons. In this case, get_icons
        # will return a dictionary mapping names to QIcons. Names that
        # are not found in the zip file will result in null QIcons.
        icon = get_icons('images/icon.png')

        # The qaction is automatically created from the action_spec defined
        # above
        self.qaction.setIcon(icon)
        self.qaction.triggered.connect(self.on_click_set_tags)

        # parent_menu
        # menu_action = self.create_menu_action(self, parent_menu, "willowcat-update-book", "Update Time To Read", icon)
        # menu_action.triggered.connect(self.on_click_time_to_read)

    def gui_layout_complete(self):
        pass

    def on_click_set_tags(self):
        '''
        Update metadata
        '''
        from calibre.ebooks.metadata.meta import set_metadata
        from calibre.gui2 import error_dialog, info_dialog
        
        # Get currently selected books
        rows = self.gui.library_view.selectionModel().selectedRows()
        if not rows or len(rows) == 0:
            return error_dialog(self.gui, 'Cannot update books', 'No books selected', show=True)
    
        fanfiction_tags_custom_field = prefs['fanfiction_tags_custom_field_name']

        # Map the rows to book ids
        ids = list(map(self.gui.library_view.model().id, rows))
        db = self.gui.current_db.new_api
        for book_id in ids:
        # Get the current metadata for this book from the db
            # self.update_tags_for_book(db, book_id, fanfiction_tags_custom_field)
            fmts = db.formats(book_id)
            if not fmts: continue

            for fmt in fmts:
                fmt = fmt.lower()
                if fmt != "epub": continue

                # Get a python file object for the format. This will be either
                # an in memory file or a temporary on disk file
                path_to_ebook = db.format(book_id, fmt, as_path=True)
                self.log("ebook path: {0}".format(path_to_ebook))
                self.update_metadata(book_id, path_to_ebook)

        info_dialog(self.gui, 'Updated files',
                'Updated metadata for %d book(s)'%len(ids),
                show=True)

    def parse_description(self, description):
        tags = []

        self.log("description:")
        description = description.replace("\n"," ").replace("\r"," ")
        print(description)

        import re
        # pattern = re.compile("<p><b>Tags\\: </b>(.+)</p>")
        pattern = re.compile("<b>Tags\\: </b>(.+)</p>", re.MULTILINE)
        match = pattern.search(description)
        self.log("match? {0}".format(match))
        if match != None:
            # self.log("found: {0}".format(match.group(1)))
            for tag in match.group(1).split(','): tags.append(tag.strip())
        return tags
    
    def update_metadata(self, book_id, path_to_ebook):
        ebook_console_app_path = prefs['ebook_console_app_path']
        
        command = [ebook_console_app_path, 'identifier', "-f", path_to_ebook]
        text = run_command(command)
        
        if (text != None) and (text != ""):
            self.gui.current_db.new_api.set_field("identifiers", {book_id: text})

    def update_tags_for_book(self, db, book_id, fanfiction_tags_custom_field):
        tags = db.get_field(book_id, "tags", index_is_id=True)
        
        description = db.get_field(book_id, "comments", index_is_id=True)
        parsed_fanfiction_tags = []
        if description != None:
            parsed_fanfiction_tags = self.parse_description(description)
            self.log("parsed tags: {0}".format(parsed_fanfiction_tags))

        current_fanfiction_tags = db.new_api.field_for("#" + fanfiction_tags_custom_field, book_id)
        self.log("current tags: {0}".format(current_fanfiction_tags))

        from calibre_plugins.willowcat_add_book.tags import TagHelper
        tags = TagHelper().process_tags(parsed_fanfiction_tags, tags)
        self.log("new tags: {0}".format(tags))

        if current_fanfiction_tags != None:
            for tag in current_fanfiction_tags: tags.append(tag)

        db.set_tags(book_id, tags)

        if fanfiction_tags_custom_field != None and fanfiction_tags_custom_field != "":
            db.set_custom(book_id, [], label=fanfiction_tags_custom_field)


    def log(self, message):
        print("Willowcat: ", message)
