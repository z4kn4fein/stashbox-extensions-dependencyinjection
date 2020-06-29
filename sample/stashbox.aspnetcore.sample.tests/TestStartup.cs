namespace Stashbox.AspNetCore.Sample.Tests
{
    public class TestStartup : Startup
    {
        public override void ConfigureContainer(IStashboxContainer container)
        {
            // empty to let StashMoq mock out the services
        }
    }
}
