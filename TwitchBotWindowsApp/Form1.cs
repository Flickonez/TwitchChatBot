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
    public partial class Form1 : Form
    {
        #region Variables
        //

        private static string userName = "jesusavgnbot";
        private static string password = "oauth:1z3eltepu9ujkg9or9yggpfmi9y04v";
        private static string TwitchClientID = "165053598";
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
        //@"C:\subday_bot\content\
        IniFile GamesIni = new IniFile(@"C:\subday_bot\content\games.ini");
        IniFile PointsIni = new IniFile(@"C:\subday_bot\content\points.ini");
        private static string htmlFile = @"C:\subday_bot\content\subday.html";
        Random rnd = new Random();
        private static string username = "";
        private static string nickname = "";


        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        //TODO: Комментарии к каждому методу, свойству или переменной

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

                }
            }

        }

        private void msg()
        {
            if (this.InvokeRequired) this.Invoke(new MethodInvoker(msg));
            else
            {
                string[] separator = new string[] { "#jesusavgn :" };
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
                catch (Exception e) { }
                //chatBox.Text = chatBox.Text + readData.ToString() + Environment.NewLine; - нахуя это
            }
        }

        private void commands(string username, string message)
        {
            string fullcommand = message;
            string command = message.Split(new[] { ' ', '!', }, StringSplitOptions.None)[1]; // !command

            switch (command.ToLower())
            {
                case "addgame":
                    try
                    {
                        if (readData.Contains("subscriber=0")) break;
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
                        irc.sendChatMessage("@" + username + ", что-то пошло не так. Игра, выбранная вами, не была добавлена в список!");
                        break;
                    }
                case "датьвсем":
                    if (username == "jesusavgn")
                    {
                        amountint = 0;
                        string amount = fullcommand.Replace("!датьвсем", "");
                        amount = amount.Trim();
                        if (amount == "") break;
                        try { amountint = Convert.ToInt32(amount); }
                        catch (Exception e)
                        { }
                        if (amountint < 0) break;
                        irc.sendChatMessage("@" + username + ", даём всем пользователям по " + amountint + " BTC...");
                        ViewerListUpdate();
                        backgroundWorker2.RunWorkerAsync();
                    }
                    break;
                case "донат":
                    if (!commandSpamFilter_gift)
                    {
                        commandSpamFilter_gift = true;
                        int howmuchpoints_int = 0;
                        string pointsfrom = PointsIni.IniReadValue("#jesusavgn." + username, "Points");
                        if (pointsfrom == "")
                        {
                            irc.sendChatMessage("/w " + username + " У вас не хватает BTC!");
                            PointsIni.IniWriteValue("#jesusavgn." + username, "Points", "0");
                            break;
                        }
                        int pointsfrom_int = Convert.ToInt32(pointsfrom);
                        if (pointsfrom_int < 50)
                        {
                            irc.sendChatMessage("/w " + username + " Для того, чтобы передать BTC другому человеку, на вашем счету должно быть не менее 50 BTC!");
                            break;
                        }
                        string[] separ = new string[] { " " };
                        string nickpointsto = fullcommand.Split(separ, StringSplitOptions.None)[1];
                        string pointsto = PointsIni.IniReadValue("#jesusavgn." + nickpointsto, "Points");
                        if (pointsto == "") break;
                        string howmuchpoints = fullcommand.Split(separ, StringSplitOptions.None)[2];
                        try { howmuchpoints_int = Convert.ToInt32(howmuchpoints); } catch (Exception e) { break; }
                        if (howmuchpoints_int <= pointsfrom_int && howmuchpoints_int > 0)
                        {
                            AddPoints(username, -howmuchpoints_int, 0);
                            AddPoints(nickpointsto, howmuchpoints_int, 0);
                            irc.sendChatMessage(username + " подарил " + nickpointsto + " " + howmuchpoints_int + " BTC!");
                        }
                        else
                        {
                            irc.sendChatMessage("/w " + username + " У вас не хватает BTC!");
                        }
                    }
                    break;
                case "дать":
                    if (username == "jesusavgn")
                    {
                        ViewerListUpdate();
                        int howmuch_give_int = 0;
                        string[] separ_give = new string[] { " " };
                        string nickpointsgive = fullcommand.Split(separ_give, StringSplitOptions.None)[1];
                        nickpointsgive = nickpointsgive.Trim().ToLower();
                        string howmuch_give = fullcommand.Split(separ_give, StringSplitOptions.None)[2];
                        string pointsgive_check = PointsIni.IniReadValue("#jesusavgn." + nickpointsgive, "Points");
                        int findnick = viewersBox.FindStringExact(nickpointsgive + Environment.NewLine, -1);
                        if (findnick == -1) { irc.sendChatMessage("@" + username + ", неверно указан ник человека, которому вы хотите добавить BTC."); break; }
                        if (pointsgive_check == "") PointsIni.IniWriteValue("#jesusavgn." + nickpointsgive, "Points", "0");
                        try { howmuch_give_int = Convert.ToInt32(howmuch_give); } catch (Exception e) { irc.sendChatMessage("@" + username + ", неверно указано количество BTC."); break; }
                        if (howmuch_give_int <= 0)
                        {
                            irc.sendChatMessage("@" + username + ", неверно указано количество BTC.");
                            break;
                        }
                        AddPoints(nickpointsgive, howmuch_give_int, 0);
                        irc.sendChatMessage("@" + username + ", вы добавили " + howmuch_give_int + " BTC пользователю с ником " + nickpointsgive + ".");
                    }
                    break;
                case "btc":
                    if (!commandSpamFilter)
                    {
                        commandSpamFilter = true;
                        string yourpoints = PointsIni.IniReadValue("#jesusavgn." + username, "Points");
                        if (yourpoints == "")
                        {
                            yourpoints = "0";
                            AddPoints(username, double.Parse(yourpoints), 0);
                        }
                        else
                        {
                            double thepoints = double.Parse(yourpoints);
                            if (thepoints < 0)
                            {
                                AddPoints(username, 0 - thepoints, 0);
                                yourpoints = PointsIni.IniReadValue("#jesusavgn." + username, "Points");
                            }
                        }
                        irc.sendChatMessage("@" + username + ", У вас " + yourpoints + " BTC.");
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
                string choiced_game = GamesIni.IniReadValue("#jesusavgn." + username, "Game");
                GamesIni.IniWriteValue("#jesusavgn." + username, "Game", game);
                if (choiced_game == "")
                {
                    irc.sendChatMessage("@" + username + ", игра " + game + " была успешно добавлена!");
                    string[] htmlLine = File.ReadAllLines(htmlFile);
                    htmlLine[64] = htmlLine[64] + Environment.NewLine + @"<tr id=" + "\"" + username + "\"" + "><td>" + username + "</td><td>" + game + "</td></tr>";
                    File.WriteAllLines(htmlFile, htmlLine);

                }
                else
                {
                    irc.sendChatMessage("@" + username + ", вы успешно заменили " + choiced_game + " на " + game + " .");
                    string[] htmlLine = File.ReadAllLines(htmlFile);
                    int k = 0;
                    for (int i = 0; i < htmlLine.Length; i++)
                    {
                        if (htmlLine[i].Contains("<td>" + username))
                        {
                            htmlLine[i] = "<tr id=" + "\"" + username + "\"" + "><td>" + username + "</td><td>" + game + "</td></tr>";
                            File.WriteAllLines(htmlFile, htmlLine);
                            break;
                            k = 1;
                        }
                        if (k == 1) break;
                    }
                }
            }
            catch (Exception e)
            {
                GamesIni.IniWriteValue("#jesusavgn." + username, "Game", game);
                irc.sendChatMessage("@" + username + ", игра " + game + " была успешно добавлена!");
                string[] htmlLine = File.ReadAllLines(htmlFile);
                htmlLine[64] = htmlLine[64] + Environment.NewLine + @"<tr id=" + "\"" + username + "\"" + "><td>" + username + "</td><td>" + game + "</td></tr>";
                File.WriteAllLines(htmlFile, htmlLine);
            }
        }

        private void AddPoints(string username, double points, double hrs)
        {
            double finalnumber = 0;
            try
            {
                string[] separator = new string[] { @"\r\n" };
                username = username.Trim().ToLower();
                string pointsofuser = PointsIni.IniReadValue("#jesusavgn." + username, "Points");
                double numberofpoints = double.Parse(pointsofuser);
                finalnumber = Convert.ToDouble(numberofpoints + points);
                PointsIni.IniWriteValue("#jesusavgn." + username, "Points", finalnumber.ToString());
            }
            catch (Exception e)
            {
                PointsIni.IniWriteValue("#jesusavgn." + username, "Points", points.ToString());
            }
        }

        private void Btc_PrivateMessage(string username)
        {
            string yourpoints = PointsIni.IniReadValue("#jesusavgn." + username, "Points");
            if (yourpoints == "")
            {
                yourpoints = "0";
                AddPoints(username, double.Parse(yourpoints), 0);
            }
            else
            {
                double thepoints = double.Parse(yourpoints);
                if (thepoints < 0)
                {
                    AddPoints(username, 0 - thepoints, 0);
                    yourpoints = PointsIni.IniReadValue("#jesusavgn." + username, "Points");
                }
            }
            irc.sendChatMessage("/w " + username + " У вас " + yourpoints + " BTC.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            irc.JoinRoom("jesusavgn");
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

        private void button1_Click(object sender, EventArgs e)
        {
            chatBox.Clear();
            ViewerListUpdate();
            //backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            irc.sendChatMessage("!subday");
        }

        private void ViewerListUpdate()
        {
            viewersBox.Items.Clear();
            Chatters AllChatters = ChatClient.GetChatters("jesusavgn");
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

        public void timer_addpoints_Tick(object sender, EventArgs e)
        {
            ViewerListUpdate();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (APIClient.IsLive("jesusavgn"))
            {
                foreach (string username in viewersBox.Items)
                {
                    AddPoints(username, rnd.Next(6, 13), 8);
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
            Btc_PrivateMessage(nickname);
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

