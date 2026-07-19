using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Model.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BE.InterfaceController
{
    public interface IUserCardController
    {
        //[Authorize]
        //[HttpGet("SearchOwnedSeparate")]
        Task<ActionResult<PagedResultDto<UserCardSearchOwnedOutputDto>>> SearchOwnedSeparate(UserCardSearchOwnedInputDto input, int page, int pageSize);

        //[Authorize]
        //[HttpGet("SearchOwnedStack")]
        Task<ActionResult<PagedResultDto<UserCardSearchOwnedOutputDto>>> SearchOwnedStack(UserCardSearchOwnedInputDto input, int page, int pageSize);

        //[NonAction]
        Task MakeOnDeal(long UserCardId);

        //[NonAction]
        Task RemoveOnDeal(long UserCardId);

        //[NonAction]
        Task ChangeOwner(long UserCardId, long newOwnerId);
    }
}