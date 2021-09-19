using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Redworth
{
    public partial class Form1 : Form
    {
        public IRCHandler.IRCHandler ircClient;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
            {
                return; // continue only if the user pressed return to submit the query.
            }
            Dictionary<string, string> CommandUsageInfo = new();
            CommandUsageInfo.Add( "connect" , "Usage: /connect server port username [password]");

            if (textBox3.Text.ToLower().StartsWith("/"))
            {
                // trim the leading / and split remaining data into parsable chunks.
                string[] userInputs = textBox3.Text[1..].Split(" ");
                string userCommand = userInputs[0].ToLower();

                if (this.ircClient == null && !userCommand.Equals("connect"))
                {
                    // Error! not yet connected to server!
                    AppendTextBox("Error: unable to comply. Not connected to server.");
                    textBox3.Clear();
                    return;
                }
                try
                {
                    switch (userCommand)
                    {
                        case "connect":
                            if (userInputs.Length < 3)
                            {
                                AppendTextBox("Unable to comply. Insufficient arguments.");
                                AppendTextBox(CommandUsageInfo["connect"]);
                            }
                            string server = userInputs[1];
                            int port = Int32.Parse(userInputs[2]);
                            string nick = userInputs[3];
                            string pass = userInputs.Length > 4 ? userInputs[4] : "";
                            
                            AppendTextBox("Connecting...");
                            Task t1 = Task.Factory.StartNew(() => Connect(server, port, nick, pass));
                            AppendTextBox("Connecting......");
                            textBox3.Clear();
                            return;
                        case "quit": // TODO: quit functions, but message is wrong.
                            this.ircClient.Quit(userInputs.Length > 1 ? String.Join(" ", userInputs[1..]) : "");
                            break;
                        case "join":
                            this.ircClient.Join(userInputs[1]);
                            break;
                        case "part":
                            this.ircClient.Part(userInputs[1], String.Join(" ", userInputs[2..]));
                            break;
                        case "nick":
                            // TODO: implement.
                            break;
                        case "me":
                            // TODO: implement.
                            break;
                        case "msg":
                            this.ircClient.Privmsg(userInputs[1], String.Join(" ", userInputs[2..]));
                            break;
                        case "notice":// TODO: implement.
                            break;
                        case "ctcp":// TODO: implement.
                            break;
                        case "mode":// TODO: implement.
                            

                            break;
                        case "topic":// TODO: implement.
                            break;

                        default:
                            AppendTextBox($"Command {userCommand} not recognized.");
                            break;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    AppendTextBox($"Command {userCommand} missing arguments.");
                }
            }

            textBox3.Clear();
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            { // TODO: wtf does this do?
                this.BeginInvoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBox2.AppendText(value + "\r\n");
        }

        public void Connect(string targetAddr, int port = 6667, string username="redworth", string password="")
        {
            //string targetAddr = "irc.libera.chat";
            //int port = 31338;// 6667;
            //string username = "";
            //string password = "";
            Boolean SSLflag = false;

            ircClient = new IRCHandler.IRCHandler(targetAddr, port, username, SSLflag: SSLflag, password: password);

            treeView1.BeginInvoke(new Action<string>((s) => { treeView1.Nodes.Add(s); }), new object[] { targetAddr });

            ircClient.AddCallback("privmsg", (user, target, msg) =>
            {
                if (msg.StartsWith("!rejoin"))
                {
                    ircClient.Part(target, $"Rejoining {target}");
                    ircClient.Join(target);
                }
            });
            ircClient.AddCallback("join", (user, target, msg) =>
            {
                // add channel to the channel list when we join it.
                if (user.Equals(username) && !listBox1.Items.Contains(target))
                {
                    treeView1.BeginInvoke(
                        new Action<string>((s) => { treeView1.Nodes.Add(s); }),
                        new object[] { target });
                }
            });
            ircClient.AddCallback("part", (user, target, msg) =>
            {
                // rem channel from the channel list when we join it.
                if (user.Equals(username) && listBox1.Items.Contains(target))
                {
                    treeView1.BeginInvoke(
                        new Action<string>((s) =>
                        {
                            foreach (TreeNode node in listBox1.Items)
                            {
                                if (node.Text.Equals(target))
                                {
                                    listBox1.Items.Remove(node);
                                    break;
                                }
                            }

                        }),
                        new object[] { target });
                }
            });
            //ircClient.AddCallback("LIST", (user, target, msg) =>
            //{ // add nicks from channel listing to the listbox, but only if that channel is currently selected(?)
            //    foreach (string nick in msg.Split(" "))
            //    {  // 
            //        listBox1.BeginInvoke(new Action<string>((s) => { listBox1.Items.Add(s); }), new object[] { nick });
            //    }
            //});
            ircClient.AddCallback("TOPIC",
                (user, target, msg) =>
                {
                    textBox1.BeginInvoke(new Action<string>((s) =>
                    {
                        textBox1.Text = "";
                        textBox1.AppendText(s);
                    }), new object[] { msg });
                });
            foreach (string msgtype in "join part privmsg quit notice topic mode nick".Split(" "))
            {
                ircClient.AddCallback(msgtype, (user, target, msg) =>
                {
                    AppendTextBox($"{msgtype}: {{target:'{target}', user:'{user}', msg:'{msg}'}}");
                });
            }
            ircClient.Connect();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // move selected channel to the fore.
            
        }
        
    }
}
