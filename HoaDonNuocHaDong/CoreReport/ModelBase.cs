using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HvitFramework
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ModelBase    
    {
        static CultureInfo culInfo = Thread.CurrentThread.CurrentCulture;
        static IFormatProvider culture = culInfo;
        public string TableName;
        public List<string> Fields = new List<string>();
        public int MaxPosModelField { get; set; }
        [JsonProperty]
        public object[] FieldsMap { get; set; }
        public List<char> FieldsType = new List<char>();
        public string DataRef = "";
        public string DataIJRef = "";
        public Dictionary<string, string> FilterData = new Dictionary<string, string>();
        Type t;
        PropertyInfo[] pi;
        public ModelBase()
        {
            t = TransferType();
            TableName = t.Name;
            pi = t.GetProperties();
            FieldsMap = new object[pi.Length];
            foreach (PropertyInfo p in pi)
            {
                string s = p.Name;
                if (s.Contains("_"))
                {

                    DataRef += "," + s.Replace("_", ".");
                    string str = s.Split('_')[0];
                    DataIJRef += string.Format(" inner join {0} on x.{0}ID={0}.{0}ID ", str);
                }
                else
                {
                    Fields.Add(s);
                    FieldsType.Add(p.PropertyType.Name[0]);
                }
            }
        }
       
        protected abstract Type TransferType();
       
        public ModelBase Clone()
        {
            return (ModelBase)this.MemberwiseClone();
        }
        public virtual void ActiveReference()
        {

        }
        public void UpdateByModel(ModelBase m)
        {
            for (int i = 0; i < m.FieldsMap.Length; i++)
                if (m.FieldsMap[i] != null)
                    FieldsMap[i] = m.FieldsMap[i];
        }
        protected void CallReference<T>(List<T> RefLst, int FieldSource, params int[] FieldDest) where T : new()
        {
            int val = (int)FieldsMap[FieldSource];
            string ID = (new T()).GetType().Name + "ID";
            T obj = RefLst.Find(x => (int)x.GetType().GetProperty(ID).GetValue(x) == val);
            if (obj != null)
                foreach (int num in FieldDest)
                {
                    FieldsMap[num] = obj.GetType().GetProperty(pi[num].Name).GetValue(obj);
                }
        }
        protected void CallReference<T>(int FieldSource,params int[] FieldDest) where T:ModelBase,new()
        {
            string TableToJoin = (new T()).TableName;
            string FieldRefID = this.Fields[FieldSource];
            string TableRef = FieldRefID.TrimEnd('D','I');
            DataIJRef += string.Format(" inner join {0} on {1}.{0}ID={0}.{0}ID ", TableRef, TableToJoin);
            foreach(int x in FieldDest)
            {
                DataRef += string.Format(" ,{0}.{1}", TableRef, Fields[x]);
            }
        }
        protected void SetINT(int pos,object val)
        {
            FieldsMap[pos] = val;
        }
        protected int GetINT(int pos)
        {
            try
            {
                return FieldsMap[pos] != null ? int.Parse(FieldsMap[pos].ToString()) : 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        protected void SetSTR(int pos,object val)
        {
            FieldsMap[pos] = val;
        }
        protected string GetSTR(int pos)
        {
            try
            {
                return !string.IsNullOrEmpty(FieldsMap[pos].ToString()) ? FieldsMap[pos].ToString() : "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        protected void SetDT(int pos, object val)
        {
            FieldsMap[pos] = val;
        }
        protected DateTime GetDT(int pos)
        {
            try
            {
                return FieldsMap[pos] != null ? DateTime.Parse(FieldsMap[pos].ToString(), culture) : new DateTime();
            }
            catch(Exception ex)
            {
                return new DateTime();
            }
        }
        protected void SetD(int pos, object val)
        {

            FieldsMap[pos] = val;
        }
        protected double GetD(int pos)
        {
            try
            {
                return FieldsMap[pos] != null ? double.Parse(FieldsMap[pos].ToString()) : 0.0;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
    }
}
