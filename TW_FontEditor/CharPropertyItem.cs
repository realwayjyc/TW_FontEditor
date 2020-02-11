using CufParser;
using System.ComponentModel;

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

        public CharPropertyItem(CharProperty charProperty)
        {
            _charProperty = charProperty;
        }


    }
}
