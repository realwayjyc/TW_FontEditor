using System;

namespace CufParser
{
    public class CharProperty
    {
        private byte[] _cufFileContent = null;

        public char Character { get; set; }

        #region 5个字节的字符大小信息
        public byte Unknown1
        {
            get
            {
                return _cufFileContent[_sizeInfoStartIndex];
            }
            set
            {
                _cufFileContent[_sizeInfoStartIndex] = value; 
            }
        }


        public byte Unknown2
        {
            get
            {
                return _cufFileContent[_sizeInfoStartIndex+1];
            }
            set
            {
                _cufFileContent[_sizeInfoStartIndex+1] = value;
            }
        }


        public byte Width
        {
            get
            {
                return _cufFileContent[_sizeInfoStartIndex + 2];
            }
            set
            {
                _cufFileContent[_sizeInfoStartIndex + 2] = value;
            }
        }


        public byte Height
        {
            get
            {
                return _cufFileContent[_sizeInfoStartIndex + 3];
            }
            set
            {
                _cufFileContent[_sizeInfoStartIndex + 3] = value;
            }
        }

        public byte Unknown3
        {
            get
            {
                return _cufFileContent[_sizeInfoStartIndex + 4];
            }
            set
            {
                _cufFileContent[_sizeInfoStartIndex + 4] = value;
            }
        }

        private readonly int _sizeInfoStartIndex;
        #endregion

        #region 4个字节的字符点阵信息的索引
        public int Index
        {
            get
            {
                return BitConverter.ToInt32(_cufFileContent, _charPointInfoStartIndex);
            }
        }

        private readonly int _charPointInfoStartIndex;
        #endregion


        public CharProperty(ushort charUnicode,byte[] cufFileContent,
            int sizeInfoStartIndex,int charPointInfoStartIndex)
        {
            Character =(char)charUnicode;
            _cufFileContent = cufFileContent;
            _sizeInfoStartIndex = sizeInfoStartIndex;
            _charPointInfoStartIndex = charPointInfoStartIndex;
        }

        public override string ToString()
        {
            return Character.ToString()+" "+Width+" "+Height;
        }
    }
}
