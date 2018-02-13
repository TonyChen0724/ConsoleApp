using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using EpServerEngine.cs;
using System.IO;

namespace CoDriverServerConsole
{
    public class ClientSocket
    {
        public INetworkSocket socket;
        public string hostname;
        public string loginname;
        public string reportEmail;
        public DateTime loginTime;
        public DateTime logoutTime;
        public DateTime connectTime;
        public DateTime reportTime;
        public int client_step;
        public TimeSpan stayTime;
        public int gameStep;
        public int uploadState;     
    }

    public class ClientSocketManager
    {
        Dictionary<INetworkSocket, ClientSocket> m_socketList = new Dictionary<INetworkSocket, ClientSocket>();
        int socket_count = 0;
        public ClientSocketManager()
        {

        }
        public int GetSocketCount()
        {
            return m_socketList.Count;
        }
        public string GetSocketDesc(INetworkSocket inSocket)
        {
            if (!m_socketList.ContainsKey(inSocket))
                return "Null Socket.";

            ClientSocket socket = m_socketList[inSocket];
            string text;
            text = string.Format("Host:{0}.Name:{1}.ConnectTime:{2:HH:mm:ss.fff}.LoginTime:{3:HH:mm:ss.fff}.{4}.Stay:{5:dd\\.hh\\:mm\\:ss}"
                , socket.hostname, socket.loginname, socket.connectTime, socket.loginTime, socket.client_step, socket.stayTime);
            return text;
        }
        public bool IsContainSocket(INetworkSocket inSocket)
        {
            return m_socketList.ContainsKey(inSocket);
        }
        public ClientSocket GetClientSocket(INetworkSocket inSocket)
        {
            return m_socketList[inSocket];
        }
        public INetworkSocket GetSocket(int idx)
        {
            return m_socketList.ElementAt(idx).Key;
        }
        public string GetSocketDesc(int idx)
        {
            ClientSocket socket = m_socketList.ElementAt(idx).Value;
            string text;
            text = string.Format("Host:{0}.Name:{1}.ConnectTime:{2:HH:mm:ss.fff}.LoginTime:{3:HH:mm:ss.fff}.{4}.Stay:{5:dd\\.hh\\:mm\\:ss}"
                , socket.hostname, socket.loginname, socket.connectTime, socket.loginTime, socket.client_step, socket.stayTime);
            return text;
        }
        public void AddSocket(INetworkSocket socket)
        {
            socket_count++;
            ClientSocket client = new ClientSocket();
            client.socket = socket;
            client.hostname = socket.IPInfo.IPAddress;
            client.connectTime = DateTime.Now;
            client.gameStep = 0;
            client.uploadState = 0;
            m_socketList.Add(socket, client);
        }

        public void RemoveSocket(INetworkSocket socket)
        {
            m_socketList.Remove(socket);            
        }

        
        public void MsgReceived(INetworkSocket socket, string dataReceived)
        {
            if(!IsContainSocket(socket))
            {
                Program.AddLog("Wrong socket!");
                return;
            }
            Program.AddLog(socket.IPInfo.IPAddress + " [Received] " + dataReceived);
            Program.g_MsgReceiver.ReceivedMsg(socket, dataReceived);
            return;

            //int msgIdx;
            //string[] msgStr = TransferProtocol.GetMsgStruct(dataReceived, out msgIdx);
            //int Idx = Convert.ToInt32(msgStr[0]);
            //ClientSocket clientSocket = GetClientSocket(socket);
            //if (msgStr[0] == "5")
            //{
            //    // Upload Files Done.
            //    if(msgStr[1] == "11")
            //    {
            //        Program.videoCollectionFunc.CheckVideoPath(0);
            //        clientSocket.uploadState = 0;
            //        return;
            //    }
            //    int nRes = Program.videoCollectionFunc.CheckIsPossibleUpload(msgStr[1]);
            //    if (clientSocket.uploadState == 0)
            //        nRes = 2;
            //    string sendData = Convert.ToString(nRes);
            //    if(nRes == 0)
            //    {
            //        sendData += "#";
            //        int ftpServerIdx = 1;
            //        sendData += Convert.ToString(ftpServerIdx);
            //        clientSocket.uploadState = 1;
            //    }
            //    string msgData = TransferProtocol.Msg_ResponseUploadVideo_SC(sendData);
            //    SendData(socket,msgData);
            //    return;
            //}
            //if (msgStr[0] == "6")
            //{
            //    if (msgStr[1] == "1")
            //    {
            //        int nRes = Program.videoCollectionFunc.CheckIsPossibleDownloadImages(msgStr[2]);
            //        string sendData = Convert.ToString(nRes);
            //        if (nRes == 0)
            //        {
            //            sendData += "#";
            //            int ftpServerIdx = 1;
            //            sendData += Convert.ToString(ftpServerIdx);
            //            clientSocket.uploadState = 1;
            //        }
            //        string msgData = TransferProtocol.Msg_ResponseDownloadImages_SC(sendData);
            //        SendData(socket, msgData);
            //    }
            //    if (msgStr[1] == "2")
            //    {
            //        int nRes = Program.videoCollectionFunc.CheckIsPossibleDownload(msgStr[2]);
            //        string sendData = Convert.ToString(nRes);
            //        if (nRes == 0)
            //        {
            //            sendData += "#";
            //            int ftpServerIdx = 1;
            //            sendData += Convert.ToString(ftpServerIdx);
            //            clientSocket.uploadState = 1;
            //        }
            //        string msgData = TransferProtocol.Msg_ResponseDownloadVideo_SC(sendData);
            //        SendData(socket, msgData);
            //    }
            //    return;
            //}
            //if (msgStr[0] == "3")
            //{
            //    string[] infoData = Program.videoCollectionFunc.GetVideoInfoList();
            //    foreach(var node in infoData)
            //    {
            //        string msgData = TransferProtocol.Msg_SendVideoList_SC(node);
            //        SendData(socket, msgData);
            //    }
            //    return;
            //}
            //return;
        }
       
