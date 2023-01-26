using Microsoft.AspNetCore.Mvc;

namespace TestApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddSingleton<IA, A>();

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}

[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IA testDependency;

    public TestController(IA testDependency)
    {
        this.testDependency = testDependency;
    }

    [HttpGet("value")]
    public string GetValue()
    {
        return this.testDependency.GetType().Name;
    }
}

public interface IA { }

public class A : IA { }

public class B : IA { }

public class C : IA, IDisposable
{
    public bool Disposed { get; private set; }
    
    public void Dispose()
    {
        if (this.Disposed)
            throw new ObjectDisposedException(nameof(C));

        this.Disposed = true;
    }
}

public class D : IA, IAsyncDisposable
{
    public bool Disposed { get; private set; }

    public ValueTask DisposeAsync()
    {
        if (this.Disposed)
            throw new ObjectDisposedException(nameof(C));

        this.Disposed = true;

        return new ValueTask(Task.CompletedTask);
    }
}