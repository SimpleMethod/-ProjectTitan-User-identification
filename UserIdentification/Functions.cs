using System;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;

namespace UserIdentification
{

    class Functions
    {
        private static string IP = "127.0.0.1";
        private static int Port = 9997;

        private static string HashData(string Data)
        {

            byte[] Hash;
            using (MD5 Md5 = MD5.Create())
            {
                Md5.Initialize();
                Md5.ComputeHash(Encoding.ASCII.GetBytes(Data));
                Hash = Md5.Hash;
            }
            Data = Convert.ToBase64String(Hash).ToString();
            return Data.Substring(0, Math.Min(Data.Length, 6));

        }
        private static int RandomNumbers()
        {
            Random Random = new Random();
            int Result = 0;
            for (int i = 0; i < 4; ++i)
            {
                Result = Result + Random.Next(0, 9999);
            }
            return Result;
        }
        public static string MachineID()
        {
            string Data = "";
            try
            {

                ManagementObjectSearcher Cpu = new ManagementObjectSearcher("Select * From Win32_processor");
                ManagementObjectCollection Cpulist = Cpu.Get();

                foreach (ManagementObject CpuL in Cpulist)
                {
                    Data += (string)CpuL["ProcessorID"];
                }
                ManagementObjectSearcher Base = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectCollection BaseBordList = Base.Get();
                foreach (ManagementObject BaseL in BaseBordList)
                {
                    Data += (string)BaseL["SerialNumber"];
                }
            }
            catch (ManagementException e)
            {
                Data = RandomNumbers().ToString();
            }
            return HashData(Data);
        }
        public static void Server(string Data)
        {
            try
            {
                IPAddress AdressIP = IPAddress.Parse(IP);
                TcpListener TCPConnect = new TcpListener(AdressIP, Port);
                TCPConnect.Start();
                Socket TCPSocket = TCPConnect.AcceptSocket();
                byte[] b = new byte[100]; int k = TCPSocket.Receive(b);
                ASCIIEncoding SafeASCII = new ASCIIEncoding();
                TCPSocket.Send(SafeASCII.GetBytes(Data));
                TCPSocket.Close();
                TCPConnect.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Błąd:" + e.StackTrace);
            }

        }
    }
}
