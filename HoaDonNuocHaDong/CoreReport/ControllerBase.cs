using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using HvitFramework.CoreData;
using System.Reflection;

namespace HvitFramework
{
    public class ObjectPool<T>
    {
        private ConcurrentBag<T> _objects;
        private Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public T GetObject()
        {
            T item;
            if (_objects.TryTake(out item)) return item;
            return _objectGenerator();
        }

        public void PutObject(T item)
        {
            _objects.Add(item);
        }
    }
    public class PageDataTable:ModelBase
    {
        public int SRecord { get { return FieldsMap[0] != null ? (int)FieldsMap[0] : 0; } set { FieldsMap[0] = value; } }
        public int SPage { get { return FieldsMap[1] != null ? (int)FieldsMap[1] : 0; } set { FieldsMap[1] = value; } }
        public int Prev2 { get { return FieldsMap[2] != null ? (int)FieldsMap[2] : 0; } set { FieldsMap[2] = value; } }
        public int Prev1 { get { return FieldsMap[3] != null ? (int)FieldsMap[3] : 0; } set { FieldsMap[3] = value; } }
        public int Curr { get { return FieldsMap[4] != null ? (int)FieldsMap[4] : 0; } set { FieldsMap[4] = value; } }
        public int Next1 { get { return FieldsMap[5] != null ? (int)FieldsMap[5] : 0; } set { FieldsMap[5] = value; } }
        public int Next2 { get { return FieldsMap[6] != null ? (int)FieldsMap[6] : 0; } set { FieldsMap[6] = value; } }
        protected override Type TransferType()
        {
            return this.GetType();
        }
    }
    public class SearchOption<T> where T : ModelBase, new()
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int SortDirection { get; set; } //0 = ASC, 1 = DESC
        public string SortColumn { get; set; }
        public string SearchData { get; set; }
        public SqlParameter[] GetSearchParams()
        {
            SqlParameter[] arrParams;

            string strSortColumn = "";
            string strPage = "";
            string strSearch = "";

            if (string.IsNullOrEmpty(SortColumn))
            {
                SortColumn = (new T()).TableName + "ID";
                SortDirection = 0;
            }
            strSortColumn = " order by " + SortColumn + (SortDirection == 0 ? " ASC" : " DESC");
            if (PageSize == 0)
            {
                PageSize = 50;
                PageIndex = 1;
            }
            strPage = string.Format(" offset {0} rows fetch next {1} rows only", PageSize * PageIndex , PageSize);
            
            if (!string.IsNullOrEmpty(SearchData)) strSearch ="where "+ SearchData;
            arrParams = new SqlParameter[]
            {
                new SqlParameter("@strDataSort",strSortColumn),
                new SqlParameter("@strDataPage",strPage),
                new SqlParameter("@strDataSearch",strSearch)
            };
            return arrParams;
        }
    }
    public class ControllerBase<T> where T:ModelBase,new()
    {
        public SearchOption<T> CurrentFilter { get; set; }
        public PageDataTable PageData { get; set; }
        
        public string InsertProcName, UpdateProcName, DeleteProcName, DeleteAllProcName, SearchProcName, ViewIDProcName, CountRecord, LastestID;
        public ControllerBase()
        {
            T objT = new T();
            CurrentFilter = new SearchOption<T>();
            InsertProcName = objT.TableName + "_ProcInsert";
            UpdateProcName = objT.TableName + "_ProcUpdate";
            DeleteProcName = objT.TableName + "_ProcDelete";
            DeleteAllProcName = objT.TableName + "_ProcDeleteAll";
            SearchProcName = objT.TableName + "_ProcView";
            ViewIDProcName = objT.TableName + "_ProcViewID";
            CountRecord = objT.TableName + "_ProcPageInfo";
            LastestID = objT.TableName + "_ProcLastestID";
        }
        public List<T> Search()
        {
            List<T> LstModelRet  = new List<T>();
            Dictionary<string, StringBuilder> DicData = new Dictionary<string, StringBuilder>();
            using (SqlConnection con = new SqlConnection(DataConfig.connectionString))
            {
                T objT = new T();
                objT.ActiveReference();
                con.Open();
                SqlCommand sc = new SqlCommand(SearchProcName, con);
                sc.CommandType = CommandType.StoredProcedure;
                sc.Parameters.AddWithValue("@strDataRef", objT.DataRef);
                sc.Parameters.AddWithValue("@strDataIRef", objT.DataIJRef);
                if (CurrentFilter != null)
                {
                    sc.Parameters.AddRange(CurrentFilter.GetSearchParams());
                    PageData = GetPageData();
                }
                SqlDataReader dreader = sc.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                if (dreader.HasRows)
                {
                    while (dreader.Read())
                    {
                        object[] objData = new object[dreader.FieldCount];
                        dreader.GetValues(objData);
                        objT.FieldsMap = objData;
                        LstModelRet.Add((T)objT.Clone());
                    }
                }
                dreader.Close();
                con.Close();
            }
            
            return LstModelRet;
        }
        public string SearchJSON()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, StringBuilder> DicData = new Dictionary<string, StringBuilder>();
            using (SqlConnection con = new SqlConnection(DataConfig.connectionString))
            {
                T objT = new T();
                objT.ActiveReference();
                con.Open();
                SqlCommand sc = new SqlCommand(SearchProcName, con);
                sc.CommandType = CommandType.StoredProcedure;
                sc.Parameters.AddWithValue("@strDataRef", objT.DataRef);
                sc.Parameters.AddWithValue("@strDataIRef", objT.DataIJRef);
                if (CurrentFilter != null)
                {
                    sc.Parameters.AddRange(CurrentFilter.GetSearchParams());
                    PageData = GetPageData();
                }
                SqlDataReader dreader = sc.ExecuteReader();
                System.IO.StringWriter sw = new System.IO.StringWriter(sb); 

                using (JsonWriter jsonWriter = new JsonTextWriter(sw))
                {
                    jsonWriter.WriteStartArray();
                    while (dreader.Read())
                    {
                        jsonWriter.WriteStartObject();

                        int fields = dreader.FieldCount;

                        for (int i = 0; i < fields; i++)
                        {
                            jsonWriter.WritePropertyName(dreader.GetName(i));
                            jsonWriter.WriteValue(dreader[i]);
                        }
                        jsonWriter.WriteEndObject();
                    }
                    jsonWriter.WriteEndArray();
                }

                dreader.Close();
                con.Close();
            }
            return sb.ToString();
        }
        public List<T> Query(string query,params SqlParameter[] sp)
        {
            List<T> LstModelRet = new List<T>();
            Dictionary<string, StringBuilder> DicData = new Dictionary<string, StringBuilder>();
            using (SqlConnection con = new SqlConnection(DataConfig.connectionString))
            {
                T objT = new T();
                objT.ActiveReference();
                con.Open();
                SqlCommand sc = new SqlCommand(query, con);
                if (query.Contains(" "))
                    sc.CommandType = CommandType.Text;
                else
                    sc.CommandType = CommandType.StoredProcedure;
                if(sp!=null && sp.Length>0)
                    sc.Parameters.AddRange(sp);
                SqlDataReader dreader = sc.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                if (dreader.HasRows)
                {
                    while (dreader.Read())
                    {
                        object[] objData = new object[dreader.FieldCount];
                        dreader.GetValues(objData);
                        objT.FieldsMap = objData;
                        LstModelRet.Add((T)objT.Clone());
                    }
                }
                dreader.Close();
                con.Close();
            }

            return LstModelRet;
        }
        public void Insert(T obj)
        {
            SqlConnection con = new SqlConnection(DataConfig.connectionString);
            con.Open();
            SqlCommand sc = new SqlCommand(InsertProcName, con);
            sc.CommandType = CommandType.StoredProcedure;
            PropertyInfo[] pi = obj.GetType().GetProperties();
            for(int i =1;i<=obj.MaxPosModelField;i++)
            {
                sc.Parameters.Add(new SqlParameter(obj.Fields[i],pi[i].GetValue(obj,null)));
            }
            sc.ExecuteNonQuery();
            con.Close();
        }
        public void Update(T obj)
        {
            SqlConnection con = new SqlConnection(DataConfig.connectionString);
            con.Open();
            SqlCommand sc = new SqlCommand(UpdateProcName, con);
            sc.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i <= obj.MaxPosModelField; i++)
            {
                sc.Parameters.Add(new SqlParameter(obj.Fields[i], obj.FieldsMap[i]));
            }
            sc.ExecuteNonQuery();
            con.Close();
        }
        public void Delete(T obj)
        {
            SqlConnection con = new SqlConnection(DataConfig.connectionString);
            con.Open();
            SqlCommand sc = new SqlCommand(UpdateProcName, con);
            sc.CommandType = CommandType.StoredProcedure;
            sc.Parameters.Add(new SqlParameter(obj.Fields[0], obj.FieldsMap[0]));
            sc.ExecuteNonQuery();
            con.Close();
        }
        public PageDataTable GetPageData()
        {
            using (SqlConnection con = new SqlConnection(DataConfig.connectionString))
            {
                PageDataTable objT = new PageDataTable();
                con.Open();
                SqlCommand sc = new SqlCommand(CountRecord, con);
                sc.CommandType = CommandType.StoredProcedure;
                sc.Parameters.AddWithValue("@PSize", CurrentFilter.PageSize);
                sc.Parameters.AddWithValue("@PCurr", CurrentFilter.PageIndex);
                SqlDataReader dreader = sc.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                if (dreader.HasRows)
                {
                    while (dreader.Read())
                    {
                        object[] objData = new object[dreader.FieldCount];
                        dreader.GetValues(objData);
                        objT.FieldsMap = objData;
                    }
                }
                dreader.Close();
                con.Close();
                return objT;
            }
        }
        
    }
}
