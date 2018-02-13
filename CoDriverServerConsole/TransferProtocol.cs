using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDriverServerConsole
{
    public enum MsgTypeSC
    {
        HeartBeat = 1,
        SendVideoList,
        ResUploadVideo,
        ResDownloadVideo,
        ResDownloadImages,
    }
    public enum MsgTypeCS
    {
        HeartBeat = 1,
        ReqUploadVideo,
        ReqVideoList,
        ReqDownloadVideo,
        ReqDownloadImages,
    }
    class TransferProtocol
    {
        
        static public string InitMsgData(MsgTypeCS msgIdx)
        {
            return string.Format("{0}#", (int)msgIdx);
        }
        static public string InitMsgData(MsgTypeSC msgIdx)
        {
            return string.Format("{0}#", (int)msgIdx);
        }
        static char[] charMsgSeparators = new char[] { '#' };
        static public string[] GetMsgStruct(string msgData, out int idx)
        {
            var values = msgData.Split(charMsgSeparators, StringSplitOptions.RemoveEmptyEntries);
            idx = Convert.ToInt32(values[0]);
            return values;
        }
        static public string Msg_HeartBeat()
        {
            return "0#";
        }
        static public string Msg_RequestClientInfo()
        {
            return "1#";
        }
        static public string Msg_RequestClientState()
        {
            return "2#";
        }
        static public string Msg_SendTask_SC(int taskIdx,string taskData)
        {
            string msg = string.Format("3#{0}#{1}", taskIdx, taskData);
            return msg;
        }
        static public string Msg_SendTask_CS(int taskIdx,string client, string taskData)
        {
            string msg = string.Format("3#{0}#{1}#{2}", taskIdx, client,taskData);
            return msg;
        }
        static public string Msg_SendVideoList_SC(string taskData)
        {
            string msg = string.Format("2#{0}", taskData);
            return msg;
        }
        static public string Msg_RequestUploadVideo_CS(string taskData)
        {
            string msg = string.Format("5#{0}", taskData);
            return msg;
        }
        static public string Msg_ResponseUploadVideo_SC(string taskData)
        {
            string msg = string.Format("5#{0}", taskData);
            return msg;
        }
        static public string Msg_ResponseDownloadImages_SC(string taskData)
        {
            string msg = string.Format("6#1#{0}", taskData);
            return msg;
        }
        static public string Msg_ResponseDownloadVideo_SC(string taskData)
        {
            string msg = string.Format("6#2#{0}", taskData);
            return msg;
        }
    }
}
