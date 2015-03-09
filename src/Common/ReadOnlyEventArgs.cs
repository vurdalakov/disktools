namespace Vurdalakov
{
    using System;

    public class ReadOnlyEventArgs<T> : EventArgs
    {
        public T Parameter { get; private set; }

        public ReadOnlyEventArgs(T parameter)
        {
            Parameter = parameter;
        }
    }
}
