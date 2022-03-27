using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public interface ICoverGenerator
    {
        CoverModel CreateCover(IBibliographyModel bibliography, string coverFilePath);
    }
}
