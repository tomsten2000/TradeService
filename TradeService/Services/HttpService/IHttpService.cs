using InventorySerivce.Models.Protobufs;

namespace TradeService.Services.HttpService
{
    public interface ITradeRequestHttpService
    {
        Task SendTradeRequestAsync(SendTradeDto trade);
    }
}
