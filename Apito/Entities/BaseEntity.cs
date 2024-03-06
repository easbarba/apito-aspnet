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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Apito.Entities;

public abstract class BaseEntity
{
    [Key]
    [Required]
    public System.Guid id { get; set; }

    [Required]
    [Column("created_at")]
    [JsonPropertyName("created_at")]
    public DateTime createdAt { get; set; }

    [Required]
    [Column("modified_at")]
    [JsonPropertyName("modified_at")]
    public DateTime modifiedAt { get; set; }
}
