// <copyright file="ShowStoredEvent.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support.Events
{
    /// <summary>
    /// A show has been stored, so the rating should be updated.
    /// </summary>
    public class ShowStoredEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowStoredEvent"/> class.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="imdbId">The IMDb identifier.</param>
        public ShowStoredEvent(int showId, string imdbId)
        {
            this.ShowId = showId;
            this.ImdbId = imdbId;
        }

        /// <summary>
        /// Gets the show identifier.
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        public int ShowId { get; }

        /// <summary>
        /// Gets the IMDb identifier.
        /// </summary>
        /// <value>
        /// The IMDb identifier.
        /// </value>
        public string ImdbId { get; }
    }
}
