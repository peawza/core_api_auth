using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using static Authentication.Models.LogImportFileModel;
using static Authentication.Models.MESScreenModel;

namespace Authentication.Controllers
{
    //[Authorize]
    [Route("system")]
    public class SystemController : ControllerBase
    {
        private readonly IScreenService service_Screen;
        private readonly IResouresService service_Resoures;
        private readonly ILogger<SystemController> logger;

        public SystemController(
            IScreenService service_Screen,
            IResouresService service_Resoures,
            ILogger<SystemController> logger
        )
        {
            this.service_Screen = service_Screen;
            this.service_Resoures = service_Resoures;
            this.logger = logger;
        }


        [HttpPost]
        [Route("getscreen")]
        //[TypeFilter(typeof(ActionExceptionFilter))]
        public async Task<IActionResult> GetScreen([FromBody] MESScreenCriteria criteria)
        {
            // LocalizedMessagesCriteria localizedMessagesCriteria = new LocalizedMessagesCriteria();
            var results = await service_Screen.getMesScreen(criteria);
            return Ok(results);
        }

        [HttpPost]
        [Route("getsitemaps")]
        //[TypeFilter(typeof(ActionExceptionFilter))]
        public async Task<IActionResult> GetSiteMaps([FromBody] GetSiteMaps_Criteria criteria)
        {
            var results = await service_Screen.GetSiteMaps(criteria);
            return Ok(results);
        }

        [HttpGet]
        [Route("getresources")]
        //[TypeFilter(typeof(ActionExceptionFilter))]
        public IActionResult getResources()
        {
            // LocalizedMessagesCriteria localizedMessagesCriteria = new LocalizedMessagesCriteria();
            var results = service_Resoures.LocalizedResources(null);
            return Ok(results);
        }
        [HttpGet]
        [Route("getmessages")]
        //[TypeFilter(typeof(ActionExceptionFilter))]
        public async Task<IActionResult> getMessages()
        {
            // LocalizedMessagesCriteria localizedMessagesCriteria = new LocalizedMessagesCriteria();
            var results = await service_Resoures.LocalizedMessages(null);
            return Ok(results);
        }



        [HttpPost]
        [Route("insertimportlog")]
        //[Authorize]
        public async Task<IActionResult> InsertImportLog([FromBody] API_InsertImportLog_Criteria Criteria)
        {
            var results = await service_Screen.InsertImportLog(Criteria.ImportLog, Criteria.ImportLogDetail);
            return Ok(new { uuid = results });
        }

    }
}