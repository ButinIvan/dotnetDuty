using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using dotnetWebApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace dotnetWebApi.Documents.Repositories;

public class CommentRepository(ApplicationDbContext dbContext) :ICommentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    
    public async Task AddCommentAsync(Comment comment)
    { 
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Comment>> GetAllCommentsAsync(Guid documentId)
    {
        return await _dbContext.Comments.Where(x => x.DocumentId == documentId).ToListAsync();
    }

    public async Task<List<Comment>> GetAllReviewerCommentsAsync(Guid documentId, Guid reviewerId)
    {
        return await _dbContext.Comments.Where(x => x.DocumentId == documentId && x.ReviewerId == reviewerId).ToListAsync();
    }

    public async Task DeleteAllReviewerCommentsAsync(Guid reviewerId)
    {
        var comments = await _dbContext.Comments.Where(x => x.ReviewerId == reviewerId).ToListAsync();
        _dbContext.Comments.RemoveRange(comments);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
    {
        return await _dbContext.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task DeleteCommentAsync(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task AddReplyAsync(Guid parentCommentId, Comment reply)
    {
        reply.SetParentComment(parentCommentId);
        await _dbContext.Comments.AddAsync(reply);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Comment>> GetRepliesAsync(Guid parentCommentId)
    {
        return await _dbContext.Comments
            .Where(c => c.ParentCommentId == parentCommentId)
            .ToListAsync();
    }

    public async Task<bool> IsReplyAsync(Guid commentId)
    {
        return await _dbContext.Comments.Where(x => x.Id == commentId).AnyAsync(x => x.ParentCommentId != null);
    }
}