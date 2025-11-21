using MicroServiceApp.Shared;

namespace MicroServiceApp.File.Api.Features.File.Delete
{
    public record DeleteFileCommand(string FileName) : IRequestByServiceResult;
}
