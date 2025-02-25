using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnetWebApi.Entities;

public class Document
{
    public string? Title { get; set; }
    public string? OwnerId { get; set; }
    public string? Body { get; set; }
}