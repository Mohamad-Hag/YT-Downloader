using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_Downloader.Classes
{
    class Settings
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }       
        public Settings()
        {
            Properties.Settings.Default.Email = Email;
            Properties.Settings.Default.Username = Username;
            Properties.Settings.Default.Password = Password;
            Properties.Settings.Default.Save();
        }
    }
}
