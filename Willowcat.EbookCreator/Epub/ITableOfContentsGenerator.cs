using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public interface ITableOfContentsGenerator
    {
        FileItemModel CreateTableOfContents(BibliographyModel bibliography, TableOfContentsModel tableOfContents);
    }
}