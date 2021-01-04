#region 驱动
using System;
using System.Drawing;
using System.Windows.Forms;
using Lz.串口;
using Lz.算法;
using Sunisoft.IrisSkin;
using System.IO;
using System.Xml;
#endregion
namespace 调试者
{
    public partial class TS调试者 : Form
    {
        #region 变量
        COM com = new COM();
        ButtonName aa = new ButtonName();
        public static Button IndexButton;
        
        #endregion

        #region 重要的函数
        private void comdk打开()
        {
            try
            {
                com.DtrEnable = true; com.RtsEnable = true;
                com.PortName = commz名字.Text;
                com.BaudRate = Convert.ToInt32(combt波特率.Text);
                com.Parity = COM.StringToParity(comjy校验位.Text);
                com.DataBits = Convert.ToInt16(comsj数据位.Text);
                com.StopBits = COM.StingToStopBits(comtz停止位.Text);
                com.Open();
                if (com.IsOpen)
                {
                    but打开.Text = "关闭";
                    labzt状态.BackColor = Color.Red;
                    labckx串口信息();
                    throw new Exception("串口打开成功！");
                }
                else
                {
                    throw new Exception("串口打开失败！请检查是否存在此串口。");
                }
            }
            catch (Exception ex)
            {
                fill状态条2(ex.Message);
            }
        }
        private void comgb关闭()
        {
            try
            {
                if (com.IsOpen)
                {
                    com.Close();
                    if (!com.IsOpen)
                    {
                        but打开.Text = "打开";
                        labzt状态.BackColor = Color.Blue;
                        throw new Exception("串口关闭成功！");
                    }
                    else
                    {
                        throw new Exception("串口关闭失败！");
                    }
                }
                else
                {
                    //串口已经被关闭了。我的笔记本上用的是usb--com转换器，经常需要重新插一下，松动时就非法关闭了，会出现这种情况。
                    //throw new Exception("不会出现这种情况。");

                    but打开.Text = "打开";
                    labzt状态.BackColor = Color.Blue;
                    throw new Exception("串口信息出错，请检查是否存在这个串口。");
                }
            }
            catch (Exception ex)
            {
                fill状态条2(ex.Message);
            }
        }
        private void comcx重新打开()
        {
            comgb关闭();
            comdk打开();
        }

        ////格式   start  LEN      type cmd  seq  data（len=N） Checksum                  end
        ///        A5FC   LL HH    TT   CC   SS    D1 Dn         =LL+HH+CC+SS+D1..Dn      FB 
        private void fs发送(string _fs)
        {
            UInt16 len = (UInt16) _fs.Length;
            string fs = _fs;
            if (_fs.Length > 0)
            {
                try
                {

                    
                    
                    if (CheckSum.Checked == true && checkfs16进制发送.Checked == true)
                    {
                        byte[] bt1 = TiaoShiQi.StringToBytes(fs, checkfs16进制发送.Checked);
                        UInt16 checksum = 0;
                        for (int i = 0; i < bt1.Length; i++)
                        {
                            checksum += bt1[i];
                        }
                        
                        len = Convert.ToUInt16(bt1.Length-3); //长度    type cmd seq  data（len） 
                        string chksum = BitConverter.ToString(BitConverter.GetBytes(checksum));
                        chksum = chksum.Replace("-", "");
                        fs = fs.Insert(fs.Length, chksum);

                    }

                    if (checkBox长度.Checked == true)
                    {
                        byte lenb = Convert.ToByte(len);
                        string m;
                        byte olx=Convert.ToByte(textBox长度.Text);
                      
                        m = BitConverter.ToString(BitConverter.GetBytes(len),0,olx);
                        m = m.Replace("-", "");
                        fs = fs.Insert(0, m);
                    }
                    if (checkBox帧头.Checked == true)
                    {
                        fs = fs.Insert(0, textBox帧头.Text.ToString());
                    }
                   

                    if (checkBox帧伟.Checked==true)
                    {                  
                          fs = fs.Insert(fs.Length, textBox帧尾.Text.ToString());
                       
                    }
                   
                        byte[] bt = TiaoShiQi.StringToBytes(fs, checkfs16进制发送.Checked);
                        com.Write(bt, 0, bt.Length);
                        TiaoShiQi.sjcl数据处理(this, richfsq发送区, bt, checkfs16xs发送区16进制显示.Checked, checkfszfxs发送区字符显示.Checked, checkfsxhxs发送区新行显示.Checked);
                   
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }
            }
            else
            {
                throw new Exception("发送失败，你没有填写任何数据。");
            }
        }        
        private void com接收(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                
                byte[] bt = new byte[com.BytesToRead];
                com.Read(bt, 0, com.BytesToRead);
               // char[] bt = new char [com.BytesToRead];
                //com.Read(bt, 0, com.BytesToRead);
                if (bt.Length > 0)
                {
                    string OpenPower = GeShi转换算法.BytesToString(bt, Enum16进制隔离符.无);                  
                    if (OpenPower.IndexOf("OK") !=-1)
                    {                   
                        timer4.Enabled = false;
                        MessageBox.Show("OK");
                        CheckOK.Checked = false;
                                                
                    }
                    TiaoShiQi.sjcl数据处理(this, richjsq接收区, bt, checkjs16xs接收区16进制显示.Checked, checkjszfxs接收区字符显示.Checked, checkjsxhxs接收区新行显示.Checked);
                }
            }
            catch (Exception ex)
            {
                fill状态条2(ex.Message.ToString());
            }
        }
        #endregion

