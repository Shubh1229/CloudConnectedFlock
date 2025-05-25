using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupchatService.DTOs;
using GroupchatService.GroupchatData;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GroupchatService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupchatController : ControllerBase
    {

        private readonly GroupChatRedisService redisService;
        public GroupchatController(GroupChatRedisService redisService)
        {
            this.redisService = redisService;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentChats()
        {
            var chats = await redisService.GetChatsAsync();
            return Ok(chats);
        }

    }
}