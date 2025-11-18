using MicroServiceApp.Catalog.Api;
using MicroServiceApp.Catalog.Api.Features.Categories;
using MicroServiceApp.Catalog.Api.Features.Courses;
using MicroServiceApp.Catalog.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptionsExt();
builder.Services.AddDatabaseServiceExt();
builder.Services.AddCommonServiceExt(typeof(CatalogAssembly));
builder.Services.AddVersioningExt();

var app = builder.Build();
app.AddSeedDataExt().ContinueWith(x =>
{
    Console.WriteLine(x.IsFaulted ? x.Exception?.Message : "Seed data has been saved successfully");
});

app.AddCategoryGroupEndpointExt(app.AddVersionSetExt());
app.AddCourseGroupEndpointExt(app.AddVersionSetExt());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();


/*
 1. Proje Yapýlandýrmasý ve Genel Mimari
Katmanlar ve Dosya Yapýsý
•	Features/: Her bir iþlevsel alan (ör. Categories, Courses) kendi altýnda CRUD ve sorgu iþlemlerini barýndýrýr. Her bir endpoint, kendi dosyasýnda ve genellikle CQRS (Command Query Responsibility Segregation) yaklaþýmýyla ayrýlmýþtýr.
•	Repositories/: Veri eriþim katmaný. Entity Framework Core ile DbContext üzerinden veri iþlemleri yapýlýr.
•	Shared/: Ortak kullanýlacak tipler, extension’lar ve utility’ler burada tutulur (ör. ServiceResult, Extensions).
•	Program.cs: Uygulamanýn giriþ noktasý ve dependency injection, middleware, endpoint registration iþlemleri burada yapýlýr.
Mimari Yaklaþým
•	Minimal API ve/veya Endpoint Routing kullanýmý (özellikle .NET 6+ ile gelen yeni yaklaþým).
•	CQRS: Komutlar (Command) ve sorgular (Query) ayrý handler’larda iþlenir.
•	MediatR: Request/response pattern ile kodun loosely coupled (gevþek baðlý) olmasýný saðlar.
•	Dependency Injection: Tüm servisler ve context DI container’a eklenir.
•	DTO Kullanýmý: Dýþarýya veri aktarýmý için DTO’lar (Data Transfer Object) kullanýlýr, entity’ler doðrudan dýþarýya açýlmaz.
---
2. Kullanýlan Kütüphaneler
•	MediatR: CQRS ve request/response pattern için.
•	Entity Framework Core: ORM olarak, veritabaný iþlemleri için.
•	Microsoft.AspNetCore.Mvc: API controller ve endpoint tanýmlarý için.
•	Refit: HTTP client abstraction (isteðe baðlý).
•	Swashbuckle/Swagger: API dokümantasyonu için.
•	System.Text.Json: JSON serialization/deserialization için (artýk varsayýlan).
 3. Kod Mantýðý ve Akýþ
Örnek: Kategori Listeleme Endpoint’i

•	Query ve Handler: Sorgu ve iþleyici ayrý. Handler, DbContext ile veritabanýndan veriyi çeker, DTO’ya map’ler ve ServiceResult ile döner.
•	ServiceResult: Baþarý/baþarýsýzlýk durumunu ve hata detaylarýný standartlaþtýrýr.
Endpoint Tanýmý (Minimal API)
app.MapGet("/api/v1/categories", async (IMediator mediator) =>
    (await mediator.Send(new GetAllCategoriesQuery())).ToGenericResult())
    .WithName("GetAllCategory")
    .RequireAuthorization("ClientCredential");
•	Minimal API: Controller yerine doðrudan endpoint tanýmý.
•	IMediator: MediatR ile handler’a yönlendirme.

4. .NET 5’ten .NET 8/9’a Geçiþte Önemli Farklar ve Best Practice’ler
Önceki .NET Mimarisinde (Örneðin .NET 5)
•	Startup.cs ve Program.cs ayrýydý, konfigürasyonlar Startup’ta yapýlýrdý.
•	Controller tabanlý API’ler yaygýndý.
•	Endpoint routing ve minimal API yoktu.
•	System.Text.Json yerine bazen Newtonsoft.Json kullanýlýrdý.
•	DI ve middleware pipeline’ý daha klasik þekilde tanýmlanýrdý.
Güncel .NET 8/9 Mimarisinde
•	Minimal API: Daha az kod, daha hýzlý baþlatma, fonksiyonel endpoint tanýmý.
•	Tek Program.cs: Tüm konfigürasyonlar burada.
•	Record ve init-only property: Immutable DTO’lar için.
•	Top-level statements: Daha sade giriþ noktasý.
•	Endpoint-based Authorization: Her endpoint’e özel yetkilendirme.
•	System.Text.Json: Varsayýlan ve performanslý JSON iþlemleri.
•	BackgroundService ve HostedService: Arka plan iþleri için.
•	Native AOT (Ahead-of-Time Compilation): Daha hýzlý ve küçük deployment’lar için (isteðe baðlý).
Best Practice’ler
•	CQRS ve MediatR ile kodun sorumluluklarýný ayýrmak.
•	DTO kullanýmý: Entity’leri dýþarýya açmamak.
•	ServiceResult gibi standart response objeleri ile hata yönetimini merkezi yapmak.
•	Dependency Injection’ý her yerde kullanmak.
•	Minimal API ile sade ve okunabilir endpoint’ler yazmak.
•	Extension method’lar ile endpoint registration ve konfigürasyonlarý modülerleþtirmek.
•	.gitignore, .gitattributes gibi dosyalarla temiz bir repo yönetimi saðlamak.
•	Swagger/OpenAPI ile API’yi dokümante etmek.
---
5. Özetle Mantýk
•	Her iþlevsel alan (feature) kendi klasöründe, kendi endpoint ve handler’larý ile ayrýlmýþtýr.
•	Veri eriþimi, iþ mantýðý ve API katmaný ayrýdýr.
•	Modern .NET ile sade, okunabilir, test edilebilir ve geniþletilebilir bir yapý hedeflenmiþtir.
•	Kütüphaneler ve mimari desenler, kodun sürdürülebilirliðini ve bakýmýný kolaylaþtýrýr.

 */

