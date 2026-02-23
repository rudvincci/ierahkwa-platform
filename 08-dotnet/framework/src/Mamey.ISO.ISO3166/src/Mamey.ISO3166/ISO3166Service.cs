using System.Collections.Concurrent;
using System.Linq;                       // LINQ ops
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;                 // SemaphoreSlim
using Mamey.ISO.Abstractions;
using Mamey.Persistence.Redis;          // ICache

namespace Mamey.ISO3166;

public class ISO3166Service : IISO3166Service
{
    private readonly ICache? _cache; // optional; works in-memory if null

    private readonly ConcurrentDictionary<string, ISOCountry> _countries =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ISOSubdivision> _subdivisions =
        new(StringComparer.OrdinalIgnoreCase);

    private const string CountriesCacheKey = "ISO3166:Countries";
    private const string SubdivisionsCacheKey = "ISO3166:Subdivisions";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromDays(30);

    private volatile bool _initialized;
    private readonly SemaphoreSlim _initGate = new(1, 1);

    public ISO3166Service() { }

    public ISO3166Service(ICache cache) => _cache = cache;

    public async Task InitializeAsync() => await EnsureInitializedAsync();

    // -------------------- Public API --------------------

    public async Task<ISOCountry?> GetCountryAsync(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentNullException(nameof(countryCode));
        await EnsureInitializedAsync();
        _countries.TryGetValue(countryCode, out var c);
        return c;
    }

    public async Task<ISOSubdivision?> GetSubDivisionAsync(string subdivisionKey)
    {
        if (string.IsNullOrWhiteSpace(subdivisionKey))
            throw new ArgumentNullException(nameof(subdivisionKey));
        await EnsureInitializedAsync();
        _subdivisions.TryGetValue(subdivisionKey, out var s);
        return s;
    }

    public async Task<TProperty?> GetCountryPropertyAsync<TProperty>(
        string countryCode, Expression<Func<ISOCountry, TProperty>> propertyExpression)
    {
        var c = await GetCountryAsync(countryCode);
        if (c is null) return default;
        return propertyExpression.Compile()(c);
    }

    public async Task<TProperty?> GetSubdivisionPropertyAsync<TProperty>(
        string subdivisionCode, Expression<Func<ISOSubdivision, TProperty>> propertyExpression)
    {
        var s = await GetSubDivisionAsync(subdivisionCode);
        if (s is null) return default;
        return propertyExpression.Compile()(s);
    }

