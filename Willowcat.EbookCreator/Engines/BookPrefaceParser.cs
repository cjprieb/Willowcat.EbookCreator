using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Willowcat.Common.Utilities;

namespace Willowcat.EbookCreator.Engines
{
    public class BookPrefaceParser
    {
        private readonly string[] _TagSections = new string[]
        {
            "Rating", "Archive Warning", "Category", "Fandom", "Relationship", "Character", "Additional Tags"
        };
        private string _FilePath = null;
        private CQ _Document = null;

        public BookPrefaceParser(string filePath)
        {
            _FilePath = filePath;
            _Document = CQ.CreateDocumentFromFile(filePath);
        }

        public string GetWorkUrl()
        {
            string url = null;
            var linkElements = _Document["a"];
            if (linkElements != null && linkElements.Any())
            {
                foreach (var ahrefElement in linkElements)
                {
                    string linkUrl = ahrefElement.GetAttribute("href");
                    if (linkUrl.Contains("works"))
                    {
                        url = linkUrl;
                        break;
                    }
                }
            }
            return url;
        }

        public Dictionary<string, List<string>> GetMetadataElements()
        {
            var tagsElement = _Document["dl.tags"];
            Dictionary<string, List<string>> metadataElements = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (tagsElement != null || tagsElement.Any())
            {
                string definitionTitle = null;
                foreach (var child in tagsElement.First().Children())
                {
                    if (child.NodeType != NodeType.ELEMENT_NODE) continue;

                    if (child.NodeName.EqualsIgnoreCase("dt"))
                    {
                        definitionTitle = GetDefinitionTitleFromText(child.InnerText);
                        metadataElements[definitionTitle] = new List<string>();
                    }
                    else if (child.NodeName.EqualsIgnoreCase("dd") && !string.IsNullOrEmpty(definitionTitle))
                    {
                        if (IsStatisticsSection(definitionTitle))
                        {
                            AddStatistics(metadataElements, child);
                        }
                        else if (IsSeriesSection(definitionTitle))
                        {
                            metadataElements[definitionTitle] = ParseSeries(child);
                        }
                        else if (IsTagSection(definitionTitle))
                        {
                            metadataElements[definitionTitle] = ParseTags(child);
                        }
                    }
                }
            }

            return metadataElements;
        }

        private void AddStatistics(Dictionary<string, List<string>> metadataElements, IDomObject child)
        {
            string statsText = child.InnerText;
            
            Regex statsPattern = new Regex(@"(\w[\w ]*\w): ([\d\-\/\?]+)");

            foreach (Match match in statsPattern.Matches(statsText))
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                metadataElements[key] = new List<string>() { value };
            }
        }

        private List<string> ParseSeries(IDomObject definitionElement)
        {
            return new List<string>()
            {
                definitionElement.InnerHTML.Replace('\n', ' ').Replace("\r", "")
            };
        }

        private List<string> ParseTags(IDomObject definitionElement)
        {
            List<string> tags = new List<string>();
            foreach (var child in definitionElement.ChildElements)
            {
                if (child.NodeType != NodeType.ELEMENT_NODE) continue;

                if (child.NodeName.EqualsIgnoreCase("a"))
                {
                    tags.Add(child.InnerText);
                }
            }
            return tags;
        }

        private string GetDefinitionTitleFromText(string innerText)
        {
            int colonIndex = innerText.IndexOf(':');
            if (colonIndex > 0)
            {
                return innerText.Substring(0, colonIndex).Trim();
            }
            else
            {
                return innerText.Trim();
            }
        }

        private bool IsSeriesSection(string definitionTitle)
        {
            return definitionTitle.Equals("Series", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsStatisticsSection(string definitionTitle)
        {
            return definitionTitle.Equals("Stats", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsTagSection(string definitionTitle)
        {
            return _TagSections.Any(tag => definitionTitle.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
