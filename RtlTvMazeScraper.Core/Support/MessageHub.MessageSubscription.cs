// <copyright file="MessageHub.MessageSubscription.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    using System;

    /// <summary>
    /// <see cref="MessageHub"/>, the class to use to store event subscriptions.
    /// </summary>
    public partial class MessageHub
    {
        /// <summary>
        /// Subscription to an event message.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <seealso cref="IMessageSubscription" />
        private sealed class MessageSubscription<TService, TMessage> : IMessageSubscription
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MessageSubscription{TService, TMessage}" /> class.
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is not supplied.</exception>
            public MessageSubscription(string methodName)
            {
                this.EventType = typeof(TMessage);
                this.ServiceType = typeof(TService);
                this.Token = Guid.NewGuid();
                this.MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            }

            /// <summary>
            /// Gets the type of the service to perform the action on.
            /// </summary>
            /// <value>
            /// The type of the service.
            /// </value>
            public Type ServiceType { get; }

            /// <summary>
            /// Gets the type of the event.
            /// </summary>
            /// <value>
            /// The type of the event.
            /// </value>
            public Type EventType { get; }

            /// <summary>
            /// Gets the method to execute.
            /// </summary>
            /// <value>
            /// The action.
            /// </value>
            public string MethodName { get; }

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
