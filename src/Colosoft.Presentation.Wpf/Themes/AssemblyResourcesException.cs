using System;

namespace Colosoft.Presentation.Themes
{
    [Serializable]
    public class AssemblyResourcesException : Exception
    {
        public AssemblyResourcesException(string message)
            : base(message)
        {
        }

        public AssemblyResourcesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AssemblyResourcesException()
        {
        }

        protected AssemblyResourcesException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
