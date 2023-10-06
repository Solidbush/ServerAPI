using ServerAPI.Services;
using static ServerAPI.Program;

namespace ServerAPI.Entities
{
    public class StorageRepository : IStorageRepository
    {
        private readonly StorageDb _context;
        public StorageRepository(StorageDb context) 
        {
            _context = context;
        }
        public async Task DeletePairAsync(string key)
        {
            var pairFromDb = await _context.Pairs.FindAsync(new object[] { key });
            if (pairFromDb == null)
            {
                return;
            }
            _context.Pairs.Remove(pairFromDb);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<Models.KeyValuePair>? GetPairAsync(string key)
        {
           return await _context.Pairs.FindAsync(new object[] {key});
        }

        public Task<List<Models.KeyValuePair>> GetPairsAsync()
        {
            return _context.Pairs.ToListAsync();
        }

        public async Task InsertPairAsync(Models.KeyValuePair pair)
        {
            await _context.Pairs.AddAsync(pair);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdetePairAsync(Models.KeyValuePair pair)
        {
            var pairFromDb = await _context.Pairs.FindAsync(new object[] {pair.Key});
            if (pairFromDb == null)
            {
                return;
            }
            pairFromDb.Value = pair.Value;
        }
    }
}
