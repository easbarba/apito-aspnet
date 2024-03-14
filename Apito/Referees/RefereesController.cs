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

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/v1/[controller]")]
public class RefereesController : ControllerBase
{
    private IRefereeService service;
    private readonly ILogger<RefereeController> logger;

    public RefereesController(IRefereeService refereeService, ILogger<RefereeController> logger)
    {
        this.service = refereeService;
        this.logger = logger;
    }

    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(IEnumerable<Referee>),
        StatusCodes.Status200OK,
        "application/json"
    )]
    public IActionResult index()
    {
        HttpContext.Response.Headers.Allow = "GET";

        var data = service.GetAll("OK");
        return data != null ? Ok(data) : NotFound();
    }

    [HttpGet("{id}")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    [ProducesResponseType(typeof(Referee), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Show(Guid id)
    {
        HttpContext.Response.Headers.Allow = "GET";

        var referee = service.Get(id, "OK");

        return referee != null ? base.Ok(referee) : base.NotFound();
    }

    [HttpPost]
    [ResponseCache(NoStore = true)]
    public IActionResult Create([FromBody] Referee model)
    {
        HttpContext.Response.Headers.Allow = "POST";

        var referee = service.Get(model.id, "OK");

        return referee != null ? base.Created() : base.NotFound();
    }

    [HttpDelete("{id}")]
    [ResponseCache(NoStore = true)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        HttpContext.Response.Headers.Allow = "DELETE";

        var referee = service.Get(id, "OK");
        if (referee != null)
            return base.NotFound();

        return service.Remove(id) ? base.NoContent() : base.NotFound();
    }
}
