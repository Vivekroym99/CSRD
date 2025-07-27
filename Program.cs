using System;
using System.Windows.Forms;
using CSRDReporting.Forms;
using CSRDReporting.DataAccess;

namespace CSRDReporting
{
    /// <summary>
    /// Main entry point for the CSRD ESG Reporting System
    /// Handles application startup, initialization, and global error handling
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles and text rendering improvements
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set up global exception handling
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // Initialize application
                InitializeApplication();

                // Run the main form
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                ShowCriticalError("Application Startup Error", ex);
            }
        }

        /// <summary>
        /// Initializes the application components
        /// </summary>
        private static void InitializeApplication()
        {
            try
            {
                // Initialize database
                DatabaseInitializer.InitializeDatabase();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize application database.", ex);
            }
        }

        /// <summary>
        /// Handles unhandled thread exceptions
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ShowError("Application Error", e.Exception);
        }

        /// <summary>
        /// Handles unhandled domain exceptions
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                ShowCriticalError("Critical Application Error", ex);
            }
        }

        /// <summary>
        /// Shows a user-friendly error message for recoverable errors
        /// </summary>
        /// <param name="title">Error dialog title</param>
        /// <param name="exception">Exception that occurred</param>
        private static void ShowError(string title, Exception exception)
        {
            string message = $"An error occurred in the application:\n\n{exception.Message}\n\n" +
                           "Please try the operation again. If the problem persists, contact system administrator.";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a critical error message for unrecoverable errors
        /// </summary>
        /// <param name="title">Error dialog title</param>
        /// <param name="exception">Critical exception that occurred</param>
        private static void ShowCriticalError(string title, Exception exception)
        {
            string message = $"A critical error occurred:\n\n{exception.Message}\n\n" +
                           "The application will now close. Please contact technical support.\n\n" +
                           $"Technical details:\n{exception}";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Environment.Exit(1);
        }
    }
}