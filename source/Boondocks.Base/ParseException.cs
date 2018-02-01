namespace Boondocks.Base
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Thrown when an error occurrs while parsing.
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}