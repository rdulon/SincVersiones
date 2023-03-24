using Microsoft.Extensions.Hosting;

using RestApiCoreProgram = JumpsellerSync.RestApi.Core.Program<JumpsellerSync.Frontend.RestApi.Startup>;

namespace JumpsellerSync.Frontend.RestApi
{
    public sealed class Program : RestApiCoreProgram
    {
        public static void Main(string[] args)
            => Main(args, 5301);

        // Migrations framework searches for this method
        public static IHostBuilder CreateHostBuilder(string[] args)
            => RestApiCoreProgram.CreateHostBuilder(args);
    }
}
