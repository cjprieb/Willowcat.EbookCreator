using Microsoft.Extensions.Logging;
using Willowcat.Common.UI.ViewModel;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class LogItemViewModel : ViewModelBase
    {
        #region Member Variables...
        private LogLevel _LogLevel;
        private string _Message = string.Empty;

        #endregion Member Variables...

        #region Properties...

        #region LogLevel
        public LogLevel LogLevel
        {
            get => _LogLevel;
            private set
            {
                _LogLevel = value;
                OnPropertyChanged();
            }
        }
        #endregion LogLevel

        #region Message
        public string Message
        {
            get => _Message;
            private set
            {
                _Message = value;
                OnPropertyChanged();
            }
        }
        #endregion Message

        #endregion Properties...

        #region Constructors...

        #region LogItemViewModel
        public LogItemViewModel(LogLevel level, string message)
        {
            LogLevel = level;
            Message = message;
        }
        #endregion LogItemViewModel

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
