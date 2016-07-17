using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ClassLibrary1
{
    public class Class1
    {

        //把地址绑定到Socket
        TcpClient client;
        private string address;//地址
        private string password;//密码
        private string command;//命令
        private int port;//端口

        public Class1(string ar, string pw)
        {
            address = ar;
            password = pw;
        }
        public Class1()
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
                IPHostEntry hostinfo = Dns.GetHostByName(address);//获取域名所对应的IP
                IPAddress[] ipadd = hostinfo.AddressList;//这三句都是获取IP的一部分
                string result = ipadd[0].ToString();//把IP转换成字符串
                //要连接的远程IP
                IPAddress remoteHost = IPAddress.Parse(result);
                //IP地址跟端口的组合
                IPEndPoint iep = new IPEndPoint(remoteHost, port);//获取带端口的地址
                //连接远程服务器
                client = new TcpClient();//创建连接的实例
                client.Connect(iep);//执行这个实例的叫做“连接”的方法
                return true;//若成功则返回真
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);//这句话不用管
                return false;//若连接失败则会跳到这里
            }

        }
        public bool SignIn()
        {
            NetworkStream streamToServer = client.GetStream();
            //获取登录命令
            byte[] buffer = GetPassword();//这是数组
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
                return false;//出错则会跳到这里
            }
            if (buffer[4] == 0 && (buffer[5] == 0) && (buffer[6] == 0) && (buffer[7] == 0) && buffer[0] == 10 && (buffer[8]) == 2) return true;//这是重点，根据服务端返回的值确定密码是否正确

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
