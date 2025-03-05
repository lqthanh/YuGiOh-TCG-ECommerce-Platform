using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE._extensions;
using BE.Context;
using BE.InterfaceController;
using BE.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BE.Controllers
{
    public class UserCardController : BaseApiController, IUserCardController
    {
        private readonly DataContext _context;
        public UserCardController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("SearchOwnedSeparate")]
        public async Task<ActionResult<List<UserCardSearchOwnedOutputDto>>> SearchOwnedSeparate([FromQuery] UserCardSearchOwnedInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (user == null) return BadRequest(new { message = "User not found!" });
            var userCard = from UserCard in _context.UserCard
                            join Card in _context.Card on UserCard.CardId equals Card.CardId
                            where (UserCard.UserId == user.UserId)
                            && (string.IsNullOrWhiteSpace(input.CardName) || Card.CardName.Contains(input.CardName))
                            && (string.IsNullOrWhiteSpace(input.CardTypeName) || Card.CardTypeName == input.CardTypeName)
                            && (string.IsNullOrWhiteSpace(input.CardOriginName) || Card.CardOriginName == input.CardOriginName)
                            && (string.IsNullOrWhiteSpace(input.CardElementName) || Card.CardElementName == input.CardElementName)
                            && (string.IsNullOrWhiteSpace(input.CardRarityName) || Card.CardRarityName == input.CardRarityName)
                            orderby Card.CardRarityName descending, Card.CardName
                            select new UserCardSearchOwnedOutputDto()
                            {
                                UserCardId = UserCard.UserCardId,
                                CardId = Card.CardId,
                                CardName = Card.CardName,
                                CardImageURL = Card.CardImageURL,
                                CardTypeName = Card.CardTypeName,
                                CardOriginName = Card.CardOriginName,
                                CardElementName = Card.CardElementName,
                                CardRarityName = Card.CardRarityName,
                                OnDeal = UserCard.OnDeal,
                                Quantity = 1,
                            };
            return await userCard.ToListAsync();
        }

        [Authorize]
        [HttpGet("SearchOwnedStack")]
        public async Task<ActionResult<List<UserCardSearchOwnedOutputDto>>> SearchOwnedStack([FromQuery] UserCardSearchOwnedInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (user == null) return BadRequest(new { message = "User not found!" });
            var result = await _context.SearchOwnedOutput
                .FromSqlRaw("EXEC SearchOwnedStack @UserId, @CardName, @CardTypeName, @CardOriginName, @CardElementName, @CardRarityName",
                    new SqlParameter("@UserId", user.UserId),
                    new SqlParameter("@CardName", input.CardName ?? ""),
                    new SqlParameter("@CardTypeName", input.CardTypeName ?? ""),
                    new SqlParameter("@CardOriginName", input.CardOriginName ?? ""),
                    new SqlParameter("@CardElementName", input.CardElementName ?? ""),
                    new SqlParameter("@CardRarityName", input.CardRarityName ?? "")
                )
                .ToListAsync();
            List<UserCardSearchOwnedOutputDto> userCard = new List<UserCardSearchOwnedOutputDto>();
            foreach (var item in result)
            {
                UserCardSearchOwnedOutputDto uc = new UserCardSearchOwnedOutputDto()
                {
                    UserCardId = 0,
                    CardId = item.CardId,
                    CardName = item.CardName,
                    CardImageURL = item.CardImageURL,
                    CardTypeName = item.CardTypeName,
                    CardOriginName = item.CardOriginName,
                    CardElementName = item.CardElementName,
                    CardRarityName = item.CardRarityName,
                    OnDeal = item.OnDeal,
                    Quantity = item.Quantity,
                };
                userCard.Add(uc);
            }
            return userCard;
        }

        [NonAction]
        public async Task MakeOnDeal(long UserCardId)
        {
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == UserCardId);
            usercard.OnDeal = true;
            _context.SaveChanges();
        }

        [NonAction]
        public async Task RemoveOnDeal(long UserCardId)
        {
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == UserCardId);
            usercard.OnDeal = false;
            _context.SaveChanges();
        }

        [NonAction]
        public async Task ChangeOwner(long UserCardId, long newOwnerId)
        {
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == UserCardId);
            usercard.UserId = newOwnerId;
            usercard.OnDeal = false;
            _context.SaveChanges();
        }
    }
}