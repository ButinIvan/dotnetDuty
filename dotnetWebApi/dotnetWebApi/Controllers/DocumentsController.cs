using Microsoft.AspNetCore.Mvc;
using dotnetWebApi.Entities;
using dotnetWebApi.Extensions;
using dotnetWebApi.Models;
using dotnetWebApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace dotnetWebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly MinioService _s3Service;

    public DocumentsController(MinioService s3Service)
    {
        _s3Service = s3Service;
    }

    
}