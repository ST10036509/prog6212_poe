using System;
using System.Collections.Generic;

namespace HoursForYourLib
{
    public class Calculations
    {
        //----------------------------------------------------------------------------------------------ModuleHoursAssignment

        //calculate and assign each week the defualt value per module
        public List<Module> ModuleHoursAssignment(double weeks, List<Module> modules)
        {
            //loop through modules in a semester
            foreach (Module module in modules)
            {
                //local variable declaration
                double hoursHolder;

                //calculate the self study hours and store in a temp variable
                hoursHolder = ((module.Credits * 10) / weeks) - module.ClassHours;

                //check if the hours calculated is a negative number
                if (hoursHolder < 0)
                {
                    //if it is negative then set it to 0
                    module.SelfStudyHours = 0;
                }
                else
                {
                    //if it is positive set it to the calculated value
                    module.SelfStudyHours = hoursHolder;
                }

                //generate dictinary of weeks and give default value of calcyulated self study hours
                for (int i = 0; i < weeks; i++)
                {
                    //add week and default value to dictionary
                    module.CompletedHours.Add(i, module.SelfStudyHours);
                }
            }

            return modules;
        }//end ModuleHoursAssignment method

        //----------------------------------------------------------------------------------------------IdentifyAndUpdateWeek

        //tuple to identify the week and update the hours
        public (Module UpdatedModule, int Key) IdentifyAndUpdateWeek(DateTime date, Module selectedModule, string hours)
        {
            //local variable declarations:
            double dayIndex = (date.Subtract(selectedModule.SemesterStartDate).TotalDays) / 7;//calculate an index to check which week the selected date is in
            int key = 0;
            //loop through the weeks
            foreach (KeyValuePair<int, double> week in selectedModule.CompletedHours)
            {
                //check if the index is within the selected week
                if (dayIndex >= week.Key + 1)
                {
                    //skip if it is bigger
                    continue;
                }
                else
                {
                    //update the hours
                    selectedModule.CompletedHours[week.Key] -= Double.Parse(hours);

                    //check if the hours are less than 0
                    if (selectedModule.CompletedHours[week.Key] < 0)
                    {
                        //set to default if is less than 0
                        selectedModule.CompletedHours[week.Key] = 0;
                    }

                    key = week.Key;
                    
                    break;
                }
            }

            return (selectedModule, key);
        }//end IdentifyAndUpdateWeek method
    }
}
//_______________________________...oooOOO000_End_Of_File_000OOOooo..._______________________________