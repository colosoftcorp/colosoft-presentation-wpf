using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation
{
    public class UserControl<TViewModel> :
        UserControl, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(TViewModel),
                typeof(UserControl<TViewModel>),
                new PropertyMetadata(null));

        public TViewModel BindingRoot => ViewModel;

        public TViewModel ViewModel
        {
            get => (TViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (TViewModel)value;
        }
    }
}
