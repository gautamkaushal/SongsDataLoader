using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using SongsDataLoaderApp.Models;

namespace SongsDataLoaderApp.Infrastructure;

public class DataLoader
{
    private Repository repository;
    private readonly ILogger<DataLoader> logger;

    public DataLoader(Repository repository, ILogger<DataLoader> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public void ParseAndLoadSongs(string filePath, int batchSize = 999)
    {
        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            this.logger.LogInformation($"******Starting to parse and load******");
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            ICollection<Genre> genres = this.repository.GetGenres();
            ICollection<Artist> artists = this.repository.GetArtists();
            ICollection<Album> albums = this.repository.GetAlbums();
            ICollection<Song> songs = this.repository.GetSongs();
            bool isFirstLine = true;
            int i = 0, j = 0;
            while (!parser.EndOfData)
            {
                this.logger.LogTrace($"Reading for line # {i}");

                while (i < batchSize)
                {
                    i++;
                    string[] fields = parser.ReadFields();
                    this.logger.LogTrace($"Read all the fields for line # {i}");
                    if (isFirstLine)
                    {
                        this.logger.LogTrace($"isFirstLine: {isFirstLine}, therefore ignoring");
                        isFirstLine = false;
                        continue;
                    }
                    if (fields == null || fields.Length <= 0)
                    {
                        this.logger.LogTrace($"Read fields are NULL or of 0 length");
                        continue;
                    }
                    Genre genre = null!;
                    string genreVal = fields[3];
                    if (string.IsNullOrEmpty(genreVal))
                    {
                        this.logger.LogTrace($"Read genreVal is null or empty");
                        continue;
                    }
                    this.logger.LogTrace($"Read genreVal: {genreVal}");
                    if (!genres.Any(g => g.Name == genreVal))
                    {
                        genre = this.repository.Add<Genre>(new Genre { Name = genreVal });
                        this.repository.SaveChanges();
                        genres.Add(genre);
                    }
                    else
                    {
                        genre = genres.Single(a => a.Name == genreVal);
                    }

                    Artist artist = null!;
                    string artistVal = fields[1];
                    if (string.IsNullOrEmpty(artistVal))
                    {
                        this.logger.LogTrace($"Read artistVal is null or empty");
                        continue;
                    }
                    this.logger.LogTrace($"Read artistVal: {artistVal}");
                    if (!artists.Any(a => a.Name == artistVal))
                    {
                        artist = this.repository.Add<Artist>(new Artist { Name = artistVal });
                        this.repository.SaveChanges();
                        artists.Add(artist);
                    }
                    else
                    {
                        artist = artists.Single(a => a.Name == artistVal);
                    }

                    Album album = null!;
                    string albumVal = fields[2];
                    if (string.IsNullOrEmpty(albumVal))
                    {
                        this.logger.LogTrace($"Read albumVal is null or empty");
                        continue;
                    }
                    this.logger.LogTrace($"Read albumVal: {albumVal}");
                    if (!albums.Any(a => a.Name == albumVal))
                    {
                        album = this.repository.Add<Album>(new Album { Name = albumVal });
                        this.repository.SaveChanges();
                        albums.Add(album);
                    }
                    else
                    {
                        album = albums.Single(a => a.Name == albumVal);
                    }

                    string songTitle = fields[0];
                    string songTimespan = fields[4];
                    if (string.IsNullOrEmpty(songTitle) || string.IsNullOrEmpty(songTimespan))
                    {
                        this.logger.LogTrace($"Read songTitle or songTimespan is null or empty");
                        continue;
                    }
                    this.logger.LogTrace($"Read songTitle: {songTitle}");
                    this.logger.LogTrace($"Read songTimespan: {songTimespan}");
                    Song song = this.repository.Add<Song>(
                        new Song
                        {
                            Title = fields[0],
                            Time = TimeSpan.Parse(fields[4]),
                            Genre = genre,
                            Artist = artist,
                            Album = album
                        });
                    songs.Add(song);
                }
                this.repository.BulkInsert<Song>(songs);
                this.repository.SaveChanges();
                this.logger.LogInformation($"******{j++}******");
                i = 0;
            };
            this.logger.LogInformation($"******Exiting parse and load******");
        }
    }

    public void LoadReferenceData()
    {
        this.LoadUsers();
        this.LoadRatings();
    }

    private void LoadUsers()
    {
        List<User> users = new();
        users.Add(new User { Name = "John Doe" });
        this.repository.AddRange(users);
        this.repository.SaveChanges();
    }

    private void LoadRatings()
    {
        List<Rating> ratings = new();
        ratings.Add(new Rating { Value = 0, Description = "None" });
        ratings.Add(new Rating { Value = 1, Description = "Thumbs Up" });
        ratings.Add(new Rating { Value = 2, Description = "Thumbs Down" });
        this.repository.AddRange(ratings);
        this.repository.SaveChanges();
    }
}
