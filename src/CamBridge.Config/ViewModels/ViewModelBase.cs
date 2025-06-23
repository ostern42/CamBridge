using CommunityToolkit.Mvvm.ComponentModel;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class ViewModelBase : ObservableValidator
    {
        private bool _isLoading;

        /// <summary>
        /// Gets or sets whether the view model is currently loading data
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected ViewModelBase()
        {
        }
    }
}
