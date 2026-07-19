using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Context;
using BE.Model.Dto;
using BE.Model.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BE.InterfaceController
{
    public interface ICardController
    {
        //[HttpGet("SearchCard")]
        Task<ActionResult<PagedResultDto<CardSearchOutputDto>>> SearchCard(CardSearchInputDto input, int page, int pageSize);

        //[HttpGet("GetCardType")]
        Task<ActionResult<List<CardType>>> GetCardType();

        //[HttpGet("GetCardOrigin")]
        Task<ActionResult<List<CardOrigin>>> GetCardOrigin();

        //[HttpGet("GetCardElement")]
        Task<ActionResult<List<CardElement>>> GetCardElement();

        //[HttpGet("GetCardRarity")]
        Task<ActionResult<List<CardRarity>>> GetCardRarity();
    }
}