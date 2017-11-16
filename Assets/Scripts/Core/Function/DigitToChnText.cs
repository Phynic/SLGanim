using System;
using System.Text;
namespace DigitToChnText
{
    /// 
    /// 本程序用于将小写数字转换为
    /// 1. 一般中文大写数字
    /// 2. 人民币大写数字
    /// 算法设计：黄晶
    /// 程序制作：黄晶
    /// 时间：2004年8月12日
    /// 
    class DigitToChnText
    {
        private readonly char[] chnGenText;
        private readonly char[] chnGenDigit;
        private readonly char[] chnRMBText;
        private readonly char[] chnRMBDigit;
        private readonly char[] chnRMBUnit;
        //
        // 构造函数
        //
        public DigitToChnText()
        {
            // 一般大写中文数字组
            chnGenText = new char[] { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九' };
            chnGenDigit = new char[] { '十', '百', '千', '万', '亿' };
            // 人民币专用数字组
            chnRMBText = new char[] { '零', '壹', '贰', '叁', '肆', '伍', '陆', '染', '捌', '玖' };
            chnRMBDigit = new char[] { '拾', '佰', '仟', '萬', '億' };
            chnRMBUnit = new char[] { '角', '分' };
        }
        //
        // 主转换函数
        // 参数
        // string strDigit - 待转换数字字符串
        // bool  bToRMB  - 是否转换成人民币
        // 返回
        // string    - 转换成的大写字符串
        //
        public string Convert(string strDigit, bool bToRMB)
        {
            // 检查输入数字有效性
            CheckDigit(ref strDigit, bToRMB);
            // 定义结果字符串
            StringBuilder strResult = new StringBuilder();
            // 提取符号部分
            ExtractSign(ref strResult, ref strDigit, bToRMB);
            // 提取并转换整数和小数部分
            ConvertNumber(ref strResult, ref strDigit, bToRMB);
            return strResult.ToString();
        }
        //
        // 转换数字
        //
        protected void ConvertNumber(ref StringBuilder strResult, ref string strDigit, bool bToRMB)
        {
            int indexOfPoint;
            if (-1 == (indexOfPoint = strDigit.IndexOf('.'))) // 如果没有小数部分
            {
                strResult.Append(ConvertIntegral(strDigit, bToRMB));
                if (bToRMB)  // 如果转换成人民币
                {
                    strResult.Append("圆整");
                }
            }
            else // 有小数部分
            {
                // 先转换整数部分
                if (0 == indexOfPoint) // 如果“.”是第一个字符
                {
                    if (!bToRMB)  // 如果转换成一般中文大写
                    {
                        strResult.Append('零');
                    }
                }
                else // 如果“.”不是第一个字符
                {
                    strResult.Append(ConvertIntegral(strDigit.Substring(0, indexOfPoint), bToRMB));
                }
                // 再转换小数部分
                if (strDigit.Length - 1 != indexOfPoint) // 如果“.”不是最后一个字符
                {
                    if (bToRMB) // 如果转换成人民币
                    {
                        if (0 != indexOfPoint) // 如果“.”不是第一个字符
                        {
                            if (1 == strResult.Length && "零" == strResult.ToString()) // 如果整数部分只是'0'
                            {
                                strResult.Remove(0, 1);  // 去掉“零”
                            }
                            else
                            {
                                strResult.Append('圆');
                            }
                        }
                    }
                    else
                    {
                        strResult.Append('点');
                    }
                    string strTmp = ConvertFractional(strDigit.Substring(indexOfPoint + 1), bToRMB);
                    if (0 != strTmp.Length) // 小数部分有返回值
                    {
                        if (bToRMB && // 如果转换为人民币
                         0 == strResult.Length && // 且没有整数部分
                         "零" == strTmp.Substring(0, 1)) // 且返回字串的第一个字符是“零”
                        {
                            strResult.Append(strTmp.Substring(1));
                        }
                        else
                        {
                            strResult.Append(strTmp);
                        }
                    }
                    if (bToRMB)
                    {
                        if (0 == strResult.Length) // 如果结果字符串什么也没有
                        {
                            strResult.Append("零圆整");
                        }
                        // 如果结果字符串最后以"圆"结尾
                        else if ("圆" == strResult.ToString().Substring(strResult.Length - 1, 1))
                        {
                            strResult.Append('整');
                        }
                    }
                }
                else if (bToRMB) // 如果"."是最后一个字符，且转换成人民币
                {
                    strResult.Append("圆整");
                }
            }
        }
        //
        // 检查输入数字有效性
        //
        private void CheckDigit(ref string strDigit, bool bToRMB)
        {
            decimal dec;
            try
            {
                dec = decimal.Parse(strDigit);
            }
            catch (FormatException)
            {
                throw new Exception("输入数字的格式不正确。");
            }
            catch (Exception e)
            {
                throw e;
            }
            if (bToRMB) // 如果转换成人民币
            {
                if (dec >= 10000000000000000m)
                {
                    throw new Exception("输入数字太大，超出范围。");
                }
                else if (dec < 0)
                {
                    throw new Exception("不允许人民币为负值。");
                }
            }
            else   // 如果转换成中文大写
            {
                if (dec <= -10000000000000000m || dec >= 10000000000000000m)
                {
                    throw new Exception("输入数字太大或太小，超出范围。");
                }
            }
        }
        //
        // 提取输入字符串的符号
        //
        protected void ExtractSign(ref StringBuilder strResult, ref string strDigit, bool bToRMB)
        {
            // '+'在最前
            if ("+" == strDigit.Substring(0, 1))
            {
                strDigit = strDigit.Substring(1);
            }
            // '-'在最前
            else if ("-" == strDigit.Substring(0, 1))
            {
                if (!bToRMB)
                {
                    strResult.Append('负');
                }
                strDigit = strDigit.Substring(1);
            }
            // '+'在最后
            else if ("+" == strDigit.Substring(strDigit.Length - 1, 1))
            {
                strDigit = strDigit.Substring(0, strDigit.Length - 1);
            }
            // '-'在最后
            else if ("-" == strDigit.Substring(strDigit.Length - 1, 1))
            {
                if (!bToRMB)
                {
                    strResult.Append('负');
                }
                strDigit = strDigit.Substring(0, strDigit.Length - 1);
            }
        }
        //
        // 转换整数部分
        //
        protected string ConvertIntegral(string strIntegral, bool bToRMB)
        {
            // 去掉数字前面所有的'0'
            // 并把数字分割到字符数组中
            char[] integral = ((long.Parse(strIntegral)).ToString()).ToCharArray();
            // 定义结果字符串
            StringBuilder strInt = new StringBuilder();

            int digit;
            digit = integral.Length - 1;
            // 使用正确的引用
            char[] chnText = bToRMB ? chnRMBText : chnGenText;
            char[] chnDigit = bToRMB ? chnRMBDigit : chnGenDigit;

            // 变成中文数字并添加中文数位
            // 处理最高位到十位的所有数字
            int i;
            for (i = 0; i < integral.Length - 1; i++)
            {
                // 添加数字
                strInt.Append(chnText[integral[i] - '0']);
                // 添加数位
                if (0 == digit % 4)     // '万' 或 '亿'
                {
                    if (4 == digit || 12 == digit)
                    {
                        strInt.Append(chnDigit[3]); // '万'
                    }
                    else if (8 == digit)
                    {
                        strInt.Append(chnDigit[4]); // '亿'
                    }
                }
                else         // '十'，'百'或'千'
                {
                    strInt.Append(chnDigit[digit % 4 - 1]);
                }
                digit--;
            }
            // 如果个位数不是'0'
            // 或者只有一位数
            // 则添加相应的中文数字
            if ('0' != integral[integral.Length - 1] || 1 == integral.Length)
            {
                strInt.Append(chnText[integral[i] - '0']);
            }

            // 遍历整个字符串
            i = 0;
            string strTemp;  // 临时存储字符串
            int j;    // 查找“零x”结构时用
            bool bDoSomething; // 找到“零万”或“零亿”时为真
            while (i < strInt.Length)
            {
                j = i;

                bDoSomething = false;
                // 查找所有相连的“零x”结构
                while (j < strInt.Length - 1 && "零" == strInt.ToString().Substring(j, 1))
                {
                    strTemp = strInt.ToString().Substring(j + 1, 1);

                    // 如果是“零万”或者“零亿”则停止查找
                    if (chnDigit[3].ToString() /* 万或萬 */ == strTemp ||
                     chnDigit[4].ToString() /* 亿或億 */ == strTemp)
                    {
                        bDoSomething = true;
                        break;
                    }
                    j += 2;
                }
                if (j != i) // 如果找到非“零万”或者“零亿”的“零x”结构，则全部删除
                {
                    strInt = strInt.Remove(i, j - i);
                    // 除了在最尾处，或后面不是"零万"或"零亿"的情况下, 
                    // 其他处均补入一个“零”
                    if (i <= strInt.Length - 1 && !bDoSomething)
                    {
                        strInt = strInt.Insert(i, '零');
                        i++;
                    }
                }

                if (bDoSomething) // 如果找到"零万"或"零亿"结构
                {
                    strInt = strInt.Remove(i, 1); // 去掉'零'
                    i++;
                    continue;
                }
                // 指针每次可移动2位
                i += 2;
            }
            // 遇到“亿万”变成“亿零”或"亿"
            strTemp = chnDigit[4].ToString() + chnDigit[3].ToString(); // 定义字符串“亿万”或“億萬”
            int index = strInt.ToString().IndexOf(strTemp);
            if (-1 != index)
            {
                if (strInt.Length - 2 != index &&  // 如果"亿万"不在末尾
                 (index + 2 < strInt.Length
                  && "零" != strInt.ToString().Substring(index + 2, 1))) // 并且其后没有"零"
                {
                    strInt = strInt.Replace(strTemp, chnDigit[4].ToString(), index, 2); // 变“亿万”为“亿零”
                    strInt = strInt.Insert(index + 1, "零");
                }
                else  // 如果“亿万”在末尾或是其后已经有“零”
                {
                    strInt = strInt.Replace(strTemp, chnDigit[4].ToString(), index, 2); // 变“亿万”为“亿”
                }
            }
            if (!bToRMB) // 如果转换为一般中文大写
            {
                // 开头为“一十”改为“十”
                if (strInt.Length > 1 && "一十" == strInt.ToString().Substring(0, 2))
                {
                    strInt = strInt.Remove(0, 1);
                }
            }
            return strInt.ToString();
        }
        //
        // 转换小数部分
        //
        protected string ConvertFractional(string strFractional, bool bToRMB)
        {
            char[] fractional = strFractional.ToCharArray();
            StringBuilder strFrac = new StringBuilder();
            // 变成中文数字
            int i;
            if (bToRMB) // 如果转换为人民币
            {
                for (i = 0; i < Math.Min(2, fractional.Length); i++)
                {
                    strFrac.Append(chnRMBText[fractional[i] - '0']);
                    strFrac.Append(chnRMBUnit[i]);
                }
                // 如果最后两位是“零分”则删除
                if ("零分" == strFrac.ToString().Substring(strFrac.Length - 2, 2))
                {
                    strFrac.Remove(strFrac.Length - 2, 2);
                }
                // 如果以“零角”开头
                if ("零角" == strFrac.ToString().Substring(0, 2))
                {
                    // 如果只剩下“零角”
                    if (2 == strFrac.Length)
                    {
                        strFrac.Remove(0, 2);
                    }
                    else // 如果还有“x分”，则删除“角”
                    {
                        strFrac.Remove(1, 1);
                    }
                }
            }
            else // 如果转换为一般中文大写
            {
                for (i = 0; i < fractional.Length; i++)
                {
                    strFrac.Append(chnGenText[fractional[i] - '0']);
                }
            }
            return strFrac.ToString();
        }
        /// 
        /// 应用程序主函数入口点
        /// 
        //[STAThread]
        //static void Main(string[] args)
        //{
        //    DigitToChnText obj = new DigitToChnText();
        //    string str = " ";
        //    while ("" != str)
        //    {
        //        Console.Write("请输入小写数字: ");
        //        if ("" == (str = Console.ReadLine()))
        //            break;

        //        try
        //        {
        //            Console.WriteLine("中文大写数字为: {0}", obj.Convert(str, false));
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("转换大写数字时出错: {0}", e.Message);
        //        }
        //        try
        //        {
        //            Console.WriteLine("人民币大写为:   {0}/n", obj.Convert(str, true));
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("转换人民币时出错:   {0}", e.Message);
        //        }
        //        Console.WriteLine();
        //    }
        //}
    }
}