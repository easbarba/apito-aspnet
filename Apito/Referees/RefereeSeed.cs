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

using Bogus;
using Microsoft.EntityFrameworkCore;

namespace Apito.Referees;

public class RefereeSeed
{
    public void Generate(ModelBuilder modelBuilder)
    {
        var referees = new Faker<Referee>()
            .RuleFor(m => m.id, f => System.Guid.NewGuid())
            .RuleFor(m => m.name, f => f.Name.FullName())
            .RuleFor(m => m.state, f => f.Address.State())
            .RuleFor(m => m.createdAt, f => DateTime.UtcNow)
            .RuleFor(m => m.modifiedAt, f => DateTime.UtcNow);

        modelBuilder.Entity<Referee>().HasData(referees.GenerateBetween(10, 10));
    }
}
