using MediatR;
using MongoDB.Driver;
using MongoDbMonitor.Commands.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventRequest : IRequest, IOnRequestProcessingError
    {
        public ChangeStreamDocument<dynamic> Document { get; set; }

        public void OnError([NotNull] Exception ex)
        {
            return;
        }
    }
}
