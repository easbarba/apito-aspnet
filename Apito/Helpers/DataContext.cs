/*
* apito-aspnet is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* apito-aspnet is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with apito-aspnet. If not, see <https://www.gnu.org/licenses/>.
*/

// using Bogus;
using Apito.Referees;
using Microsoft.EntityFrameworkCore;

namespace Apito.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(ConnectionString());
        }

        public DbSet<Referee> referees => Set<Referee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new RefereeSeed().Generate(modelBuilder);
        }

        public static string ConnectionString()
        {
            var sqlHost = System.Environment.GetEnvironmentVariable("SQL_HOST");
            var sqlUser = System.Environment.GetEnvironmentVariable("SQL_USERNAME");
            var sqlPassword = System.Environment.GetEnvironmentVariable("SQL_PASSWORD");
            var sqlName = System.Environment.GetEnvironmentVariable("SQL_DATABASE");
            var sqlPort = System.Environment.GetEnvironmentVariable("SQL_PORT");

            return $"Server={sqlHost};Database={sqlName};Port={sqlPort};User ID={sqlUser};Password={sqlPassword}";
        }
    }
}
