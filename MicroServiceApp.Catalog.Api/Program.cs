using MicroServiceApp.Bus;
using MicroServiceApp.Catalog.Api;
using MicroServiceApp.Catalog.Api.Features.Categories;
using MicroServiceApp.Catalog.Api.Features.Courses;
using MicroServiceApp.Catalog.Api.Options;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptionsExt();
builder.Services.AddDatabaseServiceExt();
builder.Services.AddCommonServiceExt(typeof(CatalogAssembly));
builder.Services.AddVersioningExt();
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
builder.Services.AddMasstransitExt(builder.Configuration);
//builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.MapDefaultEndpoints();
app.AddSeedDataExt().ContinueWith(x =>
{
    Console.WriteLine(x.IsFaulted ? x.Exception?.Message : "Seed data has been saved successfully");
});

app.AddCategoryGroupEndpointExt(app.AddVersionSetExt());
app.AddCourseGroupEndpointExt(app.AddVersionSetExt());
app.UseExceptionHandler(x => { });
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
// Program.cs veya Startup.cs'de
/*
app.Use(async (context, next) =>
{
    // 1. Gelen isteği logla
    Console.WriteLine($"\n=== REQUEST START: {context.Request.Path} ===");

    // 2. Auth header'ı kontrol et
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine($"🔐 Auth Header Present: Yes");
        Console.WriteLine($"🔐 Starts with Bearer: {authHeader.StartsWith("Bearer ")}");

        if (authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            Console.WriteLine($"🔐 Token Length: {token.Length} chars");

            try
            {
                // Token'ı manuel parse et
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                Console.WriteLine($"🔐 Token Issuer: {jwtToken.Issuer}");
                Console.WriteLine($"🔐 Token Audiences: {string.Join(", ", jwtToken.Audiences)}");
                Console.WriteLine($"🔐 Token Expires: {jwtToken.ValidTo}");
                Console.WriteLine($"🔐 Has 'sub' claim: {jwtToken.Claims.Any(c => c.Type == "sub")}");

                var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
                if (subClaim != null)
                {
                    Console.WriteLine($"🔐 Sub Claim Value: {subClaim.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔐 Token Parse Error: {ex.Message}");
            }
        }
    }
    else
    {
        Console.WriteLine($"🔐 Auth Header Present: No");
    }

    await next();

    // 3. Authentication sonrası durumu kontrol et
    Console.WriteLine($"\n=== AFTER AUTH MIDDLEWARE ===");
    Console.WriteLine($"👤 User Identity: {context.User?.Identity?.Name}");
    Console.WriteLine($"✅ IsAuthenticated: {context.User?.Identity?.IsAuthenticated}");
    Console.WriteLine($"🔧 AuthenticationType: {context.User?.Identity?.AuthenticationType}");

    if (context.User?.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine($"📋 Total Claims: {context.User.Claims.Count()}");
        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($"   {claim.Type} = {claim.Value}");
        }
    }
    else
    {
        Console.WriteLine($"❌ NOT AUTHENTICATED");

        // Response status code'u kontrol et
        Console.WriteLine($"📊 Response Status: {context.Response.StatusCode}");
    }

    Console.WriteLine($"=== REQUEST END ===\n");
});
*/
app.Run();


