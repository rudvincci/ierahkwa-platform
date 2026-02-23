using Mamey.Types;
using Microsoft.Extensions.Hosting;

namespace Mamey;

public static class ConfigurationExtensions
{
    private const string SectionName = "app";

    public static IMameyBuilder AddMamey(this IServiceCollection services, string sectionName = SectionName,
        IConfiguration? configuration = null, IHostEnvironment? hostEnvironment = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var builder = MameyBuilder.Create(services, configuration);
        var options = builder.GetOptions<AppOptions>(sectionName);
        builder.Services.AddMemoryCache();
        services.AddSingleton(options);
        services.AddSingleton<IServiceId, ServiceId>();
        if (!options.DisplayBanner || string.IsNullOrWhiteSpace(options.Name))
        {
            return builder;
        }

        return builder;
    }

    public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);

        return options;
    }

    public static TModel GetOptions<TModel>(this IMameyBuilder builder, string settingsSectionName)
        where TModel : new()
    {
        if (builder.Configuration is not null)
        {
            return builder.Configuration.GetOptions<TModel>(settingsSectionName);
        }

        using var serviceProvider = builder.Services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<TModel>(settingsSectionName);
    }


    /// <summary>
    /// Execute's an async Task<T> method which has a void return value synchronously
    /// </summary>
    /// <param name="task">Task<T> method to execute</param>
    public static void RunSync(Func<Task> task)
    {
        var oldContext = SynchronizationContext.Current;
        var synch = new ExclusiveSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(synch);
        synch.Post(async _ =>
        {
            try
            {
                await task();
            }
            catch (Exception e)
            {
                synch.InnerException = e;
                throw;
            }
            finally
            {
                synch.EndMessageLoop();
            }
        }, null);
        synch.BeginMessageLoop();

        SynchronizationContext.SetSynchronizationContext(oldContext);
    }

    /// <summary>
    /// Execute's an async Task<T> method which has a T return type synchronously
    /// </summary>
    /// <typeparam name="T">Return Type</typeparam>
    /// <param name="task">Task<T> method to execute</param>
    /// <returns></returns>
    public static T RunSync<T>(Func<Task<T>> task)
    {
        var oldContext = SynchronizationContext.Current;
        var synch = new ExclusiveSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(synch);
        T ret = default(T);
        synch.Post(async _ =>
        {
            try
            {
                ret = await task();
            }
            catch (Exception e)
            {
                synch.InnerException = e;
                throw;
            }
            finally
            {
                synch.EndMessageLoop();
            }
        }, null);
        synch.BeginMessageLoop();
        SynchronizationContext.SetSynchronizationContext(oldContext);
        return ret;
    }

    private class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private bool done;
        public Exception InnerException { get; set; }
        readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
        readonly Queue<Tuple<SendOrPostCallback, object>> items =
            new Queue<Tuple<SendOrPostCallback, object>>();

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            lock (items)
            {
                items.Enqueue(Tuple.Create(d, state));
            }
            workItemsWaiting.Set();
        }

        public void EndMessageLoop()
        {
            Post(_ => done = true, null);
        }

        public void BeginMessageLoop()
        {
            while (!done)
            {
                Tuple<SendOrPostCallback, object> task = null;
                lock (items)
                {
                    if (items.Count > 0)
                    {
                        task = items.Dequeue();
                    }
                }
                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null) // the method threw an exeption
                    {
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                    }
                }
                else
                {
                    workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
        
    }


}
