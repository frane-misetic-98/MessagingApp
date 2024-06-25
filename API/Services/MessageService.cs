using API.Data;
using API.DTOs;
using API.DTOs.Requests;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Services
{
    public class MessageService(ApplicationDbContext context, IMapper mapper, ILogger<MessageService> logger) : IMessageService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MessageService> _logger = logger;

        public async Task<ActionResult<List<MessageDto>>> GetMessagesForChat(int requestUserId, int recipientId)
        {
            _logger.LogInformation(
                    "GetMessagesForChat, (requestUserId: {requestUserId}, recipientId: {recipientId})",
                    requestUserId,
                    recipientId
                    );

            var query = _context.Messages
                            .Where(
                                x => (x.SenderId == requestUserId &&
                                x.RecipientId == recipientId) ||
                                (x.SenderId == recipientId &&
                                x.RecipientId == requestUserId)
                            )
                            .OrderBy(x => x.MessageSent)
                            .AsQueryable();

            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
        public async Task<ActionResult> SendMessage(int requestUserId, MessageSendRequest messageSendRequest)
        {
            _logger.LogInformation(
                "SendMessage, (requestUserId: {requestUserId}, messageSendRequest: {messageSendReq})",
                requestUserId,
                JsonConvert.SerializeObject(messageSendRequest));

            var sender = await _context.Users.FindAsync(requestUserId);
            if (sender == null)
            {
                return new BadRequestObjectResult($"Sender with id {requestUserId} does not exist");
            }

            var recipient = await _context.Users.FindAsync(messageSendRequest.RecipientId);
            if (recipient == null)
            {
                return new BadRequestObjectResult($"Recipient with id {messageSendRequest.RecipientId} does not exist");
            }

            Message message = new()
            {
                Sender = sender,
                Recipient = recipient,
                SenderId = requestUserId,
                Content = messageSendRequest.Content
            };

            _context.Messages.Add(message);

            if (await _context.SaveChangesAsync() > 0)
            {
                return new CreatedAtActionResult("", "", "", "Message sent successfully");
            }

            return new StatusCodeResult(500);
        }

        public async Task<ActionResult> DeleteMessage(int requestUserId, int messageId)
        {
            _logger.LogInformation("DeleteMessage, (requestUserId: {requestUserId}, messageId: {msgId})", requestUserId, messageId);

            var message = await _context.Messages.FindAsync(messageId);

            if (message.SenderId != requestUserId)
            {
                _logger.LogError("You can only delete your sent messages!");

                return new BadRequestObjectResult("You can only delete your sent messages!");
            }

            _context.Messages.Remove(message);

            if (await _context.SaveChangesAsync() > 0)
            {
                return new OkObjectResult("Message deleted successfully");
            }

            return new StatusCodeResult(500);
        }
    }
}