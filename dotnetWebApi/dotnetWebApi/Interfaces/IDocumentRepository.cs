using dotnetWebApi.Entities;

namespace dotnetWebApi.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<string?> GetUserRoleAsync(Guid documentId, Guid ownerId);
    Task AddReviewerAsync(Guid documentId, Guid userId, string role);
    Task<bool> IsReviewerAsync(Guid documentId, Guid userId);
    Task<Reviewer?> GetReviewerAsync(Guid documentId, Guid userId);
    Task<List<Document>> GetUserDocumentsAsync(Guid userId);
    Task<List<Reviewer>> GetReviewersAsync(Guid documentId);
    Task<IEnumerable<Document>> GetByOwnerIdAsync(Guid ownerId);
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task RemoveReviewersAsync(Guid documentId);
    Task DeleteAsync(Guid documentId);
}