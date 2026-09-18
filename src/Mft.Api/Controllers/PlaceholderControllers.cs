using Mft.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mft.Api.Controllers;

// These controllers back the not-yet-built left-nav sections (Milestones 2-5). They return
// empty collections so the frontend placeholder pages can call them without erroring, and are
// permission-gated the same way the real endpoints will be once implemented.

[ApiController]
[Route("api/sftpusers")]
[Authorize(Policy = PermissionKeys.SftpUsersManage)]
public class SftpUsersController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<object>> List() => Ok(Array.Empty<object>());
}

[ApiController]
[Route("api/namingconventions")]
[Authorize(Policy = PermissionKeys.NamingConventionsManage)]
public class NamingConventionsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<object>> List() => Ok(Array.Empty<object>());
}

[ApiController]
[Route("api/transferroutes")]
[Authorize(Policy = PermissionKeys.TransferRoutesManage)]
public class TransferRoutesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<object>> List() => Ok(Array.Empty<object>());
}

[ApiController]
[Route("api/externalconnections")]
[Authorize(Policy = PermissionKeys.ExternalConnectionsManage)]
public class ExternalConnectionsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<object>> List() => Ok(Array.Empty<object>());
}

[ApiController]
[Route("api/pgpkeys")]
[Authorize(Policy = PermissionKeys.PgpKeysManage)]
public class PgpKeysController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<object>> List() => Ok(Array.Empty<object>());
}

[ApiController]
[Route("api/transferhistory")]
[Authorize(Policy = PermissionKeys.TransferHistoryView)]
public class TransferHistoryController : ControllerBase
{
    [HttpGet]
    public ActionResult<PagedResult<object>> List() => Ok(new PagedResult<object> { Items = [], Page = 1, PageSize = 25, TotalCount = 0 });
}
