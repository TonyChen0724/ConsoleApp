using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class XMPGenerator
    {
        static double SMALL_NUMBER = 0.00000001;
        //public class FQuat
        //{
        //    double X;
        //    double Y;
        //    double Z;
        //    double W;
        //}
        public class FRotator
        {
            public FRotator(double p, double y, double r)
            {
                Pitch = p;
                Yaw = y;
                Roll = r;
            }
            /** Rotation around the right axis (around Y axis), Looking up and down (0=Straight Ahead, +Up, -Down) */
            public double Pitch;

            /** Rotation around the up axis (around Z axis), Running in circles 0=East, +North, -South. */
            public double Yaw;

            /** Rotation around the forward axis (around X axis), Tilting your head, 0=Straight, +Clockwise, -CCW. */
            public double Roll;

            public FRotator GetRightDirection()
            {
                double CP, SP, CY, SY;
                SinCos(out SP, out CP, DegreesToRadians(Pitch));
                SinCos(out SY, out CY, DegreesToRadians(Yaw));
                FVector V = new FVector(CP * CY, CP * SY, SP);
                FVector up = new FVector(0, 0, 1);
                FVector right = V.CrossProduct(up);
                
                FRotator outRot = new FRotator(0,0,0);
                outRot.MakeFromX(right);
                return outRot;
            }
            
            public double[] GetLeftDirMtx()
            {
                double CP, SP, CY, SY;
                SinCos(out SP, out CP, DegreesToRadians(Pitch));
                SinCos(out SY, out CY, DegreesToRadians(Yaw));
                FVector V = new FVector(CP * CY, CP * SY, SP);
                FVector up = new FVector(0, 0, 1);
                FVector right = V.CrossProduct(up);
                FVector left = right;
                left.X *= -1.0 * right.X;
                left.Y *= -1.0 * right.Y;
                left.Z *= -1.0 * right.Z;
                FRotator outRot = new FRotator(0, 0, 0);

                return outRot.MakeFromX(left);
            }
            public double[] GetRightDirMtx()
            {
                double CP, SP, CY, SY;
                SinCos(out SP, out CP, DegreesToRadians(Pitch));
                SinCos(out SY, out CY, DegreesToRadians(Yaw));
                FVector V = new FVector(CP * CY, CP * SY, SP);
                FVector up = new FVector(0, 0, 1);
                FVector right = V.CrossProduct(up);

                FRotator outRot = new FRotator(0, 0, 0);
                
                return outRot.MakeFromX(right); 
            }

            public double[] MakeFromX(FVector XAxis)
            {
                double[] mt = new double[9];
                FVector NewX = XAxis.GetSafeNormal();

                // try to use up if possible
                FVector UpVector = new FVector(0, 0, 1.0);// (Math.Abs(NewX.Z) < (1.0 - KINDA_SMALL_NUMBER)) ? FVector(0, 0, 1.f) : FVector(1.f, 0, 0);

                FVector NewY = (UpVector.CrossProduct(NewX)).GetSafeNormal();
                FVector NewZ = NewX.CrossProduct(NewY);

                //FRotator Rotator = new FRotator(Math.Atan2(NewX.Z, Math.Sqrt(NewX.X * NewX.X + NewX.Y * NewX.Y)) * 180.0 / PI,
                //                    Math.Atan2(XAxis.Y, XAxis.X) * 180.0 / PI,
                //                    0);

                //const FVector SYAxis = FRotationMatrix(Rotator).GetScaledAxis(EAxis::Y);
                //Rotator.Roll = FMath::Atan2(ZAxis | SYAxis, YAxis | SYAxis) * 180.f / PI;
                mt[0] = NewX.X;
                mt[1] = NewX.Y;
                mt[2] = NewX.Z;
                mt[3] = NewY.X;
                mt[4] = NewY.Y;
                mt[5] = NewY.Z;
                mt[6] = NewZ.X;
                mt[7] = NewZ.Y;
                mt[8] = NewZ.Z;

                return mt;
            }
        }
        public class FVector
        {
            public FVector(double x,double y,double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public double X;
            public double Y;
            public double Z;

            public double DotProduct(FVector V)
            {
                return X * V.X + Y * V.Y + Z * V.Z;
            }

            public FVector CrossProduct(FVector V)
            {
                FVector outVec = new FVector(Y * V.Z - Z * V.Y, Z * V.X - X * V.Z, X * V.Y - Y * V.X);
                return outVec;
            }
            public FVector GetSafeNormal()
            {
                double SquareSum = X * X + Y * Y + Z * Z;

                // Not sure if it's safe to add tolerance in there. Might introduce too many errors
                if (SquareSum == 1.0)
                {
                    return new FVector(X , Y, Z);
                }
                else if (SquareSum < SMALL_NUMBER)
                {
                    return new FVector(0, 0, 0);
                }
                double Scale = 1.0/ Math.Sqrt(SquareSum);
                return new FVector(X * Scale, Y * Scale, Z * Scale);
            }
        }
        public XMPGenerator()
        {
            isXMPSetting[0] = false;
            isXMPSetting[1] = false;
            isXMPSetting[2] = false;
            isXMPSetting[3] = false;
            isXMPSetting[4] = false;
        }
        XMPFile[] relativeMtXMPFiles = new XMPFile[5];
        FVector[] relPostion = new FVector[5];
        FRotator[] relRotator = new FRotator[5];
        string[] keywords = new string[5];
        int[] keyIndex = new int[5];
        bool[] isXMPSetting = new bool[5];
        string[] relativeXMPFilename = new string[5];

        //struct XMPFileInfo
        //{
        //   public XMPFile xmpfile;
        //    public FVector relPos;
        //    public FRotator relRot;
        //    public string keyword;
        //    public int keyIndex;
        //    public string xmp_filename;
        //}
        //XMPFileInfo[] xmp_FileInfo = new XMPFileInfo[5];
        struct jpgFileInfo
        {
            public string filename;
            public bool isXMP;
            public string xmp_filename;
            public XMPFile xmpFile;
            public FRotator rotation;
            public FVector position;
            public string rotationString;
        }
        Dictionary<int, jpgFileInfo> xmpFileDirectory0 = new Dictionary<int, jpgFileInfo>();
        Dictionary<int, jpgFileInfo> xmpFileDirectory1 = new Dictionary<int, jpgFileInfo>();
        Dictionary<int, jpgFileInfo> xmpFileDirectory2 = new Dictionary<int, jpgFileInfo>();
        Dictionary<int, jpgFileInfo> xmpFileDirectory3 = new Dictionary<int, jpgFileInfo>();
        Dictionary<int, jpgFileInfo> xmpFileDirectory4 = new Dictionary<int, jpgFileInfo>();
        public void BuildXMPDirectory(int index,string path)
        {
            if (path.Length < 3)
            {
                Program.AddLog("BuildXMPDirectory: wrong Path." + index + " " + path);
                return;
            }
            if (!Directory.Exists(path))
            {
                Program.AddLog("BuildXMPDirectory: No Path." + index + " " + path);
                return;
            }
            string[] files = Directory.GetFiles(path);
            foreach(var file in files)
            {
                string ext = Path.GetExtension(file);
                ext = ext.ToLower();
                if (ext == ".jpg")
                {
                    jpgFileInfo jfInfo = new jpgFileInfo();
                    jfInfo.filename = file;
                    string filename = Path.GetFileNameWithoutExtension(file);
                    string keyword = "";
                    int idx = 0;
                    bool isFilename = GetIndexAndKeyword(out keyword,out idx,filename);
                    if (!isFilename)
                        continue;
                    string xmpFileName = Path.ChangeExtension(file, "xmp");
                    jfInfo.xmp_filename = xmpFileName;
                    if (File.Exists(xmpFileName))
                    {
                        jfInfo.xmpFile = new XMPFile();
                        jfInfo.xmpFile.LoadXML(xmpFileName);
                        jfInfo.isXMP = true;
                        //if(index == 0)
                        {
                            jfInfo.rotationString = jfInfo.xmpFile.GetRotation();
                            jfInfo.rotation = GetRotator(jfInfo.xmpFile.GetRotation());
                            jfInfo.position = GetPostiion(jfInfo.xmpFile.GetPosition());
                        }
                    }
                    else
                    {
                        jfInfo.isXMP = false;
                    }
                    if (index == 0)
                        xmpFileDirectory0.Add(idx, jfInfo);
                    if (index == 1)
                        xmpFileDirectory1.Add(idx, jfInfo);
                    if (index == 2)
                        xmpFileDirectory2.Add(idx, jfInfo);
                    if (index == 3)
                        xmpFileDirectory3.Add(idx, jfInfo);
                    if (index == 4)
                        xmpFileDirectory4.Add(idx, jfInfo);
                }
            }

            Program.AddLog("Check XMP Folder:" + xmpFileDirectory0.Count + " " + xmpFileDirectory1.Count + " " + xmpFileDirectory2.Count + " " + xmpFileDirectory3.Count);
        }
        public void BuildCenterLineXMPFile()
        {

        }
        static public bool GetIndexAndKeyword(out string keyword,out int index,string strData)
        {
            keyword = "";
            index = -1;
            char[] charSeparators = new char[] { ' ', '_', '(', ')', '-' };
            var values = strData.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length < 2)
                return false;
            keyword = values[0];
            index = Convert.ToInt32(values[1]);
            return true;
        }
        public void SetRelativeXMPFile(int index ,string srcFile)
        {
            isXMPSetting[index] = false;
            if (srcFile.Length < 3)
            {
                Program.AddLog("Set RelativeM Data: wrong filename." + index + " " + srcFile);
                return;
            }
            if (!File.Exists(srcFile))
            {
                Program.AddLog("Set RelativeM Data: No file." + index + " " + srcFile);
                return;
            }
            string filename = Path.GetFileNameWithoutExtension(srcFile);
            GetIndexAndKeyword(out keywords[index], out keyIndex[index], filename);
            relativeXMPFilename[index] = srcFile;
            if (relativeMtXMPFiles[index] == null)
                relativeMtXMPFiles[index] = new XMPFile();
            relativeMtXMPFiles[index].LoadXML(srcFile);
            relPostion[index] = GetPostiion(relativeMtXMPFiles[index].GetPosition());
            relRotator[index] = GetRotator(relativeMtXMPFiles[index].GetRotation());
            isXMPSetting[index] = true;

            Program.AddLog("Set RelativeM Data: " + index + " " + srcFile);
        }

        public void BuildCalibXMPFile()
        {
            if (!isXMPSetting[4])
                return;
            XMPFile save_file = new XMPFile();
            save_file.LoadXML(relativeXMPFilename[4]);
            foreach (var node in xmpFileDirectory4)
            {
                int idx = node.Key;
                jpgFileInfo jfInfo = node.Value;
                if (!jfInfo.isXMP)
                {
                   // string posStr1 = jfInfo.xmpFile.GetPosition();
                    save_file.SetPosePrior("unknown");
                    save_file.RemoveNode(101);
                    save_file.RemoveNode(102);
                    save_file.SaveXML(jfInfo.xmp_filename);

                    Program.AddLog("SaveCalibXMP: new " + jfInfo.xmp_filename);
                    continue;
                }
                if(jfInfo.xmpFile.IsLockedData())
                {
                    continue;
                }
                //if(jfInfo.xmpFile.IsCalibrationData())
                //{
                //    string posStr1 = jfInfo.xmpFile.GetPosition();
                //    save_file.SetPosePrior();
                //    save_file.SetPosition(posStr1);
                //    save_file.RemoveNode(102);
                //    save_file.SaveXML(jfInfo.xmp_filename);
                //    continue;
                //}
                string posStr = jfInfo.xmpFile.GetPosition();
                save_file.CheckPositionNode();
                save_file.SetPosePrior("unknown");
                save_file.SetPosition(posStr);
                save_file.RemoveNode(102);
                save_file.SaveXML(jfInfo.xmp_filename);

                Program.AddLog("SaveCalibXMP: " + jfInfo.xmp_filename);

            }
            return;
        }

        public void BuildFixedRelXMPFile(string sampleXMPFilename)
        {
            int curIdx = 1;
            FVector relativePos = new FVector(0, 0, 0);
            FRotator relativeRotator = new FRotator(0, 0, 0);
            relativeRotator.Yaw = 90.0f;
            XMPFile save_file = new XMPFile();
            save_file.LoadXML(sampleXMPFilename);
            int key_offset = 0;// keyIndex[curIdx] - keyIndex[0];
            foreach (var node in xmpFileDirectory1)
            {
                int idx = node.Key;
                jpgFileInfo jfInfo = node.Value;
                int idx0 = idx - key_offset;
                if (!xmpFileDirectory0.ContainsKey(idx0))
                    continue;
                jpgFileInfo jfInfo0 = xmpFileDirectory0[idx0];
                if (!jfInfo0.isXMP)
                    continue;
                //FRotator newRot = new FRotator(0, 0, 0);
                //newRot.Pitch = -jfInfo0.rotation.Roll;
                //newRot.Yaw = jfInfo0.rotation.Yaw + 90.0;
                //newRot.Roll = 0;
                //double[] mt = BuildRotationMatrix(newRot);

                double[] mt = GetMtx(jfInfo0.rotationString,3);

                FVector newPos = new FVector(0, 0, 0);
                newPos.X = jfInfo0.position.X ;
                newPos.Y = jfInfo0.position.Y ;
                newPos.Z = jfInfo0.position.Z ;
                save_file.SetRotation(mt);
                save_file.SetPosition(newPos.X, newPos.Y, newPos.Z);
                string saveXMPFilename = Path.ChangeExtension(jfInfo.filename, "xmp");
                save_file.SaveXML(saveXMPFilename);

                Program.AddLog("SaveXML: " + saveXMPFilename);
            }

            curIdx = 2;
            //FVector relativePos = new FVector(0, 0, 0);
            //FRotator relativeRotator = new FRotator(0, 0, 0);
            relativeRotator.Yaw = -90.0;
            XMPFile save_file_2 = new XMPFile();
            save_file_2.LoadXML(sampleXMPFilename);
            //key_offset = keyIndex[curIdx] - keyIndex[0];
            foreach (var node in xmpFileDirectory2)
            {
                int idx = node.Key;
                jpgFileInfo jfInfo = node.Value;
                int idx0 = idx - key_offset;
                if (!xmpFileDirectory0.ContainsKey(idx0))
                    continue;
                jpgFileInfo jfInfo0 = xmpFileDirectory0[idx0];
                if (!jfInfo0.isXMP)
                    continue;
                //FRotator newRot = new FRotator(0, 0, 0);
                //newRot.Pitch = jfInfo0.rotation.Roll ;
                //newRot.Yaw = jfInfo0.rotation.Yaw - 90.0;
                //newRot.Roll = 0;
                //double[] mt = BuildRotationMatrix(newRot);


                double[] mt = GetMtx(jfInfo0.rotationString, 1);
                //double[] mt = jfInfo0.rotation.GetRightDirMtx();

                FVector newPos = new FVector(0, 0, 0);
                newPos.X = jfInfo0.position.X ;
                newPos.Y = jfInfo0.position.Y ;
                newPos.Z = jfInfo0.position.Z ;
                save_file_2.SetRotation(mt);
                save_file_2.SetPosition(newPos.X, newPos.Y, newPos.Z);
                string saveXMPFilename = Path.ChangeExtension(jfInfo.filename, "xmp");
                save_file_2.SaveXML(saveXMPFilename);

                Program.AddLog("SaveXML: " + saveXMPFilename);
            }


            curIdx = 3;
            //FVector relativePos = new FVector(0, 0, 0);
            //FRotator relativeRotator = new FRotator(0, 0, 0);
            relativeRotator.Yaw = 180;
            XMPFile save_file_3 = new XMPFile();
            save_file_3.LoadXML(sampleXMPFilename);
            //int key_offset = keyIndex[curIdx] - keyIndex[0];
            foreach (var node in xmpFileDirectory3)
            {
                int idx = node.Key;
                jpgFileInfo jfInfo = node.Value;
                int idx0 = idx - key_offset;
                if (!xmpFileDirectory0.ContainsKey(idx0))
                    continue;
                jpgFileInfo jfInfo0 = xmpFileDirectory0[idx0];
                if (!jfInfo0.isXMP)
                    continue;
                //FRotator newRot = new FRotator(0, 0, 0);
                //newRot.Pitch = -jfInfo0.rotation.Pitch;
                //newRot.Yaw = jfInfo0.rotation.Yaw + 180;
                //newRot.Roll = 0;
                double[] mt = GetMtx(jfInfo0.rotationString, 2);
                //double[] mt = BuildRotationMatrix(newRot);

                FVector newPos = new FVector(0, 0, 0);
                newPos.X = jfInfo0.position.X ;
                newPos.Y = jfInfo0.position.Y ;
                newPos.Z = jfInfo0.position.Z ;
                save_file_3.SetRotation(mt);
                save_file_3.SetPosition(newPos.X, newPos.Y, newPos.Z);
                string saveXMPFilename = Path.ChangeExtension(jfInfo.filename, "xmp");
                save_file_3.SaveXML(saveXMPFilename);
                Program.AddLog("SaveXML: " + saveXMPFilename);
            }

        }
       
        public void BuildRelXMPFile()
        {
            if (!isXMPSetting[0])
                return;
            if(isXMPSetting[1])
            {
                int curIdx = 1;
                FVector relativePos = new FVector(0, 0, 0);
                relativePos.X = relPostion[curIdx].X - relPostion[0].X;
                relativePos.Y = relPostion[curIdx].Y - relPostion[0].Y;
                relativePos.Z = relPostion[curIdx].Z - relPostion[0].Z;
                FRotator relativeRotator = new FRotator(0,0,0);
                relativeRotator.Pitch = relRotator[curIdx].Pitch - relRotator[0].Pitch;
                relativeRotator.Yaw = relRotator[curIdx].Yaw - relRotator[0].Yaw;
                relativeRotator.Roll = relRotator[curIdx].Roll - relRotator[0].Roll;
                XMPFile save_file = new XMPFile();
                save_file.LoadXML(relativeXMPFilename[curIdx]);
                int key_offset = keyIndex[curIdx] - keyIndex[0];
                foreach(var node in xmpFileDirectory1)
                {
                    int idx = node.Key;
                    jpgFileInfo jfInfo = node.Value;
                    int idx0 = idx - key_offset;
                    if (!xmpFileDirectory0.ContainsKey(idx0))
                        continue;
                    jpgFileInfo  jfInfo0 = xmpFileDirectory0[idx0];
                    if (!jfInfo0.isXMP)
                        continue;
                    FRotator newRot = new FRotator(0,0,0);
                    newRot.Pitch = jfInfo0.rotation.Pitch + relativeRotator.Pitch;
                    newRot.Yaw = jfInfo0.rotation.Yaw + relativeRotator.Yaw;
                    newRot.Roll = jfInfo0.rotation.Roll + relativeRotator.Roll;
                    double[] mt = BuildRotationMatrix(newRot);

                    FVector newPos = new FVector(0, 0, 0);
                    newPos.X = jfInfo0.position.X + relativePos.X;
                    newPos.Y = jfInfo0.position.Y + relativePos.Y;
                    newPos.Z = jfInfo0.position.Z + relativePos.Z;
                    save_file.SetRotation(mt);
                    save_file.SetPosition(newPos.X, newPos.Y, newPos.Z);
                    string saveXMPFilename = Path.ChangeExtension(jfInfo.filename,"xmp");
                    save_file.SaveXML(saveXMPFilename);

                    Program.AddLog("SaveXML: " +saveXMPFilename);
                }
            }
            if (isXMPSetting[2])
            {
                int curIdx = 2;
                FVector relativePos = new FVector(0, 0, 0);
                relativePos.X = relPostion[curIdx].X - relPostion[0].X;
                relativePos.Y = relPostion[curIdx].Y - relPostion[0].Y;
                relativePos.Z = relPostion[curIdx].Z - relPostion[0].Z;
                FRotator relativeRotator = new FRotator(0, 0, 0);
                relativeRotator.Pitch = relRotator[curIdx].Pitch - relRotator[0].Pitch;
                relativeRotator.Yaw = relRotator[curIdx].Yaw - relRotator[0].Yaw;
                relativeRotator.Roll = relRotator[curIdx].Roll - relRotator[0].Roll;
                XMPFile save_file = new XMPFile();
                save_file.LoadXML(relativeXMPFilename[curIdx]);
                int key_offset = keyIndex[curIdx] - keyIndex[0];
                foreach (var node in xmpFileDirectory2)
                {
                    int idx = node.Key;
                    jpgFileInfo jfInfo = node.Value;
                    int idx0 = idx - key_offset;
                    if (!xmpFileDirectory0.ContainsKey(idx0))
                        continue;
                    jpgFileInfo jfInfo0 = xmpFileDirectory0[idx0];
                    if (!jfInfo0.isXMP)
                        continue;
                    FRotator newRot = new FRotator(0, 0, 0);
                    newRot.Pitch = jfInfo0.rotation.Pitch + relativeRotator.Pitch;
                    newRot.Yaw = jfInfo0.rotation.Yaw + relativeRotator.Yaw;
                    newRot.Roll = jfInfo0.rotation.Roll + relativeRotator.Roll;
                    double[] mt = BuildRotationMatrix(newRot);

                    FVector newPos = new FVector(0, 0, 0);
                    newPos.X = jfInfo0.position.X + relativePos.X;
                    newPos.Y = jfInfo0.position.Y + relativePos.Y;
                    newPos.Z = jfInfo0.position.Z + relativePos.Z;
                    save_file.SetRotation(mt);
                    save_file.SetPosition(newPos.X, newPos.Y, newPos.Z);
                    string saveXMPFilename = Path.ChangeExtension(jfInfo.filename, "xmp");
                    save_file.SaveXML(saveXMPFilename);

                    Program.AddLog("SaveXML: " + saveXMPFilename);
                }
            }
            if (isXMPSetting[3])
            {
                int curIdx = 3;
                FVector relativePos = new FVector(0, 0, 0);
                relativePos.X = relPostion[curIdx].X - relPostion[0].X;
                relativePos.Y = relPostion[curIdx].Y - relPostion[0].Y;
                relativePos.Z = relPostion[curIdx].Z - relPostion[0].Z;
                FRotator relativeRotator = new FRotator(0, 0, 0);
                relativeRotator.Pitch = relRotator[curIdx].Pitch - relRotator[0].Pitch;
                relativeRotator.Yaw = relRotator[curIdx].Yaw - relRotator[0].Yaw;
                relativeRotator.Roll = relRotator[curIdx].Roll - relRotator[0].Roll;
                XMPFile save_file = new XMPFile();
                save_file.LoadXML(relativeXMPFilename[curIdx]);
                int key_offset = keyIndex[curIdx] - keyIndex[0];
                foreach (var node in xmpFileDirectory3)
                {
                    int idx = node.Key;
                    jpgFileInfo jfInfo = node.Value;
                    int idx0 = idx - key_offset;
                    if (!xmpFileDirectory0.ContainsKey(idx0))
                        continue;
                    jpgFileInfo jfInfo0 = xmpFileDirectory0[idx0];
                    if (!jfInfo0.isXMP)
                        continue;
                    FRotator newRot = new FRotator(0, 0, 0);
                    newRot.Pitch = jfInfo0.rotation.Pitch + relativeRotator.Pitch;
                    newRot.Yaw = jfInfo0.rotation.Yaw + relativeRotator.Yaw;
                    newRot.Roll = jfInfo0.rotation.Roll + relativeRotator.Roll;
                    double[] mt = BuildRotationMatrix(newRot);

                    FVector newPos = new FVector(0, 0, 0);
                    newPos.X = jfInfo0.position.X + relativePos.X;
                    newPos.Y = jfInfo0.position.Y + relativePos.Y;
                    newPos.Z = jfInfo0.position.Z + relativePos.Z;
                    save_file.SetRotation(mt);
                    save_file.SetPosition(newPos.X, newPos.Y, newPos.Z);
                    string saveXMPFilename = Path.ChangeExtension(jfInfo.filename, "xmp");
                    save_file.SaveXML(saveXMPFilename);
                    Program.AddLog("SaveXML: " + saveXMPFilename);
                }
            }
        }
        
        static double PI = 3.1415926535897932;
        static double INV_PI = 0.31830988618;
        static double HALF_PI = 1.57079632679;
        static void SinCos(out double ScalarSin, out double ScalarCos, double Value)
        {
            // Map Value to y in [-pi,pi], x = 2*pi*quotient + remainder.
            double quotient = (INV_PI * 0.5f) * Value;
            if (Value >= 0.0f)
            {
                quotient = (float)((int)(quotient + 0.5f));
            }
            else
            {
                quotient = (float)((int)(quotient - 0.5f));
            }
            double y = Value - (2.0f * PI) * quotient;

            // Map y to [-pi/2,pi/2] with sin(y) = sin(Value).
            double sign;
            if (y > HALF_PI)
            {
                y = PI - y;
                sign = -1.0f;
            }
            else if (y < -HALF_PI)
            {
                y = -PI - y;
                sign = -1.0f;
            }
            else
            {
                sign = +1.0f;
            }

            double y2 = y * y;

            // 11-degree minimax approximation
            ScalarSin = (((((-2.3889859e-08f * y2 + 2.7525562e-06f) * y2 - 0.00019840874f) * y2 + 0.0083333310) * y2 - 0.16666667f) * y2 + 1.0f) * y;

            // 10-degree minimax approximation
            double p = ((((-2.6051615e-07f * y2 + 2.4760495e-05f) * y2 - 0.0013888378f) * y2 + 0.041666638) * y2 - 0.5f) * y2 + 1.0f;
            ScalarCos = sign * p;
        }
        static double DegreesToRadians(double DegVal)
        {
            return DegVal * (PI / 180.0);
        }
        static double[] BuildRotationMatrix(FRotator rot)
        {
            double[] mt = new double[9];
            double SP, SY, SR;
            double CP, CY, CR;
            SinCos(out SP, out CP, DegreesToRadians(rot.Pitch));
            SinCos(out SY, out CY, DegreesToRadians(rot.Yaw));
            SinCos(out SR, out CR, DegreesToRadians(rot.Roll));
            mt[0] = CP * CY;
            mt[1] = CP * SY;
            mt[2] = SP;
            mt[3] = SR * SP * CY - CR * SY;
            mt[4] = SR * SP * SY + CR * CY;
            mt[5] = -SR * CP;
            mt[6] = -(CR * SP * CY + SR * SY);
            mt[7] = CY * SR - CR * SP * SY;
            mt[8] = CR * CP;

            return mt;
        }
        static public FVector GetPostiion(string rotData)
        {
            if (rotData.Length < 1)
                return new FVector(0, 0, 0);
            string posString = rotData;
            char[] charSeparators = new char[] { ' ' };
            var values = posString.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            return new FVector(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), Convert.ToDouble(values[2]));
        }
        static public double[] GetMtx(string rotData ,int type)
        {
            double[] mt = new double[9];
            if (rotData.Length < 1)
                return mt;
            char[] charSeparators = new char[] { ' ' };
            var values = rotData.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            //6,7,8  Forward Axis (X Axis)
            int[] idx = new int[] { 6,7,8,3,4,5,0,1,2};
            if(type == 1)
            {
                mt[0] = -Convert.ToDouble(values[6]);
                mt[1] = -Convert.ToDouble(values[7]);
                mt[2] = -Convert.ToDouble(values[8]);
                mt[3] = Convert.ToDouble(values[3]);
                mt[4] = Convert.ToDouble(values[4]);
                mt[5] = Convert.ToDouble(values[5]);

                mt[6] = Convert.ToDouble(values[0]);
                mt[7] = Convert.ToDouble(values[1]);
                mt[8] = Convert.ToDouble(values[2]);
            }
            if (type == 2)
            {
                mt[0] = -Convert.ToDouble(values[0]);
                mt[1] = -Convert.ToDouble(values[1]);
                mt[2] = -Convert.ToDouble(values[2]);
                mt[3] = Convert.ToDouble(values[3]);
                mt[4] = Convert.ToDouble(values[4]);
                mt[5] = Convert.ToDouble(values[5]);

                mt[6] = -Convert.ToDouble(values[6]);
                mt[7] = -Convert.ToDouble(values[7]);
                mt[8] = -Convert.ToDouble(values[8]);
            }
            if (type == 3)
            {
                mt[0] = Convert.ToDouble(values[6]);
                mt[1] = Convert.ToDouble(values[7]);
                mt[2] = Convert.ToDouble(values[8]);
                mt[3] = Convert.ToDouble(values[3]);
                mt[4] = Convert.ToDouble(values[4]);
                mt[5] = Convert.ToDouble(values[5]);

                mt[6] = -Convert.ToDouble(values[0]);
                mt[7] = -Convert.ToDouble(values[1]);
                mt[8] = -Convert.ToDouble(values[2]);
            }

            return mt;
        }
        static public FRotator GetRotator(string rotData)
        {
            if (rotData.Length < 1)
                return new FRotator(0, 0, 0);
            char[] charSeparators = new char[] { ' ' };
            var values = rotData.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            FVector XAxis = new FVector(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), Convert.ToDouble(values[2]));
            //XAxis = XAxis.GetSafeNormal();
            FVector YAxis = new FVector(-1.0 * Convert.ToDouble(values[3]), -1.0 * Convert.ToDouble(values[4]), -1.0 * Convert.ToDouble(values[5]));
           // YAxis = YAxis.GetSafeNormal();
            FVector ZAxis = new FVector(Convert.ToDouble(values[6]), Convert.ToDouble(values[7]), Convert.ToDouble(values[8]));
            //ZAxis = ZAxis.GetSafeNormal();

            //double sy = Math.Sqrt(XAxis.X * XAxis.X + XAxis.Y * XAxis.Y);
            //FRotator Rotator = new FRotator(Math.Atan2(-YAxis.Z, YAxis.Y) * 180.0 / PI,Math.Atan2(-ZAxis.X, sy) * 180.0 / PI, 0);

            FRotator Rotator = new FRotator(Math.Atan2(XAxis.Z, Math.Sqrt(XAxis.X * XAxis.X + XAxis.Y * XAxis.Y)) * 180.0 / PI,
                Math.Atan2(XAxis.Y, XAxis.X) * 180.0 / PI,0);

            //FRotator Rotator = new FRotator(
            //   Math.Atan2(XAxis.Y, XAxis.X) * 180.0 / PI, Math.Atan2(XAxis.Z, Math.Sqrt(XAxis.X * XAxis.X + XAxis.Y * XAxis.Y)) * 180.0 / PI, 0);
            //
            double[] mt = BuildRotationMatrix(Rotator);
            FVector SYAxis = new FVector(mt[3],mt[4],mt[5]);// FRotationMatrix(Rotator).GetScaledAxis(EAxis::Y);
            Rotator.Roll = Math.Atan2(ZAxis.DotProduct(SYAxis), YAxis.DotProduct(SYAxis)) * 180.0 / PI;
            return Rotator;
        }

        public FRotator GroundVUYawPitchRow(string rotData)
        {
            if (rotData.Length < 1)
                return new FRotator(0, 0, 0);
            char[] charSeparators = new char[] { ' ' };
            var values = rotData.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            FVector XAxis = new FVector(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), Convert.ToDouble(values[2]));
            //XAxis = XAxis.GetSafeNormal();
            FVector YAxis = new FVector(-1.0 * Convert.ToDouble(values[3]), -1.0 * Convert.ToDouble(values[4]), -1.0 * Convert.ToDouble(values[5]));
            // YAxis = YAxis.GetSafeNormal();
            FVector ZAxis = new FVector(1.0 * Convert.ToDouble(values[6]), 1.0 * Convert.ToDouble(values[7]),  -1.0 * Convert.ToDouble(values[8]));
            //ZAxis = ZAxis.GetSafeNormal();

            //double sy = Math.Sqrt(XAxis.X * XAxis.X + XAxis.Y * XAxis.Y);
            //FRotator Rotator = new FRotator(Math.Atan2(-YAxis.Z, YAxis.Y) * 180.0 / PI,Math.Atan2(-ZAxis.X, sy) * 180.0 / PI, 0);

            FRotator Rotator = new FRotator(Math.Atan2(XAxis.Z, Math.Sqrt(XAxis.X * XAxis.X + XAxis.Y * XAxis.Y)) * 180.0 / PI,
                Math.Atan2(XAxis.Y, XAxis.X) * 180.0 / PI, 0);

            //FRotator Rotator = new FRotator(
            //   Math.Atan2(XAxis.Y, XAxis.X) * 180.0 / PI, Math.Atan2(XAxis.Z, Math.Sqrt(XAxis.X * XAxis.X + XAxis.Y * XAxis.Y)) * 180.0 / PI, 0);
            //
            double[] mt = BuildRotationMatrix(Rotator);
            FVector SYAxis = new FVector(mt[3], mt[4], mt[5]);// FRotationMatrix(Rotator).GetScaledAxis(EAxis::Y);
            Rotator.Roll = Math.Atan2(ZAxis.DotProduct(SYAxis), YAxis.DotProduct(SYAxis)) * 180.0 / PI;
            Rotator.Pitch *= -1.0;
            return Rotator;
        }


    }
}
