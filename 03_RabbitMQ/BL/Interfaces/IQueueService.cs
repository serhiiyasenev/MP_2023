using BusinessLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IQueueService
    {
        Task<SendResultModel> PostMessageAsync(SendRequestModel sendRequestModel);
        Task<SendResultModel> PostFileAsync(Stream fileStream);
    }
}
