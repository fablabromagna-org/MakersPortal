using MakersPortal.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MakersPortal.WebApi.Options
{
    public class ConfigureDbContextOptionBuilder : IConfigureNamedOptions<DbContextOptionsBuilder>
    {
        private readonly ConnectionStringsOption _connectionStrings;
        
        public ConfigureDbContextOptionBuilder(ConnectionStringsOption connectionStringsOption)
        {
            _connectionStrings = connectionStringsOption;
        }
        
        public void Configure(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_connectionStrings.MicrosoftSqlServer);
        }

        public void Configure(string name, DbContextOptionsBuilder options)
        {
            Configure(options);
        }
    }
}