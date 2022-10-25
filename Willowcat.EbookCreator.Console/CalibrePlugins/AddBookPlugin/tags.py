#!/usr/bin/env python
# vim:fileencoding=UTF-8:ts=4:sw=4:sta:et:sts=4:ai

import configparser
from os.path import exists
from calibre_plugins.willowcat_add_book.config import prefs

class TagHelper():

    tags_to_remove = []
    tag_mappings = {}
    tag_fanfiction_mappings = {}

    def __init__(self):
        path = prefs['tag_conversion_config_path']
        if exists(path): self.load_from_configuration(path)
        else: print("No tag configuration file found at '{0}'".format(path))

    def load_from_configuration(self, path):
        # Config file stuff        
        config = configparser.ConfigParser(allow_no_value = True)
        config.read(path)

        for item in config["REMOVE"]: 
            self.tags_to_remove.append(item)

        for key in config["CONVERT"]: 
            self.tag_mappings[key] = config["CONVERT"][key]

            #AU.Canon Divergence, Sub.Ghosts, Tone.Angst, Tone.Fluff, Tone.Hurt/Comfort, Warning.Graphic Depictions of Violence

        # for key in config["CONVERT_FANFICTION"]: 
        #     self.tag_fanfiction_mappings[key] = config["CONVERT_FANFICTION"][key]

    def process_tags(self, fanfiction_tags, tags):
        new_tags = []
        for tag in tags: new_tags.append(tag)
        
        for tag in fanfiction_tags:
            if not self.should_remove_tag(tag.lower()):
                converted_tags = self.convert_tag_as_normal(tag.lower())
                for tag in converted_tags: new_tags.append(tag)

        return new_tags

    def should_remove_tag(self, tag):
        result = False

        for remove_tag in self.tags_to_remove:
            if remove_tag == tag: 
                result = True
                break

        # if tag == "fanworks": return True
        # if tag == "general audiences": return True
        # if tag == "teen and up audiences": return True

        # if tag == "no archive warnings apply": return True
        # if tag == "choose not to use archive warnings": return True

        # if tag == "gen" or tag == 'f/f' or tag == 'f/m' or tag == "m/m": return True

        return result

    def convert_tag_as_fanfiction(self, tag):
        import re
        new_tags = []

        for input_tag in self.tag_fanfiction_mappings.keys():
            if input_tag in tag: 
                new_tags.append(self.tag_fanfiction_mappings[input_tag])

        # if "fluff" in tag: new_tags.append("Tone.Fluff")
        # # if "humor" in tag: new_tags.append("Tone.Humor")
        # if "angst" in tag: new_tags.append("Tone.Angst")
        # if "hurt/comfort" in tag: new_tags.append("Tone.Hurt/Comfort")
        # if "happy ending" in tag: new_tags.append("Tone.Happy Ending")
        # if "crack" in tag: new_tags.append("Tone.Crack")
        # if "pining" in tag: new_tags.append("Tone.Pining")
        # if "mature" in tag: new_tags.append("Tone.Mature")

        # if "secret identity" in tag: new_tags.append("Sub.Secret Identity")

        # if "batfamily" in tag: new_tags.append("Ch.Batfamily")
        # if "bat family" in tag: new_tags.append("Ch.Batfamily")
        # if "bat siblings" in tag: new_tags.append("Ch.Batfamily")
        # if "bat brothers" in tag: new_tags.append("Ch.Batfamily")

        # if "graphic depictions of violence" in tag: new_tags.append("Warning.Graphic Depictions of Violence")
        # if "canon-typical violence" in tag: new_tags.append("Warning.Canon-Typical Violence")

        if "alternate universe" in tag:
            au_pattern = re.compile("(.+) - (.+)")
            match = au_pattern.match(tag)
            if match != None:
                new_tags.append("AU." + match.group(2))
            else: new_tags.append("AU")

        return new_tags

    def convert_tag_as_normal(self, tag):
        import re
        new_tags = []

        # if "humor" in tag: new_tags.append("Humor")

        for input_tag in self.tag_mappings.keys():
            if input_tag in tag: 
                new_tags.append(self.tag_mappings[input_tag])

        return new_tags