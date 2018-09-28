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
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A MessageBus implementation.
    /// </summary>
    /// <remarks>
    /// No persistence required, no errorhandling implemented.
    /// </remarks>
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
        /// <param name="methodName">Name of the method in the service, that accepts a single TMessage parameter and returns a Task. Use <c>nameof</c>.</param>
        /// <returns>
        /// The token for the subscription.
        /// </returns>
        public static Guid Subscribe<TService, TMessage>(string methodName)
        {
            // inspect the input: does the method exist and use the correct param?
            SubscriptionSanityCheck<TService, TMessage>(methodName);

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
                    var actionMethod = sub.ServiceType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

                    // and execute it, passing the message and expecting a Task as return.
                    var task = (Task)actionMethod.Invoke(svc, new object[] { message });

                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static void SubscriptionSanityCheck<TService, TMessage>(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException(nameof(methodName), "The subscription needs a method to call.");
            }

            var method = typeof(TService).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            if (method == null)
            {
                throw new InvalidOperationException($"The method '{methodName}' is not found. It must be a public instance method.");
            }

            var parms = method.GetParameters();
            if (parms.Length != 1 || !parms[0].ParameterType.IsAssignableFrom(typeof(TMessage)))
            {
                throw new InvalidOperationException($"The method '{methodName}' should accept 1 parameter, of type {typeof(TMessage).Name}");
            }

            var ret = method.ReturnType;
            if (ret != typeof(Task))
            {
                throw new InvalidOperationException($"The method '{methodName}' should return a Task.");
            }
        }
    }
}
