// <copyright file="StorageCount.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Transfer
{
    /// <summary>
    /// Holds the counts of the various entities in storage.
    /// </summary>
    public class StorageCount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCount"/> class.
        /// </summary>
        public StorageCount()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageCount"/> class.
        /// </summary>
        /// <param name="shows">The shows.</param>
        /// <param name="members">The members.</param>
        public StorageCount(int shows, int members)
        {
            this.ShowCount = shows;
            this.MemberCount = members;
        }

        /// <summary>
        /// Gets or sets the show count.
        /// </summary>
        /// <value>
        /// The show count.
        /// </value>
        public int ShowCount { get; set; }

        /// <summary>
        /// Gets or sets the member count.
        /// </summary>
        /// <value>
        /// The member count.
        /// </value>
        public int MemberCount { get; set; }
    }
}
