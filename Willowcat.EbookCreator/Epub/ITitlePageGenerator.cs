using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public interface ITitlePageGenerator
    {
        FileItemModel CreateTitlePageFile(IBibliographyModel bibliography);
    }
}