/*
 Projenizde MediatR ve CQRS deseninin nasýl kullanýldýðýný, kod üzerinden adým adým açýklýyorum:
1. CQRS ve MediatR Nedir?
•	CQRS (Command Query Responsibility Segregation):
Komutlar (yazma iþlemleri) ve sorgular (okuma iþlemleri) ayrý handler’larda iþlenir.
•	MediatR:
Komut ve sorgularý merkezi bir “aracý” (mediator) üzerinden handler’lara yönlendirir. Controller veya endpoint,
doðrudan iþ mantýðýna eriþmez, MediatR’a bir istek gönderir.
 
2. Kod Üzerinden Kullaným
A. Command (Komut) ve Handler
1. Command Tanýmý
public record CreateCourseCommand : IRequestByServiceResult<Guid>
{
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
}

•	CreateCourseCommand bir kurs oluþturmak için gerekli verileri taþýr.
•	IRequestByServiceResult<Guid> arayüzü, MediatR ile handler’a yönlendirilmesini saðlar ve dönüþ tipinin ServiceResult<Guid> 
olacaðýný belirtir.


2. Command Handler (Ýþleyici)
Handler dosyanýzda (ör: CreateCourseCommandHandler.cs) þöyle bir yapý olur:
public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    private readonly AppDbContext _context;
    public CreateCourseCommandHandler(AppDbContext context) => _context = context;

    public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
    {
        // Yeni kurs oluþturma iþlemi
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
•	Handler, komutu iþler ve sonucu döner.

B. Query (Sorgu) ve Handler
1. Query Tanýmý
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

C. Endpoint’te MediatR Kullanýmý
1. Minimal API veya Endpoint Extension

group.MapPost("/",
    async (CreateCourseCommand command, IMediator mediator) =>
        (await mediator.Send(command)).ToGenericResult())
    .WithName("CreateCourse")
    // ... diðer ayarlar

•	Burada endpoint, doðrudan handler’ý çaðýrmaz.
•	mediator.Send(command) ile komut MediatR’a iletilir, ilgili handler çalýþýr.
2. Query için de benzer:
group.MapGet("/",
    async (IMediator mediator) =>
        (await mediator.Send(new GetAllCategoriesQuery())).ToGenericResult())
    .WithName("GetAllCategory")

3. Özet Akýþ
1.	API’ye istek gelir (ör: POST /api/v1/courses).
2.	Endpoint, gelen veriyi bir komut (Command) nesnesine dönüþtürür.
3.	MediatR, bu komutu ilgili handler’a yönlendirir.
4.	Handler, iþ mantýðýný çalýþtýrýr ve sonucu döner.
5.	Sonuç, endpoint’te HTTP response olarak döner.

4. Modern .NET ve Best Practice
•	Controller yerine Minimal API/Endpoint Extension kullanýmý ile sade ve okunabilir kod.
•	CQRS ile okuma ve yazma iþlemlerinin ayrýlmasý.
•	MediatR ile gevþek baðlýlýk (loose coupling) ve test edilebilirlik.
•	ServiceResult ile standart hata ve baþarý yönetimi.


 */

