using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Media;
using System.Windows;
using YouChat.Core;
using YouChat.MVVM.Model;
using YouChat.Net;


namespace YouChat.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public ObservableCollection<MessageModel> MessagesModels { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }
        public string? Username { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        private Server _server;
        public Action? ScrollToBottom { get; set; }

        private ContactModel? _selectedContact;
        public ContactModel? SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
            } 
        }

        private string? _message;
        public string? Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            MessagesModels = new ObservableCollection<MessageModel>();
            Contacts = new ObservableCollection<ContactModel>();

            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.UserDisconnectEvent += UserDisconnected;
            _server.whisperMessageReceivedEvent += WhisperMSGReceived;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username)); 

            SendMessageCommand = new RelayCommand(o =>
            {
                var color = "#409aff";
                
                if (string.IsNullOrEmpty(Message)) return;

                if (SelectedContact == null)
                    _server.SendMessageToServer(Message, Username);
                else
                {
                    _server.SendMessageToUser(Message, Username, SelectedContact.Username);
                    Message = $"[WHISPER TO {SelectedContact.Username}]: " + Message;
                    color = "#E3ED34";
                }  

                MessagesModels.Add(new MessageModel
                {
                    Username = Username,
                    UsernameColor = color,
                    ImageSource = $@"C:\Users\pangz\Documents\MSSA\Data Structure and Algorithms\MidTermProject\YouChat\Icons\{Username}.png",
                    Message = Message,
                    Time = DateTime.Now,
                    IsNativeOrigin = false,
                    FirstMessage = true,
                });

                Message = "";
                SelectedContact = null;
                ScrollToBottom?.Invoke();
            });
        }
        private void UserDisconnected()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Contacts.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Contacts.Remove(user));
        }
        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            var name = _server.PacketReader.ReadMessage();

            string color;
            if (name == "[System]")
            {
                SystemSounds.Asterisk.Play();
                color = "#F0320C";
            }
            else
            {
                SystemSounds.Beep.Play();
                color = "#3bff6f";
            }
            Application.Current.Dispatcher.Invoke(() => MessagesModels.Add(new MessageModel
            {
                Username = name,
                UsernameColor = color,
                ImageSource = $@"C:\Users\pangz\Documents\MSSA\Data Structure and Algorithms\MidTermProject\YouChat\Icons\{name}.png",
                Message = msg,
                Time = DateTime.Now,
                IsNativeOrigin = false,
                FirstMessage = false,
            }));
            
        }
        private void WhisperMSGReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            var name = _server.PacketReader.ReadMessage();
            var receiverName = _server.PacketReader.ReadMessage();

            if (receiverName != Username)
                return;

            Application.Current.Dispatcher.Invoke(() => MessagesModels.Add(new MessageModel
            {
                Username = name,
                UsernameColor = "#E3ED34",
                ImageSource = $@"C:\Users\pangz\Documents\MSSA\Data Structure and Algorithms\MidTermProject\YouChat\Icons\{name}.png",
                Message = $"[WHISPER to you]: " + msg,
                Time = DateTime.Now,
                IsNativeOrigin = false,
                FirstMessage = false,
            }));
            SystemSounds.Exclamation.Play();
        }
        private void UserConnected()
        {
            string name = _server.PacketReader.ReadMessage();

            var user = new ContactModel
            {
                Username = name,
                UID = _server.PacketReader.ReadMessage(),
                ImageSource = $@"C:\Users\pangz\Documents\MSSA\Data Structure and Algorithms\MidTermProject\YouChat\Icons\{name}.png",
                Messages = MessagesModels,
            };

            if (!Contacts.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Contacts.Add(user));
            }
        }
        public void SaveChatHistoryToFile()
        {
            if (string.IsNullOrEmpty(Username)) return;

            const string PATH = @"C:\Files\";
            string fileName = PATH + $"Chat History_{Username}.txt";

            StreamWriter? fileWriter = null;
            try
            {
                if(!File.Exists(fileName))
                {
                    fileWriter = File.CreateText(fileName);
                    fileWriter.WriteLine($"New chat history created at [{DateTime.Now}]");
                    foreach (var messageModel in MessagesModels)
                    {
                        fileWriter.WriteLine($"[{DateTime.Now}]{messageModel.Username}: {messageModel.Message}");
                    }
                }
                else
                {
                    File.AppendAllText(fileName, $"\nChat History for [{DateTime.Now}]\n");
                    foreach (var messageModel in MessagesModels)
                    {
                        File.AppendAllText(fileName, $"[{DateTime.Now}]{messageModel.Username}: {messageModel.Message}\n");
                    }
                }
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                if(fileWriter != null)
                {
                    fileWriter.Close();
                }
            }
        }
    }
}
