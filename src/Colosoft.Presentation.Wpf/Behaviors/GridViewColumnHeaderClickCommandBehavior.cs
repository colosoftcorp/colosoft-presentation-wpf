using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Colosoft.Presentation.Behaviors
{
    public class GridViewColumnHeaderClickCommandBehavior
    {
        public GridViewColumnHeaderClickCommandBehavior(ListView element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(this.ClickEventHandler));
        }

        public ICommand Command { get; set; }

        private void ClickEventHandler(object sender, RoutedEventArgs e)
        {
            ICommand localCommand = this.Command;
            var columnHeader = e.OriginalSource as GridViewColumnHeader;

            var member = columnHeader.Column.DisplayMemberBinding as Binding;

            if ((localCommand != null) && localCommand.CanExecute(member.Path.Path))
            {
                localCommand.Execute(member.Path.Path);
            }
        }
    }
}