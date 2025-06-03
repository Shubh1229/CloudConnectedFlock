using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrivatechatService.DTOs;
using PrivatechatService.PrivateChatData;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PrivatechatService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrivatechatController : ControllerBase
    {

        private readonly PrivateChatRedisService redisService;
        public PrivatechatController(PrivateChatRedisService redisService)
        {
            this.redisService = redisService;
        }

        [HttpGet("recentprivate")]
        public async Task<IActionResult> GetRecentChats([FromQuery] string username, [FromQuery] string toWho)
        {
            var chats = await redisService.GetChatsAsync(username, toWho);
            return Ok(chats);
        }

    }
}