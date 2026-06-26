using Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Extensions;
using static Authentication.Models.NewFolder.UMS010;

namespace Authentication.Controllers
{
    //[Authorize]
    [Route("ums010")]
    public class UMS010Controller : AppControllerBase
    {

        private readonly ILogger<UMS010Controller> logger;

        private readonly IUMS010Service _UMS010_service;


        public UMS010Controller(

            ILogger<UMS010Controller> logger,
            IUMS010Service _PRP002_service

        )
        {
            this.logger = logger;
            this._UMS010_service = _PRP002_service;
        }



        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] UMS010_Search_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _UMS010_service.UMS010_Search(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("getby/id")]
        public async Task<IActionResult> GetUserById([FromBody] GetUserID_Info_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _UMS010_service.GetUserID_Info(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UMS010_CreateUser_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //criteria.Password = "P@ssw0rd";
                criteria.CreateBy = User.Identity.Name;
                var result = await _UMS010_service.CreateUser(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        //BackOfficeCreateUser

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UMS010_UpdateUser_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                criteria.UpdateBy = User.Identity.Name;
                var result = await _UMS010_service.UpdateUser(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost("admin/change/password")]
        public async Task<IActionResult> AdminChangePassword([FromBody] UMS010_AdminChangePassword_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _UMS010_service.AdminChangePassword(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("change/password")]
        public async Task<IActionResult> ChangePassword([FromBody] UMS010_ChangePassword_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _UMS010_service.ChangePassword(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot/password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UMS010_ForgotPassword_Criteria criteria)
        {
            if (criteria == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _UMS010_service.ForgotPassword(criteria);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }





    }
}
