using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using TwitchCSharp.Clients;
using TwitchCSharp.Models;


namespace TwitchBotWindowsApp
{
    public partial class TwitchBot : Form
    {
#warning Before using, replace all "streamer" on username of your stream channel and take a look into "Variables"
          // And remember that if your bot was not added to the whitelist of the twitch, messages in /w might not come to some users
         // To fix this, send an application to add your bot to the whitelist, or don't use /w, but in this case bot will clog your chat
        #region Variables

        private static string userName = "username"; //Bot's username
        private static string password = "oauth:1a2b3c"; //Oauth password, take it here: https://twitchapps.com/tmi/
        private static string TwitchClientID = "123456789"; // your client ID
        private static int amountint = 0;
        TwitchReadOnlyClient APIClient = new TwitchReadOnlyClient(TwitchClientID);
        TwitchROChat ChatClient = new TwitchROChat(TwitchClientID);
        IrcClient irc = new IrcClient("irc.chat.twitch.tv", 6667, userName, password);
        NetworkStream serverStream = default(NetworkStream);
        string readData = "";
        Thread chatThread;

        bool commandSpamFilter = false;
        bool commandSpamFilter_kus = false;
        bool commandSpamFilter_gift = false;
        //if for example path to the bot folder is: @"C:\subday_bot\content\, then you need: 
        IniFile GamesIni = new IniFile(@"C:\subday_bot\content\games.ini"); //path to the games.ini file
        IniFile PointsIni = new IniFile(@"C:\subday_bot\content\points.ini"); //path to the points.ini file
        private static string htmlFile = @"C:\subday_bot\content\subday.html"; //path to the subday.html file
        Random rnd = new Random();
        private static string username = "";
        private static string nickname = "";


        #endregion

        public TwitchBot()
        {
            InitializeComponent();
        }

        private void GetMessage()
        {
            serverStream = irc.tcpClient.GetStream();
            int buffsize = 0;
            byte[] inStream = new byte[10025];
            buffsize = irc.tcpClient.ReceiveBufferSize;
            while (true)
            {
                try
                {
                    readData = irc.readMessage();
                    msg();
                }
                catch (Exception e)
                {
                    chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                }
            }

        }

        private void msg()
        {
            if (this.InvokeRequired) this.Invoke(new MethodInvoker(msg));
            else
            {
                string[] separator = new string[] { "#streamer :" };
                string[] nickname_sep = new string[] { "@", "." };
                string[] nickname_sep2 = new string[] { ":", "!" };
                try
                {
                    if (readData.Contains("PING")) {
                        timer_PingResponse.Stop();
                        timer_PingResponse.Start();
                        backgroundWorker_irc_response.RunWorkerAsync();
                    }
                    if (readData.Contains("PRIVMSG"))
                    {
                        string message = readData.Split(separator, StringSplitOptions.None)[1];
                        nickname = readData.Split(nickname_sep, StringSplitOptions.None)[2];

                        if (message[0] == '!')
                        {
                            commands(nickname, message);
                            chatBox.Text = chatBox.Text + nickname + ": " + message + ";" + Environment.NewLine;
                        }
                    }
                }
                catch (Exception e)
                {
                    chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                }
               
            }
        }

