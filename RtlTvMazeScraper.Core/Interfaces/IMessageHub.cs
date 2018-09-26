// <copyright file="IMessageHub.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to MessageHub.
    /// </summary>
    public interface IMessageHub
    {
        /// <summary>
        /// Publishes the specified message to any subscriber.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">The message instance.</param>
        /// <returns>A Task.</returns>
        Task Publish<TMessage>(TMessage message);
    }
}
