using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Keeps track of how many forms are currently running. Shuts down the application when the last one closes.
    /// </summary>
    class SpreadsheetApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int windowCount = 0;

        // Singleton ApplicationContext
        private static SpreadsheetApplicationContext context;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadsheetApplicationContext.
        /// </summary>
        public static SpreadsheetApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new SpreadsheetApplicationContext();
            }
            return context;
        }

        /// <summary>
        /// Runs a form in this application context
        /// </summary>
        public void RunNew()
        {
            // Create the window and the controller
            Form1 window = new Form1();
            new Controller(window);

            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            window.Show();
        }

        public void RunNew(string fileName)
        {
            // Create the window and the controller
            Form1 window = new Form1();
            new Controller(fileName, window);

            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            window.Show();
        }
    }
}
