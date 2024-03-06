# apito-aspnet is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# apito-aspnet is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with apito-aspnet. If not, see <https://www.gnu.org/licenses/>.

ARG SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0
ARG ASPNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:8.0

FROM $SDK_IMAGE
MAINTAINER EAS Barbosa<easbarba@outlook.com>
WORKDIR /app
ENV PATH=/root/.dotnet/tools:$PATH
RUN dotnet tool update -g dotnet-ef
COPY . .
