using Colosoft.Presentation;
using WpfSample.ViewModels;

namespace WpfSample
{
    public partial class MainWindow : IViewFor<ViewModels.MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
