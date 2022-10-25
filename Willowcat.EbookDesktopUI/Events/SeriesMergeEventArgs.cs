using System;
using System.Collections.Generic;
using System.Text;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.Events
{
    public class SeriesMergeEventArgs
    {
        public SeriesMergeEventArgs(EpubDisplayModel displayModel, EpubSeriesModel seriesModel)
        {
            DisplayModel = displayModel;
            SeriesModel = seriesModel;
        }

        public EpubDisplayModel DisplayModel { get; }

        public EpubSeriesModel SeriesModel { get; }
    }
}
