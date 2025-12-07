using Vortex.Domain.Constants;
using Vortex.Domain.Dto;

namespace Vortex.Application.Dtos;
public class InviteUserDetails()
{
    public IEnumerable<DropdownOptionModel<Guid?, List<DropdownOptionModel<Guid>>>> Projects { get; set; } = [];

    public List<DropdownOptionModel<Guid>> Roles { get; set; } = [];
}