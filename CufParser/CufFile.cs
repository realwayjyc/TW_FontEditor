using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CufParser
{
    /// <summary>
    /// 0->0x1F 
    /// 字体基本信息
    /// 
    /// 0x20->0x2001F
    /// 字符表，用unicode表示，字符数量(NumberOfGlyphs)是其中unicode不是0xffff的数量
    /// 
    /// 0x20020->0x20020+字符数量*5-1
    /// 各字符的大小（宽高等）
    /// 
    /// 0x20020+字符数量*5->0x20020+字符数量*9-1
    /// 各字符的四个字节的索引，索引指向该字符的点阵，对于战锤后的CUF文件，此值无作用。
    /// 
    /// 0x20020+字符数量*9->0x20020+字符数量*9+SizeOfGlyphData-1
    /// GlyphData
    /// 
    /// 4个字节未知
    /// 
    /// 0x1600*0x16个字节
    /// 
    /// 一个整数，表示后面的widechar的长度
    /// 
    /// 以widechar表示的字符串
    /// 
    /// 4个字节未知
    /// 
    /// 2个字节的字体大小（猜测）
    /// </summary>
    public class CufFile
    {
        private byte[] _content;

        public ushort UnknownWidth1
        {
            get
            {
                return BitConverter.ToUInt16(_content, 4);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 4, 2);
            }
        }
        public ushort UnknownHeight1
        {
            get
            {
                return BitConverter.ToUInt16(_content, 6);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 6, 2);
            }
        }
        public ushort CapHeight
        {
            get
            {
                return BitConverter.ToUInt16(_content, 8);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 8, 2);
            }
        }
        public ushort UnknownWidth2
        {
            get
            {
                return BitConverter.ToUInt16(_content, 10);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 10, 2);
            }
        }
        public ushort UnknownHeight2
        {
            get
            {
                return BitConverter.ToUInt16(_content, 12);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 12, 2);
            }
        }
        public ushort VerticalAlignment
        {
            get
            {
                return BitConverter.ToUInt16(_content, 14);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 14, 2);
            }
        }
        public ushort YOffset
        {
            get
            {
                return BitConverter.ToUInt16(_content, 16);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 16, 2);
            }
        }
        public ushort SpaceWidth
        {
            get
            {
                return BitConverter.ToUInt16(_content, 18);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 18, 2);
            }
        }
        public ushort XOffset
        {
            get
            {
                return BitConverter.ToUInt16(_content, 20);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 20, 2);
            }
        }
        public ushort MaxGlyphWidth
        {
            get
            {
                return BitConverter.ToUInt16(_content, 22);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 22, 2);
            }
        }
        public ushort MaxGlyphHeight
        {
            get
            {
                return BitConverter.ToUInt16(_content, 24);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 24, 2);
            }
        }
        public ushort NumberOfGlyphs
        {
            get
            {
                return BitConverter.ToUInt16(_content, 26);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 26, 2);
            }
        }
        public ushort SizeOfGlyphData
        {
            get
            {
                return BitConverter.ToUInt16(_content, 28);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, 28, 2);
            }
        }

        private List<CharProperty> _charTable;

        public string FontName
        {
            get
            {
                int fontNameStartIndex = 0x20020 + 9 * NumberOfGlyphs + SizeOfGlyphData + 4 + 0x1600 * 0x16;
                ushort fontNameLength = BitConverter.ToUInt16(_content, fontNameStartIndex);
                fontNameStartIndex += 2;
                string ret = "";
                for (int i = 0; i < fontNameLength; i++)
                {
                    ret += (char)BitConverter.ToUInt16(_content, fontNameStartIndex + i * 2);
                }
                return ret;
            }
        }
        public ushort FontSize
        {
            get
            {
                return BitConverter.ToUInt16(_content, _content.Length - 2);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, _content, _content.Length - 2, 2);
            }
        }


        public CufFile(byte[] content)
        {
            _content = new byte[content.Length];
            Array.Copy(content, _content, content.Length);
            _charTable = new List<CharProperty>();
            for(int i=0x20;i<0x20020;i+=2)
            {
                ushort unicodeValue= BitConverter.ToUInt16(_content, i);
                if(unicodeValue!=0xffff)
                {
                    _charTable.Add(new CharProperty(unicodeValue, _content,
                        0x20020 + 5 * (i - 0x20) / 2, 0x20020 + 5 * NumberOfGlyphs + 4 * (i - 0x20) / 2));
                }
            }
            if(_charTable.Count!= NumberOfGlyphs)
            {
                Console.WriteLine("Char Table Count Error");
            }

           
        }

        public byte[] GetData()
        {
            return _content;
        }
    }
}
