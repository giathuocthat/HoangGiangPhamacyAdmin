using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class UploadedFileRepository : Repository<UploadedFile>, IUploadedFileRepository
    {
        public UploadedFileRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UploadedFile>> GetByUploadSourceAsync(UploadSource uploadSource)
        {
            return await _dbSet
                .Where(f => f.UploadSource == uploadSource && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UploadedFile>> GetByVendorIdAsync(int vendorId)
        {
            return await _dbSet
                .Where(f => f.VendorId == vendorId && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UploadedFile>> GetByRelatedEntityAsync(UploadSource uploadSource, int relatedEntityId)
        {
            return await _dbSet
                .Where(f => f.UploadSource == uploadSource && f.RelatedEntityId == relatedEntityId && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UploadedFile>> GetActiveFilesAsync()
        {
            return await _dbSet
                .Where(f => !f.IsDeleted && f.IsInUse)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }
    }
}
