var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => new { Version = "1.0" });
app.MapGet("/cardstacks", () =>
{
    return new[] {
        new CardStack("Math", 15),
        new CardStack("Spanish", 40),
        new CardStack("English", 20),
    };
});

app.Run();

record CardStack(string Name, int Amount)
{

}