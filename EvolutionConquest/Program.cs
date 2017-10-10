using System;
using System.IO;

namespace EvolutionConquest
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //try
            //{
            using (var game = new Game1())
                game.Run();
            //}
            //catch (Exception ex)
            //{
            //    File.WriteAllText("ErrorLog.log", "Message: " + ex.Message + Environment.NewLine + "Stack Strace: " + ex.StackTrace + Environment.NewLine);
            //}
        }
    }
#endif
}
