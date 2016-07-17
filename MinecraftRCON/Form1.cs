using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace MinecraftRCON
{
    public partial class Form1 : Form
    {
        RCON rcon = new RCON();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
        }//sign in按钮

        private void button2_Click(object sender, EventArgs e)
        {
            rcon.SignOut();
            toolStripStatusLabel1.Text = "Lost";
            toolStripStatusLabel1.ForeColor = Color.Red;
            rcon = new RCON();

            {
                button2.Enabled = false;
                button1.Enabled = true;
                groupBox5.Enabled = true;
                groupBox6.Enabled = false;
                textBox4.Enabled = false;
                button3.Enabled = false;
            }

        }//sign out按钮

        private void button3_Click(object sender, EventArgs e)
        {
            rcon.Command = textBox4.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//send按钮

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rcon.Command = textBox4.Text;
                listBox2.Items.Add(rcon.RunCommand());
            }
        }//命令发送回车
        
        private void button7_Click(object sender, EventArgs e)//give按钮
        {
            rcon.Command = ("give " + textBox15.Text + " " + textBox5.Text + " " + textBox6.Text);
            listBox2.Items.Add(rcon.RunCommand());
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            rcon.Command = "op " + textBox15.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//OP按钮

        private void button6_Click(object sender, EventArgs e)
        {
            rcon.Command = "deop " + textBox15.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//deop按钮

        private void button9_Click(object sender, EventArgs e)
        {
            rcon.Command = ("tp " + textBox15.Text + " " + textBox7.Text + " " + textBox8.Text + " " + textBox9.Text);
            listBox2.Items.Add(rcon.RunCommand());
        }//tp按钮
        
        private void button8_Click(object sender, EventArgs e)
        {
            string mode;
            mode = comboBox2.Text;
            if (mode != "") mode = mode.Substring(0, 1);
            rcon.Command = "gamemode " + mode + " " + textBox15.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//gamemode按钮

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) connect();
        }

        private void connect()
        {

            try
            {
                rcon.Address = textBox1.Text;
                rcon.Password = textBox2.Text;
                rcon.Port = Convert.ToInt16(textBox3.Text);
            }
            catch
            {
                MessageBox.Show("Error,Please check", "Sign in error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            bool a;
            if (rcon.Connect()) a = rcon.SignIn();
            else
            {
                MessageBox.Show("Connect error", "Sign in error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (a)//如果登录成功则开启其他功能
            {
                toolStripStatusLabel1.Text = "Connect";
                toolStripStatusLabel1.ForeColor = Color.Green;

                {
                    button1.Enabled = false;
                    button2.Enabled = true;
                    groupBox5.Enabled = true;
                    groupBox6.Enabled = true;
                    textBox4.Enabled = true;
                    button3.Enabled = true;
                }
            }
            else MessageBox.Show("Password is wrong", "Sign in error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            rcon.Command = "kick " + textBox15.Text + " " + textBox10.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//kick按钮

        private void button11_Click(object sender, EventArgs e)
        {
            rcon.Command = "clear " + textBox15.Text + " " + textBox11.Text + " " + textBox12.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//clear按钮

        private void button12_Click(object sender, EventArgs e)
        {
            rcon.Command = "tell " + textBox15.Text + " " + textBox13.Text;
            listBox2.Items.Add(rcon.RunCommand());
        }//tell按钮
        
    }



    class RCON
    {
        //把地址绑定到Socket
        TcpClient client;
        private string address;//地址
        private string password;//密码
        private string command;//命令
        private int port;//端口

        public RCON(string ar, string pw)
        {
            address = ar;
            password = pw;
        }
        public RCON()
        { }


        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string Command
        {
            get { return command; }
            set { command = value; }
        }
        public int Port
        {
            get { return port; }
            set { port = value; }
        }


        private byte[] GetPassword()
        {
            byte[] numberPassword = Encoding.Default.GetBytes(password);//密码组
            byte lengh = Convert.ToByte(numberPassword.Length + 9);//密码长度+9
            byte[] template = { lengh, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0 };//数据模板
            byte[] zero = { 0 };//数据模板
            byte[] passWord;
            passWord = new byte[4 + lengh];
            template.CopyTo(passWord, 0);
            numberPassword.CopyTo(passWord, template.Length);
            zero.CopyTo(passWord, passWord.Length - 1);
            return passWord;
        }
        private byte[] GetCommand()
        {
            byte[] numberPassword = Encoding.UTF8.GetBytes(command);//命令组
            byte lengh = Convert.ToByte(numberPassword.Length + 9);//命令长度+9
            byte[] template = { lengh, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0 };//数据模板
            byte[] zero = { 0 };//数据模板
            byte[] returnCommand;
            returnCommand = new byte[4 + lengh];
            template.CopyTo(returnCommand, 0);
            numberPassword.CopyTo(returnCommand, template.Length);
            zero.CopyTo(returnCommand, returnCommand.Length - 1);
            return returnCommand;
        }
        private string GetReturn(byte[] buffer)
        {
            int num = buffer[0];
            byte[] Copy = buffer;
            buffer = new byte[num - 10];
            int i = 12;
            while (i < (num + 2))
            {
                buffer[i - 12] = Copy[i];
                i++;
            }
            return Encoding.UTF8.GetString(buffer, 0, num - 10);
        }

        public bool Connect()
        {

            try
            {
                //获取服务端IP
                IPHostEntry hostinfo = Dns.GetHostByName(address);
                IPAddress[] ipadd = hostinfo.AddressList;
                string result = ipadd[0].ToString();
                //要连接的远程IP
                IPAddress remoteHost = IPAddress.Parse(result);
                //IP地址跟端口的组合
                IPEndPoint iep = new IPEndPoint(remoteHost, port);
                //连接远程服务器
                client = new TcpClient();
                client.Connect(iep);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        public bool SignIn()
        {
            NetworkStream streamToServer = client.GetStream();
            //获取登录命令
            byte[] buffer = GetPassword();
            //发送登录命令
            try
            {
                lock (streamToServer)
                {
                    streamToServer.Write(buffer, 0, buffer.Length);     // 发往服务器
                }
                buffer = new byte[14];
                lock (streamToServer)
                {
                    streamToServer.Read(buffer, 0, 14);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Error", "Sign in error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (buffer[4] == 0 && (buffer[5] == 0) && (buffer[6] == 0) && (buffer[7] == 0) && buffer[0] == 10 && (buffer[8]) == 2) return true;
            else return false;
        }
        public void SignOut()
        {
            //断开连接
            client.Close();
        }
        public string RunCommand()
        {
            //获取需发送的命令
            byte[] buffer = GetCommand();
            if (client.Connected)
            {
                NetworkStream streamToServer = client.GetStream();
                try
                {
                    lock (streamToServer)
                    {
                        streamToServer.Write(buffer, 0, buffer.Length);     // 发往服务器
                    }
                    buffer = new byte[250];
                    lock (streamToServer)
                    {
                        streamToServer.Read(buffer, 0, 250);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("Error", "Command error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SignOut();
                    return "Error";
                }
                int num = buffer[0];
                return GetReturn(buffer);
            }
            else return "lost";
        }



    }
}
