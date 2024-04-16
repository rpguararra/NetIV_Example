namespace WebAPI_1
{
    using Microsoft.EntityFrameworkCore;

    public class DB : DbContext
    {

        public DB(string conStr) 
        {
            this.Database.SetConnectionString(conStr);
        }

        public DB(DbContextOptions<DB> options) : base(options)
        {
        }

        public DbSet<NoticeModel> NoticeModels => Set<NoticeModel>();
        public DbSet<ManagerModel> ManagerModels => Set<ManagerModel>();

    }
}