/*
 •	ServiceResult ve ServiceResult<T> sizin projenizde oluþturduðunuz, API yanýtlarýný standartlaþtýrmak için kullanýlan sýnýflardýr.
•	IRequest<TResponse> ise MediatR kütüphanesinin bir arayüzüdür ve CQRS deseninde bir "istek" (request) nesnesini tanýmlar. Handler’lar bu isteklere yanýt verir.
Nasýl Kullanýlýyor?
public class CreateCourseCommand : IRequest<ServiceResult<Guid>>
{
    // Komutun property'leri
}
Burada:
•	CreateCourseCommand bir MediatR isteðidir (IRequest<T>).
•	T olarak sizin oluþturduðunuz ServiceResult<Guid> kullanýlýr.
•	Yani, handler bu komutu iþlediðinde bir ServiceResult<Guid> döne

Özet
•	ServiceResult = Size ait, API yanýtlarýný sarmalayan sýnýf.
•	IRequest<T> = MediatR arayüzü, CQRS pattern’inde kullanýlýr.
•	IRequest<ServiceResult<T>> = MediatR ile handler’dan standart bir sonuç döndürmek için kullanýlýr.
Bu sayede hem MediatR’ýn esnekliðinden hem de kendi hata/baþarý yönetimi standartlarýnýzdan faydalanýrsýnýz.

 */

/*
 CQRS (Command Query Responsibility Segregation) pattern’ini projenizde þu þekilde ve þu yerlerde kullanýyorsunuz:
1. CQRS Nedir?
•	Command: Sistemde bir deðiþiklik (ekleme, güncelleme, silme) yapan iþlemler.
•	Query: Sadece veri okuyan, sistemde deðiþiklik yapmayan iþlemler.
•	Amaç: Okuma ve yazma iþlemlerini ayrý handler’larda yönetmek, kodun sorumluluklarýný ayýrmak ve daha kolay test edilebilir, bakýmý kolay bir yapý kurmak.

2. Kodda CQRS Kullanýmý
A. Command (Yazma Ýþlemleri)
Örnek: Yeni bir kurs eklemek için

1. Command Sýnýfý
public record CreateCourseCommand(string Name, string Description, decimal Price, Guid CategoryId)
    : IRequestByServiceResult<Guid>;

•	Bu sýnýf, kurs ekleme isteðini temsil eder.
•	IRequestByServiceResult<Guid> arayüzü ile MediatR’a, handler’ýn bir ServiceResult<Guid> döneceðini bildirir.

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

•	Command handler, komutu iþler ve sonucu döner.

B. Query (Okuma Ýþlemleri)
Örnek: Tüm kategorileri listelemek için
1. Query Sýnýfý

public class GetAllCategoriesQuery : IRequestByServiceResult<List<CategoryDto>>;

•	Sadece veri okuma iþlemi için kullanýlýr.
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

C. Endpoint’te Kullanýmý
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

•	Burada endpoint, doðrudan iþ mantýðýna eriþmez, MediatR’a komut veya sorgu gönderir.
•	MediatR, ilgili handler’ý bulup çalýþtýrýr.

3. Özet
•	CQRS patterni, projenizde komut (yazma) ve sorgu (okuma) iþlemlerini ayrý sýnýflar ve handler’lar ile yöneterek uygulanýyor.
•	Her bir iþlem için ayrý bir command/query ve handler var.
•	Endpoint’ler, MediatR üzerinden bu handler’lara ulaþýr.
•	Bu yapý, kodun okunabilirliðini, test edilebilirliðini ve sürdürülebilirliðini artýrýr.
Daha fazla örnek veya belirli bir command/query kodunu görmek isterseniz, dosya adýný belirtmeniz yeterli!


 */

