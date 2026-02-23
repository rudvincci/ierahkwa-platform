namespace TradeX.API.Controllers.Transactions;

/// <summary>
/// Rutas registradas para la carpeta Transacciones.
/// Base: /api/transactions
/// </summary>
public static class TransactionsRoutes
{
    public const string Base = "api/transactions";

    public const string History = Base + "/history";
    public const string Vip = Base + "/vip";
    public const string Status = Base + "/status";
    public const string Deposit = Base + "/deposit";
    public const string Withdraw = Base + "/withdraw";
    public const string Transfer = Base + "/transfer";

    public static readonly string[] All =
    {
        "GET  " + History,
        "GET  " + Vip,
        "GET  " + Status,
        "POST " + Deposit,
        "POST " + Withdraw,
        "POST " + Transfer,
    };
}
