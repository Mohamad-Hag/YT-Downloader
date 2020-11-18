using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Configuration;

namespace YT_Downloader.Classes
{
    class Account
    {
        private int Id { get; set; }
        private string Username { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }
        private Settings UserSettigs { get; set; }
        private string Message { get; set; }

        public Account(string email, string password)
        {
            Email = email.Trim();
            Password = password.Trim();
            Message = "".Trim();
            UserSettigs = new Settings();            
            UserSettigs.Email = Email;
            UserSettigs.Password = Password;
        }
        public Account(string username, string email, string password)
        {
            Initialize(username, email, password);
            UserSettigs = new Settings();
            UserSettigs.Username = Username;
            UserSettigs.Email = Email;
            UserSettigs.Password = Password;
        }
        public Account(string username, string email, string password, Settings settings)
        {
            Initialize(username, email, password);
            UserSettigs = new Settings();
            UserSettigs.Username = Username;
            UserSettigs.Email = Email;
            UserSettigs.Password = Password;
            UserSettigs = settings;
        }
        public bool ShowMessage()
        {
            if (!Message.Equals(string.Empty))
            {
                return true;
            }
            return false;

        }
        public string GetMessage()
        {
            return Message;
        }
        private void Initialize(string username, string email, string password)
        {
            Username = username.Trim();
            Email = email.Trim();
            Password = password.Trim();
            Message = "".Trim();
        }
        public int GetId()
        {
            int id = 0;
            try
            {
                DatabaseConnectivity dc = new DatabaseConnectivity();
                dc.Connect();
                MySqlCommand command = new MySqlCommand();
                command.Parameters.AddWithValue("@el", Email);
                command.Parameters.AddWithValue("@pd", Password);
                dc.ExecuteReader(command, "SELECT AccountId FROM accounts WHERE Email=@el AND Password=@pd;");
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    id = Convert.ToInt32(reader["AccountId"]);
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Id = id;
            return Id;
        }
        public string GetUsername()
        {
            string username = "";
            try
            {
                DatabaseConnectivity dc = new DatabaseConnectivity();
                dc.Connect();
                MySqlCommand command = new MySqlCommand();
                command.Parameters.AddWithValue("@el", Email);
                command.Parameters.AddWithValue("@pd", Password);
                dc.ExecuteReader(command, "SELECT Username FROM accounts WHERE Email=@el AND Password=@pd;");
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    username = reader["Username"].ToString();
                dc.Unconnect();
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Username = username;
            return Username;
        }
        public string GetEmail()
        {
            return Email;
        }
        public string GetPassword()
        {
            return Password;
        }
        private bool ClientValidate()
        {
            string emailPattern = @"^((?!\.)[\w-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$";
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$";

            bool isEmailValidate = Regex.IsMatch(Email, emailPattern);
            bool isPasswordValidate = Regex.IsMatch(Password, passwordPattern);

            bool isUsernameEmpty = Username.Equals(string.Empty);
            bool isEmailEmpty = Email.Equals(string.Empty);
            bool isPasswordEmpty = Password.Equals(string.Empty);

            if (!isUsernameEmpty && !isEmailEmpty && !isEmailEmpty)
            {
                if (isEmailValidate && isPasswordValidate)
                {
                    Message = "";
                    return true;
                }
            }
            if (isUsernameEmpty || isEmailEmpty || isPasswordEmpty)
                Message = "Fill the empty fields.";
            else if (!isEmailValidate && !isEmailValidate)
                Message = "Invalid email address and password.";
            else if (!isEmailValidate)
                Message = "Invalid email address, (ex: example@gmail.com).";
            else if (!isPasswordValidate)
                Message = "Invalid password.";
            return false;
        }
        private Task<bool> ServerValidate()
        {
            Task<bool> validate = new Task<bool>(() =>
            {
                try
                {
                    DatabaseConnectivity dc = new DatabaseConnectivity();
                    dc.Connect();
                    MySqlDataReader reader = dc.ExecuteReader("SELECT Email FROM accounts;");
                    while (reader.Read())
                    {
                        if (Email.Equals(reader["Email"]))
                        {
                            Message = "This Email is already exists.";
                            return false;
                        }
                    }
                    dc.Unconnect();
                    Message = "";
                    return true;

                }
                catch (Exception e)
                {
                    Message = e.Message;
                    return false;
                }
            });
            validate.Start();
            return validate;
        }
        private Task<bool> AddAccount()
        {
            Task<bool> addAccount = new Task<bool>(() =>
            {
                try
                {
                    DatabaseConnectivity dc = new DatabaseConnectivity();
                    dc.Connect();
                    MySqlCommand command = new MySqlCommand();
                    command.Parameters.AddWithValue("@un", Username);
                    command.Parameters.AddWithValue("@el", Email);
                    command.Parameters.AddWithValue("@pd", Password);
                    command.Parameters.AddWithValue("@sid", 1);
                    dc.Execute(command, "INSERT INTO accounts (Username, Email, Password, SettingsId) VALUES (@un, @el, @pd, @sid);");
                    dc.Unconnect();
                    return true;
                }
                catch
                {
                    Message = "Something went wrong.";
                    return false;
                }
            });
            addAccount.Start();
            return addAccount;
        }
        public async Task Create()
        {
            if (!ClientValidate() || !await ServerValidate())
                return;
            await AddAccount();
        }
        private Task<bool> Login()
        {
            Task<bool> login = new Task<bool>(() =>
            {
                try
                {
                    bool isFound = false;
                    if (!Email.Equals(string.Empty) && !Password.Equals(string.Empty))
                    {
                        DatabaseConnectivity dc = new DatabaseConnectivity();
                        dc.Connect();
                        MySqlCommand command = new MySqlCommand();
                        command.Parameters.AddWithValue("@el", Email);
                        command.Parameters.AddWithValue("@pd", Password);
                        dc.ExecuteReader(command, "SELECT * FROM accounts WHERE Email=@el and Password=@pd");
                        MySqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader["Email"].Equals(Email) && reader["Password"].Equals(Password))
                            {
                                isFound = true;
                            }
                        }
                        Message = isFound ? "" : "Email address or password is incorrect.";
                        dc.Unconnect();
                    }
                    else
                    {
                        isFound = false;
                        Message = "Fill the empty fields.";
                    }
                    return isFound;
                }
                catch
                {
                    Message = "Something went wrong";
                    return false;
                }
            });
            login.Start();
            return login;
        }
        public Task Enter()
        {
            Task enter = new Task(async() =>
            {
                await Login();
            });
            enter.Start();
            return enter;
        }
    }
}
