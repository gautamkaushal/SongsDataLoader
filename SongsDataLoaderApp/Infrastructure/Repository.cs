using System;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SongsDataLoaderApp.Models;

namespace SongsDataLoaderApp.Infrastructure;

public class Repository
{
    private SongsLoaderContext context;

    public Repository(SongsLoaderContext context)
    {
        this.context = context;
    }

    public ICollection<Genre> GetGenres(){
        return this.context.Genres.ToList();
    }

    public ICollection<Artist> GetArtists(){
        return this.context.Artists.ToList();
    }

    public ICollection<Album> GetAlbums(){
        return this.context.Albums.ToList();
    }

    public ICollection<Song> GetSongs(bool includeAlbumInfo = false){
        if(includeAlbumInfo){
            return this.context.Songs
                .Include(s => s.Album)
                .ToList();
        }
        return this.context.Songs.ToList();
    }

    public T Add<T>(T item) where T:class
    {
        return this.context.Add<T>(item).Entity;
    }

    public void AddRange<T>(IList<T> items) where T:class
    {
        this.context.AddRange(items);
    }

    public void BulkInsert<T>(IEnumerable<T> values) where T: class
    {
        this.context.BulkInsert<T>(values);
    }

    public void SaveChanges(){
        this.context.SaveChanges();
    }

}
