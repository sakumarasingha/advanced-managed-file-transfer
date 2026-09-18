using Mft.Application.Abstractions;
using Mft.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mft.Api.Controllers;

public record MeResponse(
    Guid UserId,
    Guid OrganizationId,
    string OrganizationName,
    string DisplayName,
    string Email,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);

public record NotProvisionedResponse(string Code, string Message);

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly ICurrentUserContext _currentUser;
    private readonly ICurrentOrganizationContext _orgContext;
    private readonly MftDbContext _db;

    public MeController(ICurrentUserContext currentUser, ICurrentOrganizationContext orgContext, MftDbContext db)
    {
        _currentUser = currentUser;
        _orgContext = orgContext;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<MeResponse>> Get(CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticatedAndProvisioned || _currentUser.UserId is null || _orgContext.OrganizationId is null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new NotProvisionedResponse(
                "not_provisioned",
                "Your account is authenticated but not yet provisioned in this application. Contact your organization admin."));
        }

        var orgName = await _db.Organizations.Where(o => o.Id == _orgContext.OrganizationId).Select(o => o.Name).FirstOrDefaultAsync(ct);

        return new MeResponse(
            _currentUser.UserId.Value,
            _orgContext.OrganizationId.Value,
            orgName ?? string.Empty,
            _currentUser.DisplayName ?? string.Empty,
            _currentUser.Email ?? string.Empty,
            _currentUser.Roles.ToList(),
            _currentUser.Permissions.ToList());
    }
}
