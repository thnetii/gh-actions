using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace THNETII.GitHubActions.Toolkit.Core
{
    using static GhActionsCommand;

    public static class GhActionsCore
    {
        #region Variables
        /// <summary>
        /// Sets the environment variable for this action and future actions in the job
        /// </summary>
        /// <param name="name">the name of the variable to set</param>
        /// <param name="val">the value of the variable. Non-string values will be converted to a JSON-string</param>
        public static void ExportVariable<T>(string name, [MaybeNull] T val)
        {
            var convertedVal = ToCommandValue(val);
            Environment.SetEnvironmentVariable(name, convertedVal);
            IssueCommand("set-env", message: val, properties:
                new[] { ("name", (object?)name).AsKeyValuePair() });
        }

        /// <summary>
        /// Registers a secret which will get masked from logs
        /// </summary>
        /// <param name="secret">value of the secret</param>
        public static void SetSecret(string secret) =>
            IssueCommand("add-mask", default, secret);

        public static void AddPath(string inputPath)
        {
            var currentPath = Environment.GetEnvironmentVariable("PATH");
            IssueCommand("add-path", default, inputPath);
            var newPath = inputPath;
            if (string.IsNullOrWhiteSpace(currentPath))
                newPath = currentPath + Path.PathSeparator + newPath;
            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        /// <summary>
        /// Gets the value of an input. The value is also trimmed.
        /// </summary>
        /// <param name="name">name of the input to get</param>
        /// <param name="required">Optional. Whether the input is required. If required and not present, will throw. Defaults to false</param>
        public static string? GetInput(string name, bool required = false)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            var envName = "INPUT_" + name.Replace(' ', '_').ToUpperInvariant();
            var val = Environment.GetEnvironmentVariable(envName);
            if (required && string.IsNullOrEmpty(val))
                throw new InvalidOperationException($"Input required and not supplied: {name}");
            return val.Trim();
        }

        /// <summary>
        /// Sets the value of an output.
        /// </summary>
        /// <param name="name">name of the output to set</param>
        /// <param name="value">value to store. Non-string values will be converted to a JSON-string</param>
        public static void SetOutput<T>(string name, [MaybeNull] T value) =>
            IssueCommand("set-output", message: value, properties:
                new[] { ("name", (object?)name).AsKeyValuePair() });


        /// <summary>
        /// Enables or disables the echoing of commands into stdout for the rest of the step.
        /// Echoing is disabled by default if <c>ACTIONS_STEP_DEBUG</c> is not set.
        /// </summary>
        [SuppressMessage("Globalization", "CA1303: Do not pass literals as localized parameters")]
        public static void SetCommandEcho(bool enabled) =>
            Issue("echo", enabled ? "on" : "off");
        #endregion

        #region Results
        /// <summary>
        /// Sets the action status to failed.
        /// When the action exits it will be with an exit code of 1
        /// </summary>
        /// <param name="message">add error issue message</param>
        public static void SetFailed(string message)
        {
            Environment.ExitCode = 1;
            Error(message);
        }

        /// <inheritdoc cref="SetFailed(string)"/>
        /// <param name="except">serialize exception into error issue message</param>
        public static void SetFailed(Exception except)
        {
            Environment.ExitCode = 1;
            Error(except);
        }
        #endregion

        #region Logging Commands
        /// <summary>
        /// Gets whether Actions Step Debug is on or not
        /// </summary>
        public static bool IsDebug()
        {
            var runnerDebug = Environment.GetEnvironmentVariable("RUNNER_DEBUG");
            if (int.TryParse(runnerDebug, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                return intValue == 1;
            return false;
        }

        /// <summary>
        /// Writes debug message to user log
        /// </summary>
        /// <param name="message">debug message</param>
        public static void Debug(string message) =>
            IssueCommand("debug", default, message);

        /// <summary>
        /// Adds an error issue
        /// </summary>
        /// <param name="message">error issue message</param>
        public static void Error(string message) =>
            Issue("error", message);

        /// <inheritdoc cref="Error(string)"/>
        public static void Error(Exception exception) =>
            IssueCommand("error", default, exception);

        /// <summary>
        /// Adds an warning issue
        /// </summary>
        /// <param name="message">warning issue message</param>
        public static void Warning(string message) =>
            Issue("warning", message);

        /// <inheritdoc cref="Warning(string)"/>
        public static void Warning(Exception exception) =>
            IssueCommand("warning", default, exception);

        /// <summary>
        /// Writes info to log with <see cref="Console.WriteLine(string)"/>.
        /// </summary>
        /// <param name="message">info message</param>
        public static void Info(string message) =>
            Console.WriteLine(message);

        /// <summary>
        /// Begin an output group.
        /// <para>Output until the next `groupEnd` will be foldable in this group</para>
        /// </summary>
        /// <param name="name">The name of the output group</param>
        public static void StartGroup(string name) =>
            Issue("group", name);

        /// <summary>
        /// End an output group.
        /// </summary>
        public static void EndGroup() =>
            Issue("endgroup");

        /// <summary>
        /// Constructs an <see cref="IDisposable"/> instance that can be used in a using block.
        /// </summary>
        /// <param name="name">The name of the output group</param>
        public static IDisposable Group(string name) =>
            new GhLoggingGroupDisposable(name);
        #endregion

        #region Wrapper action state
        /// <summary>
        /// Saves state for current action, the state can only be retrieved by this action's post job execution.
        /// </summary>
        /// <param name="name">name of the state to store</param>
        /// <param name="value">value to store. Non-string values will be converted to a JSON-string</param>
        public static void SaveState<T>(string name, [MaybeNull] T value) =>
            IssueCommand("save-state", message: value, properties:
                new[] { ("name", (object?)name).AsKeyValuePair() });

        /// <summary>
        /// Gets the value of an state set by this action's main execution.
        /// </summary>
        /// <param name="name">name of the state to get</param>
        public static string? GetState<T>(string name) =>
            Environment.GetEnvironmentVariable("STATE_" + name);
        #endregion
    }
}
