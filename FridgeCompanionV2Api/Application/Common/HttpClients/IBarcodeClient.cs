using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.HttpClients
{
    public interface IBarcodeClient
    {
        Task<string> GetItemName(string ean);
    }
}