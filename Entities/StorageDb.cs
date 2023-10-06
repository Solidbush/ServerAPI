namespace ServerAPI.Entities
{
    public class StorageDb : DbContext 
    {
        public StorageDb(DbContextOptions<StorageDb> options) : base(options) 
        {
            Database.EnsureCreated();
        }
        public DbSet<Models.KeyValuePair> Pairs => Set<Models.KeyValuePair>();
    }
}
