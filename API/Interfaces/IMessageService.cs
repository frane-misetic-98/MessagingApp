using API.DTOs;
using API.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IMessageService
    {
        Task<ActionResult> SendMessage(int currentUserId, MessageSendRequest messageSendRequest);
        Task<ActionResult<List<MessageDto>>> GetMessagesForChat(int currentUserId, int recipientId);
        Task<ActionResult> DeleteMessage(int id, int messageId);
    }
}