        #region 系统生成
        private void TS调试者_Load(object sender, EventArgs e)
        {
            com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(com接收);//串口接到数据会激发这个事件
            commz名字.Items.AddRange(COM.AllCom());
            commz名字.SelectedIndex = 0;
            foreach (int i in COM.AllBaudRate())
            {
                combt波特率.Items.Add(i);
            }
            combt波特率.SelectedIndex = 6;
            foreach (string s in COM.AllParity())
            {
                comjy校验位.Items.Add(s);
            }
            comjy校验位.SelectedIndex = 0;
            foreach (int i in COM.AllDataBits())
            {
                comsj数据位.Items.Add(i);
            }
            comsj数据位.SelectedIndex = 2;
            foreach (string s in COM.AllStopBits())
            {
                comtz停止位.Items.Add(s);
            }
            comtz停止位.SelectedIndex = 0;
            duqu();//读取皮肤文件

            XmlDocument xmldoc;
            XmlNode xmlnode;
            
            String text;
            bool yn;

            xmldoc = new XmlDocument();
            try
            {
                xmldoc.Load("串口调试器.xml");
              
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/串口号");
                if (xmlnode != null)
                {
                   text = xmlnode.Attributes["Text"].Value;
                    commz名字.Text = text;             
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/波特率");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    combt波特率.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/校验位");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    comjy校验位.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/数据位");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    comsj数据位.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/停止位");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    comtz停止位.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/发送文本");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    richfs发送.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/更换皮肤");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    comboBox1.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/自动发送周期");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textZq周期.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/帧头");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textBox帧头.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/帧尾");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textBox帧尾.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/帧尾");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textBox帧尾.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata1");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata1.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata2");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata2.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata3");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata3.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata4");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata4.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata5");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata5.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata6");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata6.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata7");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata7.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata8");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata8.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata9");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata9.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata10");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata10.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata11");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata11.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata12");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata12.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata13");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata13.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata14");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata14.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata15");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata15.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata16");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata16.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata17");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata17.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textdata18");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textdata18.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/sendtimer2");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    sendtimer2.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textHour");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textHour.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/sendtimer2");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    sendtimer2.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textMinute");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textMinute.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/textSecond");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    textSecond.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button1");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button1.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button2");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button2.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button3");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button3.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button4");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button4.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button5");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button5.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button6");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button6.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button7");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button7.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button8");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button8.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button9");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button9.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button10");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button10.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button11");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button11.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button12");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button12.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button13");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button13.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button14");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button14.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button15");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button15.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button16");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button16.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button17");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button17.Text = text;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/Button18");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Text"].Value;
                    button18.Text = text;
                }


                xmlnode = xmldoc.SelectSingleNode("/串口调试器/CheckSum");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;               
                    yn = bool.Parse(text);
                    CheckSum.Checked = yn;                            
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkfs16xs发送区16进制显示");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkfs16xs发送区16进制显示.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkfszfxs发送区字符显示");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkfszfxs发送区字符显示.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkfsxhxs发送区新行显示");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkfsxhxs发送区新行显示.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkfs16进制发送");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkfs16进制发送.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/CheckHide");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    CheckHide.Checked = yn;

                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkjs16xs接收区16进制显示");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkjs16xs接收区16进制显示.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkjszfxs接收区字符显示");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkjszfxs接收区字符显示.Checked = yn;
                    
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkjsxhxs接收区新行显示");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkjsxhxs接收区新行显示.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex1");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex1.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex2");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex2.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex3");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex3.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex4");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex4.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex5");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex5.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex6");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex6.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex7");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex7.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex8");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex8.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex9");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex9.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex10");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex10.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex11");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex11.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex12");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex12.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex13");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex13.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex14");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex14.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex15");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex15.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex16");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex16.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex17");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex17.Checked = yn;
                }
                xmlnode = xmldoc.SelectSingleNode("/串口调试器/checkHex18");
                if (xmlnode != null)
                {
                    text = xmlnode.Attributes["Value"].Value;
                    yn = bool.Parse(text);
                    checkHex18.Checked = yn;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("未创建XML文件");
            }

        }
        private void but打开_Click(object sender, EventArgs e)//打开按钮
        {
            try
            {
                if (but打开.Text == "打开")
                {
                    comdk打开();
                }
                else
                {
                    comgb关闭();
                }
            }
            catch (Exception ex)
            {
                fill状态条2(ex.Message.ToString());
            }
        }
        private void butfs发送_Click(object sender, EventArgs e)//发送按钮
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        fs发送(richfs发送.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }
        private void butjs接收区清空_Click(object sender, EventArgs e)//发送区清空
        {
            richjsq接收区.Text = "";
        }
        private void butfs发送区清空_Click(object sender, EventArgs e)//发送区清空
        {
            richfsq发送区.Text = "";
        }
        private void butfsq发送清空_Click(object sender, EventArgs e)//发送清空
        {
            richfs发送.Text = "";
        }
        private void timer1_Tick(object sender, EventArgs e)//自动发送时间控制
        {
            try
            {
                if (com.IsOpen)
                {
                    fs发送(richfs发送.Text.ToString());
                    throw new Exception("发送成功。");
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }
        private void checkzd自动发送_CheckedChanged(object sender, EventArgs e)//自动发送选项
        {
            if (checkzd自动发送.Checked)
            {
                timer1.Interval = Convert.ToInt16(textZq周期.Text.ToString());
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
        }
        private void commz名字_SelectedIndexChanged(object sender, EventArgs e)//重新设置串口
        {
            comcx重新打开();
        }
        #endregion

        #region 换皮肤
        private SkinEngine skinEngine1 = new SkinEngine();
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            skinEngine1.SkinFile = comboBox1.Text.ToString();
        }
        public void duqu()
        {
            string[] a = Directory.GetFiles("pi");
            foreach (string file in a)
            {
                comboBox1.Items.Add(file);
            }
        }
        #endregion

        #region 无用
        public TS调试者()
        {
            InitializeComponent();
        }
        private void TS调试者_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    com.Close();
                }
            }
            catch (Exception ex)
            {
                fill状态条2(ex.Message);
            }
        }
        private void fill状态条2(string s)
        {
            toolStripStatusLabel2.Text = s.ToString();
        }
        private void labckx串口信息()
        {
            textBox1.Text = com.PortName.ToString() + "," + com.BaudRate.ToString() + "," + COM.ParityToString(com.Parity) + "," + com.DataBits.ToString() + "," +COM.StopBitsToSting(com.StopBits);
        }
      
        private void timer2_Tick(object sender, EventArgs e)
        {
           
        }
        private void 复制_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();//先获取焦点，防止点两下才运行
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Copy();
        }
        private void 粘贴_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Paste();
        }
        private void 剪切_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Cut();
        }
        private void 删除_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectedText = "";
        }
        private void 全选_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.SelectAll();
        }
        private void 撤销_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.SourceControl.Select();
            RichTextBox rtb = (RichTextBox)this.contextMenuStrip1.SourceControl;
            rtb.Undo();
        }
        #endregion

        int num = 0;
        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {
            num += 1;
            if (num > 10)
            {
                关于 guan = new 关于();
                guan.ShowDialog();
                num = 0;
            }
        }

         
        private void FormMax(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle();
            rect = Screen.GetWorkingArea(this);
            //MessageBox.Show("本机器的分辨率是 " + rect.Width.ToString() + "* " + rect.Height.ToString());

            //if (WindowState == System.Windows.Forms.FormWindowState.Maximized)
            //{
            //    groupBox2.Width = rect.Width;
            //    this.richjsq接收区.Size = new System.Drawing.Size(rect.Width - 100, 232);// new System.Drawing.Size(730 - 100, 232);\
            //}
            //else
            //{
            //    this.richjsq接收区.Size = new System.Drawing.Size(rect.Width * 50 / 100 - 100, 232);// new System.Drawing.Size(730 - 100, 232);
            //    groupBox2.Width = rect.Width * 50 / 100;
            //}
                    
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex1.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else 
                        {
                            checkfs16进制发送.Checked = false;
                        }

                        fs发送(textdata1.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex2.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }

                        fs发送(textdata2.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex3.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata3.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex4.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata4.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex5.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata5.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex6.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata6.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex7.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata7.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex8.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata8.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex9.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata9.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex10.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata10.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex11.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata11.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex12.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata12.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex13.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata13.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex14.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata14.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex15.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata15.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex16.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata16.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex17.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata17.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        if (checkHex18.Checked)
                        {
                            checkfs16进制发送.Checked = true;
                        }
                        else
                        {
                            checkfs16进制发送.Checked = false;
                        }
                        fs发送(textdata18.Text.ToString());
                        throw new Exception("发送成功。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void checkcycle_CheckedChanged(object sender, EventArgs e)
        {
            if (checkcycle.Checked)
            {
                timer3.Interval = Convert.ToInt16(sendtimer2.Text.ToString());
                timer3.Enabled = true;
            }
            else
            {
                timer3.Enabled = false;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    if (checkcommand1.Checked)
                         button1_Click(null, null);
                    if (checkcommand2.Checked)
                        button2_Click(null, null);
                    if (checkcommand3.Checked)
                        button3_Click(null, null);
                    if (checkcommand4.Checked)
                        button4_Click(null, null);
                    if (checkcommand5.Checked)
                        button5_Click(null, null);
                    if (checkcommand6.Checked)
                        button6_Click(null, null);
                    if (checkcommand7.Checked)
                        button7_Click(null, null);
                    if (checkcommand8.Checked)
                        button8_Click(null, null);
                    if (checkcommand9.Checked)
                        button9_Click(null, null);
                    if (checkcommand10.Checked)
                        button10_Click(null, null);
                    if (checkcommand11.Checked)
                        button11_Click(null, null);
                    if (checkcommand12.Checked)
                        button12_Click(null, null);
                    if (checkcommand13.Checked)
                        button13_Click(null, null);
                    if (checkcommand14.Checked)
                        button14_Click(null, null);
                    if (checkcommand15.Checked)
                        button15_Click(null, null);
                    if (checkcommand16.Checked)
                        button16_Click(null, null);
                    if (checkcommand17.Checked)
                        button17_Click(null, null);
                    if (checkcommand18.Checked)
                        button18_Click(null, null);


                    
                    throw new Exception("发送成功。");
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            switch (comboMenthod.Text)
            {
                case "Dec->Hex":                   
                    try
                    {
                        int result = (int)Convert.ToDouble(Inputdata.Text);
                        Outputdata.Text = (Convert.ToString(result, 16)).ToUpper();                         
                       
                    }
                    catch
                    {
                        MessageBox.Show("转换失败,请输入正确数值");                              
                    }        
                    break;

                case "Hex->Dec":
                    try
                    {                     
                        Outputdata.Text = Convert.ToString(Convert.ToInt32(Inputdata.Text, 16), 10);

                    }
                    catch
                    {
                        MessageBox.Show("转换失败,请输入正确数值");
                    }
                    break;
                    
 
            }
            
        }

        private void button19_Click(object sender, EventArgs e)
        {                      
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        int hr = (int)Convert.ToDouble(textHour.Text);
                        int min = (int)Convert.ToDouble(textMinute.Text);
                        int sec = (int)Convert.ToDouble(textSecond.Text);
                        int time = hr * 60 * 60 + min * 60 + sec;

                        String str = Convert.ToString(time, 16);
                        while (str.Length < 8)
                        {
                            str = str.Insert(0, "0");
                        }
                        str=str.Insert(0, "06");

                        fs发送(str);
                        throw new Exception("发送成功。");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
           
        }

        private void MaxCurrentButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {                                             
                        fs发送("f0");
                        throw new Exception("发送成功。");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }

        }

        private void OpenCurrentButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        fs发送("f1");
                        throw new Exception("发送成功。");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }

        }

        private void CheckOK_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckOK.Checked)
            {
                timer4.Interval = 1;
                timer4.Enabled = true;
            }
            else
            {
                timer4.Enabled = false;
               
            }        

        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    fs发送("3a");                  
                    throw new Exception("发送成功。");                   
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }

        }

        private void CheckHide_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckHide.Checked)
            {
                tabControl1.Hide();
                //tabControl1.Width = 0;
                //tabControl1.Height = 0;
                groupBox2.Width = 613+303+33;
                groupBox2.Height = 350;

                groupBox6.Width = 462+303+33;
                groupBox6.Height = 255;
             
           
                Invalidate(true);

                
            }
            else 
            {
                tabControl1.Show();
                //tabControl1.Width = 303;
                //tabControl1.Height = 633;
                
                groupBox2.Width=613;
                groupBox2.Height=350;

                groupBox6.Width=462;
                groupBox6.Height=255;

                
                Invalidate(true);
                           
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elementH, elementM;

            //root = doc.CreateXmlDeclaration("1.0", null, null);
            //doc.AppendChild(root);
            //ProjectName = doc.CreateElement("串口调试器");           
            //XmlElement ComName= doc.CreateElement("串口号");
            //ComName.SetAttribute("Text", commz名字.Text);
            //ProjectName.AppendChild(ComName);
            //doc.AppendChild(ProjectName);

            elementH = doc.CreateElement("串口调试器");

            elementM = doc.CreateElement("串口号");
            elementM.SetAttribute("Text", commz名字.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("波特率");
            elementM.SetAttribute("Text", combt波特率.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("校验位");
            elementM.SetAttribute("Text", comjy校验位.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("数据位");
            elementM.SetAttribute("Text", comsj数据位.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("停止位");
            elementM.SetAttribute("Text", comtz停止位.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("更换皮肤");
            elementM.SetAttribute("Text", comboBox1.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("发送文本");
            elementM.SetAttribute("Text", richfs发送.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("自动发送周期");
            elementM.SetAttribute("Text", textZq周期.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("帧头");
            elementM.SetAttribute("Text", textBox帧头.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("帧尾");
            elementM.SetAttribute("Text", textBox帧尾.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata1");
            elementM.SetAttribute("Text", textdata1.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata2");
            elementM.SetAttribute("Text", textdata2.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata3");
            elementM.SetAttribute("Text", textdata3.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata4");
            elementM.SetAttribute("Text", textdata4.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata5");
            elementM.SetAttribute("Text", textdata5.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata6");
            elementM.SetAttribute("Text", textdata6.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata7");
            elementM.SetAttribute("Text", textdata7.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata8");
            elementM.SetAttribute("Text", textdata8.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata9");
            elementM.SetAttribute("Text", textdata9.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata10");
            elementM.SetAttribute("Text", textdata10.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata11");
            elementM.SetAttribute("Text", textdata11.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata12");
            elementM.SetAttribute("Text", textdata12.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata13");
            elementM.SetAttribute("Text", textdata13.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata14");
            elementM.SetAttribute("Text", textdata14.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata15");
            elementM.SetAttribute("Text", textdata15.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata16");
            elementM.SetAttribute("Text", textdata16.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata17");
            elementM.SetAttribute("Text", textdata17.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textdata18");
            elementM.SetAttribute("Text", textdata18.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("sendtimer2");
            elementM.SetAttribute("Text", sendtimer2.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textHour");
            elementM.SetAttribute("Text", textHour.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("sendtimer2");
            elementM.SetAttribute("Text", sendtimer2.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textMinute");
            elementM.SetAttribute("Text", textMinute.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("textSecond");
            elementM.SetAttribute("Text", textSecond.Text);
            elementH.AppendChild(elementM);

            elementM = doc.CreateElement("Button1");
            elementM.SetAttribute("Text", button1.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button2");
            elementM.SetAttribute("Text", button2.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button3");
            elementM.SetAttribute("Text", button3.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button4");
            elementM.SetAttribute("Text", button4.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button5");
            elementM.SetAttribute("Text", button5.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button6");
            elementM.SetAttribute("Text", button6.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button7");
            elementM.SetAttribute("Text", button7.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button8");
            elementM.SetAttribute("Text", button8.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button9");
            elementM.SetAttribute("Text", button9.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button10");
            elementM.SetAttribute("Text", button10.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button11");
            elementM.SetAttribute("Text", button11.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button12");
            elementM.SetAttribute("Text", button12.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button13");
            elementM.SetAttribute("Text", button13.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button14");
            elementM.SetAttribute("Text", button14.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button15");
            elementM.SetAttribute("Text", button15.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button16");
            elementM.SetAttribute("Text", button16.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button17");
            elementM.SetAttribute("Text", button17.Text);
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("Button18");
            elementM.SetAttribute("Text", button18.Text);
            elementH.AppendChild(elementM);
                                          

            elementM = doc.CreateElement("CheckSum");
            elementM.SetAttribute("Value", CheckSum.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkfs16xs发送区16进制显示");
            elementM.SetAttribute("Value", checkfs16xs发送区16进制显示.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkfszfxs发送区字符显示");
            elementM.SetAttribute("Value", checkfszfxs发送区字符显示.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkfsxhxs发送区新行显示");
            elementM.SetAttribute("Value", checkfsxhxs发送区新行显示.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkfs16进制发送");
            elementM.SetAttribute("Value", checkfs16进制发送.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("CheckHide");
            elementM.SetAttribute("Value", CheckHide.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkjs16xs接收区16进制显示");
            elementM.SetAttribute("Value", checkjs16xs接收区16进制显示.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkjszfxs接收区字符显示");
            elementM.SetAttribute("Value", checkjszfxs接收区字符显示.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkjsxhxs接收区新行显示");
            elementM.SetAttribute("Value", checkjsxhxs接收区新行显示.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex1");
            elementM.SetAttribute("Value", checkHex1.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex2");
            elementM.SetAttribute("Value", checkHex2.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex3");
            elementM.SetAttribute("Value", checkHex3.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex4");
            elementM.SetAttribute("Value", checkHex4.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex5");
            elementM.SetAttribute("Value", checkHex5.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex6");
            elementM.SetAttribute("Value", checkHex6.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex7");
            elementM.SetAttribute("Value", checkHex7.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex8");
            elementM.SetAttribute("Value", checkHex8.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex9");
            elementM.SetAttribute("Value", checkHex9.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex10");
            elementM.SetAttribute("Value", checkHex10.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex11");
            elementM.SetAttribute("Value", checkHex11.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex12");
            elementM.SetAttribute("Value", checkHex12.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex13");
            elementM.SetAttribute("Value", checkHex13.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex14");
            elementM.SetAttribute("Value", checkHex14.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex15");
            elementM.SetAttribute("Value", checkHex15.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex16");
            elementM.SetAttribute("Value", checkHex16.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex17");
            elementM.SetAttribute("Value", checkHex17.Checked.ToString());
            elementH.AppendChild(elementM);
            elementM = doc.CreateElement("checkHex18");
            elementM.SetAttribute("Value", checkHex18.Checked.ToString());
            elementH.AppendChild(elementM);
          
            
            
            doc.AppendChild(elementH);
            
         
            try
            {
                doc.Save("串口调试器.xml");              
                base.OnClosed(e);
                         
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败");             
            }            
        }

        private void CurrentPower_Click(object sender, EventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    try
                    {
                        int CurrentPower = (int)Convert.ToDouble(CurrentPowerText.Text);

                        String str = Convert.ToString(CurrentPower, 16);
                        while (str.Length < 4)
                        {
                            str = str.Insert(0, "0");
                        }
                        str = str.Insert(0, "04");

                        fs发送(str);
                        throw new Exception("发送成功。");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                }
                else
                {
                    throw new Exception("串口没有打开。");
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel2.Text = ex.Message.ToString();
            }
            
        }
        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button1;
                aa = new ButtonName();
                aa.ShowDialog();                              
            }
        }
        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button2;
                aa=new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button3;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button4;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button5;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button6;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button7;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button8_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button8;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button9;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button10;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button11_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button11;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button12;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button13_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button13;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button14_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button14;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button15_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button15;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button16_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button16;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button17_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button17;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }
        private void button18_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                IndexButton = button18;
                aa = new ButtonName();
                aa.ShowDialog();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}