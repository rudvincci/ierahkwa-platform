using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using InventoryManager.Data;
using InventoryManager.Forms;

namespace InventoryManager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Set DevExpress skin
            DevExpress.UserSkins.BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful);
            
            // Initialize database
            DatabaseManager.Initialize();
            
            // Show splash screen
            using (var splash = new SplashScreenForm())
            {
                splash.ShowDialog();
            }
            
            // Show login form
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Start main application
                    Application.Run(new MainForm(loginForm.CurrentUser));
                }
            }
        }
    }
}
