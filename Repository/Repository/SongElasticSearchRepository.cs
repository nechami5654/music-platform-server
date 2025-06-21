using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class SongElasticSearchRepository : ISongElasticSearchRepository
    {
        private readonly ElasticsearchClient _client;
        private readonly IContext _context;

        public SongElasticSearchRepository(ElasticsearchClient client, IContext context)
        {
            _client = client;
            _context = context;
        }

        public async Task<List<Song>> SearchSongsByLyricsAsync(string lyrics)
        {
            if (string.IsNullOrWhiteSpace(lyrics))
                throw new ArgumentException("Lyrics query is required.", nameof(lyrics));

            try
            {
                var wordCount = lyrics.Split(new[] { ' ', ',', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var minMatch = $"{Math.Max((int)(wordCount * 0.7), 4)}";

                var response = await _client.SearchAsync<Song>(s => s
                    .Index("songs")
                    .Query(q => q
                        .Match(m => m
                            .Field(f => f.Words)
                            .Query(lyrics)
                            .Fuzziness(new Fuzziness(2))
                            .MinimumShouldMatch(minMatch)
                        )
                    )
                );

                if (response == null || !response.IsValidResponse || response.Documents == null || !response.Documents.Any())
                {
                    Console.WriteLine("No results found / error");
                    return new List<Song>();
                }

                var scoredSongs = response.Hits
                    .Select(hit => new { Song = hit.Source, Score = hit.Score ?? 0 })
                    .OrderByDescending(x => x.Score)
                    .ToList();

                var songIds = scoredSongs.Select(x => x.Song.Id).ToList();
                var filteredSongs = _context.songs.Where(s => songIds.Contains(s.Id)).ToList();

                var orderedSongs = scoredSongs
                    .Select(scored => filteredSongs.FirstOrDefault(s => s.Id == scored.Song.Id))
                    .Where(s => s != null)
                    .ToList();

                return orderedSongs;
            }
            catch (Exception ex)
            {
                return new List<Song>();
            }
        }

        private string RemoveStopWords(string query)
        {
            HashSet<string> StopWords = new HashSet<string>
            {
                "שיר", "שירים", "של", "את", "עם", "גם", "אבל", "או", "כי", "הוא", "היא", "על", "זה",
                "זהו", "זאת", "מה", "מי", "איך", "למה", "ככה", "כן", "לא", "פה", "שם", "עוד"
            };
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var filteredWords = words.Where(word => !StopWords.Contains(word));
            return string.Join(" ", filteredWords);
        }

        private string RemovePrefix(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length < 2)
                return word; 

            char firstLetter = word[0];

            char[] prefixes = { 'ל', 'מ', 'ו', 'כ', 'ב' };

            return prefixes.Contains(firstLetter) ? word.Substring(1) : word;
        }

        public async Task<List<Song>> FreeSearchAsync(string text)
        {
            try
            {
                text = RemoveStopWords(text);

                var termsList = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(t => RemovePrefix(t))
                                    .Select(t => (object)t)
                                    .ToList();

                var response = await _client.SearchAsync<Song>(s => s
                    .Index("songs")
                    .Query(q => q
                        .Bool(b => b
                            .Should(
                                sh => sh.MultiMatch(m => m
                                    .Fields(new[] { "name^5", "words^3", "singer.name^5", "category^5", "targetAge^5", "type^5" })
                                    .Query(string.Join(" ", termsList))
                                    .Type(TextQueryType.BestFields)
                                    .Operator(Operator.Or)
                                    .Fuzziness(new Fuzziness(2))
                                    .MinimumShouldMatch("20%")
                                ),
                                sh => sh.MatchPhrasePrefix(m => m
                                    .Field("category")
                                    .Query(string.Join(" ", termsList))
                                    .Boost(6)
                                ),
                                sh => sh.MatchPhrasePrefix(m => m
                                    .Field("type")
                                    .Query(string.Join(" ", termsList))
                                    .Boost(6)
                                ),
                                sh => sh.Match(m => m.Field("targetAge").Query(string.Join(" ", termsList)).Fuzziness(new Fuzziness(2)).Boost(5)),
                                sh => sh.Match(m => m.Field("type").Query(string.Join(" ", termsList)).Fuzziness(new Fuzziness(2))),
                                sh => sh.Match(m => m.Field("category").Query(string.Join(" ", termsList)).Fuzziness(new Fuzziness(2))),
                                sh => sh.Match(m => m.Field("singer.name").Query(string.Join(" ", termsList)).Fuzziness(new Fuzziness(2)))
                            )
                           .MinimumShouldMatch(1)
                        )

                    )
                );

                if (!response.IsValidResponse || response.Documents == null || !response.Documents.Any())
                {
                    Console.WriteLine("No results found.");
                    return new List<Song>();
                }

                var scoredSongs = response.Hits
                    .Select(hit => new { Song = hit.Source, Score = hit.Score ?? 0 })
                    .OrderByDescending(x => x.Score)
                    .ToList();

                var songIds = scoredSongs.Select(x => x.Song.Id).ToList();
                var filteredSongs = _context.songs.Where(s => songIds.Contains(s.Id)).ToList();

                var listSong = scoredSongs
                    .Select(scored => filteredSongs.FirstOrDefault(s => s.Id == scored.Song.Id))
                    .Where(s => s != null)
                    .ToList();
                return listSong;
            }
            catch { 
                return new List<Song>();
            }
            
        }

        private static List<string> ConvertEnumToList<T>(T enumValue) where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Where(e => enumValue.HasFlag(e) && Convert.ToInt32(e) != 0)
                       .Select(e => e.ToString())
                       .ToList();
        }

        private static string ConvertEnumToString<T>(T enumValue) where T : Enum
        {
            return enumValue.ToString() ?? string.Empty;
        }

        public async Task AddIndexSongAsync(Song song)
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song));

            try
            {
                var singerName = await _context.singers
                    .Where(s => s.Id == song.SingerId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                var indexedSong = new
                {
                    id = song.Id,
                    name = song.Name,
                    date = song.Date,
                    words = song.Words,
                    numViews = song.NumViews,
                    singerId = song.SingerId,
                    singer = new { name = singerName },
                    type = ConvertEnumToString(song.Type),
                    category = ConvertEnumToString(song.Category),
                    targetAge = song.TargetAge
                };

                var response = await _client.IndexAsync(indexedSong, i => i
                    .Index("songs")
                    .Id(song.Id)
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"❌ Error indexing song: {response.DebugInformation}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding song to index.", ex);
            }
        }

        public async Task DeleteIndexSongAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid song id.", nameof(id));

            try
            {
                var response = await _client.DeleteAsync<Song>(id, d => d.Index("songs"));
                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"❌ Error deleting song from index: {response.DebugInformation}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting song from index.", ex);
            }
        }

        public async Task UpdateIndexSongAsync(Song song)
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song));

            try
            {
                var singerName = await _context.singers
                                   .Where(s => s.Id == song.SingerId)
                                   .Select(s => s.Name)
                                   .FirstOrDefaultAsync();

                var updatedSong = new
                {
                    id = song.Id,
                    name = song.Name,
                    date = song.Date,
                    words = song.Words,
                    numViews = song.NumViews,
                    singerId = song.SingerId,
                    singer = new { name = singerName },
                    type = ConvertEnumToList(song.Type),
                    category = ConvertEnumToList(song.Category),
                    targetAge = song.TargetAge
                };

                var response = await _client.UpdateAsync<Song, object>(song.Id, u => u
                    .Index("songs")
                    .Doc(updatedSong)
                    .DocAsUpsert(true)
                    .Id(song.Id)
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"❌ Error updating song in index: {response.DebugInformation}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating song in index.", ex);
            }
        }
    }
}
