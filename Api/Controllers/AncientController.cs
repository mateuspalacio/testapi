using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AncientController : ControllerBase
    {
        private static readonly string[] Ancients = new[]
         {
        "Venat", "Hades", "Hythlodaeus", "Hermes", "Themis", "Erichthonios", "Hesperos", "Meteion", "Athena", "Phoinix"
    };

        private readonly ILogger<AncientController> _logger;

        public AncientController(ILogger<AncientController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetAncient")]
        public Ancient Get()
        {
            return new Ancient()
            {
                Name = Ancients[Random.Shared.Next(Ancients.Length)]
            };
        }
    }
}