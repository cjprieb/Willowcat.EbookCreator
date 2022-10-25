using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public interface ITableOfContentsGenerator
    {
        FileItemModel CreateTableOfContents(IBibliographyModel bibliography, TableOfContentsModel tableOfContents);
    }
}