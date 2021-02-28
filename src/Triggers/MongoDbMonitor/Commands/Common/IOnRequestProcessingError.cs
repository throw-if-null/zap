using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbMonitor.Commands.Common
{
    public interface IOnRequestProcessingError
    {
        void OnError([NotNull] Exception ex);
    }
}
