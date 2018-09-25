// <copyright file="MessageHub.MessageSubscription.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// <see cref="MessageHub"/>, the class to use to store event subscriptions.
    /// </summary>
    public partial class MessageHub
    {
        /// <summary>
        /// Subscription to an event message.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <seealso cref="IMessageSubscription" />
        private sealed class MessageSubscription<T> : IMessageSubscription
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MessageSubscription{T}"/> class.
            /// </summary>
            /// <param name="action">The action.</param>
            public MessageSubscription(Func<T, Task> action)
            {
                this.EventType = typeof(T);
                this.Token = Guid.NewGuid();
                this.Action = action ?? throw new ArgumentNullException(nameof(action));
            }

            /// <summary>
            /// Gets the type of the event.
            /// </summary>
            /// <value>
            /// The type of the event.
            /// </value>
            public Type EventType { get; }

            /// <summary>
            /// Gets the action to perform.
            /// </summary>
            /// <value>
            /// The action.
            /// </value>
            public Func<T, Task> Action { get; }

            /// <summary>
            /// Gets the token.
            /// </summary>
            /// <value>
            /// The token.
            /// </value>
            public Guid Token { get; }
        }
    }
}
