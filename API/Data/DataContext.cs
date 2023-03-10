using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{

    public interface IDataContext { }
    public class DataContext : DbContext,IDataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
    }
}
