using System;
using System.IO;
using System.Windows.Forms;

namespace CSRDReporting.Utilities
{
    /// <summary>
    /// Centralized error handling and logging utility for the CSRD application
    /// Provides consistent error logging and user notification mechanisms
    /// </summary>
    public static class ErrorHandler
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CSRDReporting",
            "Logs",
            $"CSRDReporting_{DateTime.Now:yyyyMMdd}.log"
        );

        /// <summary>
        /// Handles and logs exceptions with user notification
        /// </summary>
        /// <param name="exception">Exception to handle</param>
        /// <param name="context">Context where the exception occurred</param>
        /// <param name="showMessageToUser">Whether to show message to user</param>
        public static void HandleException(Exception exception, string context = "", bool showMessageToUser = true)
        {
            try
            {
                // Log the exception
                LogException(exception, context);

                // Show user-friendly message if requested
                if (showMessageToUser)
                {
                    ShowUserErrorMessage(exception, context);
                }
            }
            catch (Exception loggingException)
            {
                // If logging fails, show basic error message
                MessageBox.Show($"A critical error occurred and could not be logged:\n{exception.Message}\n\nLogging error: {loggingException.Message}",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Logs exception details to file
        /// </summary>
        /// <param name="exception">Exception to log</param>
        /// <param name="context">Context information</param>
        private static void LogException(Exception exception, string context)
        {
            try
            {
                // Ensure log directory exists
                var logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create log entry
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR in {context}\n" +
                              $"Exception Type: {exception.GetType().Name}\n" +
                              $"Message: {exception.Message}\n" +
                              $"Stack Trace: {exception.StackTrace}\n";

                if (exception.InnerException != null)
                {
                    logEntry += $"Inner Exception: {exception.InnerException.Message}\n";
                }

                logEntry += new string('-', 80) + "\n";

                // Write to log file
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // Silent fail for logging - we don't want logging errors to crash the application
            }
        }

        /// <summary>
        /// Shows user-friendly error message based on exception type
        /// </summary>
        /// <param name="exception">Exception that occurred</param>
        /// <param name="context">Context where exception occurred</param>
        private static void ShowUserErrorMessage(Exception exception, string context)
        {
            string title;
            string message;
            MessageBoxIcon icon;

            switch (exception)
            {
                case ValidationException validationEx:
                    title = "Data Validation Error";
                    message = $"Please check your input data:\n\n{validationEx.Message}";
                    icon = MessageBoxIcon.Warning;
                    break;

                case UnauthorizedAccessException:
                    title = "Access Denied";
                    message = "You don't have permission to perform this operation.\n\nPlease contact your system administrator.";
                    icon = MessageBoxIcon.Warning;
                    break;

                case FileNotFoundException:
                    title = "File Not Found";
                    message = "A required file could not be found.\n\nPlease ensure all application files are present and try again.";
                    icon = MessageBoxIcon.Error;
                    break;

                case System.Data.Common.DbException:
                    title = "Database Error";
                    message = "A database error occurred. Your data may not have been saved.\n\nPlease try again. If the problem persists, contact technical support.";
                    icon = MessageBoxIcon.Error;
                    break;

                case OutOfMemoryException:
                    title = "Memory Error";
                    message = "The application is running low on memory.\n\nPlease close other applications and try again.";
                    icon = MessageBoxIcon.Error;
                    break;

                case System.Net.NetworkInformation.NetworkInformationException:
                    title = "Network Error";
                    message = "A network error occurred.\n\nPlease check your internet connection and try again.";
                    icon = MessageBoxIcon.Warning;
                    break;

                default:
                    title = "Application Error";
                    message = $"An unexpected error occurred:\n\n{exception.Message}\n\n" +
                             "Please try the operation again. If the problem persists, contact technical support.";
                    icon = MessageBoxIcon.Error;
                    break;
            }

            // Add context information if provided
            if (!string.IsNullOrEmpty(context))
            {
                message += $"\n\nOperation: {context}";
            }

            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Logs informational messages
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="context">Context information</param>
        public static void LogInformation(string message, string context = "")
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO {context}: {message}\n";
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // Silent fail for logging
            }
        }

        /// <summary>
        /// Logs warning messages
        /// </summary>
        /// <param name="message">Warning message to log</param>
        /// <param name="context">Context information</param>
        public static void LogWarning(string message, string context = "")
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING {context}: {message}\n";
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // Silent fail for logging
            }
        }

        /// <summary>
        /// Shows confirmation dialog for potentially destructive operations
        /// </summary>
        /// <param name="message">Confirmation message</param>
        /// <param name="title">Dialog title</param>
        /// <returns>True if user confirmed, false otherwise</returns>
        public static bool ConfirmOperation(string message, string title = "Confirm Operation")
        {
            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Shows information message to user
        /// </summary>
        /// <param name="message">Information message</param>
        /// <param name="title">Dialog title</param>
        public static void ShowInformation(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows warning message to user
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="title">Dialog title</param>
        public static void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Gets the current log file path
        /// </summary>
        /// <returns>Path to the current log file</returns>
        public static string GetLogFilePath()
        {
            return LogFilePath;
        }

        /// <summary>
        /// Clears old log files (keeps only last 30 days)
        /// </summary>
        public static void CleanupOldLogs()
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDirectory))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-30);
                var logFiles = Directory.GetFiles(logDirectory, "CSRDReporting_*.log");

                foreach (var logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        fileInfo.Delete();
                    }
                }
            }
            catch
            {
                // Silent fail for cleanup - not critical
            }
        }
    }
}