/*
 1. Proje Yapılandırması ve Genel Mimari
Katmanlar ve Dosya Yapısı
•	Features/: Her bir işlevsel alan (ör. Categories, Courses) kendi altında CRUD ve sorgu işlemlerini barındırır. Her bir endpoint, kendi dosyasında ve genellikle CQRS (Command Query Responsibility Segregation) yaklaşımıyla ayrılmıştır.
•	Repositories/: Veri erişim katmanı. Entity Framework Core ile DbContext üzerinden veri işlemleri yapılır.
•	Shared/: Ortak kullanılacak tipler, extension’lar ve utility’ler burada tutulur (ör. ServiceResult, Extensions).
•	Program.cs: Uygulamanın giriş noktası ve dependency injection, middleware, endpoint registration işlemleri burada yapılır.
Mimari Yaklaşım
•	Minimal API ve/veya Endpoint Routing kullanımı (özellikle .NET 6+ ile gelen yeni yaklaşım).
•	CQRS: Komutlar (Command) ve sorgular (Query) ayrı handler’larda işlenir.
•	MediatR: Request/response pattern ile kodun loosely coupled (gevşek bağlı) olmasını sağlar.
•	Dependency Injection: Tüm servisler ve context DI container’a eklenir.
•	DTO Kullanımı: Dışarıya veri aktarımı için DTO’lar (Data Transfer Object) kullanılır, entity’ler doğrudan dışarıya açılmaz.
---
2. Kullanılan Kütüphaneler
•	MediatR: CQRS ve request/response pattern için.
•	Entity Framework Core: ORM olarak, veritabanı işlemleri için.
•	Microsoft.AspNetCore.Mvc: API controller ve endpoint tanımları için.
•	Refit: HTTP client abstraction (isteğe bağlı).
•	Swashbuckle/Swagger: API dokümantasyonu için.
•	System.Text.Json: JSON serialization/deserialization için (artık varsayılan).
 3. Kod Mantığı ve Akış
Örnek: Kategori Listeleme Endpoint’i

•	Query ve Handler: Sorgu ve işleyici ayrı. Handler, DbContext ile veritabanından veriyi çeker, DTO’ya map’ler ve ServiceResult ile döner.
•	ServiceResult: Başarı/başarısızlık durumunu ve hata detaylarını standartlaştırır.
Endpoint Tanımı (Minimal API)
app.MapGet("/api/v1/categories", async (IMediator mediator) =>
    (await mediator.Send(new GetAllCategoriesQuery())).ToGenericResult())
    .WithName("GetAllCategory")
    .RequireAuthorization("ClientCredential");
•	Minimal API: Controller yerine doğrudan endpoint tanımı.
•	IMediator: MediatR ile handler’a yönlendirme.

4. .NET 5’ten .NET 8/9’a Geçişte Önemli Farklar ve Best Practice’ler
Önceki .NET Mimarisinde (Örneğin .NET 5)
•	Startup.cs ve Program.cs ayrıydı, konfigürasyonlar Startup’ta yapılırdı.
•	Controller tabanlı API’ler yaygındı.
•	Endpoint routing ve minimal API yoktu.
•	System.Text.Json yerine bazen Newtonsoft.Json kullanılırdı.
•	DI ve middleware pipeline’ı daha klasik şekilde tanımlanırdı.
Güncel .NET 8/9 Mimarisinde
•	Minimal API: Daha az kod, daha hızlı başlatma, fonksiyonel endpoint tanımı.
•	Tek Program.cs: Tüm konfigürasyonlar burada.
•	Record ve init-only property: Immutable DTO’lar için.
•	Top-level statements: Daha sade giriş noktası.
•	Endpoint-based Authorization: Her endpoint’e özel yetkilendirme.
•	System.Text.Json: Varsayılan ve performanslı JSON işlemleri.
•	BackgroundService ve HostedService: Arka plan işleri için.
•	Native AOT (Ahead-of-Time Compilation): Daha hızlı ve küçük deployment’lar için (isteğe bağlı).
Best Practice’ler
•	CQRS ve MediatR ile kodun sorumluluklarını ayırmak.
•	DTO kullanımı: Entity’leri dışarıya açmamak.
•	ServiceResult gibi standart response objeleri ile hata yönetimini merkezi yapmak.
•	Dependency Injection’ı her yerde kullanmak.
•	Minimal API ile sade ve okunabilir endpoint’ler yazmak.
•	Extension method’lar ile endpoint registration ve konfigürasyonları modülerleştirmek.
•	.gitignore, .gitattributes gibi dosyalarla temiz bir repo yönetimi sağlamak.
•	Swagger/OpenAPI ile API’yi dokümante etmek.
---
5. Özetle Mantık
•	Her işlevsel alan (feature) kendi klasöründe, kendi endpoint ve handler’ları ile ayrılmıştır.
•	Veri erişimi, iş mantığı ve API katmanı ayrıdır.
•	Modern .NET ile sade, okunabilir, test edilebilir ve genişletilebilir bir yapı hedeflenmiştir.
•	Kütüphaneler ve mimari desenler, kodun sürdürülebilirliğini ve bakımını kolaylaştırır.

 */

