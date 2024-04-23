namespace WebAPI_1
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Win32;
    using System.Data;

    public class DB : DbContext
    {

        const string subkey = "ConValue";
        private string ConStr;
        public string ConnectionStr
        {
            get { return ConStr; }
            set { ConStr = value; }
        }

        public void SetConnectionString()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(subkey))
            {
                Registry.GetValue(Registry.CurrentUser + "\\" + subkey, ConnectionStr, "");
            }

        }


        public DB() 
        {
            this.Database.SetConnectionString(ConnectionStr);
        }

        public DB(DbContextOptions<DB> options) : base(options)
        {
        }

        public DbSet<NoticeModel> NoticeModels => Set<NoticeModel>();
        public DbSet<ManagerModel> ManagerModels => Set<ManagerModel>();

    }
}
