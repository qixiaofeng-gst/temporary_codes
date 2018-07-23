using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PDPU
{
    [Serializable]
    public class FrameNodeModel : ICloneable, INotifyPropertyChanged
    {
        private TypeAndRule typeAndRule;

        public TypeAndRule TypeAndRule
        {
            get { return typeAndRule; }
        }

        public FrameNodeModel()
        {
            
        }


        public void ParseSubInfor()
        {
            typeAndRule = new TypeAndRule(this);
        }

        private string id;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private int pos;
        public int Pos
        {
            get { return pos; }
            set
            {
                pos = value;
                NotifyPropertyChanged("Pos");
            }
        }

        private int len;
        public int Len
        {
            get { return len; }
            set
            {
                len = value;
                NotifyPropertyChanged("Len");
            }
        }

        private string dataType;
        public string DataType
        {
            get { return dataType; }
            set
            {
                dataType = value;
                NotifyPropertyChanged("DataType");
            }
        }

        private string rule;
        public string Rule
        {
            get { return rule; }
            set
            {
                rule = value;
                NotifyPropertyChanged("Rule");
            }
        }

        public object Clone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                object copy = formatter.Deserialize(objectStream);
                objectStream.Close();
                return copy;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
