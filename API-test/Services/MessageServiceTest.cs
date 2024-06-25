using API.Data;
using API.DTOs;
using API.DTOs.Requests;
using API.Entities;
using API.Services;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_test.Services
{
    public class MessageServiceTest
    {
        private ApplicationDbContext _applicationDbContext;
        private IMapper _mapper;
        private ILogger<MessageService> _logger;
        private MessageService _messageService;

        public MessageServiceTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                                     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                     .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            _applicationDbContext = dbContext;
            _mapper = A.Fake<IMapper>();
            _logger = A.Fake<ILogger<MessageService>>();

            _messageService = new MessageService(_applicationDbContext, _mapper, _logger);
        }

        [Fact]
        public async Task MessageService_GetMessagesForChat_ShouldReturnListOfMessageDto()
        {
            //ARRANGE
            User user1 = new() { Id = 1, UserName = "User 1" };
            User user2 = new() { Id = 2, UserName = "User 2" };

            _applicationDbContext.Users.AddRange(new List<User> { user1, user2 });

            await _applicationDbContext.SaveChangesAsync();

            _applicationDbContext.Messages.AddRange(new List<Message>
            {
                new() {Id = 1, Sender = user1, Recipient = user2, Content = "Message 1", MessageSent = new DateTime(2010, 1, 1)},
                new() {Id = 2, Sender = user1, Recipient = user2, Content = "Message 2", MessageSent = new DateTime(2010, 1, 2)},
                new() {Id = 3, Sender = user2, Recipient = user1, Content = "Message 3", MessageSent = new DateTime(2010, 1, 3)},
            });

            await _applicationDbContext.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(config => config.CreateMap<Message, MessageDto>());

            A.CallTo(() => _mapper.ConfigurationProvider).Returns(mapperConfig);
            //ACT
            var result = await _messageService.GetMessagesForChat(1, 2);
            //ASSERT
            var list = result.Value!;

            list.Count.Should().Be(3);

            list[0].Id.Should().Be(1);
            list[0].SenderId.Should().Be(1);
            list[0].RecipientId.Should().Be(2);
            list[0].Content.Should().Be("Message 1");
            list[0].MessageSent.Should().Be(new DateTime(2010, 1, 1));

            list[1].Id.Should().Be(2);
            list[1].SenderId.Should().Be(1);
            list[1].RecipientId.Should().Be(2);
            list[1].Content.Should().Be("Message 2");
            list[1].MessageSent.Should().Be(new DateTime(2010, 1, 2));

            list[2].Id.Should().Be(3);
            list[2].SenderId.Should().Be(2);
            list[2].RecipientId.Should().Be(1);
            list[2].Content.Should().Be("Message 3");
            list[2].MessageSent.Should().Be(new DateTime(2010, 1, 3));

            _applicationDbContext.Users.RemoveRange();
            _applicationDbContext.Messages.RemoveRange();

            await _applicationDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task MessageService_SendMessage_ShouldReturnOkIfMessageSent()
        {
            //ARRANGE
            User user1 = new() { Id = 1, UserName = "User 1" };
            User user2 = new() { Id = 2, UserName = "User 2" };

            _applicationDbContext.Users.AddRange(new List<User> { user1, user2 });

            await _applicationDbContext.SaveChangesAsync();

            MessageSendRequest request = new()
            {
                RecipientId = user2.Id,
                Content = "Hello"
            };
            //ACT
            var result = await _messageService.SendMessage(user1.Id, request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedAtActionResult>();
            result.As<CreatedAtActionResult>().Value.Should().Be("Message sent successfully");

            _applicationDbContext.Users.RemoveRange();
            _applicationDbContext.Messages.RemoveRange();

            await _applicationDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task MessageService_SendMessage_ShouldReturnBadRequestIfSenderNotFound()
        {
            //ARRANGE
            var senderId = 1;

            User user = new() { Id = 2, UserName = "User 1" };

            _applicationDbContext.Users.AddRange(new List<User> { user });

            await _applicationDbContext.SaveChangesAsync();

            MessageSendRequest request = new()
            {
                RecipientId = user.Id,
                Content = "Hello"
            };
            //ACT
            var result = await _messageService.SendMessage(senderId, request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().Value.Should().Be($"Sender with id {senderId} does not exist");

            _applicationDbContext.Users.RemoveRange();
            _applicationDbContext.Messages.RemoveRange();

            await _applicationDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task MessageService_SendMessage_ShouldReturnBadRequestIfRecipientNotFound()
        {
            //ARRANGE
            var recipientId = 2;

            User user = new() { Id = 1, UserName = "User 1" };

            _applicationDbContext.Users.AddRange(new List<User> { user });

            await _applicationDbContext.SaveChangesAsync();

            MessageSendRequest request = new()
            {
                RecipientId = recipientId,
                Content = "Hello"
            };
            //ACT
            var result = await _messageService.SendMessage(user.Id, request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().Value.Should().Be($"Recipient with id {recipientId} does not exist");

            _applicationDbContext.Users.RemoveRange();
            _applicationDbContext.Messages.RemoveRange();

            await _applicationDbContext.SaveChangesAsync();
        }

        // [Fact]
        // public async Task MessageService_SendMessage_ShouldReturn500IfSendingFails()
        // {
        //     //ARRANGE
        //     //ACT
        //     //ASSERT
        // }

        [Fact]
        public async Task MessageService_DeleteMessage_ShouldReturnOkIfMessageIsDeleted()
        {
            //ARRANGE
            User user1 = new() { Id = 1, UserName = "User 1" };
            User user2 = new() { Id = 2, UserName = "User 2" };

            _applicationDbContext.Users.AddRange(new List<User> { user1, user2 });

            Message message = new() { Id = 1, Sender = user1, Recipient = user2, Content = "Message 1", MessageSent = new DateTime(2010, 1, 1) };

            _applicationDbContext.Messages.Add(message);

            await _applicationDbContext.SaveChangesAsync();

            //ACT
            var result = await _messageService.DeleteMessage(user1.Id, message.Id);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be("Message deleted successfully");

            _applicationDbContext.Users.RemoveRange();
            _applicationDbContext.Messages.RemoveRange();

            await _applicationDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task MessageService_DeleteMessage_ShouldReturn400IfRequestUserIsNotSender()
        {
            //ARRANGE
            User user1 = new() { Id = 1, UserName = "User 1" };
            User user2 = new() { Id = 2, UserName = "User 2" };

            _applicationDbContext.Users.AddRange(new List<User> { user1, user2 });

            Message message = new() { Id = 1, Sender = user1, Recipient = user2, Content = "Message 1", MessageSent = new DateTime(2010, 1, 1) };

            _applicationDbContext.Messages.Add(message);

            await _applicationDbContext.SaveChangesAsync();
            //ACT
            var result = await _messageService.DeleteMessage(user2.Id, message.Id);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().Value.Should().Be("You can only delete your sent messages!");

            _applicationDbContext.Users.RemoveRange();
            _applicationDbContext.Messages.RemoveRange();

            await _applicationDbContext.SaveChangesAsync();

        }

        // [Fact]
        // public async Task MessageService_DeleteMessage_ShouldReturn500IfDeletionFails()
        // {
        //     //ARRANGE
        //     //ACT
        //     //ASSERT
        // }
    }
}