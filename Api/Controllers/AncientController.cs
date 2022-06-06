using Api.Exceptions;
using Api.Models;
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

        [HttpPost]
        public Ancient Post([FromBody] Req r)
        {
            if(r != null)
            {
                return new Ancient()
                {
                    Name = Ancients[Random.Shared.Next(Ancients.Length)]
                };
            }
            throw new UserNotFoundException();
        }
    }
}