/*
 Dependency Injection (DI) iþlemi, .NET 6 ve sonrasýnda genellikle Program.cs dosyasýnda gerçekleþir. Projenizde de baðýmlýlýklarýn (örneðin, MediatR, DbContext, repository’ler, filtreler) DI container’a eklenmesi burada yapýlýr.

Nasýl Çalýþýr?
1.	Servislerin Eklenmesi (Registration)
•	builder.Services.AddXyz() þeklinde servisler DI container’a eklenir.
•	Örneðin:
•	AddMediatR(...) ile MediatR handler’larý
•	AddDbContext<AppDbContext>(...) ile Entity Framework context’i
•	Kendi servisleriniz veya filtreleriniz
2.	Kullanýmý (Injection)
•	Controller, handler veya endpoint’lerde constructor veya parametre ile otomatik olarak ilgili servis DI tarafýndan verilir.

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

// Diðer servisler...

Kullaným Örneði

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    private readonly AppDbContext _context;
    public CreateCourseCommandHandler(AppDbContext context) => _context = context;
    // ...
}

Burada AppDbContext otomatik olarak DI tarafýndan verilir.

Minimal API endpoint’te:
group.MapPost("/",
    async (CreateCourseCommand command, IMediator mediator) =>
        (await mediator.Send(command)).ToGenericResult())

Burada da IMediator parametresi DI ile otomatik olarak saðlanýr.

Özet:
Dependency Injection iþlemi, Program.cs dosyasýnda builder.Services.Add... ile servislerin eklenmesiyle baþlar. Sonrasýnda bu servisler, ihtiyaç duyulan yerde otomatik olarak constructor veya parametre ile kullanýma sunulur. Bu, kodun test edilebilirliðini ve sürdürülebilirliðini artýrýr.

 */

/*
 Minimal API yerine klasik Controller yapýsý kullansaydýnýz, projenizde aþaðýdaki deðiþiklikler olurdu:
1. Endpoint Tanýmlarý

Minimal API (Þu anki yapý)
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

•	Controller sýnýfý oluþturulur.
•	Endpoint’ler [HttpGet], [HttpPost] gibi attribute’larla iþaretlenir.
•	Dependency injection constructor üzerinden yapýlýr.

2. Dependency Injection ve Program.cs
•	Minimal API’de endpoint’ler doðrudan app.MapX ile tanýmlanýr.
•	Controller yapýsýnda ise:
•	builder.Services.AddControllers(); eklenir.
•	app.MapControllers(); ile controller’lar route edilir.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMediatR(...);
// diðer servisler

var app = builder.Build();
app.MapControllers();


3. Validation, Filter ve Response Handling
•	Minimal API’de endpoint’e doðrudan filter eklenebilir.
•	Controller’da ise global veya attribute tabanlý filter’lar ([ServiceFilter], [TypeFilter], [ValidateModel] gibi) kullanýlýr.

4. Swagger ve OpenAPI
•	Her iki yapýda da Swagger/OpenAPI desteði vardýr.
•	Controller yapýsýnda action’lar otomatik olarak Swagger’a eklenir

5. Best Practice ve Farklar
Minimal API	                            Controller Yapýsý
Daha az kod, hýzlý prototipleme	        Büyük projelerde daha okunabilir
Fonksiyonel, parametre ile DI	        Sýnýf tabanlý, constructor ile DI
Özellikle küçük servisler için iyi	    Büyük, modüler projeler için ideal
Endpoint baþýna filter eklenebilir	    Global/attribute filter desteði

6. Özetle Deðiþecek Yerler
•	Endpoint tanýmlarý controller sýnýflarýna taþýnýr.
•	Program.cs’de AddControllers ve MapControllers eklenir.
•	DI constructor üzerinden yapýlýr.
•	Validation ve filter’lar attribute veya global olarak uygulanýr.


Sonuç:
Controller yapýsý, özellikle büyük ve modüler projelerde okunabilirlik, test edilebilirlik ve geniþletilebilirlik açýsýndan avantaj saðlar. Minimal API ise daha sade ve hýzlý geliþtirme için uygundur. Her iki yapýda da MediatR ve CQRS ayný þekilde kullanýlabilir; sadece endpoint tanýmlama ve DI yöntemi deðiþir.


 */

/*
 Aþaðýda, minimal API yerine klasik Controller yapýsý ile bir örnek (örneðin: Course ekleme) gösteriyorum.
Ayrýca dependency injection, filter, program.cs ve response handling gibi önemli noktalarý da ekliyorum.

1. Controller Sýnýfý

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

3. Filter Kullanýmý (Örnek: Validation Filter)

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

Controller'da kullanýmý:

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
// Diðer servisler...

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();

5. Özet
•	Controller: [ApiController] ve [Route] ile tanýmlanýr, action metodunda DI ile IMediator kullanýlýr.
•	Action: [HttpPost] ile iþaretlenir, parametre [FromBody] ile alýnýr.
•	CQRS: Command ve handler ayrý dosyalarda, MediatR ile baðlanýr.
•	Filter: Validation gibi iþlemler için attribute veya global olarak eklenir.
•	Program.cs: Tüm servisler ve filter’lar DI container’a eklenir, AddControllers ve MapControllers ile yapý tamamlanýr.




 */