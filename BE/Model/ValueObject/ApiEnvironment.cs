namespace BE.Model.ValueObject
{
    public class ApiEnvironment
    {
        public const string SectionName = "ApiEnvironment";

        public string ClientURL { get; set; } = "";
        public string ApiURL { get; set; } = "";
        public int NormalPrice { get; set; }
        public int DeluxePrice { get; set; }
        public int WaifuPrice { get; set; }
        public int DiscountPercent { get; set; }
        public int DefRegMoney { get; set; }
    }
}
