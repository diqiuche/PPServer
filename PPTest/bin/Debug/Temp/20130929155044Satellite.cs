using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
//using GeoVis.Plugin.卫星任务计划仿真;
//using GeoVis.Plugin.卫星任务计划仿真;
using Newtonsoft.Json;
using SatelliteCore.Common;

namespace SatelliteCore.Models
{
    [DataContract]
    public class  Satellite
    {
        //private SatelliteOrbit _orbit;

        /// <summary>
        /// 观测平台Id
        /// </summary>
        [DataMember(Name = "编号")]
        public int Id { get; set; }

        public string space;

        /// <summary>
        /// 卫星模型对应的ID
        /// </summary>
        [DataMember(Name = "模型Id")]
        public int TypeId { get; set; }


        /// <summary>
        /// 卫星名称
        /// </summary>
        [DataMember(Name = "名称")]
        public string Name { get; set; }


        /// <summary>
        /// 英文名称
        /// </summary>
        [DataMember(Name = "英文名称")]
        public string EngName { get; set; }

        /// <summary>
        /// 卫星类型
        /// </summary>
        [DataMember(Name = "类型")]
        public string Type { get; set; }



        /// <summary>
        /// 卫星描述
        /// </summary>
        [DataMember(Name = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 卫星用途
        /// </summary>
        [DataMember(Name = "用途")]
        public string UseMode { get; set; }

        /// <summary>
        /// 使用状态
        /// </summary>
        [DataMember(Name = "状态")]
        public string Status { get; set; }

        /// <summary>
        /// 轨道类型
        /// </summary>
        [DataMember(Name = "轨道类型")]
        public string OrbitType { get; set; }


        //TODO:change double

        /// <summary>
        /// 轨道高度
        /// </summary>
        [DataMember(Name = "轨道高度")]
        public string OrbitHeight { get; set; }


        ////Sar传感器的卫星的侧摆角度
        //public static double Sarangle = 10;

        //public static double hendingangle;

        /// <summary>
        /// 卫星运行时的当前偏转角度
        /// </summary>
        [JsonIgnore]
        public double Angle { get; set; }

        /// <summary>
        /// 卫星运行的最大侧摆角
        /// </summary>
        [DataMember(Name = "最大侧摆角")]
        public double MaxAngle { get; set; }

        /// <summary>
        /// 卫星运行的最大俯仰角度
        /// </summary>
       // [DataMember(Name = "最大侧摆角")]
        public double PitchAngle { get; set; }

        /// <summary>
        /// 传感器为Sar类型卫星的偏转角
        /// </summary>
        [DataMember(Name = "Sar偏转角")]
        public double SarAngle { get; set; }
        

        /// <summary>
        /// 卫星条带半宽度
        /// </summary>
        [JsonIgnore]
        public double Stripwidth { get; set; }

        /// <summary>
        /// 获取或设置卫星当前拍摄的半张角
        /// </summary>
        [DataMember(Name = "传感器角度")]
        public double SensorAngle { get; set; }


        /// <summary>
        /// 长名称
        /// </summary>
        [DataMember(Name = "长名称")]
        public string LongName { get; set; }
        /// <summary>
        /// 侧摆角范围
        /// </summary>
        [DataMember(Name = "侧摆角范围")]
        public string RollRange { get; set; }

        /// <summary>
        /// 发射时间
        /// </summary>
        [DataMember(Name = "发射时间")]
        public DateTime LaunchTime { get; set; }

        /// <summary>
        ///  设计寿命
        /// </summary>
        [DataMember(Name = "设计寿命")]
        public string DesignLife { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [DataMember(Name = "重量")]
        public double Weight { get; set; }

        /// <summary>
        /// 幅宽范围（或最大？）
        /// </summary>
        [DataMember(Name = "幅宽范围")]
        public string BreadthRange { get; set; }

        /// <summary>
        /// 分辨率范围（或最大？）
        /// </summary>
        [DataMember(Name = "分辨率范围")]
        public string ResolutionRange { get; set; }

        [DataMember(Name = "显示传感器")]
        public string SensorType { get; set; }


        [JsonIgnore]
        public string GroupName { get; set; }

        /// <summary>
        /// 轨道根数
        /// </summary>
        [JsonIgnore]
        public PSE PSE { get; set; }

        //[JsonIgnore]
        //public SatelliteOrbit Orbit
        //{
        //    get
        //    {
        //        if(_orbit==null)
        //            _orbit=new SatelliteOrbit(PSE);

        //        return _orbit;
        //    }
        //}

        [JsonIgnore]
        public SatelliteDrawElement SatelliteDrawElement { get; set; }

        /// <summary>
        /// 传感器
        /// </summary>
        [DataMember(Name = "传感器")]
        public List<Sensor> Sensors { get; set; }

        public Satellite(PSE pse)
        {
            this.PSE = pse;
            this.Sensors=new List<Sensor>();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }


    //public class SatelliteGroup
    //{
    //    public string Name { get; set; }
    //    public List<Satellite> Items { get; set; }
    //    public bool? IsChecked { get; set; }

    //    public SatelliteGroup()
    //    {
    //        Items=new List<Satellite>();
    //    }
    //}
}
