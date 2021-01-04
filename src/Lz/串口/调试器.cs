using System;
using System.Drawing;
using System.Windows.Forms;
using Lz.算法;

namespace Lz.串口
{
    public class TiaoShiQi
    {
        #region 转换函数
        /// <summary>
        /// 把字符串传进来，输出一个byte数组，可以把此byte数组直接发送到串口中。
        /// </summary>
        /// <param name="inString">要转换的字符串</param>
        /// <param name="is16">是否已经是16进制数据，true时已经是(已经转换好的数据)，false时不是(需要内部转换)</param>
        /// <returns>输出一个byte数组</returns>
        public static byte[] StringToBytes(string inString, bool is16)
        {
            if (is16)
            {
                return GeShi转换算法._16ToBtyes(inString);
            }
            else
            {
                return GeShi转换算法.StringToBtyes(inString);
            }
        }
        #endregion

        #region 委托管理，直接调用sjcl数据处理（）就可以把字符串放到控件中。可以跨线程。
        delegate void weituo(Form owner, RichTextBox richTextBox, string inString, Color co);
        /// <summary>
        /// 委托的实现。会被sjcl事件处理函数调用。
        /// </summary>
        /// <param name="rich">控件</param>
        /// <param name="s">字符串</param>
        /// <param name="co">颜色</param>
        private static void jieshou(Form f, RichTextBox rich, string s, Color co)
        {
            if (!rich.InvokeRequired)
            {
                toRichBox(rich, s, co);
            }
            else
            {
                weituo jie = new weituo(jieshou);
                f.BeginInvoke(jie, new object[] { f, rich, s, co });//带参数的委托调用
            }
        }
        /// <summary>
        /// 把字符串添加到控件中，可以选择控件，可以选择颜色
        /// </summary>
        /// <param name="rich">控件</param>
        /// <param name="inStr">字符串</param>
        /// <param name="co">颜色</param>
        private static void toRichBox(RichTextBox rich, string inStr, Color co)
        {
            rich.Select(rich.Text.Length, 0);
            rich.SelectionColor = co;
            rich.AppendText(inStr);
        }
        /// <summary>
        /// 处理接收来的数据,会调用一个委托jieshou。
        /// </summary>
        /// <param name="bt">需要处理的数据数组</param>
        /// <param name="_xs16">是否要显示16进制数据</param>
        /// <param name="_xsstr">是否要以字符串形式显示</param>
        /// <param name="xin">是否新行显示</param>
        public static void sjcl数据处理(Form f, RichTextBox rich, byte[] bt, bool _xs16, bool _xsstr, bool xin)
        {
            string _16 = GeShi转换算法.BytesTo16(bt, Enum16进制隔离符.空格);//把byte转换成16进制
            string _str = GeShi转换算法.BytesToString(bt, Enum16进制隔离符.无);//把byte转换成字符串
            string xinstr = "\n";        

            if (_xs16)//16进制显示
            {
                jieshou(f, rich, _16, Color.Black);
                if (_xsstr)//字符串显示
                { //16进制和字符串都要显示
                    jieshou(f, rich, _str, Color.Red);
                    if (xin)
                    { //新行显示
                        jieshou(f, rich, xinstr, Color.Black);
                    }
                }
                else
                { //只显示16进制
                    if (xin)
                    { //新行显示
                        jieshou(f, rich, xinstr, Color.Black);
                    }
                }
            }
            else
            {
                if (_xsstr)//显示字符串
                {
                    jieshou(f, rich, _str, Color.Red);
                    if (xin)
                    { //新行显示
                        jieshou(f, rich, xinstr, Color.Black);
                    }
                }
                else//不显示任何信息
                {

                }
            }
        }
        #endregion
    }
}