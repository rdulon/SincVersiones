using Microsoft.Extensions.Hosting;

using RestApiCoreProgram = JumpsellerSync.RestApi.Core.Program<JumpsellerSync.RestApi.Provider.Linkstore.Startup>;

namespace JumpsellerSync.RestApi.Provider.Linkstore
{
    public sealed class Program : RestApiCoreProgram
    {
        public static void Main(string[] args)
            => Main(args, 5303);

        // Migrations framework searches for this method
        public static IHostBuilder CreateHostBuilder(string[] args)
            => RestApiCoreProgram.CreateHostBuilder(args);
    }
}
