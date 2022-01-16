#!/usr/bin/env python
# vim:fileencoding=utf-8

from calibre.customize import InterfaceActionBase


class SetTagsOnBookInterfacePlugin(InterfaceActionBase):

    name = 'Set custom tags from description' # Name of the plugin
    version = (1, 0, 0)
    author = 'Willowcat' # The author of this plugin
    supported_platforms = ['windows'] # Platforms this plugin will run on
    description = 'Parses tags from description and sets them as custom tags'
    minimum_calibre_version = (5, 30, 0)
    
    actual_plugin       = 'calibre_plugins.willowcat_update_tags.ui:InterfacePlugin'

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

