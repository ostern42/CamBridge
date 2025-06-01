using CommunityToolkit.Mvvm.ComponentModel;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class ViewModelBase : ObservableValidator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ViewModelBase()
        {
        }
    }
}
