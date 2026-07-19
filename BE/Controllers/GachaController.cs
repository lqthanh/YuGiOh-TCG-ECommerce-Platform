using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE._extensions;
using BE.Context;
using BE.Model.Dto;
using BE.Model.Entity;
using BE.Model.ValueObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace BE.Controllers
{
    public class GachaController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly int normalPrice;
        private readonly int deluxePrice;
        private readonly int waifuPrice;
        public GachaController(DataContext context, IOptionsSnapshot<ApiEnvironment> apiEnvironment)
        {
            _context = context;
            var env = apiEnvironment.Value;
            var actualPercent = 100 - env.DiscountPercent;
            normalPrice = env.NormalPrice*actualPercent/100;
            deluxePrice = env.DeluxePrice*actualPercent/100;
            waifuPrice = env.WaifuPrice*actualPercent/100;
        }

        [Authorize]
        [HttpGet("Gacha")]
        public async Task<ActionResult<List<GachaOutputDto>>> Gacha([FromQuery] GachaInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == User.GetName());
            Card randomCard;
            List<Card> allCards = new List<Card>();
            if(input.Pack == "normal")
            {
                if(user.Money >= normalPrice)
                {
                    user.Money -= normalPrice;
                    _context.SaveChanges();
                }
                else return BadRequest(new {message = "Account don't have enough money!"});

                for (int i = 0; i < 10; i++)
                {
                    var rate = new Random().Next(1, 101);

                    if (rate <= 40) randomCard = await GetRandomCardsByRarity("N");
                    else if (rate <= 70) randomCard = await GetRandomCardsByRarity("R");
                    else if (rate <= 95) randomCard = await GetRandomCardsByRarity("SR");
                    else randomCard = await GetRandomCardsByRarity("UR");
                    allCards.Add(randomCard);
                }
            }
            
            if(input.Pack == "deluxe")
            {
                if(user.Money >= deluxePrice)
                {
                    user.Money -= deluxePrice;
                    _context.SaveChanges();
                }
                else return BadRequest(new {message = "Account don't have enough money!"});

                for (int i = 0; i < 10; i++)
                {
                    var rate = new Random().Next(1, 101);

                    if (rate <= 80) randomCard = await GetRandomCardsByRarity("SR");
                    else randomCard = await GetRandomCardsByRarity("UR");
                    allCards.Add(randomCard);
                }
            }

            if(input.Pack == "waifu")
            {
                if(user.Money >= waifuPrice)
                {
                    user.Money -= waifuPrice;
                    _context.SaveChanges();
                }
                else return BadRequest(new {message = "Account don't have enough money!"});

                for (int i = 0; i < 10; i++)
                {
                    var rate = new Random().Next(1, 101);

                    if (rate <= 40) randomCard = await GetRandomCardsLabrynth("N");
                    else if (rate <= 70) randomCard = await GetRandomCardsLabrynth("R");
                    else if (rate <= 95) randomCard = await GetRandomCardsLabrynth("SR");
                    else randomCard = await GetRandomCardsLabrynth("UR");
                    allCards.Add(randomCard);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                var card = allCards[i];

                //Tạo mới UserCard và thêm vào cơ sở dữ liệu
                var userCard = new UserCard
                {
                    UserId = user.UserId, // Sử dụng UserId từ bảng User
                    CardId = card.CardId,
                };

                _context.UserCard.Add(userCard);
                _context.SaveChanges();
            }

            return Ok(allCards);
        }

        private async Task<Card> GetRandomCardsByRarity(string rarity)
        {
            return await _context.Card
                .Where(c => c.CardRarityName == rarity)
                .OrderBy(c => Guid.NewGuid())
                .Take(1)
                .SingleOrDefaultAsync();
        }

        private async Task<Card> GetRandomCardsLabrynth(string rarity)
        {
            return await _context.Card
                .Where(c => c.CardRarityName == rarity && c.CardName.Contains("Labrynth"))
                .OrderBy(c => Guid.NewGuid())
                .Take(1)
                .SingleOrDefaultAsync();
        }

    }
}