﻿using CufParser;
using System.ComponentModel;
using System.Reflection;

namespace TW_FontEditor
{
    public class CharPropertyItem: INotifyPropertyChanged
    {
        private CharProperty _charProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler hander = PropertyChanged;
            if (hander != null)
                hander(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Char
        {
            get
            {
                if (_charProperty == null) return null;
                return _charProperty.Character.ToString();
            }
        }
        public string Unicode
        {
            get
            {
                if (_charProperty == null) return "-1";
                return ((int)_charProperty.Character).ToString();
            }
        }


        public byte Width
        {
            get
            {
                if (_charProperty == null) return 255;
                return _charProperty.Width;
            }
            set
            {
                if (_charProperty == null) return;
                _charProperty.Width = value;
            }
        }

        public byte Height
        {
            get
            {
                if (_charProperty == null) return 255;
                return _charProperty.Height;
            }
            set
            {
                if (_charProperty == null) return;
                _charProperty.Height = value;
            }
        }

        public byte Unknown1
        {
            get
            {
                if (_charProperty == null) return 255;
                return _charProperty.Unknown1;
            }
            set
            {
                if (_charProperty == null) return;
                _charProperty.Unknown1 = value;
            }
        }

        public byte Unknown2
        {
            get
            {
                if (_charProperty == null) return 255;
                return _charProperty.Unknown2;
            }
            set
            {
                if (_charProperty == null) return;
                _charProperty.Unknown2 = value;
            }
        }

        public byte WidthFull
        {
            get
            {
                if (_charProperty == null) return 255;
                return _charProperty.WidthFull;
            }
            set
            {
                if (_charProperty == null) return;
                _charProperty.WidthFull = value;
            }
        }



        public ushort UnicodeValue
        {
            get
            {
                return _charProperty.Character;
            }
        }

        public CharPropertyItem(CharProperty charProperty)
        {
            _charProperty = charProperty;
        }
    }

    public class HeaderPropertyItem : INotifyPropertyChanged
    {
        private CufFile _cufFile;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler hander = PropertyChanged;
            if (hander != null)
                hander(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _headerName;
        public string HeaderName
        {
            get { return _headerName; }
            set { _headerName = value; }
        }

        public string HeaderValue
        {
            get
            {
                if (_cufFile == null) return null;
                PropertyInfo propertyInfo = _cufFile.GetType().GetProperty(_headerName);
                if (propertyInfo == null) return null;
                object retValue = propertyInfo.GetValue(_cufFile);
                if (retValue == null) return null;
                return retValue.ToString();
            }
            set
            {
                if (_cufFile == null) return;
                PropertyInfo propertyInfo = _cufFile.GetType().GetProperty(_headerName);
                if (propertyInfo == null) return;
                ushort toSetValue = 0;
                if (ushort.TryParse(value,out toSetValue))
                {
                    propertyInfo.SetValue(_cufFile, toSetValue);
                }
            }
        }

        public HeaderPropertyItem(CufFile cufFile,string headerName)
        {
            _cufFile = cufFile;
            _headerName = headerName;
        }
    }

}
