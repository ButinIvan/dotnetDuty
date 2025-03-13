using System.Text;
using Microsoft.AspNetCore.Mvc;
using dotnetWebApi.Extensions;
using dotnetWebApi.Models;
using dotnetWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Minio;
using Minio.DataModel.Args;

namespace dotnetWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly MinioService _s3Service;

    public DocumentsController(MinioService s3Service)
    {
        _s3Service = s3Service;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        var userId = this.GetUserId();
        
        await _s3Service.UploadDocumentAsync(userId, request.Content, "dotnet-duty-bucket");
        return Ok("Document is created");
    }
}