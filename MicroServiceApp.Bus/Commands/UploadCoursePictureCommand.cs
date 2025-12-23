using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceApp.Bus.Commands
{
    public record UploadCoursePictureCommand(Guid courseId, byte[] picture, string FileName);
}
