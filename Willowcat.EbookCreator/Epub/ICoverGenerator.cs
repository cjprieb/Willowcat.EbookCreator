using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public interface ICoverGenerator
    {
        CoverModel CreateCover(BibliographyModel bibliography, string coverFilePath);
    }
}
