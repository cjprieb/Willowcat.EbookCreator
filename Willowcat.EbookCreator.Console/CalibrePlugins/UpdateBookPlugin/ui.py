#!/usr/bin/env python
# vim:fileencoding=utf-8

from calibre.gui2.actions import InterfaceAction
from calibre_plugins.willowcat_add_book.config import prefs
from calibre_plugins.willowcat_add_book.read import getTimeToRead, parseTimeToReadAsMinutes

class InterfacePlugin(InterfaceAction):

    #: Set this to a unique name it will be used as a key
    name = 'willowcat-time-to-read-tool'

    # Declare the main action associated with this plugin
    # The keyboard shortcut can be None if you dont want to use a keyboard
    # shortcut. Remember that currently calibre has no central management for
    # keyboard shortcuts, so try to use an unusual/unused shortcut.
    action_spec = ('Update Time To Read', None, 'Compute the new time to read the book', 'Ctrl+Shift+R')
    
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
        self.qaction.triggered.connect(self.on_click_time_to_read)

        # parent_menu
        # menu_action = self.create_menu_action(self, parent_menu, "willowcat-update-book", "Update Time To Read", icon)
        # menu_action.triggered.connect(self.on_click_time_to_read)

    def gui_layout_complete(self):
        pass

    def on_click_time_to_read(self):
        '''
        Compute the time to read for the selected book
        '''
        from calibre.ebooks.metadata.meta import set_metadata
        from calibre.gui2 import error_dialog, info_dialog
        
        # Get currently selected books
        rows = self.gui.library_view.selectionModel().selectedRows()
        if not rows or len(rows) == 0:
            return error_dialog(self.gui, 'Cannot compute time to read', 'No books selected', show=True)
    
        read_time_custom_field = prefs['time_to_read_custom_field_name']

        # Map the rows to book ids
        ids = list(map(self.gui.library_view.model().id, rows))
        db = self.gui.current_db.new_api
        for book_id in ids:
            # Get the current metadata for this book from the db
            fmts = db.formats(book_id)
            if not fmts: continue

            for fmt in fmts:
                fmt = fmt.lower()
                if fmt != "epub": continue

                # Get a python file object for the format. This will be either
                # an in memory file or a temporary on disk file
                path_to_ebook = db.format(book_id, fmt, as_path=True)
                
                self.log("ebook path: " + path_to_ebook)

                if read_time_custom_field != "":
                    time_to_read = getTimeToRead(path_to_ebook)
                    self.gui.current_db.set_custom(book_id, time_to_read, label=read_time_custom_field)

                    read_minutes_custom_field = prefs['time_to_read_minutes_custom_field_name']
                    if read_minutes_custom_field != "":
                        time_to_read_minutes = parseTimeToReadAsMinutes(time_to_read)
                        self.gui.current_db.set_custom(book_id, time_to_read_minutes, label=read_minutes_custom_field)

        info_dialog(self.gui, 'Updated files',
                'Updated the time to read for %d book(s)'%len(ids),
                show=True)

    # def getTimeToRead(self, path_to_ebook, ebook_console_app_path, words_per_minute):
    #     import subprocess
    #     self.log("computed time to read for " + path_to_ebook)

    #     text = ""
    #     if (words_per_minute != "") and (ebook_console_app_path != ""):
    #         command = [ebook_console_app_path, 'readtime', "-f", path_to_ebook, "-w", words_per_minute]
    #         p = subprocess.Popen(command, stdout=subprocess.PIPE)
    #         text = p.stdout.read().decode("utf-8").strip()
    #         self.log("result: " + text)
            
    #     return text

    def log(self, message):
        print("Willowcat: ", message)
