using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoursForYourLib
{
    public class Module
    {
        private string moduleName;
        private string moduleCode;
        private double credits;
        private double classHours;

        public Module(string moduleName, string moduleCode, double credits, double classHours)
        {
            this.moduleName=moduleName;
            this.moduleCode=moduleCode;
            this.credits=credits;
            this.classHours=classHours;
        }

        public string ModuleName { get => moduleName; set => moduleName=value; }
        public string ModuleCode { get => moduleCode; set => moduleCode=value; }
        public double Credits { get => credits; set => credits=value; }
        public double ClassHours { get => classHours; set => classHours=value; }
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________