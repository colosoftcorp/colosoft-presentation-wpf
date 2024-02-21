using System.Windows.Input;

namespace Colosoft.Presentation.Input
{
    public class RoutedCommandFactory : IRoutedCommandFactory
    {
        private readonly System.Windows.Input.KeyGestureConverter keyGestureConverter = new System.Windows.Input.KeyGestureConverter();

        public IRoutedCommand Create(string name, ICommand command, CommandDescription description)
        {
            return new RoutedCommandWrapper(
                command,
                name,
                typeof(PresentationData.ControlData),
                description,
                this.GetGestures(description?.Gestures));
        }

        private System.Windows.Input.InputGestureCollection GetGestures(InputGestureCollection source)
        {
            if (source == null)
            {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
                return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
            }

            var result = new System.Windows.Input.InputGestureCollection();

            foreach (InputGesture i in source)
            {
                if (i is KeyGesture keyGesture)
                {
                    var keyGesture2 = this.keyGestureConverter.ConvertFromInvariantString(keyGesture.GetDisplayStringForCulture(System.Globalization.CultureInfo.CurrentUICulture));
                    result.Add(keyGesture2 as System.Windows.Input.KeyGesture);
                }
            }

            return result;
        }
    }
}
