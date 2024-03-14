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

namespace ApitoAspnet.Tests.Controllers;

using System.Net.Http.Json;
using Apito.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient client;
    private readonly WebApplicationFactory<Program> factory;

    public HomeControllerTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
        client = this.factory.CreateClient();
    }

    [Fact]
    public async Task Home_ReturnsWelcomeMessage()
    {
        var response = await client.GetAsync("/");
        ResponseDTO<String>? result = await response.Content.ReadFromJsonAsync<
            ResponseDTO<String>
        >();

        response.EnsureSuccessStatusCode();
        Assert.Equal("Welcome to Apito!", result?.data);
        Assert.Null(result?.data);
    }
}
