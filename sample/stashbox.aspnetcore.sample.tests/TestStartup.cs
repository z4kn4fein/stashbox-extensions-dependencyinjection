namespace Stashbox.AspNetCore.Sample.Tests
{
    public class TestStartup : Startup
    {
        public override void ConfigureContainer(IStashboxContainer container)
        {
            // emtpy to let StashMoq mock out the services
        }
    }
}
