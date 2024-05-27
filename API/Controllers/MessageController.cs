using API.DTOs;
using API.DTOs.Requests;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Authorize]
    public class MessageController(IMessageService messageService, ILogger<MessageController> logger, IHttpContextAccessor httpContextAccessor) : ApplicationBaseController
    {
        private readonly IMessageService _messageService = messageService;
        private readonly ILogger<MessageController> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HttpGet("{recipientId}")]
        public async Task<ActionResult<List<MessageDto>>> GetMessagesForChat(int recipientId)
        {
            _logger.LogInformation("GetMessagesForChat: (requestUserId: {requestUserId}, recipientId: {recipientId})", _httpContextAccessor.GetRequestUserId(), recipientId);

            return await _messageService.GetMessagesForChat(_httpContextAccessor.GetRequestUserId(), recipientId);
        }

        [HttpPost("send-message")]
        public async Task<ActionResult> SendMessage(MessageSendRequest messageSendRequest)
        {
            _logger.LogInformation("SendMessage: (requestUserId: {requestUserid}, request: {req})", _httpContextAccessor.GetRequestUserId(), JsonConvert.SerializeObject(messageSendRequest));

            return await _messageService.SendMessage(_httpContextAccessor.GetRequestUserId(), messageSendRequest);
        }

        [HttpDelete("{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            _logger.LogInformation("DeleteMessage: (requestUserId: {reqUserId}, messageId: {msgId})", _httpContextAccessor.GetRequestUserId(), messageId);

            return await _messageService.DeleteMessage(_httpContextAccessor.GetRequestUserId(), messageId);
        }
    }
}