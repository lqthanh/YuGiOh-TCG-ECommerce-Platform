using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE._extensions;
using BE._iservices;
using BE.Context;
using BE.InterfaceController;
using BE.Model.Dto;
using BE.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    public class DealController : BaseApiController, IDealController
    {
        TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        DateTime vietnamTime;
        private readonly DataContext _context;
        private readonly UserCardController _userCard;
        private readonly IContentService _contentService;

        public DealController(DataContext context, UserCardController userCard, IContentService contentService)
        {
            _context = context;
            _userCard = userCard;
            _contentService = contentService;
            vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }

        [HttpGet("SearchDeal")]
        public async Task<ActionResult<List<DealSearchOutputDto>>> SearchDeal([FromQuery] DealSearchInputDto input, string? sort, bool sortAscending = true)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == input.MyUsername);
            var deal = from Deal in _context.Deal 
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            join Card in _context.Card on UserCard.CardId equals Card.CardId 
            join User in _context.User on UserCard.UserId equals User.UserId
            where (string.IsNullOrWhiteSpace(input.SellUsername) || User.Username.Contains(input.SellUsername))
            && (string.IsNullOrWhiteSpace(input.CardName) || Card.CardName.Contains(input.CardName))
            && (string.IsNullOrWhiteSpace(input.CardTypeName) || Card.CardTypeName == input.CardTypeName)
            && (string.IsNullOrWhiteSpace(input.CardOriginName) || Card.CardOriginName == input.CardOriginName)
            && (string.IsNullOrWhiteSpace(input.CardElementName) || Card.CardElementName == input.CardElementName)
            && (string.IsNullOrWhiteSpace(input.CardRarityName) || Card.CardRarityName == input.CardRarityName)
            && (input.PriceFrom == null || input.PriceFrom == 0 || Deal.Price >= input.PriceFrom) && (input.PriceTo == null || Deal.Price <= input.PriceTo)
            && (input.DateFrom == null || Deal.CreateDate >= input.DateFrom) && (input.DateTo == null || Deal.CreateDate <= input.DateTo.Value.AddDays(1))
            && (Deal.BuyUserId == null)
            && (User.Username != input.MyUsername)
            select new DealSearchOutputDto() {
                DealId = Deal.DealId,
                SellUsername = User.Username,
                SellUsernameAvatarUrl = User.AvatarUrl,
                UserCardId = UserCard.UserCardId,
                CardId = Card.CardId,
                CardName = Card.CardName,
                CardImageURL = Card.CardImageURL,
                CardTypeName = Card.CardTypeName,
                CardOriginName = Card.CardOriginName,
                CardElementName = Card.CardElementName,
                CardRarityName = Card.CardRarityName,
                Price = Deal.Price,
                CreateDate = Deal.CreateDate,
            };
            if (sort == "time")
            {
                deal = sortAscending ? deal.OrderBy(d => d.CreateDate) : deal.OrderByDescending(d => d.CreateDate);
            }
            if (sort == "rarity")
            {
                deal = sortAscending ? deal.OrderBy(d => d.CardRarityName) : deal.OrderByDescending(d => d.CardRarityName);
            }
            if (sort == "price")
            {
                deal = sortAscending ? deal.OrderBy(d => d.Price) : deal.OrderByDescending(d => d.Price);
            }
            return await deal.ToListAsync();
        }

        [HttpGet("GetCardPrice/{cardId}/{currentYear}")]
        public async Task<ActionResult> GetCardPrice(long cardId, int currentYear)
        {
            var deal = from Deal in _context.Deal
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            where (UserCard.CardId == cardId)
            && (Deal.AcceptedDate.Value.Year == currentYear)
            && (Deal.BuyUserId != null)
            orderby Deal.AcceptedDate
            select new Deal() {
                Price = Deal.Price,
                AcceptedDate = Deal.AcceptedDate
            };
            return Content(await _contentService.PriceChartWrite(await deal.ToListAsync()), "text/html");
        }

        [Authorize]
        [HttpGet("GetBoughtDeal")]
        public async Task<ActionResult<List<DealGetBuyedOutputDto>>> GetBoughtDeal([FromQuery] string Username)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (user == null) return BadRequest(new {message = "User not found!"});
            var deal = from Deal in _context.Deal 
            join User in _context.User on Deal.SellUserId equals User.UserId 
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            join Card in _context.Card on UserCard.CardId equals Card.CardId 
            orderby Deal.AcceptedDate descending
            where (Deal.BuyUserId != null)
            && (Deal.BuyUserId == user.UserId)
            select new DealGetBuyedOutputDto() {
                DealId = Deal.DealId,
                SellUsername = User.Username,
                CardId = Card.CardId,
                CardName = Card.CardName,
                CardImageURL = Card.CardImageURL,
                CardTypeName = Card.CardTypeName,
                CardOriginName = Card.CardOriginName,
                CardElementName = Card.CardElementName,
                CardRarityName = Card.CardRarityName,
                Price = Deal.Price,
                CreateDate = Deal.CreateDate,
                AcceptedDate = Deal.AcceptedDate,
            };
            return await deal.ToListAsync();
        }

        [Authorize]
        [HttpGet("GetSoldDeal")]
        public async Task<ActionResult<List<DealGetSelledOutputDto>>> GetSoldDeal([FromQuery] string Username)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (user == null) return BadRequest(new {message = "User not found!"});
            var deal = from Deal in _context.Deal 
            join User in _context.User on Deal.BuyUserId equals User.UserId 
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            join Card in _context.Card on UserCard.CardId equals Card.CardId 
            where (Deal.BuyUserId != null)
            && (Deal.SellUserId == user.UserId)
            orderby Deal.AcceptedDate descending
            select new DealGetSelledOutputDto() {
                DealId = Deal.DealId,
                BuyUsername = User.Username,
                CardId = Card.CardId,
                CardName = Card.CardName,
                CardImageURL = Card.CardImageURL,
                CardTypeName = Card.CardTypeName,
                CardOriginName = Card.CardOriginName,
                CardElementName = Card.CardElementName,
                CardRarityName = Card.CardRarityName,
                Price = Deal.Price,
                CreateDate = Deal.CreateDate,
                AcceptedDate = Deal.AcceptedDate,
            };
            return await deal.ToListAsync();
        }

        [Authorize]
        [HttpPost("CreateDeal")]
        public async Task<ActionResult> CreateDeal([FromBody] DealCreateInputDto input)
        {
            var selluser = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (selluser == null) return BadRequest(new {message = "User not found!"});
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == input.UserCardId);
            if (usercard == null) return BadRequest(new {message = "Your Card not found!"});
            if (usercard.UserId != selluser.UserId) return BadRequest(new {message = "You don't owned this Card!"});
            if (usercard.OnDeal == true) return BadRequest(new {message = "Card already on another Deal!"});
            if (input.Price <= 0) return BadRequest(new {message = "Price invalid!"});
            else 
            {
                var newdeal = new Deal 
                {
                    SellUserId = selluser.UserId,
                    BuyUserId = null,
                    UserCardId = usercard.UserCardId,
                    Price = input.Price,
                    CreateDate = vietnamTime,
                    AcceptedDate = null,
                };
                _context.Deal.Add(newdeal);
                await _userCard.MakeOnDeal(usercard.UserCardId);
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Create Deal successfully!"});
        }

        [Authorize]
        [HttpPut("EditDeal")]
        public async Task<ActionResult> EditDeal([FromBody] DealEditInputDto input)
        {
            var selluser = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (selluser == null) return BadRequest(new {message = "User not found!"});
            var deal = await _context.Deal.SingleOrDefaultAsync(d => d.DealId == input.DealId);
            if (deal == null) return NotFound(new {message = "Deal not found!"});
            if (deal.SellUserId != selluser.UserId) return NotFound(new {message = "You don't owned this Deal!"});
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == input.UserCardId);
            if (usercard == null) return BadRequest(new {message = "Your Card not found!"});
            if (usercard.UserId != selluser.UserId) return BadRequest(new {message = "You don't owned this Card!"});
            if (usercard.OnDeal == true && usercard.UserCardId != deal.UserCardId) return BadRequest(new {message = "Card already on another Deal!"});
            if (input.Price <= 0) return BadRequest(new {message = "Price invalid!"});
            else
            {
                await _userCard.RemoveOnDeal(deal.UserCardId);
                await _userCard.MakeOnDeal(input.UserCardId);
                deal.UserCardId = input.UserCardId;
                deal.Price = input.Price;
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Edit Deal successfully!"});
        }

        [Authorize]
        [HttpDelete("DeleteDeal")]
        public async Task<ActionResult> DeleteDeal([FromBody] DealDeleteInputDto input)
        {
            var selluser = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (selluser == null) return BadRequest(new {message = "User not found!"});
            var deal = await _context.Deal.SingleOrDefaultAsync(d => d.DealId == input.DealId);
            if (deal == null) return NotFound(new {message = "Deal not found!"});
            if (deal.SellUserId != selluser.UserId) return NotFound(new {message = "You don't owned this Deal!"});
            else
            {
                _context.Deal.Remove(deal);
                await _userCard.RemoveOnDeal(deal.UserCardId);
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Delete Deal successfully!"});
        }
        
        [Authorize]
        [HttpPost("AcceptDeal")]
        public async Task<ActionResult> AcceptDeal([FromBody] DealAcceptInputDto input)
        {
            var buyuser = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            if (buyuser == null) return BadRequest(new {message = "User not found!"});
            var deal = await _context.Deal.SingleOrDefaultAsync(d => d.DealId == input.DealId);
            if (deal == null) return NotFound(new {message = "Deal not found!"});
            if (buyuser.UserId == deal.SellUserId) return BadRequest(new {message = "Can't buy your own card!"});
            if (buyuser.Money < deal.Price) return BadRequest(new {message = "Account don't have enough money!"});
            if (deal.BuyUserId != null) return BadRequest(new {message = "Deal does not exist!"});
            else
            {
                deal.BuyUserId = buyuser.UserId;
                deal.AcceptedDate = vietnamTime;
                await _userCard.ChangeOwner(deal.UserCardId, buyuser.UserId);
                buyuser.Money -= deal.Price;
                var selluser = await _context.User.SingleOrDefaultAsync(u => u.UserId == deal.SellUserId);
                if (selluser != null)
                {
                    selluser.Money += deal.Price;
                }
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Purchase successfully!"});
        }
    }
}