using Mamey.CQRS.Commands;
using Mamey.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Policies;

public static class Extensions
{
    public static IMameyBuilder AddPolicyHandlers(this IMameyBuilder builder)
    {
        builder.Services.Scan(s =>
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IPolicyHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        return builder;
    }

    public static IMameyBuilder AddInMemoryPolicyDispatcher(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<IPolicyDispatcher, PolicyEvaluatorDispatcher>();
        return builder;
    }
    public static IApplicationBuilder UsePolicies(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<PolicyEnforcementMiddleware>();
        return builder;
    }
}


public interface IPolicyMapper
{
    IPolicy? Map(IPolicy policy);
    IEnumerable<IPolicy?> MapAll(IEnumerable<IPolicy> policies);
}
public interface IPolicy
{

}
public interface IPolicy<T> : IPolicy where T : ICommand
{
    
}
public interface IPolicy<T1, T2> : IPolicy<T1>
    where T1: ICommand
    where T2: ICommand
{

}
public interface IPolicy<T1, T2, T3> : IPolicy<T1, T2>
    where T1 : ICommand
    where T2 : ICommand
    where T3 : ICommand
{

}

public abstract class Policy<T> : IPolicy<T> where T : ICommand
{
    private readonly List<IPolicy> _policies = new();
    protected void AddPolicy(IPolicy policy)
    {
        if(_policies.Any(c => c == policy))
        {
            return;
        }
        
        _policies.Add(policy);
    }

    public void ClearEvents() => _policies.Clear();
}