using System;
using System.Collections.Generic;

namespace HoursForYourLib
{
    public class Module
    {
        //declare fields:
        private string moduleName;
        private string moduleCode;
        private double credits;
        private double classHours;
        private DateTime semesterStartDate;
        private double selfStudyHours;
        private Dictionary<string, double> completedHours;

        public Module()
        { 
        }

        //constructor
        public Module(string moduleName, string moduleCode, double credits, double classHours, DateTime semesterStartDate)
        {
            this.moduleName=moduleName;
            this.moduleCode=moduleCode;
            this.credits=credits;
            this.classHours=classHours;
            this.SelfStudyHours = 0;
            this.completedHours = new Dictionary<string, double>();
            this.semesterStartDate = semesterStartDate;
        }//end constructor

        //getter and setters
        public string ModuleName { get => moduleName; set => moduleName=value; }
        public string ModuleCode { get => moduleCode; set => moduleCode=value; }
        public double Credits { get => credits; set => credits=value; }
        public double ClassHours { get => classHours; set => classHours=value; }
        public Dictionary<string, double> CompletedHours { get => completedHours; set => completedHours=value; }
        public double SelfStudyHours { get => selfStudyHours; set => selfStudyHours=value; }
        public DateTime SemesterStartDate { get => semesterStartDate; set => semesterStartDate=value; }
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________