        public void SendData(INetworkSocket socket, string strData)
        {
            byte[] bytes = BytesFromString(strData);
            Program.AddLog(socket.IPInfo.IPAddress + " [Send] "  +  strData);
            socket.Send(new Packet(bytes, 0, bytes.Count(), false));
        }
        byte[] BytesFromString(String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        byte[] BytesFromStringWithMsgId(byte msgId, String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char) + 1];
            bytes[0] = msgId;
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 1, str.Length * sizeof(char));
            return bytes;
        }
    }
    public class ConsoleServer : INetworkServerAcceptor, INetworkServerCallback, INetworkSocketCallback
    {
        //public ListBox SocketList;
        public ClientSocketManager m_socket_manager = new ClientSocketManager();
        INetworkServer m_server = new IocpTcpServer();


        Dictionary<INetworkSocket, int> m_socket_item_list = new Dictionary<INetworkSocket, int>();

        public void OnServerStarted(INetworkServer server, StartStatus status)
        {
            if (status == StartStatus.FAIL_ALREADY_STARTED || status == StartStatus.SUCCESS)
            {
                if (status == StartStatus.SUCCESS)
                    Program.AddLog("Server Socket Started");
                if (status == StartStatus.FAIL_ALREADY_STARTED)
                    Program.AddLog("Server already started");
            }
            else
                Program.AddLog("Unknown Error occurred");
        }

        public bool OnAccept(INetworkServer server, IPInfo ipInfo)
        {
            return true;
        }

        public INetworkSocketCallback GetSocketCallback()
        {
            return this;
        }
        public void Send(INetworkSocket socket,string sendText)
        {
            if (sendText.Length <= 0)
            {
                Program.AddLog("Please type in something to send.");
            }
            m_socket_manager.SendData(socket, sendText);
        }
        public void OnServerAccepted(INetworkServer server, INetworkSocket socket)
        {
        }
        public void OnServerStopped(INetworkServer server)
        {
        }
        public bool IsServerStarted = false;
        public bool Start_ConsoleServer()
        {
            if (!IsServerStarted)
            {
                string port;
                port = string.Format("{0:D3}", Settings.Default.PortServer);
                ServerOps ops = new ServerOps(this, port, this);
                m_server.StartServer(ops);
                IsServerStarted = true;
            }
            else
            {
                if (m_server.IsServerStarted)
                {
                    m_server.StopServer();
                    IsServerStarted = false;
                    Program.AddLog("Server Stopped");
                }
            }
            return IsServerStarted;
        }

        public void OnNewConnection(INetworkSocket socket)
        {
            m_socket_manager.AddSocket(socket);
            String sendString = "** New user(" + socket.IPInfo.IPAddress + ") connected!";
            Program.AddLog(sendString);

            //UpdateSocketList();
        }

        public void OnReceived(INetworkSocket socket, Packet receivedPacket)
        {
            if (receivedPacket.PacketByteSize <= 0)
            {
                String sendString = "User(" + socket.IPInfo.IPAddress + "): null packet!";
                Program.AddLog(sendString);
                return;
            }
            string dataReceived = StringFromByteArr(receivedPacket.PacketRaw);
            m_socket_manager.MsgReceived(socket, dataReceived);
            return;
        }
        String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public void OnSent(INetworkSocket socket, SendStatus status, Packet sentPacket)
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
        public void OnDisconnect(INetworkSocket socket)
        {
            m_socket_manager.RemoveSocket(socket);
            String sendString = "** User(" + socket.IPInfo.IPAddress + ") disconnected!";
            Program.AddLog(sendString);

            //UpdateSocketList();
        }

        private Dictionary<string, string> simulatedDataLib = new Dictionary<string, string>();
        public void LoadSimulatedData()
        {
            StreamReader reader = new StreamReader("SimulatedCommandLine.ini");
            int count = 0;
            char[] charSeparators = new char[] { '=', ';', ',' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 3)
                    continue;
                if (line[0] == ';')
                    continue;
                if (line[0] == '/'
                    && line[1] == '/')
                    continue;               
                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;

                simulatedDataLib.Add(values[0], values[1]);
                count++;
            }
            reader.Close();
            return;
        }
        public string GetSimulatedData(string keyword)
        {
            if (simulatedDataLib.ContainsKey(keyword))
                return simulatedDataLib[keyword];
            return "";
        }

    }
}
