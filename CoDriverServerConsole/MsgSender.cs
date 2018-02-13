using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpServerEngine.cs;

namespace CoDriverServerConsole
{
    public delegate void MsgSendEventHandler(INetworkSocket socket,int idx,string[] msgData);
    public class MsgSendEvent
    {
        public MsgSendEvent(MsgTypeSC e)
        {
            msgId = (int)e;
            Program.g_MsgSender.AddMsgDefine(msgId, this);
        }
        public void Run(INetworkSocket socket,string[] msgData)
        {
            msgSendHandler.Invoke(socket,msgId, msgData);
        }
        public int msgId;
        public event MsgSendEventHandler msgSendHandler;
    }
    class MsgSender
    {

        //private Dictionary<MsgTypeCS, MsgTypeSC> msgLinkDic = new Dictionary<MsgTypeCS, MsgTypeSC>();
        private Dictionary<int, MsgSendEvent> msgDictionary = new Dictionary<int, MsgSendEvent>();
        public MsgSender()
        {
           // msgLinkDic.Add(MsgTypeCS.ReqUploadVideo, MsgTypeSC.ResUploadVideo);
           // msgLinkDic.Add(MsgTypeCS.ReqVideoList, MsgTypeSC.SendVideoList);
          //  msgLinkDic.Add(MsgTypeCS.ReqDownloadVideo, MsgTypeSC.ResDownloadVideo);
           // msgLinkDic.Add(MsgTypeCS.ReqDownloadImages, MsgTypeSC.ResDownloadImages);
        }

        public void AddMsgDefine(int idx, MsgSendEvent mse)
        {
            msgDictionary.Add(idx, mse);
        }

        public void SendMsg(INetworkSocket socket,int msgIdx,string[] msgData)
        {
            if (!msgDictionary.ContainsKey(msgIdx))
                return;
            msgDictionary[msgIdx].Run(socket,msgData);
        }

        /************************************ start define msg ***************************************/
        public void Init()
        {
            MsgSendEvent mse = new MsgSendEvent(MsgTypeSC.SendVideoList);
            mse.msgSendHandler += Msg_CommonRequest;
            MsgSendEvent mse1 = new MsgSendEvent(MsgTypeSC.ResUploadVideo);
            mse1.msgSendHandler += Msg_CommonRequest;
            MsgSendEvent mse2 = new MsgSendEvent(MsgTypeSC.ResDownloadVideo);
            mse2.msgSendHandler += Msg_CommonRequest;
            MsgSendEvent mse3 = new MsgSendEvent(MsgTypeSC.ResDownloadImages);
            mse3.msgSendHandler += Msg_CommonRequest;
        }
        private static void Msg_CommonRequest(INetworkSocket socket, int msgIdx,string[] msgData)
        {
            string sendData = TransferProtocol.InitMsgData((MsgTypeCS)msgIdx);
            foreach(var node in msgData)
            {
                sendData += node;
                sendData += "#";
            }
            Program.g_console_server.Send(socket,sendData);
        }
        static string[] default_msg_data = new string[1];
        public string[] GetReqUploadVideoEnd()
        {
            default_msg_data[0] = "11";
            return default_msg_data;
        }
        public string[] GetReqVideoList()
        {
            default_msg_data[0] = "1";
            return default_msg_data;
        }
        //private static void Msg_ReqVideoList(string[] msgData)
        //{
        //    string sendData = TransferProtocol.InitMsgData(MsgTypeCS.ReqVideoList);
        //    sendData += "1";
        //    Program.g_console_client.Send(sendData);
        //    return;
        //}
    }
}
