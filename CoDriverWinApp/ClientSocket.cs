using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpServerEngine.cs;
namespace CoDriverMessageSegments
{
    /**
    * Enumerates message segment types.
    */
    enum Type
    {
        None,

        /** Request to abort the sending of a message. */
        Abort,

        /** Acknowledges that the message was received successfully. */
        Acknowledge,

        /** Notifies the bus that an endpoint has left. */
        Quit,

        /** A message data segment. */
        Data,

        TriggerEvent,

        /** Notifies the bus that an endpoint has joined. */
        Login,

        /** Request to retransmit selected data segments. */
        Retransmit,

        /** Notification that an inbound message timed out. */
        Timeout,

        HeartBeat,

        ClientStep,

        ReportResult

    };
}

namespace CoDriverWinApp
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
        public struct ReportResult
        {
            //public ReportResult()
            //{
            //    result = 0;
            //    count_lowSpd = 0;
            //    count_overSpd = 0;
            //    count_overSpd_corner = 0;
            //    count_StopOnRoad = 0;
            //    count_wrong_steering = 0;
            //    stop_time = 0;
            //    stop_time_intersection = 0;
            //    wrong_side_time = 0;
            //    wrong_side_time_corner = 0;
            //    wrong_side_time_startarea = 0;
            //    wrong_side_time_intersection = 0;
            //    off_road_time = 0;
            //    speedResult = new int[3];   //0 Legal speed limits were exceeded.  1  Speed was sometimes to slow.   2  stopped the vehicle in a very dangerous place.
            //    cornerResult = new int[3];  //0 Corner speed was sometimes excessive. 1 steering technique.  2 centreline was crossed during corner.
            //    intersectionResult = new int[3]; //0 Turned onto the wrong side of the road.  1 Didn't respond to STOP sign. 2 give way.
            //    roadPosResult = new int[3]; //0 chose the wrong side of the road. 1 Didn't stay left. 2 Drove off the side of the road.
            //}
            public int result;  // 0 low 1 middle  2 high
            public int count_lowSpd;
            public int count_overSpd;
            public int count_overSpd_corner;
            public int count_StopOnRoad;
            public int count_wrong_steering;
            public float stop_time;
            public float stop_time_intersection;
            public float wrong_side_time;
            public float wrong_side_time_corner;
            public float wrong_side_time_startarea;
            public float wrong_side_time_intersection;
            public float off_road_time;


            public int[] speedResult;
            public int[] cornerResult;
            public int[] intersectionResult;
            public int[] roadPosResult; 

            
        };
        public ReportResult reportRes;
        public int gameStep;

        public void WriteToFile()
        {
            string filename = string.Format("Report/{0}_{1}_{2:HHmmss}.txt", loginname,DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
            System.IO.StreamWriter report_file = new System.IO.StreamWriter(filename);

            string str_data;
            str_data = string.Format("Login Name: {0}", loginname);
            report_file.WriteLine(str_data);
            str_data = string.Format("Host:{0}", hostname);
            report_file.WriteLine(str_data);
            str_data = string.Format("Login Time: {0} {1:HH:mm:ss}.", loginTime.ToString("yyyy-MM-dd"), loginTime);
            report_file.WriteLine(str_data);
            str_data = string.Format("Report Time: {0} {1:HH:mm:ss}.", reportTime.ToString("yyyy-MM-dd"), reportTime);
            report_file.WriteLine(str_data);
            str_data = string.Format("Result: {0}.", reportRes.result);
            report_file.WriteLine(str_data);
            str_data = string.Format("      Speed: {0} {1} {2}.", reportRes.speedResult[0], reportRes.speedResult[1], reportRes.speedResult[2]);
            report_file.WriteLine(str_data);
            str_data = string.Format("      Intersection: {0} {1} {2}.", reportRes.intersectionResult[0], reportRes.intersectionResult[1], reportRes.intersectionResult[2]);
            report_file.WriteLine(str_data);
            str_data = string.Format("      Corner: {0} {1} {2}.", reportRes.cornerResult[0], reportRes.cornerResult[1], reportRes.cornerResult[2]);
            report_file.WriteLine(str_data);
            str_data = string.Format("      RoadPosition: {0} {1} {2}.", reportRes.roadPosResult[0], reportRes.roadPosResult[1], reportRes.roadPosResult[2]);
            report_file.WriteLine(str_data);
            str_data = string.Format("Details: OverSpd {0},LowSpd {1},OverSpdCorner {2}.", reportRes.count_overSpd, reportRes.count_lowSpd, reportRes.count_overSpd_corner);
            report_file.WriteLine(str_data);
            str_data = string.Format("Details: Stop {0},StopTime {1}s,StopTimeIntersection {2}s.", reportRes.count_StopOnRoad, reportRes.stop_time, reportRes.stop_time_intersection);
            report_file.WriteLine(str_data);
            str_data = string.Format("Details: WrongSide time {0}s,corner time {1}s,start area {2}s, intersection {3}s.", reportRes.wrong_side_time, reportRes.wrong_side_time_corner, reportRes.wrong_side_time_startarea, reportRes.wrong_side_time_intersection);
            report_file.WriteLine(str_data);
            str_data = string.Format("Details: OffRoad time {0}s,wrong steering {1}s.", reportRes.off_road_time, reportRes.count_wrong_steering);
            report_file.WriteLine(str_data);

            report_file.Close();

        }
        public void UpdateReportResult(byte[] bytes)
        {
            int size = bytes.Length;
            byte[] data_bytes = new byte[size - 24];
            Array.Copy(bytes, 24, data_bytes, 0, size - 24);
            string str = System.Text.Encoding.UTF8.GetString(data_bytes);
            string[] strArr = str.Split(';');
            is_update_report_result = true;

            reportRes.result = bytes[1];
            reportRes.speedResult[0] = bytes[2];
            reportRes.speedResult[1] = bytes[3];
            reportRes.speedResult[2] = bytes[4];
            reportRes.cornerResult[0] = bytes[5];
            reportRes.cornerResult[1] = bytes[6];
            reportRes.cornerResult[2] = bytes[7];
            reportRes.intersectionResult[0] = bytes[8];
            reportRes.intersectionResult[1] = bytes[9];
            reportRes.intersectionResult[2] = bytes[10];
            reportRes.roadPosResult[0] = bytes[11];
            reportRes.roadPosResult[1] = bytes[12];
            reportRes.roadPosResult[2] = bytes[13];

            reportRes.count_lowSpd = Convert.ToInt32(strArr[0]);
            reportRes.count_overSpd = Convert.ToInt32(strArr[1]);
            reportRes.count_overSpd_corner = Convert.ToInt32(strArr[2]);
            reportRes.count_StopOnRoad = Convert.ToInt32(strArr[3]);
            reportRes.count_wrong_steering = Convert.ToInt32(strArr[4]);

            reportRes.stop_time = Convert.ToSingle(strArr[5]);
            reportRes.stop_time_intersection = Convert.ToSingle(strArr[6]);
            reportRes.wrong_side_time = Convert.ToSingle(strArr[7]);
            reportRes.wrong_side_time_corner = Convert.ToSingle(strArr[8]);
            reportRes.wrong_side_time_intersection = Convert.ToSingle(strArr[9]);
            reportRes.wrong_side_time_startarea = Convert.ToSingle(strArr[10]);
            reportRes.off_road_time = Convert.ToSingle(strArr[11]);

            reportTime = DateTime.Now;

            WriteToFile();

        }
        public bool is_update_report_result;
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
                , socket.hostname,socket.loginname,socket.connectTime,socket.loginTime,socket.client_step, socket.stayTime);           
            return text;
        }
        public void AddSocket(INetworkSocket socket)
        {
            socket_count ++;
            ClientSocket client = new ClientSocket();
            client.socket = socket;
            client.hostname = socket.IPInfo.IPAddress;
            client.connectTime = DateTime.Now;
            client.reportRes.speedResult = new int[3];
            client.reportRes.intersectionResult = new int[3];
            client.reportRes.cornerResult = new int[3];
            client.reportRes.roadPosResult = new int[3];
            client.gameStep = 0;
            client.is_update_report_result = false;
            m_socketList.Add(socket, client);
        }

        public void RemoveSocket(INetworkSocket socket)
        {
            m_socketList.Remove(socket);

            //foreach (var pair in m_socketList)
            //{
            //    ClientSocket value = pair.Value;
            //    if (value.socket == socket)
            //    {
                    
            //    }
            //}
        }
        public void SendMsg_HeartBeat(INetworkSocket socket)
        {
            string msgData = TransferProtocol.Msg_HeartBeat();
            byte[] bytes  = BytesFromString(msgData);
            socket.Send(new Packet(bytes, 0, bytes.Count(), false));
        }
        public void SendMsg_RequestClientInfo(INetworkSocket socket)
        {
            string msgData = TransferProtocol.Msg_RequestClientInfo();
            byte[] bytes = BytesFromString(msgData);
            socket.Send(new Packet(bytes, 0, bytes.Count(), false));
        }
        public void SendMsg_RequestClientState(INetworkSocket socket)
        {
            string msgData = TransferProtocol.Msg_RequestClientState();
            byte[] bytes = BytesFromString(msgData);
            socket.Send(new Packet(bytes, 0, bytes.Count(), false));
        }
       


        public void MsgReceived(INetworkSocket socket, int msgId, Packet receivedPacket)
        {
            bool isContain = m_socketList.ContainsKey(socket);
            if (!isContain)
            {
                Program.AddLog("Errpr, socket list.");
                return;
            }
            if (msgId == (int)CoDriverMessageSegments.Type.HeartBeat)
            {
                m_socketList[socket].stayTime = DateTime.Now - m_socketList[socket].connectTime;
                SendMsg_HeartBeat(socket);
            }
            if (msgId == (int)CoDriverMessageSegments.Type.Login)
            {
                byte[] bytes = receivedPacket.PacketRaw;
                String login_name = StringFromByteArrWithMsgId(bytes);
                m_socketList[socket].loginname = login_name;
                m_socketList[socket].loginTime = DateTime.Now;
            }
            if (msgId == (int)CoDriverMessageSegments.Type.ReportResult)
            {
                byte[] bytes = receivedPacket.PacketRaw;
                
                m_socketList[socket].UpdateReportResult(bytes);

                String sendString = "User(" + socket.IPInfo.IPAddress + "): Report Result.";
                Program.AddLog(sendString);

            }

            if (msgId == (int)CoDriverMessageSegments.Type.TriggerEvent)
            {
                byte[] bytes = receivedPacket.PacketRaw;
                int size = receivedPacket.PacketByteSize;
                byte[] data_bytes = new byte[size - 16];
                Array.Copy(bytes,8,data_bytes,0, size - 16);
                string str = System.Text.Encoding.UTF8.GetString(data_bytes);
                String sendString = "User(" + socket.IPInfo.IPAddress + "): Trigger Event: " + str;
                Program.AddLog(sendString);
            }

            if (msgId == (int)CoDriverMessageSegments.Type.ClientStep)
            {
                byte[] bytes = receivedPacket.PacketRaw;
                int step = bytes[1];
                m_socketList[socket].gameStep = step;
                String sendString = "User(" + socket.IPInfo.IPAddress + "): Step Update " + step;
                Program.AddLog(sendString);
            }
            //float.Parse("41.00027357629127", CultureInfo.InvariantCulture.NumberFormat);
        }

        String StringFromByteArrWithMsgId(byte[] bytes)
        {
            char[] chars = new char[bytes.Length - 1];
            int end_idx = 0;
            for (int i = 0; i < bytes.Length - 1; i++)
            {
                byte temp = bytes[i + 1];
                
                chars[i] = (char)temp;
                if (temp == 0)
                {
                    end_idx = i;
                    break;
                }
            }
            string outStr = new string(chars);
            
            return outStr.Substring(0,end_idx);
        }
        String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            // string str = System.Text.Encoding.UTF8.GetString(bytes);
            for (int i = 0; i < bytes.Length; i++)
            {
                byte temp = bytes[i];
                chars[i] = (char)temp;
            }
            //    System.Buffer.SetByte(chars, i, bytes[i]);
            //System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
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

}