/*
 Projenizde MediatR ve CQRS deseninin nasıl kullanıldığını, kod üzerinden adım adım açıklıyorum:
1. CQRS ve MediatR Nedir?
•	CQRS (Command Query Responsibility Segregation):
Komutlar (yazma işlemleri) ve sorgular (okuma işlemleri) ayrı handler’larda işlenir.
•	MediatR:
Komut ve sorguları merkezi bir “aracı” (mediator) üzerinden handler’lara yönlendirir. Controller veya endpoint,
doğrudan iş mantığına erişmez, MediatR’a bir istek gönderir.
 
2. Kod Üzerinden Kullanım
A. Command (Komut) ve Handler
1. Command Tanımı
public record CreateCourseCommand : IRequestByServiceResult<Guid>
{
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
}

•	CreateCourseCommand bir kurs oluşturmak için gerekli verileri taşır.
•	IRequestByServiceResult<Guid> arayüzü, MediatR ile handler’a yönlendirilmesini sağlar ve dönüş tipinin ServiceResult<Guid> 
olacağını belirtir.


2. Command Handler (İşleyici)
Handler dosyanızda (ör: CreateCourseCommandHandler.cs) şöyle bir yapı olur:
public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    private readonly AppDbContext _context;
    public CreateCourseCommandHandler(AppDbContext context) => _context = context;

    public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
    {
        // Yeni kurs oluşturma işlemi
        var course = new Course
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            CategoryId = command.CategoryId
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.SuccessAsCreated(course.Id, $"/api/v1/courses/{course.Id}");
    }
}
•	Handler, komutu işler ve sonucu döner.

B. Query (Sorgu) ve Handler
1. Query Tanımı
public class GetAllCategoriesQuery : IRequest<ServiceResult<List<CategoryDto>>> { }
•	Tüm kategorileri listelemek için bir sorgu nesnesi.

2. Query Handler

public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoriesQuery, ServiceResult<List<CategoryDto>>>
{
    private readonly AppDbContext _context;
    public GetAllCategoryQueryHandler(AppDbContext context) => _context = context;

    public async Task<ServiceResult<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories.ToListAsync(cancellationToken);
        var dtos = categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
        return ServiceResult<List<CategoryDto>>.SuccessAsOk(dtos);
    }
}

C. Endpoint’te MediatR Kullanımı
1. Minimal API veya Endpoint Extension

group.MapPost("/",
    async (CreateCourseCommand command, IMediator mediator) =>
        (await mediator.Send(command)).ToGenericResult())
    .WithName("CreateCourse")
    // ... diğer ayarlar

•	Burada endpoint, doğrudan handler’ı çağırmaz.
•	mediator.Send(command) ile komut MediatR’a iletilir, ilgili handler çalışır.
2. Query için de benzer:
group.MapGet("/",
    async (IMediator mediator) =>
        (await mediator.Send(new GetAllCategoriesQuery())).ToGenericResult())
    .WithName("GetAllCategory")

3. Özet Akış
1.	API’ye istek gelir (ör: POST /api/v1/courses).
2.	Endpoint, gelen veriyi bir komut (Command) nesnesine dönüştürür.
3.	MediatR, bu komutu ilgili handler’a yönlendirir.
4.	Handler, iş mantığını çalıştırır ve sonucu döner.
5.	Sonuç, endpoint’te HTTP response olarak döner.

4. Modern .NET ve Best Practice
•	Controller yerine Minimal API/Endpoint Extension kullanımı ile sade ve okunabilir kod.
•	CQRS ile okuma ve yazma işlemlerinin ayrılması.
•	MediatR ile gevşek bağlılık (loose coupling) ve test edilebilirlik.
•	ServiceResult ile standart hata ve başarı yönetimi.


 */

