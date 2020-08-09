using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace THNETII.GitHubActions.Toolkit.Core
{
    public class GhActionsCommand
    {
        public static void IssueCommand<T>(string command,
            IEnumerable<KeyValuePair<string, object?>>? properties = null,
            [MaybeNull] T message = default)
        {
            var cmd = Create(command, properties, message);
            Console.WriteLine(cmd.ToString());
        }

        public static void Issue(string name, string? message = null) =>
            IssueCommand(name, default, message);

        internal const string CMD_STRING = "::";

        public string Command { get; }
        public string Properties { get; }
        public object Message { get; }

        public static GhActionsCommand Create<T>(string? command,
            IEnumerable<KeyValuePair<string, object?>>? properties = null,
            [MaybeNull] T message = default)
        {
            if (string.IsNullOrEmpty(command))
            {
                command = "missing.command";
            }

            properties ??= Enumerable.Empty<KeyValuePair<string, object?>>();

            return new GhActionsCommand(command,
                string.Join(",", properties.Select(EscapePropertyKeyValuePair)),
                EscapeData(message)
                );
        }

        private static readonly Func<KeyValuePair<string, object?>, string> EscapePropertyKeyValuePair = kvp =>
        {
            var (key, val) = kvp;
            if (val is null || (val is string valStr && string.IsNullOrEmpty(valStr)))
                return string.Empty;
            return key + "=" + EscapeProperty(val);
        };

        private GhActionsCommand(string? command,
            string properties,
            string message) : base() =>
            (Command, Properties, Message) = (command!, properties, message);

        public override string ToString()
        {
            StringBuilder cmdStr = new StringBuilder();
            cmdStr.Append(CMD_STRING).Append(Command);
            if (string.IsNullOrEmpty(Properties))
            {
                cmdStr.Append(' ');
                cmdStr.Append(Properties);
            }
            cmdStr.Append(CMD_STRING).Append(Message);
            return cmdStr.ToString();
        }

        public static string ToCommandValue<T>([MaybeNull] T input,
            JsonSerializerOptions? options = null)
        {
            return input switch
            {
                null => string.Empty,
                string s => s,
                T inst => JsonSerializer.Serialize(inst, options),
            };
        }

        private static string EscapeData<T>([MaybeNull] T s) =>
            ToCommandValue(s)
            .ReplaceOrdinal("%", "%25")
            .ReplaceOrdinal("\r", "%0D")
            .ReplaceOrdinal("\n", "%0A")
            ;

        private static string EscapeProperty<T>([MaybeNull] T s) =>
            ToCommandValue(s)
            .ReplaceOrdinal("%", "%25")
            .ReplaceOrdinal("\r", "%0D")
            .ReplaceOrdinal("\n", "%0A")
            .ReplaceOrdinal(":", "%3A")
            .ReplaceOrdinal(",", "%2C")
            ;
    }
}
