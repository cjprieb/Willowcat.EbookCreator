using System;
using System.Collections.Generic;
using System.Linq;
using Willowcat.EbookCreator.Engines;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookDesktopUI.Services
{
    public class Ao3BibliographyModelFactory : BibliographyModelFactory
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        public override IBibliographyModel ExtractAdditionalMetadata(ExtractedEpubFilesModel ebook, IBibliographyModel model)
        {
            Ao3BibliographModel Result = null;
            try
            {
                var parser = new CalibreContentParser(ebook.ContentFilePath, true);
                Result = new Ao3BibliographModel(parser.ParseForBibliography());

                if (ebook.ChaptersFilePaths.Count >= 1)
                {
                    var metadata = new BookPrefaceParser(ebook.ChaptersFilePaths[0]);
                    Result.SetMetadata(metadata.GetMetadataElements());
                }
            }
            catch (Exception)
            {
                //throw new ApplicationException($"Error extracting files from {filePath}", ex);
            }
            return Result;
        }

        public override MergedBibliographyModel CreateMergedBibliographyModel(IBibliographyModel original)
        {
            return original is Ao3BibliographModel ? (Ao3BibliographModel)original : new Ao3BibliographModel(original);
        }

        #endregion Methods...
    }

    public class Ao3BibliographModel : MergedBibliographyModel
    {
        private Dictionary<string, List<string>> _SplitTags = new Dictionary<string, List<string>>();

        public Ao3BibliographModel(IBibliographyModel original) : base(original)
        {

        }

        internal void SetMetadata(Dictionary<string, List<string>> dictionaries)
        {
            _SplitTags = dictionaries;
        }

        protected override List<string> GetTags()
        {
            var rating = ParseRating("Rating");
            var warningTags = ParseWarning("Archive Warning");
            var fandomTags = GetSplitTags("Fandom");
            var relationshipTags = GetSplitTags("Relationship");
            var characterTags = GetSplitTags("Character");
            var parsedTags = GetSplitTags("Additional Tags");

            return rating
                .Union(warningTags)
                .Union(fandomTags)
                .Union(relationshipTags)
                .Union(characterTags)
                .Union(parsedTags)
                .ToList();
        }

        #region GetSplitTags
        private IEnumerable<string> GetSplitTags(string key)
        {
            if (!_SplitTags.ContainsKey(key))
            {
                return new string[] { };
            }

            return _SplitTags[key];
        }
        #endregion GetSplitTags

        protected override void MergeTags(IBibliographyModel bibliography)
        {
            if (bibliography is Ao3BibliographModel ao3Model)
            {
                foreach (var kvp in ao3Model._SplitTags)
                {
                    if (!_SplitTags.ContainsKey(kvp.Key))
                    {
                        _SplitTags[kvp.Key] = new List<string>();
                    }
                    var baseTags = _SplitTags[kvp.Key];
                    var newTags = kvp.Value;
                    foreach (var newTag in newTags)
                    {
                        if (!baseTags.Contains(newTag))
                        {
                            baseTags.Add(newTag);
                        }
                    }
                }
            }
            else
            {
                //don't do this?
                //base.MergeTags(bibliography);
            }
        }

        #region ParseRating
        private IEnumerable<string> ParseRating(string key)
        {
            var result = new string[] { };
            var list = GetSplitTags(key);
            if (list != null && list.Any())
            {
                var firstItem = list.First();
                if (firstItem != "General Audiences" && !firstItem.Contains("Teen"))
                {
                    result = new string[] { firstItem };
                }
            }
            return result;
        }
        #endregion ParseRating

        #region ParseWarning
        private IEnumerable<string> ParseWarning(string key)
        {
            var list = GetSplitTags(key);
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    if (item != "No Archive Warnings Apply" && item != "Creator Chose Not To Use Archive Warnings")
                    {
                        yield return item;
                    }
                }
            }
        }
        #endregion ParseWarning
    }
}
