namespace MongoDbMonitor.Commands.Common.ExceptionHandlers
{
    public class ExceptionHandlerOptions
    {
        /// <summary>
        /// Gets or sets the flag that determines whether exception handlers are disabled or not
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the flag that determines if <see cref="GlobalExceptionHandler{TRequest, TResponse, TException}"/> is enabled.
        /// </summary>
        /// <remarks>
        /// This flag overrides <see cref="Disabled"/> flag, meaning if <see cref="Disabled"/> flag is set to `true` and
        /// OnlyGlobal flag is also set to `true` global exception handling would be used.
        /// </remarks>
        public bool OnlyGlobal { get; set; }
    }
}