/*
 •	ServiceResult ve ServiceResult<T> sizin projenizde oluşturduğunuz, API yanıtlarını standartlaştırmak için kullanılan sınıflardır.
•	IRequest<TResponse> ise MediatR kütüphanesinin bir arayüzüdür ve CQRS deseninde bir "istek" (request) nesnesini tanımlar. Handler’lar bu isteklere yanıt verir.
Nasıl Kullanılıyor?
public class CreateCourseCommand : IRequest<ServiceResult<Guid>>
{
    // Komutun property'leri
}
Burada:
•	CreateCourseCommand bir MediatR isteğidir (IRequest<T>).
•	T olarak sizin oluşturduğunuz ServiceResult<Guid> kullanılır.
•	Yani, handler bu komutu işlediğinde bir ServiceResult<Guid> döne

Özet
•	ServiceResult = Size ait, API yanıtlarını sarmalayan sınıf.
•	IRequest<T> = MediatR arayüzü, CQRS pattern’inde kullanılır.
•	IRequest<ServiceResult<T>> = MediatR ile handler’dan standart bir sonuç döndürmek için kullanılır.
Bu sayede hem MediatR’ın esnekliğinden hem de kendi hata/başarı yönetimi standartlarınızdan faydalanırsınız.

 */

/*
 CQRS (Command Query Responsibility Segregation) pattern’ini projenizde şu şekilde ve şu yerlerde kullanıyorsunuz:
1. CQRS Nedir?
•	Command: Sistemde bir değişiklik (ekleme, güncelleme, silme) yapan işlemler.
•	Query: Sadece veri okuyan, sistemde değişiklik yapmayan işlemler.
•	Amaç: Okuma ve yazma işlemlerini ayrı handler’larda yönetmek, kodun sorumluluklarını ayırmak ve daha kolay test edilebilir, bakımı kolay bir yapı kurmak.

2. Kodda CQRS Kullanımı
A. Command (Yazma İşlemleri)
Örnek: Yeni bir kurs eklemek için

1. Command Sınıfı
public record CreateCourseCommand(string Name, string Description, decimal Price, Guid CategoryId)
    : IRequestByServiceResult<Guid>;

•	Bu sınıf, kurs ekleme isteğini temsil eder.
•	IRequestByServiceResult<Guid> arayüzü ile MediatR’a, handler’ın bir ServiceResult<Guid> döneceğini bildirir.

2. Command Handler

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    private readonly AppDbContext _context;
    public CreateCourseCommandHandler(AppDbContext context) => _context = context;

    public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            CategoryId = command.CategoryId
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.SuccessAsCreated(course.Id, $"/api/v1/courses/{course.Id}");
    }
}

•	Command handler, komutu işler ve sonucu döner.

B. Query (Okuma İşlemleri)
Örnek: Tüm kategorileri listelemek için
1. Query Sınıfı

public class GetAllCategoriesQuery : IRequestByServiceResult<List<CategoryDto>>;

•	Sadece veri okuma işlemi için kullanılır.
2. Query Handler

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, ServiceResult<List<CategoryDto>>>
{
    private readonly AppDbContext _context;
    public GetAllCategoriesQueryHandler(AppDbContext context) => _context = context;

    public async Task<ServiceResult<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories.ToListAsync(cancellationToken);
        var dtos = categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
        return ServiceResult<List<CategoryDto>>.SuccessAsOk(dtos);
    }
}

C. Endpoint’te Kullanımı
Minimal API veya endpoint extension’da:

group.MapPost("/",
    async (CreateCourseCommand command, IMediator mediator) =>
        (await mediator.Send(command)).ToGenericResult())
    .WithName("CreateCourse");

veya

group.MapGet("/",
    async (IMediator mediator) =>
        (await mediator.Send(new GetAllCategoriesQuery())).ToGenericResult())
    .WithName("GetAllCategory");

•	Burada endpoint, doğrudan iş mantığına erişmez, MediatR’a komut veya sorgu gönderir.
•	MediatR, ilgili handler’ı bulup çalıştırır.

3. Özet
•	CQRS patterni, projenizde komut (yazma) ve sorgu (okuma) işlemlerini ayrı sınıflar ve handler’lar ile yöneterek uygulanıyor.
•	Her bir işlem için ayrı bir command/query ve handler var.
•	Endpoint’ler, MediatR üzerinden bu handler’lara ulaşır.
•	Bu yapı, kodun okunabilirliğini, test edilebilirliğini ve sürdürülebilirliğini artırır.
Daha fazla örnek veya belirli bir command/query kodunu görmek isterseniz, dosya adını belirtmeniz yeterli!


 */

