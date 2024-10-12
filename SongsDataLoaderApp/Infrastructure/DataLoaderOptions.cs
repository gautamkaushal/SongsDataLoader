using System;

namespace SongsDataLoaderApp.Infrastructure;

public class DataLoaderOptions
{
    public const string LoaderSettings = "LoaderSettings";
    public string FilePath { get; set; }
    public int BatchSize { get; set; }
}
