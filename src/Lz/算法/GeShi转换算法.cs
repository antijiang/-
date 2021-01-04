using System;
using System.Text;

namespace Lz.算法
{
    public class GeShi转换算法
    {
        #region 其他函数
        /// <summary>
        /// 把Enum16进制隔离符转换成实际的字符串
        /// </summary>
        /// <param name="enum16">Enum16进制隔离符</param>
        /// <returns></returns>
        private static string AddGeLi(Enum16进制隔离符 enum16)
        {
            switch (enum16)
            {
                case Enum16进制隔离符.无:
                    return "";
                case Enum16进制隔离符.Ox:
                    return "0x";
                case Enum16进制隔离符.OX:
                    return "0X";
                case Enum16进制隔离符.空格:
                    return " ";
                default:
                    return ""; 
            }
        }
        /// <summary>
        /// 去掉16进制字符串中的隔离符
        /// </summary>
        /// <param name="inString">需要转换的字符串</param>
        /// <returns></returns>
        private static string DelGeLi(string inString)
        {
            string outString = "";
            string[] del = { " ", "0x", "0X" };
            if (inString.Contains(" ") || inString.Contains("0x") || inString.Contains("0X"))//存在隔离符
            {
                string[] strS = inString.Split(del, System.StringSplitOptions.RemoveEmptyEntries);//以隔离符进行转换数组，去掉隔离符,去掉空格。
                for (int i = 0; i < strS.Length; i++)
                {
                    outString += strS[i].ToString();
                }
                return outString;
            }
            else//不存在隔离符
            {
                return inString;
            }
        }
        #endregion
        #region 汉字、英文、纯16进制数、byte[]，之间的各种转换函数。
        /// <summary>
        /// 字符串转换成16进制
        /// </summary>
        /// <param name="inSting"></param>
        /// <param name="enum16"></param>
        /// <returns></returns>
        public static string StringTo16(string inSting,Enum16进制隔离符 enum16)
        {
            string outString = "";
            byte[] bytes = Encoding.Default.GetBytes(inSting);
            for (int i = 0; i < bytes.Length; i++)
            {
                int strInt = Convert.ToInt16(bytes[i] - '\0');
                string s = strInt.ToString("X");
                if (s.Length == 1)
                {
                    s = "0" + s;
                }
                s = s + AddGeLi(enum16);
                outString += s;
            }
            return outString;
        }
        /// <summary>
        /// 字符串转换成byte[]
        /// </summary>
        /// <param name="inSting"></param>
        /// <returns></returns>
        public static byte[] StringToBtyes(string inSting)
        {
            inSting = StringTo16(inSting,Enum16进制隔离符.无);//把字符串转换成16进制数
            return _16ToBtyes(inSting);//把16进制数转换成byte[]
        }
        /// <summary>
        /// 把16进制字符串转换成byte[]
        /// </summary>
        /// <param name="inSting"></param>
        /// <returns></returns>
        public static byte[] _16ToBtyes(string inSting)
        {
            inSting= DelGeLi(inSting);//去掉隔离符
            byte[] strBt = new byte[inSting.Length / 2];
            for (int i = 0, j = 0; i < inSting.Length; i = i + 2, j++)
            {
                string s = inSting.Substring(i, 2);
                try
                {
                    strBt[j] = (byte)Convert.ToInt16(s, 16);
                }
                catch (Exception e)
                {
                    throw new Exception("你填写的数据不是纯16进制数，请检查。");
                }
            }

            return strBt;
        }
        /// <summary>
        /// 把16进制字符串变成英文数字和汉字混合的格式。
        /// </summary>
        /// <param name="str">需要转换的16进制字符串</param>
        /// <returns>转换好的字符串</returns>
        public static string _16ToString(string inSting)
        {
            inSting = DelGeLi(inSting);
            return Encoding.Default.GetString(_16ToBtyes(inSting));
        }
        /// <summary>
        /// 把byte[]转换成String
        /// </summary>
        /// <param name="bytes">需要转换的byte[]</param>
        /// <param name="enum16">隔离符</param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes,Enum16进制隔离符 enum16)
        {
            return _16ToString(BytesTo16(bytes,enum16));
        }
        /// <summary>
        /// byte[]转换成16进制字符串
        /// </summary>
        /// <param name="bytes">需要转换的byte[]</param>
        /// <param name="enum16"></param>
        /// <returns></returns>
        public static string BytesTo16(byte[] bytes, Enum16进制隔离符 enum16)
        {
            string outString="";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i].ToString("X").Length < 2)//16进制数为一位时前面填充0
                {
                    outString += "0" + bytes[i].ToString("X") + AddGeLi(enum16);//转成16进制数据
                }
                else//
                {
                    outString += bytes[i].ToString("X") + AddGeLi(enum16);//转成16进制数据
                }
            }
            return outString;
        }
        /// <summary>
        /// 把byte[]直接转换成字符串，直接以2进制形式显示出来。
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesTo2String(byte[] bytes,Enum16进制隔离符 enum16)
        {
            string outString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                string tempString = Convert.ToString(bytes[i], 2);
                if (tempString.Length != 8)
                {
                    string add0 = "";
                    for (int j = 0; j < 8 - tempString.Length; j++)
                    {
                        add0 += "0";
                    }
                    outString += add0 + tempString + AddGeLi(enum16);
                }
                else
                {
                    outString += tempString + AddGeLi(enum16);
                }
            }
            return outString;

        }
        #endregion
    }
}