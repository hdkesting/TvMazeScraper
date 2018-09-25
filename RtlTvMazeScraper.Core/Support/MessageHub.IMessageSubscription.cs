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
            /// Gets the type of the event.
            /// </summary>
            /// <value>
            /// The type of the event.
            /// </value>
            Type EventType { get; }

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
