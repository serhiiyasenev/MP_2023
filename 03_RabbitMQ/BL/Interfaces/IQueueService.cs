using BusinessLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IQueueService
    {
        Task<SendResultModel> PostMessageAsync(SendRequestModel sendRequestModel);
        Task<SendResultModel> PostFileAsync(string fileName, Stream fileStream);
        Task<List<FileInfoModel>> GetUploadedFilesInfoAsync();
        Task<(string FileName, byte[] FileContent)> GetFileByIdAsync(string fileId);
    }
}
