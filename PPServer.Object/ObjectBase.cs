using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.Data;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/*************************************************
 * Creater:liuyan
 * Version:0.1.0.0
 * Create Date:2013/7/2
 * Description:提供传输对象的基类，包括序列化，反序列化等方法
 * 
 * Modification Records:
 * Version     Date         User            Description
 * 
 * 
 *************************************************/
namespace PPServer.Object
{
    /// <summary>
    /// 基类
    /// </summary>
    [Serializable]
    public class ObjectBase
    {
        #region 对象序列化到XML
        /// <summary>
        /// 序列化对象到XML文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        public void SaveObjToXML(string FilePath)
        {
            if (this == null) return;
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            MemoryStream stream = new MemoryStream();
            XmlTextWriter xtw = new XmlTextWriter(stream, Encoding.UTF8);
            xtw.Formatting = Formatting.Indented;
            try
            {
                serializer.Serialize(stream, this);
            }
            catch { return; }

            stream.Position = 0;
            string returnStr = string.Empty;
            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
            {
                returnStr = sr.ReadToEnd();
            }
            File.WriteAllText(FilePath, returnStr, Encoding.UTF8);
        }

        /// <summary>
        /// 序列化对象到内存流
        /// </summary>
        /// <returns></returns>
        public MemoryStream TurnXmlFromObj()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            MemoryStream stream = new MemoryStream();
            try
            {
                XmlTextWriter xtw = new XmlTextWriter(stream, Encoding.UTF8);
                xtw.Formatting = Formatting.Indented;
                serializer.Serialize(stream, this);
            }
            catch { }
            return stream;
        }

        #endregion

        #region DataContract对象序列化到XML
        /// <summary>
        /// DataContract对象序列化到XML
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        public void SaveDataContractToXML(string FilePath)
        {
            if (this == null) return;
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(this.GetType());
                XmlTextWriter writer = new XmlTextWriter(FilePath, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                serializer.WriteObject(writer, this);
                writer.Close();
            }
            catch { }
        }
        #endregion

        #region 对象序列化为二进制文件
        /// <summary>
        /// 用二进制流序列化对象到文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        public void SaveObjToFile(string FilePath)
        {
            if (this == null) return;
            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, this);
                }
            }
            catch { }
        }
        #endregion

        #region XML到反序列化到对象
        /// <summary>
        /// XML到反序列化到对象
        /// </summary>
        /// <typeparam name="T">反序列化类</typeparam>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static T LoadObjFromXML<T>(string FilePath) where T : ObjectBase
        {
            string data = File.ReadAllText(FilePath, Encoding.UTF8);
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
                {
                    sw.Write(data);
                    sw.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return ((T)serializer.Deserialize(stream));
                }
            }
        }

        /// <summary>
        /// XML到反序列化到对象
        /// </summary>
        /// <typeparam name="T">反序列化类</typeparam>
        /// <param name="mstream">内存流</param>
        /// <returns></returns>
        public static T LoadObjFromXML<T>(MemoryStream mstream) where T : ObjectBase
        {
                mstream.Seek(0, SeekOrigin.Begin);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return ((T)serializer.Deserialize(mstream));
        }

        #endregion

        #region XML到反序列化到DataContract
        /// <summary>
        /// XML到反序列化到DataContract
        /// </summary>
        /// <typeparam name="T">输出类型</typeparam>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static T LoadDataContractFromXML<T>(string FilePath) where T : ObjectBase
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                XmlTextReader reader = new XmlTextReader(FilePath);
                T newInstance=(T)serializer.ReadObject(reader);
                reader.Close();
                return newInstance;
            }
            catch { }
            return null;
        }
        #endregion

        #region 从文件反序列化到对象
        /// <summary>
        /// 从文件反序列化到对象
        /// </summary>
        /// <typeparam name="T">输出的类型</typeparam>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static T LoadObjFromFile<T>(string FilePath) where T : ObjectBase
        {
            T _data = default(T);
            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _data = (T)formatter.Deserialize(fs);
                }
            }
            catch { }
            return _data;
        }
        #endregion

        /// <summary>
        /// 从对象列表生成数据表
        /// </summary>
        /// <typeparam name="T">数据源类</typeparam>
        /// <param name="objs">对象列表</param>
        /// <returns></returns>
        public static DataTable GetDataTableFromClass<T>(List<T> objs) where T : ObjectBase
        {
            DataTable dt = new DataTable();
            PropertyInfo[] pis = typeof(T).GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                dt.Columns.Add(pi.Name, pi.PropertyType);
            }
            foreach (T obj in objs)
            {
                DataRow dr = dt.NewRow();
                if (obj != null)
                {
                    foreach (PropertyInfo pi in pis)
                    {
                        dr[pi.Name] = pi.GetValue(obj, null);
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}
