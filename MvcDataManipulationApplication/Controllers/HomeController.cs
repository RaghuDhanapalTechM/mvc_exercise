using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;
using MvcDataManipulationApplication.Models;
namespace MvcDataManipulationApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            DataTable dt = new DataTable("Data");
            //dt.Columns.Add("Id", Type.GetType("System.Int32"));
            //dt.Columns.Add("FName", Type.GetType("System.String"));
            //dt.Columns.Add("LName", Type.GetType("System.String"));
            //dt.Columns.Add("Age", Type.GetType("System.String"));
            //dt.Columns.Add("Dept", Type.GetType("System.String"));

            //DataRow dr = dt.NewRow();
            //dr["Id"] = 1;
            //dr["FName"] = "Surendra";
            //dr["LName"] = "Kushwaha";
            //dr["Age"] = "34";
            //dr["Dept"] = "IT";
            //dt.Rows.Add(dr);
            //dt.Rows.Add("2,Raghu,Dhanapal,28,S/W Development".Split(",".ToArray()));
            string filepath = Path.Combine(Server.MapPath("App_Data"), "MyData.xml");
            
           // dt.WriteXml (filepath, XmlWriteMode.WriteSchema);
            dt.ReadXml(filepath); 
            List<MyData> lstdata = DataTableToListOfObject(dt);
            ViewBag.DataDispaly = lstdata;
            return View();
        }

        public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }
        public List<MyData> JSONStringDeSerializer(string data)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<MyData> lstdata =
                   (List<MyData>)json_serializer.DeserializeObject(data);
            return lstdata;
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact(int id)
        {
            DataTable dt = new DataTable("Data");
           
            string filepath = Path.Combine(Server.MapPath("../../App_Data"), "MyData.xml");

           
            dt.ReadXml(filepath);
            MyData data = DataTableToListOfObject(dt,  id);
            ViewBag.SingleDataDispaly = data;
            ViewBag.Message = "Record is in Edit Mode.";

            return View();
        }
        [HttpPost]
        public ActionResult Update([Bind(Include = "Id,FName")] MyData data )
        {
            DataTable dt = new DataTable("Data");

            string filepath = Path.Combine(Server.MapPath("../App_Data"), "MyData.xml");


            dt.ReadXml(filepath);
            DataRow[] rds = dt.Select(string.Format("Id={0}", data.Id) ) ;
            if (rds.Length == 1) {
                rds[0]["FName"] = data.FName;
            }
            dt.AcceptChanges();
            dt.WriteXml(filepath, XmlWriteMode.WriteSchema);
            return  RedirectToAction("index" );
        }
        public List<MyData> DataTableToListOfObject(DataTable dt)
        {
            List<MyData> lstdata = null;
            lstdata = (from r in dt.AsEnumerable()
                           // where r.Field<Int32?>("Id").Equals(1)
                       select new MyData
                       {
                           Id = r.Field<Int32?>("Id"),
                           FName = r.Field<string>("FName"),
                           LName = r.Field<string>("LName"),
                           Age = r.Field<string>("Age"),
                           Dept = r.Field<string>("Dept")
                       }
                ).ToList();

            return lstdata;
        }

        private  MyData  DataTableToListOfObject(DataTable dt, int id)
        {
            List<MyData> lstdata = null;
            lstdata = (from r in dt.AsEnumerable()
                           where r.Field<Int32?>("Id").Equals(id)
                       select new MyData
                       {
                           Id = r.Field<Int32?>("Id"),
                           FName = r.Field<string>("FName"),
                           LName = r.Field<string>("LName"),
                           Age = r.Field<string>("Age"),
                           Dept = r.Field<string>("Dept")
                       }
                ).ToList();

            return lstdata[0];
        }
    }
}