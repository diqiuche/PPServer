using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SatelliteCore.Models
{
    [DataContract]
    public class  PSE
    {
        //public string TypeID { get; set; }

        /// <summary>
        /// 根数标识
        /// </summary>
        [DataMember(Name = "ID")]
        public long ID { get; set; }

        ///// <summary>
        ///// 卫星
        ///// </summary>
        //public Satellite Owner { get; set; }

        /// <summary>
        /// 根数时间
        /// </summary>
        [DataMember(Name = "PseTime")]
        public DateTime PseTime { get; set; }

        /// <summary>
        /// 天文时间
        /// </summary>
        [DataMember(Name = "AstronomyTime")]
        public DateTime? AstronomyTime { get; set; }

        //public string Name { get; set; }

        /// <summary>
        /// 轨道长半轴(Km)
        /// </summary>
        [DataMember(Name = "A")]
        public double A { get; set; }

        /// <summary>
        /// 轨道偏心率
        /// </summary>
        [DataMember(Name = "E")]
        public double E { get; set; }

        /// <summary>
        /// 轨道平面倾角(度)
        /// </summary>
        [DataMember(Name = "I")]
        public double I { get; set; }

        /// <summary>
        /// 升交点赤经(度)
        /// </summary>
        [DataMember(Name = "Omg")]
        public double Omg { get; set; }

        /// <summary>
        /// 近地点俯角(度)
        /// </summary>
        [DataMember(Name = "W")]
        public double W { get; set; }
        /// <summary>
        /// 平近点角(度)
        /// </summary>
        [DataMember(Name = "M")]
        public double M { get; set; }

        /// <summary>
        /// 面质比
        /// </summary>
        [DataMember(Name = "Cdsm")]
        public double Cdsm { get; set; }

        /// <summary>
        /// (圈每天)
        /// </summary>
        [DataMember(Name = "N0")]
        public double N0 { get; set; }
        /// <summary>
        /// (圈每天平方)
        /// </summary>
        [DataMember(Name = "N1")]
        public double N1 { get; set; }
        /// <summary>
        /// (圈每天立方)
        /// </summary>
        [DataMember(Name = "N2")]
        public double N2 { get; set; }

        /// <summary>
        /// 大气阻尼系数
        /// </summary>
        [DataMember(Name = "Dqznxs")]
        public double Dqznxs { get; set; }

        /// <summary>
        /// 光压辐射系数
        /// </summary>
        [DataMember(Name = "Gyfsxs")]
        public double Gyfsxs { get; set; }

        /// <summary>
        /// 轨道圈号
        /// </summary>
        [DataMember(Name = "OrbitId")]
        public double OrbitId { get; set; }

        /// <summary>
        /// 静止卫星经度
        /// </summary>
        [DataMember(Name = "Lon")]
        public double? StaticLon { get; set; }

        /// <summary>
        /// 静止卫星纬度
        /// </summary>
        [DataMember(Name = "Lat")]
        public double? StaticLat { get; set; }

        /// <summary>
        /// 静止卫星高度
        /// </summary>
        [DataMember(Name = "Height")]
        public double? StaticHeight { get; set; }

        //public Color color { get; set; }

        /// <summary>
        /// 征兆半径
        /// </summary>
        [DataMember(Name = "R")]
        public double R { get; set; }
        ///// <summary>
        ///// 卫星最大的侧摆角
        ///// </summary>
        //public double MaxAngle { get; set; }
        ///// <summary>
        ///// 传感器夹角
        ///// </summary>
        //public double SensorAngle { get; set; }
        ///// <summary>
        ///// 对于Sar类型的传感器卫星的偏转角度
        ///// </summary>
        //public double SarAngle { get; set; }
    }



}
