namespace WebAPI_1
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Win32;

    public class DB : DbContext
    {
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "ConnectionStr";
        const string keyName = userRoot + "\\" + subkey;

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
