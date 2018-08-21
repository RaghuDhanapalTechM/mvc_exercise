using MvcCrudOperations.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCrudOperations.Controllers
{
    public class ResultController : Controller
    {//assigning variables
        DataTable dtStudents = new DataTable("Students");
        string strXmlFilePath = string.Empty;
        string strAppPath = string.Empty;
        List<StudentData> lstStudentData = null;
        List<StudentData> lstEditedStudentData = null;
        // GET: Result
        public ActionResult Index()
        {
            return View();
        }
        // GET: Result
        public JsonResult GetData()
        {
            strAppPath = AppDomain.CurrentDomain.BaseDirectory;
            strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");

            //fill the table with data from xml
            dtStudents.ReadXml(strXmlFilePath);
            lstStudentData = PopulateDataInList(dtStudents);

            return Json(lstStudentData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSingleData(int id)
        {
            strAppPath = AppDomain.CurrentDomain.BaseDirectory;
            strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");
            dtStudents.ReadXml(strXmlFilePath);
            lstEditedStudentData = PopulateEditedDataInList(dtStudents, id);
            StudentData sData = lstEditedStudentData[0];

            return Json(new { Data = sData, Status = true, Message = "Record Found" }, JsonRequestBehavior.AllowGet);
        }

        //add
        public JsonResult InsertData(StudentData data)
        {
            string message = string.Empty;
            bool flag = false;
            try
            {
                strAppPath = AppDomain.CurrentDomain.BaseDirectory;
                strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");

                dtStudents.ReadXml(strXmlFilePath);

                DataRow rds = dtStudents.NewRow();

                rds["Id"] = MaxValueofId(dtStudents) + 1;
                rds["Name"] = data.strStudentName;
                rds["Department"] = data.strStudentDepartment;

                dtStudents.Rows.Add(rds);
                dtStudents.AcceptChanges();
                dtStudents.WriteXml(strXmlFilePath, XmlWriteMode.WriteSchema);
                message = "Record Inserted successfully";
                flag = true;
            }
            catch
            {
                message = "Error occured please try after some time";
                flag = false;
            }

            return Json(new { Message = message, Status = flag }, JsonRequestBehavior.AllowGet);
        }
        //add

        //get the maximum value of id coulumn in data table
        public int MaxValueofId(DataTable dtStudents)
        {
            var MaxValue = dtStudents.AsEnumerable()
                               .Max(row => row["Id"]);

            MaxValue = (MaxValue ?? 0);

            return (int)MaxValue;
        }


        //delete
        public JsonResult GetSingleDataToDelete(int id)
        {
            string message = string.Empty;
            bool flag = false;
            try
            {
                
                string searchExpression = string.Empty;

                strAppPath = AppDomain.CurrentDomain.BaseDirectory;
                strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");

                dtStudents.ReadXml(strXmlFilePath);
                lstEditedStudentData = PopulateEditedDataInList(dtStudents, id);
                searchExpression = "ID = " + id;

                //delete the row for the selected id
                DataRow[] dr = dtStudents.Select(searchExpression);
                dr[0].Delete();
                dtStudents.AcceptChanges();
                dtStudents.WriteXml(strXmlFilePath, XmlWriteMode.WriteSchema);
                message = "Record updated successfully";
                
            }
            catch
            {
                message = "Error occured please try after some time";
                flag = false;
            }
            return Json(new { Message = message, Status = flag }, JsonRequestBehavior.AllowGet);
        }
        //delete

        public JsonResult UpdateData(StudentData data)
        {
            string message = string.Empty;
            bool flag = false;
            try
            {
                strAppPath = AppDomain.CurrentDomain.BaseDirectory;
                strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");

                dtStudents.ReadXml(strXmlFilePath);
                DataRow[] rds = dtStudents.Select(string.Format("Id={0}", data.intStudentId));
                if (rds.Length == 1)
                {
                    rds[0]["Name"] = data.strStudentName;
                    rds[0]["Department"] = data.strStudentDepartment;
                }

                dtStudents.AcceptChanges();
                dtStudents.WriteXml(strXmlFilePath, XmlWriteMode.WriteSchema);
                message = "Record updated successfully";
                flag = true;
            }
            catch
            {
                message = "Error occured please try after some time";
                flag = false;
            }

            return Json(new { Message = message, Status = flag }, JsonRequestBehavior.AllowGet);
        }
        // GET: Result/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Result/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Result/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Result/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Result/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Result/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Result/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //------------------------
        //populate all the data in list
        public List<StudentData> PopulateDataInList(DataTable dtStudents)
        {
            List<StudentData> lstStudentData = null;

            lstStudentData = (from r in dtStudents.AsEnumerable()
                              select new StudentData
                              {
                                  intStudentId = r.Field<Int32>("Id"),
                                  strStudentName = r.Field<string>("Name"),
                                  strStudentDepartment = r.Field<string>("Department")

                              }).ToList();
            return lstStudentData;
        }
        //populate data in list with filter condition
        public List<StudentData> PopulateEditedDataInList(DataTable dtStudents, int id)
        {
            lstEditedStudentData = null;
            lstEditedStudentData = (from r in dtStudents.AsEnumerable()
                                    where r.Field<Int32>("Id").Equals(id)
                                    select new StudentData
                                    {
                                        intStudentId = r.Field<Int32>("Id"),
                                        strStudentName = r.Field<string>("Name"),
                                        strStudentDepartment = r.Field<string>("Department")

                                    }).ToList();
            return lstEditedStudentData;
        }

    }
}
