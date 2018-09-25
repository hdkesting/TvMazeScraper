// <copyright file="MessageHub.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
    public sealed partial class MessageHub
    {
        private readonly List<IMessageSubscription> subscriptions = new List<IMessageSubscription>();

        /// <summary>
        /// Gets the subscription count.
        /// </summary>
        /// <value>
        /// The subscription count.
        /// </value>
        public int SubscriptionCount => this.subscriptions.Count;

        /// <summary>
        /// Subscribes to events of the specified type to execute the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="action">The async action to perform.</param>
        /// <returns>The token for the subscription.</returns>
        public Guid Subscribe<T>(Func<T, Task> action)
        {
            lock (this.subscriptions)
            {
                var sub = new MessageSubscription<T>(action);
                this.subscriptions.Add(sub);
                return sub.Token;
            }
        }

        /// <summary>
        /// Unsubscribes the action with the specified token.
        /// </summary>
        /// <param name="token">The token to unsubscribe.</param>
        public void Unsubscribe(Guid token)
        {
            lock (this.subscriptions)
            {
                this.subscriptions.RemoveAll(ms => ms.Token == token);
            }
        }

        /// <summary>
        /// Publishes the specified message to any subscriber.
        /// </summary>
        /// <typeparam name="T">Type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>A Task.</returns>
        public async Task Publish<T>(T message)
        {
            List<Task> tasks;

            // first get back to the UI thread
            await Task.Yield();

            // then process this event
            lock (this.subscriptions)
            {
                tasks = this.subscriptions
                    .Where(s => s.EventType == typeof(T))
                    .Select(s => (MessageSubscription<T>)s)
                    .Select(ts => ts.Action(message))
                    .ToList();
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
