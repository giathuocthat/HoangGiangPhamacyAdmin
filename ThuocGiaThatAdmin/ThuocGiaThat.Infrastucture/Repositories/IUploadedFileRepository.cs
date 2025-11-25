using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IUploadedFileRepository : IRepository<UploadedFile>
    {
        Task<IEnumerable<UploadedFile>> GetByUploadSourceAsync(UploadSource uploadSource);
        Task<IEnumerable<UploadedFile>> GetByVendorIdAsync(int vendorId);
        Task<IEnumerable<UploadedFile>> GetByRelatedEntityAsync(UploadSource uploadSource, int relatedEntityId);
        Task<IEnumerable<UploadedFile>> GetActiveFilesAsync();
    }
}
