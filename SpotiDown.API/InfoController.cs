using Microsoft.AspNetCore.Mvc;

namespace SpotiDown.API
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public string Get() => "{SpotiDown Valid Web API}";
    }
}
