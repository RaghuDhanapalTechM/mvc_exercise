using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDataManipulationApplication.Models
{
    public class Student
    {
       public int intStudentId { get; set; }

        public string strStudentName { get; set; }

        public string stStudentSubject { get; set; }

  
    }

    public class MyData
    {
        public int? Id { get; set; }

        public string FName { get; set; }

        public string LName { get; set; }
        public string Age { get; set; }
        public string Dept { get; set; }


    }
}