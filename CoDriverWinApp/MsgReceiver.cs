using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDriverWinApp
{
    public delegate void MsgReceivedEventHandler(string[] msgData);
    public class MsgReceivedEvent
    {
        public MsgReceivedEvent(MsgTypeSC e)
        {
            msgId = (int)e;
            Program.g_MsgReceiver.AddMsgDefine(msgId, this);
        }
        public void Run(string[] msgData)
        {
            msgReceived.Invoke(msgData);
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
        public void ReceivedMsg(string msgData)
        {
            int msgIdx;
            string[] msgStr = TransferProtocol.GetMsgStruct(msgData, out msgIdx);
            if (!msgDictionary.ContainsKey(msgIdx))
                return;
            msgDictionary[msgIdx].Run(msgStr);
        }


        /************************************ start define msg ***************************************/
        public void Init()
        {
            MsgReceivedEvent mre = new MsgReceivedEvent(MsgTypeSC.SendVideoList);
            mre.msgReceived += Msg_SendVideoList;
            MsgReceivedEvent mre2 = new MsgReceivedEvent(MsgTypeSC.ResUploadVideo);
            mre2.msgReceived += Msg_ResUploadVideo;
            MsgReceivedEvent mre3 = new MsgReceivedEvent(MsgTypeSC.ResDownloadVideo);
            mre3.msgReceived += Msg_ResDownloadVideo;
            MsgReceivedEvent mre4 = new MsgReceivedEvent(MsgTypeSC.ResDownloadImages);
            mre4.msgReceived += Msg_ResDownloadImages;
        }


        private static void Msg_SendVideoList(string[] msgData)
        {
            Program.g_videoInfoList.AddVideoListFromMsg(msgData);
            return;
        }
        private static void Msg_ResUploadVideo(string[] msgData)
        {
            int res = Convert.ToInt32(msgData[1]);
            if(res == 0)
            {
                Program.g_ClientData.upload_progress = 2;
            }
            else
            {
                Program.g_ClientData.upload_progress = 0;
                Program.g_ClientData.error_code = (int)MsgTypeSC.ResUploadVideo * 10000 + res;
            }
            return;
        }
        private static void Msg_ResDownloadVideo(string[] msgData)
        {
            return;
        }
        private static void Msg_ResDownloadImages(string[] msgData)
        {
            return;
        }
    }


}
