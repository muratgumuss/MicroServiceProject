namespace MicroServiceApp.Shared.Services
{
    public class IdentityServiceFake : IIdentityService
    {
        public Guid UserId => Guid.Parse("64580000-5af6-3473-8d80-08de1bad0b1d");
        public string UserName => "Ahmet16";
        public List<string> Roles => [];
    }
}
