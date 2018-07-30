// <copyright file="StaticQueue.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Workers
{
    using System.Collections.Generic;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    /// <summary>
    /// A static queue of show IDs to check.
    /// </summary>
    public static class StaticQueue
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private static readonly Queue<int> ShowIdQueue = new Queue<int>(50);

        /// <summary>
        /// Adds the show ids to the queue.
        /// </summary>
        /// <param name="startId">The start identifier.</param>
        /// <param name="count">The count.</param>
        public static void AddShowIds(int startId, int count = 10)
        {
            if (startId > 0)
            {
                lock (ShowIdQueue)
                {
                    for (int n = 0; n < count; n++)
                    {
                        int id = startId + n;
                        if (!ShowIdQueue.Contains(id))
                        {
                            ShowIdQueue.Enqueue(id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public static void ClearQueue()
        {
            lock (ShowIdQueue)
            {
                ShowIdQueue.Clear();
            }
        }

        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <returns>The next ID or <c>null</c> when empty.</returns>
        public static int? GetNextId()
        {
            lock (ShowIdQueue)
            {
                if (ShowIdQueue.Count == 0)
                {
                    return null;
                }

                return ShowIdQueue.Dequeue();
            }
        }
    }
}
