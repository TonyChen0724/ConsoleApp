using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpServerEngine.cs;

namespace CoDriverServerConsole
{
    public delegate void MsgReceivedEventHandler(INetworkSocket socket, string[] msgData);
    public class MsgReceivedEvent
    {
        public MsgReceivedEvent(MsgTypeCS e)
        {
            msgId = (int)e;
            Program.g_MsgReceiver.AddMsgDefine(msgId, this);
        }
        public void Run(INetworkSocket socket,string[] msgData)
        {
            msgReceived.Invoke(socket,msgData);
        }
        public int msgId;
        public event MsgReceivedEventHandler msgReceived;
    }
    class MsgReceiver
    {
        private Dictionary<int, MsgReceivedEvent> msgDictionary = new Dictionary<int, MsgReceivedEvent>();
        public MsgReceiver()
        {
        }

        public void AddMsgDefine(int idx, MsgReceivedEvent mre)
        {
            msgDictionary.Add(idx, mre);
        }
        public void ReceivedMsg(INetworkSocket socket, string msgData)
        {
            int msgIdx;
            string[] msgStr = TransferProtocol.GetMsgStruct(msgData, out msgIdx);
            if (!msgDictionary.ContainsKey(msgIdx))
                return;
            bool isValid = Program.g_console_server.m_socket_manager.IsContainSocket(socket);
            if (!isValid)
            {
                return;
            }
            msgDictionary[msgIdx].Run(socket,msgStr);
        }


        /************************************ start define msg ***************************************/
        public void Init()
        {
            MsgReceivedEvent mre = new MsgReceivedEvent(MsgTypeCS.ReqUploadVideo);
            mre.msgReceived += Msg_ReqUploadVideo;
            MsgReceivedEvent mre2 = new MsgReceivedEvent(MsgTypeCS.ReqVideoList);
            mre2.msgReceived += Msg_ReqVideoList;
            MsgReceivedEvent mre3 = new MsgReceivedEvent(MsgTypeCS.ReqDownloadVideo);
            mre3.msgReceived += Msg_ReqDownloadVideo;
            MsgReceivedEvent mre4 = new MsgReceivedEvent(MsgTypeCS.ReqDownloadImages);
            mre4.msgReceived += Msg_ReqDownloadImages;
        }


        private static void Msg_ReqUploadVideo(INetworkSocket socket, string[] msgData)
        {
            
            ClientSocket clientSocket = Program.g_console_server.m_socket_manager.GetClientSocket(socket);
            // Upload Files Done.
            if (msgData[1] == "11")
            {
                Program.videoCollectionFunc.CheckVideoPath(0);
                clientSocket.uploadState = 0;
                return;
            }
            string[] sendData = new string[2];
            int nRes = Program.videoCollectionFunc.CheckIsPossibleUpload(msgData[1]);
            //if (clientSocket.uploadState == 0)
            //    nRes = 2;
            sendData[0] = Convert.ToString(nRes);
            if (nRes == 0)
            {
                int ftpServerIdx = 1;
                sendData[1] += Convert.ToString(ftpServerIdx);
                clientSocket.uploadState = 1;
            }
            //string msgData = TransferProtocol.Msg_ResponseUploadVideo_SC(sendData);
            Program.g_MsgSender.SendMsg(socket, (int)MsgTypeSC.ResUploadVideo, sendData);
            return;
        }
        private static void Msg_ReqVideoList(INetworkSocket socket, string[] msgData)
        {
            string[] infoData = Program.videoCollectionFunc.GetVideoInfoList();
            foreach (var node in infoData)
            {
                //string msgData = TransferProtocol.Msg_SendVideoList_SC(node);
                //SendData(socket, msgData);
                string[] sendData = new string[1];
                sendData[0] = node;
                Program.g_MsgSender.SendMsg(socket, (int)MsgTypeSC.SendVideoList, sendData);
            }
            return;
        }
        private static void Msg_ReqDownloadVideo(INetworkSocket socket, string[] msgData)
        {
            ClientSocket clientSocket = Program.g_console_server.m_socket_manager.GetClientSocket(socket);
            int nRes = Program.videoCollectionFunc.CheckIsPossibleDownload(msgData[2]);
            string[] sendData = new string[2];
            sendData[0] = Convert.ToString(nRes);
            if (nRes == 0)
            {
                int ftpServerIdx = 1;
                sendData[1] = Convert.ToString(ftpServerIdx);
                clientSocket.uploadState = 1;
            }
            Program.g_MsgSender.SendMsg(socket, (int)MsgTypeSC.ResDownloadImages, sendData);
            return;
        }
        private static void Msg_ReqDownloadImages(INetworkSocket socket, string[] msgData)
        {
            ClientSocket clientSocket = Program.g_console_server.m_socket_manager.GetClientSocket(socket);
            int nRes = Program.videoCollectionFunc.CheckIsPossibleDownloadImages(msgData[2]);
            string[] sendData = new string[2];
            sendData[0] = Convert.ToString(nRes);
            if (nRes == 0)
            {
                int ftpServerIdx = 1;
                sendData[1] = Convert.ToString(ftpServerIdx);
                clientSocket.uploadState = 1;
            }
            Program.g_MsgSender.SendMsg(socket, (int)MsgTypeSC.ResDownloadImages, sendData);
            return;
        }
    }


}
