using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZUCManagementSystemIA.Server.Services;
using NZUCManagementSystemIA.Shared.MailingSystem;

namespace NZUCManagementSystemIA.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailingController : ControllerBase
    {
        private readonly IMailingService _emailService;
        public MailingController(IMailingService service)
        {
            _emailService = service;
        }

        [HttpPost("SendEmail")]

        public IActionResult sendMail(EmailDto email)
        {
            _emailService.SendEmail(email);
            return Ok();
        }

        public IActionResult AddTransactionAsync(EmailDto email)
        {
            try
            {
                _emailService.SendEmail(email);

                return Ok(email);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
