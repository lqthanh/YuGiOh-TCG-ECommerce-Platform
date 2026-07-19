using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Model.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BE.InterfaceController
{
    public interface IDealController
    {
        //[HttpGet("SearchDeal")]
        Task<ActionResult<PagedResultDto<DealSearchOutputDto>>> SearchDeal(DealSearchInputDto input, string? sort, bool sortAscending, int page, int pageSize);

        //[Authorize]
        //[HttpGet("GetBoughtDeal")]
        Task<ActionResult<PagedResultDto<DealGetBuyedOutputDto>>> GetBoughtDeal(string Username, int page, int pageSize);

        //[Authorize]
        //[HttpGet("GetSoldDeal")]
        Task<ActionResult<PagedResultDto<DealGetSelledOutputDto>>> GetSoldDeal(string Username, int page, int pageSize);
    
        //[Authorize]
        //[HttpPost("CreateDeal")]
        Task<ActionResult> CreateDeal(DealCreateInputDto input);

        //[Authorize]
        //[HttpPut("EditDeal")]
        Task<ActionResult> EditDeal(DealEditInputDto input);
        
        //[Authorize]
        //[HttpDelete("DeleteDeal")]
        Task<ActionResult> DeleteDeal(DealDeleteInputDto input);

        //[Authorize]
        //[HttpPost("AcceptDeal")]
        Task<ActionResult> AcceptDeal(DealAcceptInputDto input);
    }
}