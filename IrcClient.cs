using System.Net.Sockets;
using System.IO;

namespace TwitchBotWindowsApp
{
    class IrcClient
    {
        private string userName;
        private string channel;

        public TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;


        public IrcClient(string ip, int port, string userName, string password)
        {
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + userName);
            outputStream.WriteLine("USER " + userName + " 8 * :" + userName);
            outputStream.WriteLine("CAP REQ :twitch.tv/membership");
            outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            outputStream.WriteLine("CAP REQ :twitch.tv/tags");
            outputStream.Flush();

        }

        public void JoinRoom(string channel)
        {
            this.channel = channel;
            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
        }

        public void LeaveRoom()
        {
            outputStream.Close();
            inputStream.Close();
        }

        public void sendIrcMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void sendChatMessage(string message)
        {
            sendIrcMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
        }

        public void PingResponse()
        {
            sendIrcMessage("PONG :tmi.twitch.tv\r\n");
        }

        public string readMessage()
        {
            string message = "";
            message = inputStream.ReadLine();
            return message;
        }

    }
}