    public async Task<Dictionary<string, ISOCountry>> GetAllCountriesAsync()
    {
        await EnsureInitializedAsync();
        // defensive copy
        return _countries.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<Dictionary<string, ISOSubdivision>> GetAllSubdivisionsAsync()
    {
        await EnsureInitializedAsync();
        return _subdivisions.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
    }

    public List<KeyValuePair<string, ISOCountry>>? Countries => _countries.ToList();

    public async Task<IEnumerable<CountryPhoneCode>> GetContriesAreaCodesAsync()
    {
        await EnsureInitializedAsync();
        return _countries.Values.Select(c => new CountryPhoneCode(c.ISOShortName, c.Alpha2, c.CountryCode));
    }

    public async Task<IEnumerable<CountryPhoneCode>> GetCountriesAreaCodesAsync()
    {
        await EnsureInitializedAsync();

        var list = _countries.Values
            .Where(c => !string.IsNullOrWhiteSpace(c.CountryCode)
                        && !string.IsNullOrWhiteSpace(c.ISOShortName)
                        && !string.IsNullOrWhiteSpace(c.Alpha2))
            .Select(c => new CountryPhoneCode(
                name: c.ISOShortName!,
                alpha2: c.Alpha2!,
                phoneCode: NormalizeDialCode(c.CountryCode!)
            ))
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.Alpha2, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return list;
    }

    public async Task<IEnumerable<CountryPhoneCode>> GetUniqueAreaCodesAsync()
    {
        await EnsureInitializedAsync();

        var unique = _countries.Values
            .Where(c => !string.IsNullOrWhiteSpace(c.CountryCode))
            .Select(c => new { c.ISOShortName, c.Alpha2, Code = NormalizeDialCode(c.CountryCode!) })
            .GroupBy(x => x.Code)
            .Select(g =>
            {
                var first = g.OrderBy(x => x.ISOShortName, StringComparer.OrdinalIgnoreCase).First();
                return new CountryPhoneCode(first.ISOShortName!, first.Alpha2!, first.Code);
            })
            .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return unique;
    }

    // -------------------- Initialization & Loading --------------------

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initGate.WaitAsync();
        try
        {
            if (_initialized) return;

            // 1) Try Redis cache (if available)
            Dictionary<string, ISOCountry>? cachedCountries = null;
            Dictionary<string, ISOSubdivision>? cachedSubdivisions = null;

            if (_cache is not null)
            {
                // Start both requests concurrently
                var countriesTask    = _cache.GetAsync<Dictionary<string, ISOCountry>>(CountriesCacheKey);
                var subdivisionsTask = _cache.GetAsync<Dictionary<string, ISOSubdivision>>(SubdivisionsCacheKey);

                // Await both without blocking the thread
                await Task.WhenAll(countriesTask, subdivisionsTask);

                // Now safely retrieve results (Task<T>.Result exists here, but 'await' is clearer)
                cachedCountries     = await countriesTask;
                cachedSubdivisions  = await subdivisionsTask;
            }

            if (cachedCountries is not null && cachedCountries.Count > 0 &&
                cachedSubdivisions is not null && cachedSubdivisions.Count > 0)
            {
                // hydrate in-memory from cache
                foreach (var kv in cachedCountries) _countries.TryAdd(kv.Key, kv.Value);
                foreach (var kv in cachedSubdivisions) _subdivisions.TryAdd(kv.Key, kv.Value);
                _initialized = true;
                return;
            }

            // 2) Fallback to embedded resources
            await Task.WhenAll(LoadCountriesFromResourcesAsync(), LoadSubdivisionsFromResourcesAsync());

            // 3) Store in Redis for future boots
            if (_cache is not null && _countries.Count > 0 && _subdivisions.Count > 0)
            {
                await _cache.SetAsync(CountriesCacheKey,
                    _countries.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase),
                    CacheTtl);

                await _cache.SetAsync(SubdivisionsCacheKey,
                    _subdivisions.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase),
                    CacheTtl);
            }

            _initialized = true;
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task LoadCountriesFromResourcesAsync()
    {
        var resourceName = $"Mamey.ISO3166.Data.Countries";
        var assembly = AppDomain.CurrentDomain.Load("Mamey.ISO.ISO3166");
        var resources = assembly?.GetManifestResourceNames();

        var jsonResources = resources?.Where(c => c.StartsWith(resourceName) && c.EndsWith(".json"));
        if (jsonResources is null) return;

        foreach (var resource in jsonResources)
        {
            var countryKV =
                await ResourceExtensions.GetJsonFromEmbeddedResourceAsync<Dictionary<string, ISOCountry>>(resource, "Mamey.ISO.ISO3166");

            if (countryKV is null || countryKV.Count == 0) continue;

            foreach (var kv in countryKV)
                _countries[kv.Key] = kv.Value; // upsert
        }
    }

    private async Task LoadSubdivisionsFromResourcesAsync()
    {
        var resourceName = $"Mamey.ISO3166.Data.Subdivisions";
        var assembly = AppDomain.CurrentDomain.Load("Mamey.ISO.ISO3166");
        var resources = assembly?.GetManifestResourceNames();

        var jsonResources = resources?.Where(c => c.StartsWith(resourceName) && c.EndsWith(".json"));
        if (jsonResources is null) return;

        foreach (var resource in jsonResources)
        {
            var subDivisionKV =
                await ResourceExtensions.GetJsonFromEmbeddedResourceAsync<Dictionary<string, ISOSubdivision>>(resource, "Mamey.ISO.ISO3166");

            if (subDivisionKV is null || subDivisionKV.Count == 0) continue;

            foreach (var kv in subDivisionKV)
                _subdivisions[kv.Key] = kv.Value; // upsert
        }
    }

    // -------------------- Utilities --------------------

    private static string NormalizeDialCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return string.Empty;
        var s = code.Trim();
        if (s.StartsWith("+", StringComparison.Ordinal)) s = s[1..];
        return s;
    }
}
