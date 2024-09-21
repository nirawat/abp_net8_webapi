using Microsoft.AspNetCore.Builder;
using Net8;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<Net8WebTestModule>();

public partial class Program
{
}
