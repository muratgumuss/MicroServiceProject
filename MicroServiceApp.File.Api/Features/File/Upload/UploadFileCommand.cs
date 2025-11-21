using MicroServiceApp.Shared;

namespace MicroServiceApp.File.Api.Features.File.Upload
{
    public record UploadFileCommand(IFormFile File) : IRequestByServiceResult<UploadFileCommandResponse>;
}
