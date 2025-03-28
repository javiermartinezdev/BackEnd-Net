using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using productos.Models;

namespace productos.Data
{
    public class DataContextProduct : DbContext
    {
        public DataContextProduct(DbContextOptions<DataContextProduct>options):base (options){}
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //leemos nuestro archivo .env
            Env.Load();
            //obtenemos la cadena de coneccion
            string connectionString = Env.GetString("CONNECTIONSTRINGS__DEFAULTCONNECTION");
            optionsBuilder.UseNpgsql(connectionString);
        }

    }
}