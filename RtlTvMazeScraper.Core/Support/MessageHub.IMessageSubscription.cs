// <copyright file="MessageHub.IMessageSubscription.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    using System;

    /// <summary>
    /// <see cref="MessageHub"/>, the interface to use to store event subscriptions.
    /// </summary>
    public partial class MessageHub
    {
        /// <summary>
        /// Interface to non-generic message subscription.
        /// </summary>
        private interface IMessageSubscription
        {
            /// <summary>
            /// Gets the type of the service to perform the action on.
            /// </summary>
            /// <value>
            /// The type of the service.
            /// </value>
            Type ServiceType { get; }

            /// <summary>
            /// Gets the type of the event to subscribe to.
            /// </summary>
            /// <value>
            /// The type of the event.
            /// </value>
            Type EventType { get; }

            /// <summary>
            /// Gets the method to execute.
            /// </summary>
            /// <value>
            /// The action.
            /// </value>
            string MethodName { get; }

            /// <summary>
            /// Gets the token.
            /// </summary>
            /// <value>
            /// The token.
            /// </value>
            Guid Token { get; }
        }
    }
}
