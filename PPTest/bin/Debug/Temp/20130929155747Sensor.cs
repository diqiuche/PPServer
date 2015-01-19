using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SatelliteCore.Models
{
    [DataContract]
    public class Sensor : INotifyPropertyChanged
    {
        private SensorModel _selectedModel;

        [DataMember(Name = "@编号")]
        public int Id { get; set; }

        /// <summary>
        /// 传感器名称
        /// </summary>
        [DataMember(Name = "@名称")]
        public string Name { get; set; }

        /// <summary>
        /// 传感器类型
        /// </summary>
        [DataMember(Name = "@类型")]
        public string Type { get; set; }

        /// <summary>
        /// 传感器描述
        /// </summary>
        [DataMember(Name = "@描述")]
        public string Description { get; set; }

        /// <summary>
        /// 使用状态
        /// </summary>
        [DataMember(Name = "@状态")]
        public string Status { get; set; }



        /// <summary>
        /// 空间分辨率
        /// </summary>
        [DataMember(Name = "@空间分辨率")]
        public string SpaceResolution { get; set; }

        /// <summary>
        /// 光谱分辨率
        /// </summary>
        [DataMember(Name = "@光谱分辨率")]
        public string OpticalResolution { get; set; }

        /// <summary>
        /// 是否可以请求拍摄
        /// </summary>
        //[DataMember(Name = "@是否可以请求拍摄")]
        public bool IsCanShoot { get; set; }

        [DataMember(Name = "@详细信息")]
        public string DetailInfo { get; set; }

        [DataMember(Name = "模式")]
        public List<SensorModel> Models { get; set; }


        public Sensor()
        {
            Models=new List<SensorModel>();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public SensorModel SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("SelectedModel");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    [DataContract]
    public class SensorModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Name = "@名称")]
        public string Name { get; set; }
        /// <summary>
        /// 分辨率
        /// </summary>
        [DataMember(Name = "@分辨率")]
        public string Resolution { get; set; }
        /// <summary>
        /// 幅宽
        /// </summary>
        [DataMember(Name = "@幅宽")]
        public double Breadth { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
