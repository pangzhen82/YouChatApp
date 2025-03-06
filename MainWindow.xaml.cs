using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YouChat.MVVM.ViewModel;
namespace YouChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm;
        private readonly string[] emojis = new string[]
        {

            "😀", "😁", "😂", "🤣", "😃", "😄", "😅", "😆", "😉", "😊",
            "😋", "😎", "😍", "😘", "🥰", "😗", "😙", "😚", "🙂", "🤗",
            "🤩", "🤔", "🤨", "😐", "😑", "😶", "🙄", "😏", "😣", "😥",
            "😮", "😯", "😪", "😫", "🥱", "😴", "😌", "😛", "😜", "😝",
            "🤤", "😒", "😓", "😔", "😕", "🙃", "🤑", "😲", "🤖", "🎃",
            "🙁", "😖", "😞", "😟", "😤", "😢", "😭", "😦", "😧", "😨",
            "😩", "🤯", "😬", "😰", "😱", "🥵", "🥶", "😳", "🤪", "😵",
            "😡", "😠", "🤬", "😷", "🤒", "🤕", "🤢", "🤮", "🤧", "🥴",
            "😇", "🤠", "🥳", "🥺", "🤥", "🤫", "🤭", "🧐", "❤️",
            "🤓", "😈", "👿", "💀", "☠️", "👻", "👽", "🔥", "👍", "💯",
            
        };
        public MainWindow()
        {
            InitializeComponent();
            LoadEmojis();
            vm = new MainViewModel();
            this.DataContext = vm;
            vm.ScrollToBottom = ScrollToBottom;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if(Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveChatHistoryToFile();
            Application.Current.Shutdown();
        }
        private void AutoScrollDown(object sender, RoutedEventArgs e)
        {
            ScrollToBottom();
        }
        private void ScrollToBottom()
        {
            if (MainMessageView.Items.Count > 0)
            {
                MainMessageView.ScrollIntoView(MainMessageView.Items[MainMessageView.Items.Count - 1]);
            }
        }
        /*adding emojis code*/
        private void LoadEmojis()
        {
            // Populate emoji picker dynamically
            foreach (var emoji in emojis)
            {
                Button emojiButton = new Button
                {
                    Content = emoji,
                    Width = 60,
                    Height = 35,
                    FontSize = 20,
                    FontFamily = new FontFamily("Segoe UI Emoji"),
                    //Margin = new Thickness(0)
                };
                
                emojiButton.Click += EmojiButton_Click;
                emojiPanel.Children.Add(emojiButton);
            }
        }
        private void EmojiButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button emojiButton)
            {
                MessageInputBox.Text += emojiButton.Content.ToString(); // Append selected emoji
                emojiPopup.IsOpen = false; // Close the picker after selection
            }
        }
        private void OpenEmojiPicker(object sender, RoutedEventArgs e)
        {
            emojiPopup.IsOpen = true;
        }
    }
}
