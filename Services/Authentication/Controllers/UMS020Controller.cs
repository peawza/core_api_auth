using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using Utils.Extensions;
using static Authentication.Models.UMS020.UMS020;

namespace Authentication.Controllers
{
    [Route("ums020")]
    public class UMS020Controller : AppControllerBase
    {

        private readonly ILogger<UMS020Controller> logger;

        private readonly IUMS020Service _service;

        public UMS020Controller(

            ILogger<UMS020Controller> logger,
            IUMS020Service _UMS020_service
        )
        {
            this.logger = logger;
            this._service = _UMS020_service;
        }



        [HttpPost]
        [Route("roles/search")]
        public async Task<IActionResult> SearchRole([FromBody] UMS020_SearchRole_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var results = await _service.UMS020_SearchRole(criteria);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("roles/add")]
        public async Task<IActionResult> AddRole([FromBody] UMS020_AddRole_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                criteria.CreateBy = User.Identity.Name;
                var result = await _service.UMS020_AddRole(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("roles/update")]
        public async Task<IActionResult> UpdateRole([FromBody] UMS020_UpdateRole_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                criteria.UpdateBy = User.Identity.Name;
                var result = await _service.UMS020_UpdateRole(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("roles/delete")]
        public async Task<IActionResult> DeleteRole([FromBody] UMS020_DeleteRole_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.DeleteRoleAsync(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("roles/users/add")]
        public async Task<IActionResult> AddUsersToRole([FromBody] UMS020_UserRoleMapping_Criteria data)
        {
            try
            {
                if (data == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.AddUsersToRole(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("roles/usermapping/search")]
        public async Task<IActionResult> GetUserRoleMapping([FromBody] UMS020_GetUserRoleMapping_Criteria data)
        {
            try
            {
                if (data == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.GetUserRoleMapping(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
