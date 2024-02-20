using System.Windows.Input;

namespace Colosoft.Presentation.Input
{
    public class RoutedCommandFactory : IRoutedCommandFactory
    {
        private readonly KeyGestureConverter keyGestureConverter = new KeyGestureConverter();

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
                    result.Add(this.keyGestureConverter.ConvertFromInvariantString(keyGesture.GetDisplayStringForCulture(System.Globalization.CultureInfo.CurrentUICulture)) as System.Windows.Input.KeyGesture);
                }
            }

            return result;
        }
    }
}
