using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using Utils.Extensions;
using static Authentication.Models.CommonModels;

namespace Authentication.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    public class commonController : AppControllerBase
    {

        private readonly ICommonService _commonService;
        private readonly ILogger<commonController> logger;

        public commonController(
            ICommonService common_Service,
            ILogger<commonController> logger
        )
        {
            _commonService = common_Service;
            logger = logger;
        }

        [HttpPost]
        [Route("departments")]
        public async Task<IActionResult> GetDepartmentsAsync([FromBody] Common_Department_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var results = await _commonService.GetDepartmentsAsync(criteria);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("positions")]
        public async Task<IActionResult> GetPositionsAsync([FromBody] Common_Position_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var results = await _commonService.GetPositionsAsync(criteria);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("roles")]
        public async Task<IActionResult> GetRolesAsync([FromBody] Common_Role_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var results = await _commonService.GetRolesAsync(criteria);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("miscellaneous/modules")]
        public async Task<IActionResult> GetMiscellaneousModules([FromBody] Common_MiscellaneousModules_Criteria criteria)
        {
            try
            {
                if (criteria == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var results = await _commonService.GetMiscellaneousModules(criteria);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}