using System.Net;
using System.Net.Mail;
using System.Text;
using SpaceMission.Models;
using SpaceMission.Pathfinding;
using SpaceMission.UI;

namespace SpaceMission.Services;

/// <summary>
/// Sends a mission report via SMTP email.
/// </summary>
public static class EmailReporter
{
    public static void SendReport(
        string senderEmail,
        string senderPassword,
        string receiverEmail,
        string smtpHost,
        int smtpPort,
        CosmicMap map,
        IReadOnlyList<PathResult> results)
    {
        var body = BuildEmailBody(map, results);

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 15_000
        };

        using var message = new MailMessage(senderEmail, receiverEmail)
        {
            Subject = $"🚀 SPACE 2026 — Mission Report [{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC]",
            Body = body,
            IsBodyHtml = false
        };

        client.Send(message);
    }

    private static string BuildEmailBody(CosmicMap map, IReadOnlyList<PathResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine("╔══════════════════════════════════════╗");
        sb.AppendLine("║     SPACE 2026 — MISSION REPORT      ║");
        sb.AppendLine("╚══════════════════════════════════════╝");
        sb.AppendLine();
        sb.AppendLine($"Report generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Map size: {map.Rows} rows × {map.Cols} columns");
        sb.AppendLine();
        sb.AppendLine(new string('─', 42));

        foreach (var result in results)
        {
            sb.AppendLine();
            if (!result.Success)
            {
                sb.AppendLine($"⚠  Mission failed — Astronaut {result.AstronautId} lost in space!");
            }
            else
            {
                sb.AppendLine($"✔  Astronaut {result.AstronautId} — Shortest path: {result.TotalCost} steps");
                sb.AppendLine();
                sb.AppendLine(ConsoleRenderer.BuildMapString(map, result));
            }
            sb.AppendLine(new string('─', 42));
        }

        return sb.ToString();
    }
}