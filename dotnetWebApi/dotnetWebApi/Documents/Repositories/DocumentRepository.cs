using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using dotnetWebApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace dotnetWebApi.Documents.Repositories;

public class DocumentRepository(ApplicationDbContext dbContext) :IDocumentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    
    public Task<Document?> GetByIdAsync(Guid id)
    {
        return _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Document>> GetUserDocumentsAsync(Guid userId)
    {
        return await _dbContext.Documents.Where(x => x.OwnerId == userId).ToListAsync();
    }

    public async Task<string?> GetUserRoleAsync(Guid documentId, Guid userId)
    {
        var reviewer =
            await _dbContext.Reviewers.FirstOrDefaultAsync(x => x.DocumentId == documentId && x.UserId == userId);
        return reviewer?.Role;
    }

    public async Task AddReviewerAsync(Guid documentId, Guid userId, string role)
    {
        var reviewer = new Reviewer(documentId, userId, role);
        await _dbContext.Reviewers.AddAsync(reviewer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsReviewerAsync(Guid documentId, Guid userId)
    {
        return await _dbContext.Reviewers.AnyAsync(x => x.DocumentId == documentId && x.UserId == userId);
    }

    public async Task<Reviewer?> GetReviewerAsync(Guid documentId, Guid ownerId)
    {
        return await _dbContext.Reviewers.FirstOrDefaultAsync(x => x.DocumentId == documentId && x.UserId == ownerId);
    }

    public async Task<List<Reviewer>> GetReviewersAsync(Guid documentId)
    {
        return await _dbContext.Reviewers.Where(x => x.DocumentId == documentId).ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _dbContext.Documents.Where(x => x.OwnerId == ownerId).ToListAsync();
    }

    public async Task AddAsync(Document document)
    {
        await _dbContext.Documents.AddAsync(document);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Document document)
    {
        _dbContext.Documents.Update(document);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveReviewersAsync(Guid documentId)
    {
        var reviewers = _dbContext.Reviewers.Where(x => x.DocumentId == documentId);
        _dbContext.Reviewers.RemoveRange(reviewers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid documentId)
    {
        var document = await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == documentId);
        if (document != null) _dbContext.Documents.Remove(document);
        await _dbContext.SaveChangesAsync();
    }
}