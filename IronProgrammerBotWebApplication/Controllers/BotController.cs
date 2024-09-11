using StreamingCourses_Contracts.Abstractions;
using IronProgrammerBotWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace IronProgrammerBotWebApplication.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class BotController(ILogger<BotController> logger) : Controller
    {
        private readonly ILogger<BotController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] Update update,
            [FromServices] IUpdateHandlers handleUpdateService,
            CancellationToken cancellationToken)
        {
            try
            {
                await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Произошла ошибка");
                return BadRequest();
            }
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }
    }
}
