using System;
using System.Collections.Generic;

namespace CufParser
{
    public enum FileType
    {
        WH=0,
        TK=1,
    }
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
        private FileType _fileType;
        public const int OtherCharacterMinus = 35;
        public const int ChineseCharacterMinus_3K = 10673;
        public const int ChineseCharacterMinus_WH = 10670;
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
        public ushort HeightFull
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

        /// <summary>
        /// 有效
        /// </summary>
        public ushort YOffset
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
        public ushort UnknownValue
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
        public List<CharProperty> CharTable
        {
            get
            {
                return _charTable;
            }
        }

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
        /// <summary>
        /// 有效
        /// </summary>
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


        public CufFile(byte[] content,FileType fileType)
        {
            _fileType = fileType;
            _content = new byte[content.Length];
            Array.Copy(content, _content, content.Length);
            _charTable = new List<CharProperty>();
            int index = 0;
            int offset = 0;
            switch(_fileType)
            {
                case FileType.TK:
                    offset = ChineseCharacterMinus_3K;
                    break;
                case FileType.WH:
                    offset = ChineseCharacterMinus_WH;
                    break;
            }
            for(int i=0x20;i<0x20020;i+=2)
            {
                ushort unicodeValue= BitConverter.ToUInt16(_content, i);
                if(unicodeValue!=0xffff)
                {
                    int charOffset = index;
                    if(index> (offset + 1000))
                    {
                        charOffset = index - offset;
                    }
                    else if(index>161)
                    {
                        charOffset -= OtherCharacterMinus;
                    }
                    _charTable.Add(new CharProperty(unicodeValue, _content,
                        0x20020 + 5 * charOffset, 
                        0x20020 + 5 * NumberOfGlyphs + 4 * index));
                    index++;
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
