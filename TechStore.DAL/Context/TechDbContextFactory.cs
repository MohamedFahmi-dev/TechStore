using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TechStore.DAL.Context;

public class TechDbContextFactory : IDesignTimeDbContextFactory<TechDbContext>
{
    public TechDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TechDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
            ?? "Data Source=MOHAMEDFAHMII\\SQLEXPRESS;Initial Catalog=Tech_store;Integrated Security=True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new TechDbContext(optionsBuilder.Options);
    }
}
