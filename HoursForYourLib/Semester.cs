using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoursForYourLib
{
    public class Semester
    {
        private string semesterName;
        private double numberOfWeeks;
        private DateTime startDate;
        private List<Module> modules;

        public Semester(string semesterName, double numberOfWeeks, DateTime startDate, List<Module> modules)
        {
            this.semesterName=semesterName;
            this.numberOfWeeks=numberOfWeeks;
            this.startDate=startDate;
            this.modules=modules;
        }

        public string SemesterName { get => semesterName; set => semesterName=value; }
        public double NumberOfWeeks { get => numberOfWeeks; set => numberOfWeeks=value; }
        public DateTime StartDate { get => startDate; set => startDate=value; }
        public List<Module> Modules { get => modules; set => modules=value; }
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________