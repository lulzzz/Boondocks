using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetFusion.Messaging;
using NetFusion.Messaging.Core;
using NetFusion.Messaging.Types;

namespace Boondocks.Auth.Tests.Mocks
{
    /// <summary>
    /// Mocks the service responsible for dispatching commands with known results for unit-testing.
    /// </summary>
    public class MockMessagingService : IMessagingService
    {
        private Dictionary<Type, object> _mockCommandResponses = new Dictionary<Type, object>();
        private List<IMessage> _receivedMessages = new List<IMessage>();

        /// <summary>
        /// The received messages that were dispatched.
        /// </summary>
        public IReadOnlyCollection<IMessage> ReceivedMessages { get; }

        private MockMessagingService()
        {
            ReceivedMessages = _receivedMessages.AsReadOnly();
        }

        /// <summary>
        /// Allows specifying a method that is invoked to allow configuration of the
        /// messaging service with known message responses.
        /// </summary>
        /// <param name="results">Action passed the created messaging service to be configured.</param>
        /// <returns>The created messaging service.</returns>
        public static MockMessagingService Setup(Action<MockMessagingService> results = null)
        {
            var service = new MockMessagingService();
            results?.Invoke(service);
            return service;
        }

        /// <summary>
        /// Allows the registration of a know result for a specific type of command.
        /// </summary>
        /// <typeparam name="TCommand">The command that will be dispatched.</typeparam>
        /// <typeparam name="TResult">The expected result for the dispatched command.</typeparam>
        /// <param name="expectedResult">The expected command result.</param>
        public void RegisterResponse<TCommand, TResult>(TResult expectedResult)
            where TCommand : ICommand<TResult>
        {
            _mockCommandResponses[typeof(TCommand)] = expectedResult;
        }

        public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, 
            CancellationToken cancellationToken = default(CancellationToken),
            IntegrationTypes integrationType = IntegrationTypes.All)
        {
            var result = (TResult)_mockCommandResponses[command.GetType()];

            _receivedMessages.Add(command);
            return Task.FromResult(result);
        }

        public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default(CancellationToken), IntegrationTypes integrationType = IntegrationTypes.All)
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync(IEventSource eventSource, CancellationToken cancellationToken = default(CancellationToken), IntegrationTypes integrationType = IntegrationTypes.All)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken), IntegrationTypes integrationType = IntegrationTypes.All)
        {
            throw new NotImplementedException();
        }
    }
}