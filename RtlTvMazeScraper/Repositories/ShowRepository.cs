using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using RtlTvMazeScraper.Interfaces;
using RtlTvMazeScraper.Models;

namespace RtlTvMazeScraper.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly string connstr;

        public ShowRepository(ISettingRepository settingRepository)
        {
            this.connstr = settingRepository.ConnectionString;
        }

        public async Task<List<Show>> GetShows(int startId, int count)
        {
            var result = new List<Show>();

            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                SqlCommand showCmd = new SqlCommand("SELECT id, name FROM Shows WHERE id BETWEEN @start and @end", conn);
                showCmd.Parameters.Add(new SqlParameter("start", System.Data.SqlDbType.Int) { Value = startId });
                showCmd.Parameters.Add(new SqlParameter("end", System.Data.SqlDbType.Int) { Value = startId + count - 1 });

                conn.Open();

                using (var showreader = await showCmd.ExecuteReaderAsync())
                {
                    while (showreader.Read())
                    {
                        var show = new Show()
                        {
                            Id = showreader.GetInt32(0),
                            Name = showreader.GetString(1)
                        };

                        result.Add(show);
                    }
                }

                SqlCommand castCmd = new SqlCommand("SELECT MemberId, Name, Birthdate FROM CastMembers WHERE ShowId = @show", conn);
                var showId = new SqlParameter("show", System.Data.SqlDbType.Int);
                castCmd.Parameters.Add(showId);

                foreach (var show in result)
                {
                    showId.Value = show.Id;
                    using (var castreader = await castCmd.ExecuteReaderAsync())
                    {
                        var member = new CastMember
                        {
                            Id = castreader.GetInt32(0),
                            Name = castreader.GetString(1),
                            Birthdate = castreader.IsDBNull(2) ? default(DateTime?) : castreader.GetDateTime(2)
                        };
                        show.Cast.Add(member);
                    }
                }
            }

            return result;
        }

        public async Task<List<Show>> GetShowsWithCast(int page, int pagesize)
        {
            var shows = await this.GetShowsByPage(page, pagesize);

            // get cast for these shows
            await this.ReadCast(shows);

            return shows;
        }

        public async Task StoreShowList(List<Show> list, Func<int, Task<List<CastMember>>> getCastOfShow)
        {
            // alleen als de ID niet bekend is, dan INSERT
            // mogelijk een callback om de cast op te halen?

            foreach (var show in list)
            {
                var existing = await GetShowById(show.Id);

                if (existing == null)
                {
                    await AddShow(show);
                }
                else if (existing.Name != show.Name)
                {
                    await UpdateShowName(show);
                }
                // else: already stored

                List<CastMember> cast;
                if (getCastOfShow == null || show.Cast.Any())
                {
                    cast = show.Cast;
                }
                else
                {
                    cast = await getCastOfShow(show.Id);
                }

                await StoreCastList(show.Id, cast);
            }
        }

        public async Task<(int shows, int members)> GetCounts()
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand("SELECT count(1) FROM Shows", conn);

                var shows = (await showCmd.ExecuteScalarAsync()) as int?;

                showCmd = new SqlCommand("SELECT count(1) FROM CastMembers", conn);
                var members = (await showCmd.ExecuteScalarAsync()) as int?;

                return (shows.GetValueOrDefault(), members.GetValueOrDefault());
            }
        }

        public async Task<int> GetMaxShowId()
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand("SELECT max(id) FROM Shows", conn);

                var max = (await showCmd.ExecuteScalarAsync()) as int?;

                return max.GetValueOrDefault();
            }
        }

        public async Task<List<CastMember>> GetCastOfShow(int showId)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT memberId, name, birthdate FROM CastMembers WHERE showId=@showId", conn);
                cmd.Parameters.Add(new SqlParameter("showId", System.Data.SqlDbType.Int) { Value = showId });

                var reader = await cmd.ExecuteReaderAsync();

                var cast = new List<CastMember>();
                while (reader.Read())
                {
                    var member = new CastMember
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Birthdate = reader.IsDBNull(2) ? default(DateTime?) : reader.GetDateTime(2)
                    };
                    cast.Add(member);
                }

                return cast;
            }
        }

        /// <summary>
        /// Add the cast to the supplied shows.
        /// </summary>
        /// <param name="shows"></param>
        /// <returns></returns>
        private async Task ReadCast(List<Show> shows)
        {
            int minShowId = shows.Select(s => s.Id).Min();
            int maxShowId = shows.Select(s => s.Id).Max();

            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT ShowId, MemberId, Name, Birthdate 
FROM CastMembers 
WHERE showId BETWEEN @min and @max
ORDER BY ShowId, Birthdate desc", conn);
                cmd.Parameters.Add(new SqlParameter("min", System.Data.SqlDbType.Int) { Value = minShowId });
                cmd.Parameters.Add(new SqlParameter("max", System.Data.SqlDbType.Int) { Value = maxShowId });

                var reader = await cmd.ExecuteReaderAsync();

                var allcast = new List<(int show, CastMember member)>();
                while (reader.Read())
                {
                    var showId = reader.GetInt32(0);
                    var member = new CastMember
                    {
                        Id = reader.GetInt32(1),
                        Name = reader.GetString(2),
                        Birthdate = reader.IsDBNull(3) ? default(DateTime?) : reader.GetDateTime(3)
                    };
                    allcast.Add((showId, member));
                }

                foreach (var show in shows)
                {
                    var cast = allcast.Where(c => c.show == show.Id).Select(c => c.member).ToList();
                    show.Cast.AddRange(cast);
                }
            }
        }

        /// <summary>
        /// Gets the shows by page.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The page size.</param>
        /// <returns></returns>
        private async Task<List<Show>> GetShowsByPage(int page, int pagesize)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand(@"
SELECT Id, Name 
FROM Shows
ORDER BY Id
OFFSET @start Rows
FETCH NEXT @size ROWS ONLY", conn);
                showCmd.Parameters.Add(new SqlParameter("start", System.Data.SqlDbType.Int) { Value = page * pagesize + 1 });
                showCmd.Parameters.Add(new SqlParameter("size", System.Data.SqlDbType.Int) { Value = pagesize });

                var result = new List<Show>();
                var reader = await showCmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var show = new Show
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };

                    result.Add(show);
                }

                return result;
            }

        }

        private async Task StoreCastList(int showId, List<CastMember> cast)
        {
            var existingCast = await GetCastOfShow(showId);
            foreach (var member in cast)
            {
                var existingMember = existingCast.FirstOrDefault(m => m.Id == member.Id);

                if (existingMember == null)
                {
                    await AddCastMember(showId, member);
                    existingCast.Add(member);
                }
                else if (existingMember.Name != member.Name || existingMember.Birthdate != member.Birthdate)
                {
                    await UpdateCastMember(showId, member);
                    existingMember.Name = member.Name;
                    existingMember.Birthdate = member.Birthdate;
                }
                // else: already stored
            }
        }

        private async Task AddCastMember(int showId, CastMember member)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand("INSERT INTO CastMembers (ShowId, MemberId, Name, Birthdate) VALUES (@showId, @memberId, @name, @birthdate)", conn);
                showCmd.Parameters.Add(new SqlParameter("showId", System.Data.SqlDbType.Int) { Value = showId });
                showCmd.Parameters.Add(new SqlParameter("memberId", System.Data.SqlDbType.Int) { Value = member.Id });
                showCmd.Parameters.Add(new SqlParameter("name", System.Data.SqlDbType.NVarChar, 256) { Value = member.Name });
                showCmd.Parameters.Add(new SqlParameter("birthdate", System.Data.SqlDbType.Date)
                {
                    Value = member.Birthdate.HasValue
                        ? (object)member.Birthdate.Value.Date
                        : DBNull.Value
                });

                await showCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateCastMember(int showId, CastMember member)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand(@"
UPDATE CastMembers 
SET Name=@name,
    Birthdate=@birthdate
WHERE ShowId=@showId
  AND MemberId=@memberId", conn);
                showCmd.Parameters.Add(new SqlParameter("name", System.Data.SqlDbType.NVarChar, 256) { Value = member.Name });
                showCmd.Parameters.Add(new SqlParameter("birthdate", System.Data.SqlDbType.Date)
                {
                    Value = member.Birthdate.HasValue
                        ? (object)member.Birthdate.Value.Date
                        : DBNull.Value
                });
                showCmd.Parameters.Add(new SqlParameter("showId", System.Data.SqlDbType.Int) { Value = showId });
                showCmd.Parameters.Add(new SqlParameter("memberId", System.Data.SqlDbType.Int) { Value = member.Id });

                await showCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateShowName(Show show)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand("UPDATE Shows SET Name=@Name WHERE ID=@Id", conn);
                showCmd.Parameters.Add(new SqlParameter("Name", System.Data.SqlDbType.NVarChar) { Value = show.Name });
                showCmd.Parameters.Add(new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = show.Id });

                await showCmd.ExecuteNonQueryAsync();

            }
        }

        private async Task AddShow(Show show)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand("INSERT INTO Shows (Id, Name) VALUES (@Id, @Name)", conn);
                showCmd.Parameters.Add(new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = show.Id });
                showCmd.Parameters.Add(new SqlParameter("Name", System.Data.SqlDbType.NVarChar) { Value = show.Name });

                await showCmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<Show> GetShowById(int id)
        {
            using (SqlConnection conn = new SqlConnection(this.connstr))
            {
                conn.Open();

                SqlCommand showCmd = new SqlCommand("SELECT Name FROM Shows WHERE id=@Id", conn);
                showCmd.Parameters.Add(new SqlParameter("Id", System.Data.SqlDbType.Int) { Value = id });
                var name = (await showCmd.ExecuteScalarAsync()) as string;

                if (!String.IsNullOrEmpty(name))
                {
                    return new Show { Id = id, Name = name };
                }
            }

            return null;
        }
    }
}