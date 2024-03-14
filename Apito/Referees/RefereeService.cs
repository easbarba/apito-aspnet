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

namespace Apito.Referees;

using Apito.Common;
using Apito.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class RefereeService : IRefereeService
{
    private readonly DataContext context;
    private readonly IMapper mapper;

    public RefereeService(DataContext dataContext, IMapper mapper)
    {
        context = dataContext;
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Referee, RefereeDTO>());
        this.mapper = config.CreateMapper();
    }

    public async Task<ResponseDTO<IEnumerable<RefereeDTO>>> GetAll(string status)
    {
        var referees = await context
            .referees.AsQueryable()
            .OrderBy(b => b.name)
            .Select(r => mapper.Map<RefereeDTO>(r))
            .ToList();

        var data = new ResponseDTO<IEnumerable<RefereeDTO>> { data = referees };

        return data;
    }

    public Task<ResponseDTO<RefereeDTO?>> Get(Guid id, string status)
    {
        var referee = context.referees.Find(id) ?? null;
        var data = new ResponseDTO<RefereeDTO> { data = mapper.Map<RefereeDTO>(referee) };

        return data;
    }

    public async Task<ResponseDTO<RefereeDTO?>> Save(RefereeDTO model, string status)
    {
        var referee = context.referees.Find(model.id) ?? null;
        if (referee == null)
            return null;

        context.Add(referee);

        var saved = context.SaveChanges();
        if (saved == 0)
            return null;

        var data = new ResponseDTO<RefereeDTO> { data = mapper.Map<RefereeDTO>(referee) };

        return data;
    }

    public async Task<bool> Remove(Guid id)
    {
        var referee = await context.referees.Where<Referee>(r => r.id == id).FirstAsync();
        if (referee == null)
            return false;

        context.referees.Remove(referee);
        await context.SaveChangesAsync();

        return true;
    }
}
