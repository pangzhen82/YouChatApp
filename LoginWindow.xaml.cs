using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace YouChat
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();

            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Dark);
            }
            paletteHelper.SetTheme(theme);
        }
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Login Button Clicked!");
            string user, pw;
            if (txtUsername.Text.Length != 0)
            {
                user = txtUsername.Text;
                if (Data.credentials.ContainsKey(user))
                {
                    Data.credentials.TryGetValue(user, out pw);
                    if (pw == txtPassword.Password)
                    {
                        MessageBox.Show("Login Successfully! Welcome to YouChat!");
                        MainWindow mainWindow = new MainWindow();
                        this.Hide();
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Wrong password! Please try again!");
                        txtPassword.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("User not registered! Click Register now!");
                    txtPassword.Clear();
                }
            }
            else
            {
                MessageBox.Show("Error! User Name can't be empty!");
                txtUsername.Clear();
                txtPassword.Clear();
            }
        }
        private void signupBtn_Click(object sender, RoutedEventArgs e)
        {
            string user, pw;
            if (txtUsername.Text.Length != 0)
            {
                user = txtUsername.Text;
                if (Data.credentials.ContainsKey(user))
                {
                    MessageBox.Show("User already exist!");
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
                else
                {
                    pw = txtPassword.Password;
                    if (pw.Length == 0)
                    {
                        MessageBox.Show("Error! Password can't be empty!");
                        txtUsername.Clear();
                        txtPassword.Clear();
                    }
                    else
                    {
                        Data.credentials.Add(user, pw);  // add the user and pw to new creditial list
                        MessageBox.Show("User register successfully!\nSign in with your password!");
                        txtPassword.Clear();
                    }
                }
            }
            else
            {
                MessageBox.Show("Error! User Name can't be empty!");
                txtUsername.Clear();
                txtPassword.Clear();
            }
        }
    }
}
