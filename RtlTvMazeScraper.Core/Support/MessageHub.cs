// <copyright file="MessageHub.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A MessageBus implementation.
    /// </summary>
    /// <remarks>
    /// No persistence required, no errorhandling implemented.
    /// </remarks>
    /// <example>
    /// Publish:
    /// <code>await Support.MessageHub.Instance.Publish(new Support.Events.BoardIsFinished());</code>
    /// Subscribe (usually in ctor):
    /// <code>Support.MessageHub.Instance.Subscribe&lt;Support.Events.BoardIsFinished&gt;(m => this.gameBoard.FlashAllGroups());</code>
    /// Note that Subscribe returns a token (<see cref="Guid"/>), remember that to clean up in Dispose:
    /// <code>private readonly List&lt;Guid&gt; eventTokens = new List&lt;Guid&gt;();</code>
    /// <code>this.eventTokens.ForEach(Support.MessageHub.Instance.Unsubscribe);</code>
    /// </example>
    public sealed partial class MessageHub : IMessageHub
    {
        private static readonly List<IMessageSubscription> Subscriptions = new List<IMessageSubscription>();
        private readonly IServiceProvider services;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHub"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        public MessageHub(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// Gets the subscription count.
        /// </summary>
        /// <value>
        /// The subscription count.
        /// </value>
        public static int SubscriptionCount => Subscriptions.Count;

        /// <summary>
        /// Subscribes to events of the specified type to execute the specified action.
        /// </summary>
        /// <typeparam name="TService">The type of the service (or -interface) to activate.</typeparam>
        /// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
        /// <param name="methodName">Name of the method in the service, that accepts a single TMessage parameter and returns a Task.</param>
        /// <returns>
        /// The token for the subscription.
        /// </returns>
        public static Guid Subscribe<TService, TMessage>(string methodName)
        {
            //// TODO inspect the input: does the method exist and use the correct params?

            lock (Subscriptions)
            {
                var sub = new MessageSubscription<TService, TMessage>(methodName);
                Subscriptions.Add(sub);
                return sub.Token;
            }
        }

        /// <summary>
        /// Unsubscribes the action with the specified token.
        /// </summary>
        /// <param name="token">The token to unsubscribe.</param>
        public static void Unsubscribe(Guid token)
        {
            lock (Subscriptions)
            {
                Subscriptions.RemoveAll(ms => ms.Token == token);
            }
        }

        /// <summary>
        /// Publishes the specified message to any subscriber.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <returns>A Task.</returns>
        public async Task Publish<TMessage>(TMessage message)
        {
            var tasks = new List<Task>();

            // first get back to the UI thread
            await Task.Yield();

            // then process this event
            lock (Subscriptions)
            {
                foreach (var sub in Subscriptions.Where(s => s.EventType == typeof(TMessage)))
                {
                    // get a fresh instance of the service
                    var svcType = sub.ServiceType;
                    var svc = this.services.GetService(sub.ServiceType);

                    // get the method to execute
                    var methodName = sub.MethodName;
                    var actionMethod = sub.ServiceType.GetMethod(methodName);

                    // and execute it, passing the message and expecting a Task as return.
                    var task = (Task)actionMethod.Invoke(svc, new object[] { message });

                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
