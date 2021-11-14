#!/usr/bin/env python
# vim:fileencoding=UTF-8:ts=4:sw=4:sta:et:sts=4:ai

from qt.core import QWidget, QHBoxLayout, QLabel, QLineEdit, QVBoxLayout

from calibre.utils.config import JSONConfig

# This is where all preferences for this plugin will be stored
# Remember that this name (i.e. plugins/interface_demo) is also
# in a global namespace, so make it as unique as possible.
# You should always prefix your config file name with plugins/,
# so as to ensure you dont accidentally clobber a calibre config file
prefs = JSONConfig('plugins/willowcat')

# Set defaults
prefs.defaults['ebook_console_app_path'] = ''
prefs.defaults['words_per_minute'] = '471'
prefs.defaults['time_to_read_custom_field_name'] = 'read_time'
prefs.defaults['format_custom_field_name'] = 'format'


class ConfigWidget(QWidget):

    def __init__(self):
        QWidget.__init__(self)
        self.layout = QVBoxLayout()
        self.setLayout(self.layout)

        self.layout_row_1 = QHBoxLayout()
        self.layout.addLayout(self.layout_row_1)

        self.layout_row_2 = QHBoxLayout()
        self.layout.addLayout(self.layout_row_2)

        self.layout_row_3 = QHBoxLayout()
        self.layout.addLayout(self.layout_row_3)

        self.layout_row_4 = QHBoxLayout()
        self.layout.addLayout(self.layout_row_4)

        # Path to console app
        self.label_app_path = QLabel('&Path to Ebook Console App:')
        self.layout_row_1.addWidget(self.label_app_path)

        self.edit_app_path = QLineEdit(self)
        self.edit_app_path.setText(prefs['ebook_console_app_path'])
        self.layout_row_1.addWidget(self.edit_app_path)
        self.label_app_path.setBuddy(self.edit_app_path)

        # Words Per Minute
        self.label_words_per_minute = QLabel('&Words Read Per Minute:')
        self.layout_row_2.addWidget(self.label_words_per_minute)

        self.edit_words_per_minute = QLineEdit(self)
        self.edit_words_per_minute.setText(prefs['words_per_minute'])
        self.layout_row_2.addWidget(self.edit_words_per_minute)
        self.label_words_per_minute.setBuddy(self.edit_words_per_minute)

        # time_to_read_custom_field_name
        self.label_time_to_read_custom_field = QLabel('Custom Field Name  for "time to read":')
        self.layout_row_3.addWidget(self.label_time_to_read_custom_field)

        self.edit_time_to_read_custom_field = QLineEdit(self)
        self.edit_time_to_read_custom_field.setText(prefs['time_to_read_custom_field_name'])
        self.layout_row_3.addWidget(self.edit_time_to_read_custom_field)
        self.label_time_to_read_custom_field.setBuddy(self.edit_time_to_read_custom_field)

        # format_custom_field_name
        self.label_format_custom_field = QLabel('Custom Field Name  for "format":')
        self.layout_row_4.addWidget(self.label_format_custom_field)

        self.edit_format_custom_field = QLineEdit(self)
        self.edit_format_custom_field.setText(prefs['format_custom_field_name'])
        self.layout_row_4.addWidget(self.edit_format_custom_field)
        self.label_format_custom_field.setBuddy(self.edit_format_custom_field)

    def save_settings(self):
        prefs['ebook_console_app_path'] = self.edit_app_path.text()
        prefs['words_per_minute'] = self.edit_words_per_minute.text()
        prefs['time_to_read_custom_field_name'] = self.edit_time_to_read_custom_field.text()
        prefs['format_custom_field_name'] = self.edit_format_custom_field.text()
