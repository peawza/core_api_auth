using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using Utils.Extensions;
using static Authentication.Models.UMS030.UMS030;

namespace Authentication.Controllers
{
    //[Authorize]
    [Route("ums030")]
    public class UMS030Controller : AppControllerBase
    {

        private readonly ILogger<UMS030Controller> logger;

        private readonly IUMS030Service _UMS030_service;

        public UMS030Controller(

            ILogger<UMS030Controller> logger,
            IUMS030Service _PRP002_service
        )
        {
            this.logger = logger;
            this._UMS030_service = _PRP002_service;
        }


        [HttpPost("list/user-group")]
        public async Task<IActionResult> ListByUserGroup([FromBody] UMS030_ListByUserGroup_Criteria criteria)
        {
            //if (criteria.userGroupId <= 0)
            //    return BadRequest("Invalid user group ID.");

            try
            {
                var result = await _UMS030_service.ListByUserGroup(criteria.userGroupId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("list/functions")]
        public async Task<IActionResult> ListScreenFunctions()
        {
            try
            {
                var result = await _UMS030_service.ListScreenFunctions();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("update/permissions")]
        public async Task<IActionResult> UpdatePermissions([FromBody] UMS030_UpdatePermission_Criteria permissions)
        {
            //if (permissions == null || !permissions.Any())
            //    return BadRequest("Permissions list is empty.");

            try
            {
                //permissions.CreateBy = User.Identity.Name;
                var result = await _UMS030_service.UpdatePermission(permissions);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPost]
        [Route("getrole")]
        public async Task<IActionResult> GetRoles([FromBody] UMS030_GetRoles_Criteria criteria)
        {
            var results = await _UMS030_service.GetRoles(criteria);
            return Ok(results);
        }
        [HttpPost]
        [Route("getmodules")]
        public async Task<IActionResult> getModules([FromBody] UMS030_GetModule_Criteria criteria)
        {
            var results = await _UMS030_service.GetModule(criteria);
            return Ok(results);
        }
    }
}
