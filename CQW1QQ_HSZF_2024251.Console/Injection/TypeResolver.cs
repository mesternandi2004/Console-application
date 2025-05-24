using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
 
namespace CQW1QQ_HSZF_2024251.Console.Injection
{
    public sealed class TypeResolver : ITypeResolver
    {
        private readonly IServiceProvider _provider;

        public TypeResolver(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object Resolve(Type type)
        {
            return _provider.GetRequiredService(type);
        }
    }
}