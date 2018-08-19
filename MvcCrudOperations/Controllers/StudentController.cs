using MvcCrudOperations.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCrudOperations.Controllers
{
    public class StudentController : Controller
    {
        //assigning variables
        DataTable dtStudents = new DataTable("Students");
        string strXmlFilePath = string.Empty;
        string strAppPath = string.Empty;
        List<StudentData> lstStudentData = null;
        List<StudentData> lstEditedStudentData = null;

        //give the link to see all the records
        public ActionResult Information()
        {
            //dtStudents = CreateDataTable(dtStudents);
            //dtStudents.WriteXml(strXmlFilePath,XmlWriteMode.WriteSchema);
            //fill the data table from xml
            return View();
        }

        //get the records from xml and display it
        public ActionResult GetRecords()
        {
            strAppPath = AppDomain.CurrentDomain.BaseDirectory;
            strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");

            //fill the table with data from xml
            dtStudents.ReadXml(strXmlFilePath);
            lstStudentData = PopulateDataInList(dtStudents);
            ViewBag.listOfStudents = lstStudentData;

            return View();
        }

        //Create a new record
        public ActionResult CreateRecords()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert([Bind(Include = "intStudentId,strStudentName,strStudentDepartment")] StudentData data)
        {
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
                TempData["Message"] = "Record Added successfully";
                return RedirectToAction("GetRecords");
            }
            catch
            {
                TempData["Message"] = "Error occured please try after some time";
                return RedirectToAction("GetRecords");
            }
        }

        //get the details of the edited record alone and display it
        public ActionResult EditRecords(int id)
        {
            strAppPath = AppDomain.CurrentDomain.BaseDirectory;
            strXmlFilePath = Path.Combine((strAppPath + "App_Data"), "StudentInfo.xml");
            dtStudents.ReadXml(strXmlFilePath);
            lstEditedStudentData = PopulateEditedDataInList(dtStudents, id);
            StudentData sData = lstEditedStudentData[0];
            ViewBag.EditedRecord = sData;
            ViewBag.Message = "Record is in edit mode";
            return View();
        }

        //update the edited record
        [HttpPost]
        public ActionResult Update([Bind(Include = "intStudentId,strStudentName,strStudentDepartment")] StudentData data)
        {
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
                TempData["Message"] = "Record updated successfully";
                return RedirectToAction("GetRecords");
            }
            catch
            {
                TempData["Message"] = "Error occured please try after some time";
                return RedirectToAction("GetRecords");
            }
        }

        //get the details of the record to be deleted and then delete it
        public ActionResult DeleteRecords(int id)
        {
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

                TempData["Message"] = "Record deleted successfully";

                return RedirectToAction("GetRecords");
            }
            catch
            {
                TempData["Message"] = "Error occured please try after some time";
                return RedirectToAction("GetRecords");
            }
        }

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

        //get the maximum value of id coulumn in data table
        public int MaxValueofId(DataTable dtStudents)
        {
            var MaxValue = dtStudents.AsEnumerable()
                               .Max(row => row["Id"]);

            MaxValue = (MaxValue ?? 0);

            return (int)MaxValue;
        }

        //creating a data Table with temporary Data
        public DataTable CreateDataTable(DataTable dtStudents)
        {
            dtStudents.Columns.Add("Id", Type.GetType("System.Int32"));
            dtStudents.Columns.Add("Name", Type.GetType("System.String"));
            dtStudents.Columns.Add("Department", Type.GetType("System.String"));

            dtStudents.Rows.Add("1", "Raghu", "Physics");
            dtStudents.Rows.Add("1", "Surendar", "Chemistry");
            dtStudents.Rows.Add("1", "Prem", "Botany");

            return dtStudents;
        }

    }
}
