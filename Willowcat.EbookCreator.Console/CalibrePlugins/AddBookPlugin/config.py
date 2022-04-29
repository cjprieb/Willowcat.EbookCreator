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
prefs.defaults['fanfiction_tags_custom_field_name'] = 'fandom_tags'
prefs.defaults['time_to_read_minutes_custom_field_name'] = 'read_time_minutes'
prefs.defaults['tag_conversion_config_path'] = ''
# prefs.defaults['sync_custom_field_name'] = 'sync_book'
# prefs.defaults['fanfiction_tags_custom_field_name'] = 'fandom_tags'

class SettingKey:
    def __init__(self, label_text, setting_key):
        self.label_text = label_text
        self.setting_key = setting_key

class ConfigWidget(QWidget):

    def __init__(self):
        QWidget.__init__(self)
        self.layout = QVBoxLayout()
        self.setLayout(self.layout)

        self.settings_rows = []
        self.settings_rows.append(SettingKey('&Path to Ebook Console App:', 'ebook_console_app_path'))
        self.settings_rows.append(SettingKey('&Words Read Per Minute:', 'words_per_minute'))
        self.settings_rows.append(SettingKey('Custom Field Name for "time to read":', 'time_to_read_custom_field_name'))
        self.settings_rows.append(SettingKey('Custom Field Name for "format":', 'format_custom_field_name'))
        self.settings_rows.append(SettingKey('Custom Field Name for "fandom tags":', 'fanfiction_tags_custom_field_name'))
        self.settings_rows.append(SettingKey('Custom Field Name for "time to read (minutes)":', 'time_to_read_minutes_custom_field_name'))
        self.settings_rows.append(SettingKey('Custom Field Name for "sync":', 'sync_custom_field_name'))
        self.settings_rows.append(SettingKey('Tag Conversion Config Path:', 'tag_conversion_config_path'))

        for setting in self.settings_rows:
            self.add_setting_row(setting)

    def add_setting_row(self, setting):
        current_label = QLabel(setting.label_text)

        current_edit_box = QLineEdit(self)
        current_edit_box.setText(prefs[setting.setting_key])
        current_label.setBuddy(current_edit_box)

        current_layout_row = QHBoxLayout()
        current_layout_row.addWidget(current_label)
        current_layout_row.addWidget(current_edit_box)

        setting.label_control = current_label
        setting.edit_control = current_edit_box

        self.layout.addLayout(current_layout_row)

    def save_settings(self):
        for setting in self.settings_rows:
            prefs[setting.setting_key] = setting.edit_control.text()
