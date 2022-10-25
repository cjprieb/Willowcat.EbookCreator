#!/usr/bin/env python
# vim:fileencoding=UTF-8:ts=4:sw=4:sta:et:sts=4:ai

import configparser
from os.path import exists
from calibre_plugins.willowcat_add_book.config import prefs

def run_command(command):
    import subprocess
    p = subprocess.Popen(command, stdout=subprocess.PIPE, creationflags=subprocess.CREATE_NO_WINDOW)
    text = p.stdout.read().decode("utf-8").strip()
    read_log("result: " + text)        
    return text

def getTimeToRead(path_to_ebook):
    read_log("computed time to read for " + path_to_ebook)

    ebook_console_app_path = prefs['ebook_console_app_path']
    words_per_minute = prefs['words_per_minute']
    read_log("console path " + ebook_console_app_path)
    read_log("words per minute " + words_per_minute)

    text = ""
    if (words_per_minute != "") and (ebook_console_app_path != ""):
        command = [ebook_console_app_path, 'readtime', "-f", path_to_ebook, "-w", words_per_minute]
        text = run_command(command)
        
    return text

def parseTimeToReadAsMinutes(time_to_read):
    result = ""
    read_log("parsing time_to_read as  minutes: {0}".format(time_to_read))
    if time_to_read != None and time_to_read != "":
        import re
        match = re.search(r"\*+ \((\d+)h (\d+)m\)", time_to_read)
        if match:
            hours = int(match.group(1))
            minutes = int(match.group(2))
            result = (hours * 60) + minutes
    read_log("minutes to read: {0}".format(result))
    return result

def read_log(message):
    print("Willowcat ", ": ", message)