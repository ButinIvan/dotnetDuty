﻿namespace dotnetWebApi.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsClosedToComment { get; private set; }
    public DateTime LastEdited { get; private set; }
    public string S3Path { get; private set; }
    
    public User Owner { get; private set; }
    public ICollection<Reviewer> Reviewers { get; private set; } = new List<Reviewer>();

    private Document()
    {
    }

    public Document(string title, Guid ownerId, string s3Path)
    {
        Id = Guid.NewGuid();
        Title = title;
        OwnerId = ownerId;
        IsFinished = false;
        IsClosedToComment = true;
        LastEdited = DateTime.UtcNow;
        S3Path = s3Path;
    }
    
    public void UpdateTitle(string title)
    {
        Title = title;
        LastEdited = DateTime.UtcNow;
    }

    public void UpdateLastEdited()
    {
        LastEdited = DateTime.UtcNow;
    }

    public void SetFinished(bool isFinished)
    {
        IsFinished = isFinished;
    }
}