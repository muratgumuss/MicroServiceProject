namespace MicroServiceApp.Catalog.Api.Features.Categories.Create
{
    public record CreateCategoryCommand(string Name) : IRequestByServiceResult<CreateCategoryResponse>;

    //public record CreateCategoryCommand1
    //{
    //    public string Name { get; init; }

    //    public CreateCategoryCommand1(string name)
    //    {
    //        Name = name;
    //        var createCategoryCommand = new CreateCategoryCommand1("test");
    //        createCategoryCommand.Name = "test";
    //    }

    //}
}