        private void commands(string username, string message)
        {
            string fullcommand = message;
            string command = message.Split(new[] { ' ', '!', }, StringSplitOptions.None)[1]; // !command

            switch (command.ToLower())
            {
                case "addgame": // command "addgame" allows to subs add any game to the list
                    try        // example: /addgame Don't Starve
                    {
                        if (readData.Contains("subscriber=0")) break; //if non-sub try to add game nothing will happen
                        string tmp = fullcommand.Split(new[] { '!', ' ' }, StringSplitOptions.None)[1];
                        string tmp_2 = tmp.ToLower();
                        fullcommand = fullcommand.Replace(tmp, tmp_2);
                        string game = fullcommand.Replace("!addgame", "");
                        if (game == "") break;
                        game = game.TrimStart();
                        AddGame(username, game);
                        break;
                    }
                    catch (Exception e)
                    {
                        chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                        irc.sendChatMessage("@" + username + ", something went wrong. The game, you selected was not added to the list!");
                        break;
                    }
                case "givetoall": // give to all specified amount of points, command only for streamer, example: /givetoall 50
                    if (username == "streamer")
                    {
                        amountint = 0;
                        string amount = fullcommand.Replace("!givetoall", "");
                        amount = amount.Trim();
                        if (amount == "") break;
                        try { amountint = Convert.ToInt32(amount); }
                        catch (Exception e)
                        {
                            chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                        }
                        if (amountint < 0) break;
                        irc.sendChatMessage("@" + username + ", gives all users " + amountint + " points...");
                        ViewerListUpdate();
                        backgroundWorker2.RunWorkerAsync();
                    }
                    break;
                case "donate": // Allows you to transfer points to other users, example: /donate Jepe34 100
                    if (!commandSpamFilter_gift)
                    {
                        commandSpamFilter_gift = true;
                        int howmuchpoints_int = 0;
                        string pointsfrom = PointsIni.IniReadValue("#streamer." + username, "Points");
                        if (pointsfrom == "")
                        {
                            irc.sendChatMessage("/w " + username + " you don't have enough points!");
                            PointsIni.IniWriteValue("#streamer." + username, "Points", "0");
                            break;
                        }
                        int pointsfrom_int = Convert.ToInt32(pointsfrom);
                        if (pointsfrom_int < 50)
                        {
                            irc.sendChatMessage("/w " + username + " To transfer points to another user on your account should be at least 50 points!");
                            break;
                        }
                        string[] separ = new string[] { " " };
                        string nickpointsto = fullcommand.Split(separ, StringSplitOptions.None)[1];
                        string pointsto = PointsIni.IniReadValue("#streamer." + nickpointsto, "Points");
                        if (pointsto == "") break;
                        string howmuchpoints = fullcommand.Split(separ, StringSplitOptions.None)[2];
                        try { howmuchpoints_int = Convert.ToInt32(howmuchpoints); } catch (Exception e) { chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine; break; }
                        if (howmuchpoints_int <= pointsfrom_int && howmuchpoints_int > 0)
                        {
                            AddPoints(username, -howmuchpoints_int);
                            AddPoints(nickpointsto, howmuchpoints_int);
                            irc.sendChatMessage(username + " gave " + nickpointsto + " " + howmuchpoints_int + " points!");
                        }
                        else
                        {
                            irc.sendChatMessage("/w " + username + " you don't have enough points!");
                        }
                    }
                    break;
                case "give": // Only for streamer. Allows to give some points to the specific user, example: /give Jepe34 100
                    if (username == "streamer")
                    {
                        ViewerListUpdate();
                        int howmuch_give_int = 0;
                        string[] separ_give = new string[] { " " };
                        string nickpointsgive = fullcommand.Split(separ_give, StringSplitOptions.None)[1];
                        nickpointsgive = nickpointsgive.Trim().ToLower();
                        string howmuch_give = fullcommand.Split(separ_give, StringSplitOptions.None)[2];
                        string pointsgive_check = PointsIni.IniReadValue("#streamer." + nickpointsgive, "Points");
                        int findnick = viewersBox.FindStringExact(nickpointsgive + Environment.NewLine, -1);
                        if (findnick == -1) { irc.sendChatMessage("@" + username + ", the nickname of the person you want to give points to is incorrect"); break; }
                        if (pointsgive_check == "") PointsIni.IniWriteValue("#streamer." + nickpointsgive, "Points", "0");
                        try { howmuch_give_int = Convert.ToInt32(howmuch_give); } catch (Exception e)
                        {
                            chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                            irc.sendChatMessage("@" + username + ", incorrect number of points!");
                            break;
                        }
                        if (howmuch_give_int <= 0)
                        {
                            irc.sendChatMessage("@" + username + ", incorrect number of points!");
                            break;
                        }
                        AddPoints(nickpointsgive, howmuch_give_int);
                        irc.sendChatMessage("@" + username + ", you gave " + howmuch_give_int + " points to the " + nickpointsgive + ".");
                    }
                    break;
                case "points": // Allows to see the number of your points
                    if (!commandSpamFilter)
                    {
                        commandSpamFilter = true;
                        string yourpoints = PointsIni.IniReadValue("#streamer." + username, "Points");
                        if (yourpoints == "")
                        {
                            yourpoints = "0";
                            AddPoints(username, double.Parse(yourpoints));
                        }
                        else
                        {
                            double thepoints = double.Parse(yourpoints);
                            if (thepoints < 0)
                            {
                                AddPoints(username, 0 - thepoints);
                                yourpoints = PointsIni.IniReadValue("#streamer." + username, "Points");
                            }
                        }
                        irc.sendChatMessage("@" + username + ", you have " + yourpoints + " points.");
                    }
                    else
                    {
                    backgroundWorker3.RunWorkerAsync();
                    }
                    break;
                default:
                    break;
            }
        }

