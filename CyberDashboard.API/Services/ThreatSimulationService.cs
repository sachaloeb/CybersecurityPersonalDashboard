// Services/ThreatSimulationService.cs
using MongoDB.Driver;
using System.Linq;
using Renci.SshNet;
using System.Collections.Concurrent;
public class ThreatSimulationService
{
    private readonly MongoLogService _mongo;
    public ThreatSimulationService(MongoLogService mongo) => _mongo = mongo;

    /* ----------  BRUTE FORCE  ---------- */
    public async Task<IReadOnlyCollection<ThreatLog>> SimulateBruteAsync(
        BruteForceRequest req,
        int maxParallel            = 50,
        int timeoutMs              = 3_000,
        bool stopAfterFirstHit     = false)
    {
        /* ----------------   PREPARE WORD‑LIST   ---------------- */
        var sourcePwds = (req.Passwords?.Any() == true)
            ? req.Passwords
            : Enumerable.Range(1, req.AttemptCount).Select(i => $"guess{i:000}");

        // If AttemptCount limits the run, respect it — otherwise run whole list
        var pwds = (req.AttemptCount > 0 ? sourcePwds.Take(req.AttemptCount) : sourcePwds).ToArray();

        /* ----------------   SET‑UP CONCURRENCY   ---------------- */
        using var throttler = new SemaphoreSlim(maxParallel);
        using var cts       = new CancellationTokenSource();
        var logs            = new ConcurrentBag<ThreatLog>();

        var tasks = pwds.Select(async pwd =>
        {
            await throttler.WaitAsync(cts.Token);
            try
            {
                if (cts.IsCancellationRequested) return; // short‑circuit

                bool ok;
                try
                {
                    using var ssh     = new SshClient(req.TargetIp, req.Username, pwd)
                    {
                        ConnectionInfo = { Timeout = TimeSpan.FromMilliseconds(timeoutMs) }
                    };
                    ssh.Connect();
                    ok = ssh.IsConnected;
                    ssh.Disconnect();
                }
                catch
                {
                    ok = false;
                }

                var log = new ThreatLog
                {
                    Timestamp  = DateTime.UtcNow,
                    Target     = req.TargetIp,
                    AttackType = AttackKind.BRUTE_FORCE,
                    Result     = ok,
                    ExtraInfo  = $"user={req.Username}; pwd={pwd}",
                    IP         = req.SourceIp ?? string.Empty
                };
                logs.Add(log);

                if (ok && stopAfterFirstHit)
                {
                    // success -> signal cancellation to any awaiting tasks
                    cts.Cancel();
                }
            }
            finally
            {
                throttler.Release();
            }
        });

        await Task.WhenAll(tasks);

        /* ----------------   BULK‑WRITE TO MONGO   ---------------- */
        if (!logs.IsEmpty)
        {
            await _mongo.InsertManyAsync(logs); // extension added to MongoLogService
        }

        return logs.ToArray();
    }

    /* ----------  XSS  ---------- */
    public async Task<ThreatLog> SimulateXssAsync(XssRequest req)
    {
        var success = req.Payload.Contains("<script", StringComparison.OrdinalIgnoreCase);

        var log = new ThreatLog
        {
            Target = req.TargetUrl,
            AttackType = AttackKind.XSS,
            Result = success,
            ExtraInfo = $"payload={req.Payload}"
        };
        await _mongo.InsertAsync(log);
        return log;
    }

    // /* ----------  SQL INJECTION  ---------- */
    // public async Task<ThreatLog> SimulateSqlAsync(SqlRequest req)
    // {
    //     var success = req.Query.Contains("SELECT", StringComparison.OrdinalIgnoreCase);
    //
    //     var log = new ThreatLog
    //     {
    //         Target = req.TargetUrl,
    //         AttackType = AttackKind.SQL_INJECTION,
    //         Result = success,
    //         ExtraInfo = $"query={req.Query}"
    //     };
    //     await _mongo.InsertAsync(log);
    //     return log;
    // }
}