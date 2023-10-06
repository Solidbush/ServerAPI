namespace ServerAPI.Services
{
    public interface IStorageRepository : IDisposable
    {
        Task<List<Models.KeyValuePair>> GetPairsAsync();
        Task<Models.KeyValuePair>? GetPairAsync(string key);
        Task InsertPairAsync(Models.KeyValuePair pair);
        Task DeletePairAsync(string key);
        Task UpdetePairAsync(Models.KeyValuePair pair);
        Task SaveAsync();
    }
}
