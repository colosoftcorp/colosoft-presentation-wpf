namespace Colosoft.Presentation.Input
{
    internal class InputGestureWrapper : InputGesture
    {
        private readonly System.Windows.Input.InputGesture inner;

        public InputGestureWrapper(System.Windows.Input.InputGesture inner)
        {
            this.inner = inner;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            return this.inner.Matches(
                targetElement,
                new System.Windows.Input.InputEventArgs(
                    null,
                    inputEventArgs.Timestamp));
        }
    }
}