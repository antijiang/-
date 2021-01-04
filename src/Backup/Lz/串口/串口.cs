using System;
using System.Collections;//对ArrayList的支持
using System.IO.Ports;//对串口操作的支持
using Lz.算法;//对字符格式转换的支持

namespace Lz.串口
{
    public class COM:SerialPort
    {
        #region 常量定义
        private static int[] _baudRate = { 300, 600, 900, 1200, 2400,4800, 9600, 19200, 115200 };
        private static int[] _shujv = { 6, 7, 8 };
        #endregion

        #region 获取串口参数填充到界面
        /// <summary>
        /// 获取机器上所有的串口
        /// </summary>
        /// <returns></returns>
        public static string[] AllCom()
        {
            return SerialPort.GetPortNames();
        }
        /// <summary>
        /// 获取所有的波特率(在常量区定义)
        /// </summary>
        /// <returns></returns>
        public static int[] AllBaudRate()
        {
            return _baudRate;
        }
        /// <summary>
        /// 获取所有的校验位
        /// </summary>
        /// <returns></returns>
        public static ArrayList AllParity()
        {
            ArrayList al = new ArrayList();
            foreach (Parity pt in Enum.GetValues(typeof(Parity)))
            {
                al.Add(ParityToString(pt).ToString());
            }
            return al;
        }
        /// <summary>
        /// 获取所有的数据位(在常量区定义)
        /// </summary>
        /// <returns></returns>
        public static int[] AllDataBits()
        {
            return _shujv;
        }
        /// <summary>
        /// 获取所有的停止位
        /// </summary>
        /// <returns></returns>
        public static ArrayList AllStopBits()
        {
            ArrayList al = new ArrayList();
            al.Add("1位");
            al.Add("2位");
            return al;
        }
        #endregion

        #region 串口参数与汉字的转换(例：Even变成偶校验)
        /// <summary>
        /// 把停止位StopBits转换成汉字(例："StopBits.None"变成"0位")
        /// </summary>
        /// <param name="_p"></param>
        /// <returns></returns>
        public static string StopBitsToSting(StopBits stopBits)
        {
            string s;
            switch (stopBits)
            {
                case StopBits.None:
                    s = "0位";
                    break;
                case StopBits.One:
                    s = "1位";
                    break;
                case StopBits.Two:
                    s = "2位";
                    break;
                case StopBits.OnePointFive:
                    s = "1.5位";
                    break;
                default:
                    s = "1位";
                    break;
            }
            return s;
        }
        /// <summary>
        /// 把校验位Parity转换成汉字(例：Even变成偶校验)
        /// </summary>
        /// <param name="parity"></param>
        /// <returns></returns>
        public static string ParityToString(Parity parity)
        {
            string s;
            switch (parity)
            {
                case Parity.Even:
                    s = "偶校验";
                    break;
                case Parity.Mark:
                    s = "保留为1";
                    break;
                case Parity.None:
                    s = "不校验";
                    break;
                case Parity.Odd:
                    s = "奇校验";
                    break;
                case Parity.Space:
                    s = "保留为0";
                    break;
                default:
                    s = "保留为0";
                    break;
            }
            return s;
        }        
        /// <summary>
        /// 把汉字转换成停止位StopBits(例："0位"变成"StopBits.None")
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static StopBits StingToStopBits(string inString)
        {
            StopBits p;
            switch (inString)
            {
                case "0位":
                    p = StopBits.None;
                    break;
                case "1位":
                    p = StopBits.One;
                    break;
                case "2位":
                    p = StopBits.Two;
                    break;
                case "1.5位":
                    p = StopBits.OnePointFive;
                    break;
                default:
                    p = StopBits.One;
                    break;
            }
            return p;
        }        
        /// <summary>
        /// 把汉字转换成校验位Parity(例："偶校验"变成"Parity.Even")
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static Parity StringToParity(string inString)
        {
            Parity p;
            switch (inString)
            {
                case "偶校验":
                    p = Parity.Even;
                    break;
                case "保留为1":
                    p = Parity.Mark;
                    break;
                case "不校验":
                    p = Parity.None;
                    break;
                case "奇校验":
                    p = Parity.Odd;
                    break;
                case "保留为0":
                    p = Parity.Space;
                    break;
                default:
                    p = Parity.None;
                    break;
            }
            return p;
        }
        #endregion
    }
}