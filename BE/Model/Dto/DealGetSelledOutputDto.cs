using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Model.Dto
{
    public class DealGetSelledOutputDto
    {
        public long DealId { get; set; }
        public string BuyUsername { get; set; }
        public long CardId { get; set; }
        public string CardName { get; set; }
        public string CardImageURL { get; set; }
        public string CardTypeName { get; set; }
        public string? CardOriginName { get; set; }
        public string? CardElementName { get; set; }
        public string CardRarityName { get; set; }
        public int Price { get; set; }
        public DateTime CreateDate { get; set; }
    }
}