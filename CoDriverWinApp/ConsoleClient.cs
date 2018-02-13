using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using EpServerEngine.cs;

namespace CoDriverWinApp
{
    public class ConsoleClient : INetworkClientCallback
    {
        INetworkClient m_client = new IocpTcpClient();

        public bool IsConnect = false;
        public string hostName;
        public void Update()
        {
            if(!IsConnect)
            {
                Connect();
            }
            return;
        }
        public void Connect()
        {
            if (!IsConnect)
            {
                hostName = Properties.Settings.Default.HostIP;
                //string hostname = tbHostname.Text;
                //string port = tbPort.Text;
                //tbHostname.Enabled = false;
                //tbPort.Enabled = false;
                //tbSend.Enabled = true;
                //btnSend.Enabled = true;
                //btnConnect.Text = "Disconnect";
                string port;
                port = string.Format("{0:D3}", Properties.Settings.Default.PortServer);
                ClientOps ops = new ClientOps(this, hostName, port);
                m_client.Connect(ops);
                IsConnect = true;
            }
            else
            {
                //tbHostname.Enabled = true;
                //tbPort.Enabled = true;
                //btnConnect.Text = "Connect";
                //tbSend.Enabled = false;
                //btnSend.Enabled = false;
                if (m_client.IsConnectionAlive)
                {
                    m_client.Disconnect();
                    IsConnect = false;
                }
            }
        }

        //public void SendMsg_RequestVideoList()
        //{
        //    string sendData = TransferProtocol.Msg_RequestVideoList_CS("1");
        //    Send(sendData);
        //}
        //public void SendMsg_RequestUploadVideo(string msgData)
        //{
        //    string sendData = TransferProtocol.Msg_RequestUploadVideo_CS(msgData);
        //    Send(sendData);
        //}
        public void Send(string sendText)
        {
            if (sendText.Length <= 0)
            {
                Program.AddLog("Please type in something to send.");
            }
            byte[] bytes = BytesFromString(sendText);
            Packet packet = new Packet(bytes, 0, bytes.Count(), false);
            m_client.Send(packet);
        }

        public void OnConnected(INetworkClient client, ConnectStatus status)
        {
            if(status == ConnectStatus.SUCCESS)
                Program.AddLog("CONNECT Success!");
            if (status == ConnectStatus.FAIL_TIME_OUT)
                Program.AddLog("CONNECT failed due to time out!");
            if (status == ConnectStatus.FAIL_ALREADY_CONNECTED)
                Program.AddLog("CONNECT failed due to already connected!");
            if (status == ConnectStatus.FAIL_SOCKET_ERROR)
                Program.AddLog("CONNECT Socket Error!");
        }

        public void OnReceived(INetworkClient client, Packet receivedPacket)
        {
            string dataReceived = StringFromByteArr(receivedPacket.PacketRaw);
            Program.AddLog("Received: " + dataReceived);
            Program.g_MsgReceiver.ReceivedMsg(dataReceived);
            return;
        }

        public void OnSent(INetworkClient client, SendStatus status, Packet sentPacket)
        {
            switch (status)
            {
                case SendStatus.SUCCESS:
                    Debug.WriteLine("SEND Success");
                    break;
                case SendStatus.FAIL_CONNECTION_CLOSING:
                    Debug.WriteLine("SEND failed due to connection closing");
                    break;
                case SendStatus.FAIL_INVALID_PACKET:
                    Debug.WriteLine("SEND failed due to invalid socket");
                    break;
                case SendStatus.FAIL_NOT_CONNECTED:
                    Debug.WriteLine("SEND failed due to no connection");
                    break;
                case SendStatus.FAIL_SOCKET_ERROR:
                    Debug.WriteLine("SEND Socket Error");
                    break;
            }
        }

        public void OnDisconnect(INetworkClient client)
        {
            //tbHostname.Enabled = true;
            //tbPort.Enabled = true;
            //btnConnect.Text = "Connect";
            //tbSend.Enabled = false;
            //btnSend.Enabled = false;
            IsConnect = false;
            Program.AddLog("Disconnected from the server!");
        }



        String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        byte[] BytesFromString(String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
