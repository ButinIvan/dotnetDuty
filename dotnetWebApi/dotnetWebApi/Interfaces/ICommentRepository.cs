using dotnetWebApi.Entities;

namespace dotnetWebApi.Interfaces;

public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);
    Task<List<Comment>> GetAllCommentsAsync(Guid documentId);
    Task<List<Comment>> GetAllReviewerCommentsAsync(Guid documentId, Guid reviewerId);
    Task DeleteAllReviewerCommentsAsync(Guid reviewerId);
    Task<Comment?> GetCommentByIdAsync(Guid commentId);
    Task DeleteCommentAsync(Comment comment);
    Task AddReplyAsync(Guid parentCommentId, Comment comment);
    Task<List<Comment>> GetRepliesAsync(Guid parentCommentId);
    Task<bool> IsReplyAsync(Guid commentId);
}