        private void AddGame(string username, string game)
        {
            try
            {
                string[] separator = new string[] { @"\r\n" };
                username = username.Trim().ToLower();
                string choiced_game = GamesIni.IniReadValue("#streamer." + username, "Game");
                GamesIni.IniWriteValue("#streamer." + username, "Game", game);
                if (choiced_game == "")
                {
                    irc.sendChatMessage("@" + username + ", game " + game + " was successful added!");
                    string[] htmlLine = File.ReadAllLines(htmlFile);
                    htmlLine[64] = htmlLine[64] + Environment.NewLine + @"<tr id=" + "\"" + username + "\"" + "><td>" + username + "</td><td>" + game + "</td></tr>";
                    File.WriteAllLines(htmlFile, htmlLine);
                }
                else
                {
                    irc.sendChatMessage("@" + username + ", you successful replaced " + choiced_game + " with " + game + " .");
                    string[] htmlLine = File.ReadAllLines(htmlFile);
                    int k = 0;
                    for (int i = 0; i < htmlLine.Length; i++)
                    {
                        if (htmlLine[i].Contains("<td>" + username))
                        {
                            htmlLine[i] = "<tr id=" + "\"" + username + "\"" + "><td>" + username + "</td><td>" + game + "</td></tr>";
                            File.WriteAllLines(htmlFile, htmlLine);
                            break;
                        }
                        if (k == 1) break;
                    }
                }
            }
            catch (Exception e)
            {
                chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                GamesIni.IniWriteValue("#streamer." + username, "Game", game);
                irc.sendChatMessage("@" + username + ", game " + game + " was successful added!");
                string[] htmlLine = File.ReadAllLines(htmlFile);
                htmlLine[64] = htmlLine[64] + Environment.NewLine + @"<tr id=" + "\"" + username + "\"" + "><td>" + username + "</td><td>" + game + "</td></tr>";
                File.WriteAllLines(htmlFile, htmlLine);
            }
        }

        private void AddPoints(string username, double points) 
        {
            double finalnumber = 0;
            try
            {
                string[] separator = new string[] { @"\r\n" };
                username = username.Trim().ToLower();
                string pointsofuser = PointsIni.IniReadValue("#streamer." + username, "Points");
                double numberofpoints = double.Parse(pointsofuser);
                finalnumber = Convert.ToDouble(numberofpoints + points);
                PointsIni.IniWriteValue("#streamer." + username, "Points", finalnumber.ToString());
            }
            catch (Exception e)
            {
                chatBox.Text = chatBox.Text + "Exception catched: " + e.Message + Environment.NewLine;
                PointsIni.IniWriteValue("#streamer." + username, "Points", points.ToString());
            }
        }

        private void Points_PrivateMessage(string username)
        {
            string yourpoints = PointsIni.IniReadValue("#streamer." + username, "Points");
            if (yourpoints == "")
            {
                yourpoints = "0";
                AddPoints(username, double.Parse(yourpoints));
            }
            else
            {
                double thepoints = double.Parse(yourpoints);
                if (thepoints < 0)
                {
                    AddPoints(username, 0 - thepoints);
                    yourpoints = PointsIni.IniReadValue("#streamer." + username, "Points");
                }
            }
            irc.sendChatMessage("/w " + username + " You have " + yourpoints + " points.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            irc.JoinRoom("streamer"); // nickname of streamer, connecting to the the chat
            chatThread = new Thread(GetMessage);
            chatThread.Start();
            timer_PingResponse.Start();
            timer_clear.Start();
            timer_addpoints.Start();
            CommandSpamTimer.Start();
            ViewerListUpdate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            irc.LeaveRoom();
            serverStream.Dispose();
            Environment.Exit(0);
        }

        private void timer_clear_Tick(object sender, EventArgs e)
        {
            chatBox.Clear();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            chatBox.Clear();
            ViewerListUpdate();
        }

        private void ViewerListUpdate() // Getting all viewers of the stream and adding them to the viewersBox
        {
            viewersBox.Items.Clear();
            Chatters AllChatters = ChatClient.GetChatters("streamer");
            chatBox.Text += "Checking the viewer list..." + Environment.NewLine;

            foreach (string admin in AllChatters.Admins)
            {
                viewersBox.Items.Add(admin + Environment.NewLine);
            }

            foreach (string staff in AllChatters.Staff)
            {
                viewersBox.Items.Add(staff + Environment.NewLine);
            }

            foreach (string globalmod in AllChatters.GlobalMods)
            {
                viewersBox.Items.Add(globalmod + Environment.NewLine);
            }

            foreach (string moderator in AllChatters.Moderators)
            {
                viewersBox.Items.Add(moderator + Environment.NewLine);
            }

            foreach (string viewer in AllChatters.Viewers)
            {
                viewersBox.Items.Add(viewer + Environment.NewLine);
            }
        }

        public void timer_addpoints_Tick(object sender, EventArgs e) //timer to adding points, it runs every 8 minutes and add from 6 to 12 points to every user, wathing the stream
        {
            ViewerListUpdate();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (APIClient.IsLive("streamer")) // checking stream online
            {
                foreach (string username in viewersBox.Items)
                {
                    AddPoints(username, rnd.Next(6, 13));
                }
            }
        }

        private void CommandSpamTimer_Tick(object sender, EventArgs e)
        {
            commandSpamFilter = false;
        }

        private void CommandSpamTimer_gift_Tick(object sender, EventArgs e)
        {
            commandSpamFilter_gift = false;
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            Points_PrivateMessage(nickname);
        }

        private void backgroundWorker_irc_response_DoWork(object sender, DoWorkEventArgs e)
        {
            irc.PingResponse();
        }


        private void timer_PingResponse_Tick(object sender, EventArgs e)
        {
            irc.sendIrcMessage("PING :tmi.twitch.tv");
        }
    }
    }

