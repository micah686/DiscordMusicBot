using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace Yuna.Services
{
    public static class LoggingService
    {
        public static void Log(string message)
        {
            LogInternal(message, GetLogColor(LogSeverity.Info), LogSeverity.Info);
        }

        public static void Log(string message, Color color, bool isMusicBot = false)
        {
            if (isMusicBot)
            {
                LogInternalMusic(message, color);
            }
            else
            {
                LogInternal(message, color, LogSeverity.Info);
            }
            
        }
        public static void Log(string message, LogSeverity severity)
        {
            LogInternal(message, GetLogColor(severity), severity);
        }

        public static void Log(string message, Color color, LogSeverity severity)
        {
            LogInternal(message, color, severity);
        }

        public static void LogCritical(string message)
        {
            LogInternal(message, GetLogColor(LogSeverity.Error), LogSeverity.Critical);
        }
        public static void LogException(Exception ex)
        {
            var msg = $"{ex.Message}{Environment.NewLine}";
            LogInternal(msg, GetLogColor(LogSeverity.Critical), LogSeverity.Critical);
            AnsiConsole.WriteException(ex);
        }

        public static void LogInfo(string message)
        {
            LogInternal(message, GetLogColor(LogSeverity.Info), LogSeverity.Info);
        }

        public static void LogDebug(string message)
        {
            LogInternal(message, GetLogColor(LogSeverity.Debug), LogSeverity.Debug);
        }

        private static void LogInternal(string message, Color color, LogSeverity? severity)
        {
            if(severity== null) {  severity = LogSeverity.Info; }
            var esc = Markup.Escape($"[{severity}]");
            var logMessage = $"{esc} {DateTime.Now} {message}";

            var markup = new Markup($"[{color.ToMarkup()}]{logMessage}[/]{Environment.NewLine}");
            AnsiConsole.Write(markup);
        }

        private static void LogInternalMusic(string message, Color color)
        {
            var esc = Markup.Escape($"[MUSICBOT]");
            var logMessage = $"{esc} {DateTime.Now} {message}";

            var markup = new Markup($"[{color.ToMarkup()}]{logMessage}[/]{Environment.NewLine}");
            AnsiConsole.Write(markup);
        }

        private static Color GetLogColor(LogSeverity logLevel)
        {
            return logLevel switch
            {
                LogSeverity.Info => Color.Turquoise2,
                LogSeverity.Critical => Color.DarkRed,
                LogSeverity.Error => Color.Red3_1,
                LogSeverity.Warning => Color.DarkOrange3_1,
                LogSeverity.Debug => Color.Purple4_1,
                LogSeverity.Verbose => Color.Grey50,
                _ => Color.Fuchsia,
            };
        }        
    }
}