/*
 Dependency Injection (DI) işlemi, .NET 6 ve sonrasında genellikle Program.cs dosyasında gerçekleşir. Projenizde de bağımlılıkların (örneğin, MediatR, DbContext, repository’ler, filtreler) DI container’a eklenmesi burada yapılır.

Nasıl Çalışır?
1.	Servislerin Eklenmesi (Registration)
•	builder.Services.AddXyz() şeklinde servisler DI container’a eklenir.
•	Örneğin:
•	AddMediatR(...) ile MediatR handler’ları
•	AddDbContext<AppDbContext>(...) ile Entity Framework context’i
•	Kendi servisleriniz veya filtreleriniz
2.	Kullanımı (Injection)
•	Controller, handler veya endpoint’lerde constructor veya parametre ile otomatik olarak ilgili servis DI tarafından verilir.

Örnek: Program.cs’de DI

var builder = WebApplication.CreateBuilder(args);

// MediatR ekleniyor
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// DbContext ekleniyor
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kendi servisleriniz, filtreleriniz vs.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// Diğer servisler...

Kullanım Örneği

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    private readonly AppDbContext _context;
    public CreateCourseCommandHandler(AppDbContext context) => _context = context;
    // ...
}

Burada AppDbContext otomatik olarak DI tarafından verilir.

Minimal API endpoint’te:
group.MapPost("/",
    async (CreateCourseCommand command, IMediator mediator) =>
        (await mediator.Send(command)).ToGenericResult())

Burada da IMediator parametresi DI ile otomatik olarak sağlanır.

Özet:
Dependency Injection işlemi, Program.cs dosyasında builder.Services.Add... ile servislerin eklenmesiyle başlar. Sonrasında bu servisler, ihtiyaç duyulan yerde otomatik olarak constructor veya parametre ile kullanıma sunulur. Bu, kodun test edilebilirliğini ve sürdürülebilirliğini artırır.

 */

