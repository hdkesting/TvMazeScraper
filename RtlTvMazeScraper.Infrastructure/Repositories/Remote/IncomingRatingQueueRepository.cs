// <copyright file="IncomingRatingQueueRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Repositories.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Transfer;

    public class IncomingRatingQueueRepository : IIncomingRatingRepository
    {
        public async Task<List<ShowRating>> GetQueuedRatings()
        {
            // TODO connect to Azure queue, get a list of ratings
            throw new NotImplementedException();
        }
    }
}
