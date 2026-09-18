using Mft.Application.Abstractions;
using Mft.Application.Common;
using Mft.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mft.Api.Controllers;

public record OrganizationResponse(Guid Id, string Name, string Slug, string TimeZoneId, bool IsActive);
public record UpdateOrganizationRequest(string Name, string TimeZoneId);

[ApiController]
[Route("api/organizations")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly MftDbContext _db;

    public OrganizationsController(MftDbContext db)
    {
        _db = db;
    }

    [HttpGet("current")]
    public async Task<ActionResult<OrganizationResponse>> GetCurrent(CancellationToken ct)
    {
        var org = await _db.Organizations.FirstOrDefaultAsync(ct);
        if (org is null)
        {
            return NotFound();
        }

        return new OrganizationResponse(org.Id, org.Name, org.Slug, org.TimeZoneId, org.IsActive);
    }

    [HttpPut("current")]
    [Authorize(Policy = PermissionKeys.OrganizationManage)]
    public async Task<ActionResult<OrganizationResponse>> UpdateCurrent(UpdateOrganizationRequest request, CancellationToken ct)
    {
        var org = await _db.Organizations.FirstOrDefaultAsync(ct);
        if (org is null)
        {
            return NotFound();
        }

        org.Name = request.Name;
        org.TimeZoneId = request.TimeZoneId;
        await _db.SaveChangesAsync(ct);

        return new OrganizationResponse(org.Id, org.Name, org.Slug, org.TimeZoneId, org.IsActive);
    }
}
