using Mft.Application.Abstractions;
using Mft.Application.Common;
using Mft.Domain.Entities;
using Mft.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mft.Api.Controllers;

public record UserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    bool IsActive,
    bool IsClaimed,
    DateTime? LastLoginAtUtc,
    IReadOnlyList<string> RoleNames,
    IReadOnlyList<Guid> RoleIds);

public record InviteUserRequest(string Email, string DisplayName, List<Guid> RoleIds);
public record UpdateUserActiveRequest(bool IsActive);
public record SetUserRolesRequest(List<Guid> RoleIds);

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly MftDbContext _db;
    private readonly ICurrentOrganizationContext _orgContext;
    private readonly ICurrentUserContext _currentUser;

    public UsersController(MftDbContext db, ICurrentOrganizationContext orgContext, ICurrentUserContext currentUser)
    {
        _db = db;
        _orgContext = orgContext;
        _currentUser = currentUser;
    }

    [HttpGet]
    [Authorize(Policy = PermissionKeys.UsersManage)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> List(CancellationToken ct)
    {
        var users = await _db.AppUsers
            .Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role)
            .OrderBy(u => u.Email)
            .ToListAsync(ct);

        return Ok(users.Select(Map).ToList());
    }

    [HttpPost("invite")]
    [Authorize(Policy = PermissionKeys.UsersManage)]
    public async Task<ActionResult<UserResponse>> Invite(InviteUserRequest request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.AppUsers.AnyAsync(u => u.Email == normalizedEmail, ct);
        if (exists)
        {
            return Conflict($"A user with email '{normalizedEmail}' already exists in this organization.");
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            OrganizationId = _orgContext.OrganizationId!.Value,
            Email = normalizedEmail,
            DisplayName = request.DisplayName,
            IsActive = true,
            InvitedAtUtc = DateTime.UtcNow,
            InvitedByUserId = _currentUser.UserId
        };
        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync(ct);

        await SetRolesAsync(user.Id, request.RoleIds, ct);

        var reloaded = await _db.AppUsers.Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role)
            .FirstAsync(u => u.Id == user.Id, ct);
        return Map(reloaded);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.UsersManage)]
    public async Task<ActionResult<UserResponse>> UpdateActive(Guid id, UpdateUserActiveRequest request, CancellationToken ct)
    {
        var user = await _db.AppUsers.Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null)
        {
            return NotFound();
        }

        user.IsActive = request.IsActive;
        await _db.SaveChangesAsync(ct);
        return Map(user);
    }

    [HttpPost("{id:guid}/roles")]
    [Authorize(Policy = PermissionKeys.UsersManage)]
    public async Task<ActionResult<UserResponse>> SetRoles(Guid id, SetUserRolesRequest request, CancellationToken ct)
    {
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null)
        {
            return NotFound();
        }

        await SetRolesAsync(id, request.RoleIds, ct);

        var reloaded = await _db.AppUsers.Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role)
            .FirstAsync(u => u.Id == id, ct);
        return Map(reloaded);
    }

    private async Task SetRolesAsync(Guid userId, List<Guid> roleIds, CancellationToken ct)
    {
        var existing = await _db.RoleAssignments.Where(ra => ra.AppUserId == userId).ToListAsync(ct);
        _db.RoleAssignments.RemoveRange(existing);

        foreach (var roleId in roleIds.Distinct())
        {
            _db.RoleAssignments.Add(new RoleAssignment
            {
                Id = Guid.NewGuid(),
                OrganizationId = _orgContext.OrganizationId!.Value,
                AppUserId = userId,
                RoleId = roleId,
                AssignedByUserId = _currentUser.UserId
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    private static UserResponse Map(AppUser u) => new(
        u.Id,
        u.Email,
        u.DisplayName,
        u.IsActive,
        u.EntraObjectId is not null,
        u.LastLoginAtUtc,
        u.RoleAssignments.Select(ra => ra.Role!.Name).ToList(),
        u.RoleAssignments.Select(ra => ra.RoleId).ToList());
}