/*
 Minimal API yerine klasik Controller yapısı kullansaydınız, projenizde aşağıdaki değişiklikler olurdu:
1. Endpoint Tanımları

Minimal API (Şu anki yapı)
app.MapPost("/api/v1/courses", async (CreateCourseCommand command, IMediator mediator) =>
    (await mediator.Send(command)).ToGenericResult());

Controller ile

[ApiController]
[Route("api/v1/courses")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToGenericResult();
    }
}

•	Controller sınıfı oluşturulur.
•	Endpoint’ler [HttpGet], [HttpPost] gibi attribute’larla işaretlenir.
•	Dependency injection constructor üzerinden yapılır.

2. Dependency Injection ve Program.cs
•	Minimal API’de endpoint’ler doğrudan app.MapX ile tanımlanır.
•	Controller yapısında ise:
•	builder.Services.AddControllers(); eklenir.
•	app.MapControllers(); ile controller’lar route edilir.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMediatR(...);
// diğer servisler

var app = builder.Build();
app.MapControllers();


3. Validation, Filter ve Response Handling
•	Minimal API’de endpoint’e doğrudan filter eklenebilir.
•	Controller’da ise global veya attribute tabanlı filter’lar ([ServiceFilter], [TypeFilter], [ValidateModel] gibi) kullanılır.

4. Swagger ve OpenAPI
•	Her iki yapıda da Swagger/OpenAPI desteği vardır.
•	Controller yapısında action’lar otomatik olarak Swagger’a eklenir

5. Best Practice ve Farklar
Minimal API	                            Controller Yapısı
Daha az kod, hızlı prototipleme	        Büyük projelerde daha okunabilir
Fonksiyonel, parametre ile DI	        Sınıf tabanlı, constructor ile DI
Özellikle küçük servisler için iyi	    Büyük, modüler projeler için ideal
Endpoint başına filter eklenebilir	    Global/attribute filter desteği

6. Özetle Değişecek Yerler
•	Endpoint tanımları controller sınıflarına taşınır.
•	Program.cs’de AddControllers ve MapControllers eklenir.
•	DI constructor üzerinden yapılır.
•	Validation ve filter’lar attribute veya global olarak uygulanır.


Sonuç:
Controller yapısı, özellikle büyük ve modüler projelerde okunabilirlik, test edilebilirlik ve genişletilebilirlik açısından avantaj sağlar. Minimal API ise daha sade ve hızlı geliştirme için uygundur. Her iki yapıda da MediatR ve CQRS aynı şekilde kullanılabilir; sadece endpoint tanımlama ve DI yöntemi değişir.


 */

/*
 Aşağıda, minimal API yerine klasik Controller yapısı ile bir örnek (örneğin: Course ekleme) gösteriyorum.
Ayrıca dependency injection, filter, program.cs ve response handling gibi önemli noktaları da ekliyorum.

1. Controller Sınıfı

// MicroServiceApp.Catalog.Api/Controllers/CoursesController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroServiceApp.Shared;
using MicroServiceApp.Catalog.Api.Features.Courses.Create;

namespace MicroServiceApp.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToGenericResult();
        }
    }
}


2. Command ve Handler (CQRS + MediatR)
// MicroServiceApp.Catalog.Api/Features/Courses/Create/CreateCourseCommand.cs
using MicroServiceApp.Shared;

public record CreateCourseCommand(string Name, string Description, decimal Price, Guid CategoryId)
    : IRequestByServiceResult<Guid>;


// MicroServiceApp.Catalog.Api/Features/Courses/Create/CreateCourseCommandHandler.cs
using MediatR;
using MicroServiceApp.Shared;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    private readonly AppDbContext _context;
    public CreateCourseCommandHandler(AppDbContext context) => _context = context;

    public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            CategoryId = command.CategoryId
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.SuccessAsCreated(course.Id, $"/api/v1/courses/{course.Id}");
    }
}

3. Filter Kullanımı (Örnek: Validation Filter)

// MicroServiceApp.Catalog.Api/Filters/ValidationFilter.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

Controller'da kullanımı:

[ServiceFilter(typeof(ValidationFilter))]
public async Task<IActionResult> Create([FromBody] CreateCourseCommand command)
{
    // ...
}


4. Program.cs (DI ve Controller Registration)
// MicroServiceApp.Catalog.Api/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<ValidationFilter>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Diğer servisler...

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();

5. Özet
•	Controller: [ApiController] ve [Route] ile tanımlanır, action metodunda DI ile IMediator kullanılır.
•	Action: [HttpPost] ile işaretlenir, parametre [FromBody] ile alınır.
•	CQRS: Command ve handler ayrı dosyalarda, MediatR ile bağlanır.
•	Filter: Validation gibi işlemler için attribute veya global olarak eklenir.
•	Program.cs: Tüm servisler ve filter’lar DI container’a eklenir, AddControllers ve MapControllers ile yapı tamamlanır.




 */