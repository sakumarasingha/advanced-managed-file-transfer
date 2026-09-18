using Mft.Application.Abstractions;
using Mft.Application.Common;
using Mft.Domain.Entities;
using Mft.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mft.Api.Controllers;

public record PermissionResponse(Guid Id, string Key, string Category, string Description);
public record RoleResponse(Guid Id, string Name, string? Description, bool IsSystemDefined, IReadOnlyList<string> PermissionKeys);
public record CreateRoleRequest(string Name, string? Description, List<Guid> PermissionIds);
public record UpdateRoleRequest(string Name, string? Description, List<Guid> PermissionIds);

[ApiController]
[Route("api/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly MftDbContext _db;

    public PermissionsController(MftDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PermissionResponse>>> List(CancellationToken ct)
    {
        var permissions = await _db.Permissions.OrderBy(p => p.Category).ThenBy(p => p.Key).ToListAsync(ct);
        return Ok(permissions.Select(p => new PermissionResponse(p.Id, p.Key, p.Category, p.Description)).ToList());
    }
}

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly MftDbContext _db;
    private readonly ICurrentOrganizationContext _orgContext;

    public RolesController(MftDbContext db, ICurrentOrganizationContext orgContext)
    {
        _db = db;
        _orgContext = orgContext;
    }

    [HttpGet]
    [Authorize(Policy = PermissionKeys.RolesManage)]
    public async Task<ActionResult<IReadOnlyList<RoleResponse>>> List(CancellationToken ct)
    {
        var roles = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.Name).ToListAsync(ct);
        return Ok(roles.Select(Map).ToList());
    }

    [HttpPost]
    [Authorize(Policy = PermissionKeys.RolesManage)]
    public async Task<ActionResult<RoleResponse>> Create(CreateRoleRequest request, CancellationToken ct)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            OrganizationId = _orgContext.OrganizationId!.Value,
            Name = request.Name,
            Description = request.Description,
            IsSystemDefined = false
        };
        _db.Roles.Add(role);
        await _db.SaveChangesAsync(ct);

        foreach (var permissionId in request.PermissionIds.Distinct())
        {
            _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
        }
        await _db.SaveChangesAsync(ct);

        var reloaded = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).FirstAsync(r => r.Id == role.Id, ct);
        return Map(reloaded);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.RolesManage)]
    public async Task<ActionResult<RoleResponse>> Update(Guid id, UpdateRoleRequest request, CancellationToken ct)
    {
        var role = await _db.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role is null)
        {
            return NotFound();
        }

        if (role.IsSystemDefined)
        {
            return BadRequest("System-defined roles cannot be modified.");
        }

        role.Name = request.Name;
        role.Description = request.Description;

        _db.RolePermissions.RemoveRange(role.RolePermissions);
        foreach (var permissionId in request.PermissionIds.Distinct())
        {
            _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permissionId });
        }
        await _db.SaveChangesAsync(ct);

        var reloaded = await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).FirstAsync(r => r.Id == id, ct);
        return Map(reloaded);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionKeys.RolesManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role is null)
        {
            return NotFound();
        }

        if (role.IsSystemDefined)
        {
            return BadRequest("System-defined roles cannot be deleted.");
        }

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static RoleResponse Map(Role r) => new(
        r.Id, r.Name, r.Description, r.IsSystemDefined,
        r.RolePermissions.Select(rp => rp.Permission!.Key).ToList());
}
