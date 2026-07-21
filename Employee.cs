using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Text;

namespace GolfCoastEstatesBillingLogicDemo
{
    public class Employee : Person
    {
        private DayOfWeek[] workDay;

        //I can see workDay been an array with a max amount of 7 days. Then another tab for exception day's which are dates that the employee doesn't work.
        private List<Exception> exceptionDay;

        private int maxDailyHouse = 6;

        public Employee(string name, DayOfWeek[] workDay)
        {
            this.name = name;
            this.workDay = workDay;
            exceptionDay = new List<Exception>();
        }

        public int getCapacityForDate(DateOnly date)
        {
            Exception found = exceptionDay.Find(e => e.getDate().Equals(date));
            if (found != null)
            {
                return found.getHouses();
            }

            return workDay.Contains(date.DayOfWeek) ? maxDailyHouse : 0;
        }

        public class Exception
        {
            DateOnly date;
            int numHouses;
            string reason;

            public DateOnly getDate()
            {
                return date;
            }

            public void setDate(DateOnly date)
            {
                this.date = date;
            }

            public int getHouses()
            {
                return numHouses;
            }

            public void setHouses(int houses)
            {
                numHouses = houses;
            }

            public string getReason()
            {
                return reason;
            }

            public void setReason(string reason)
            {
                this.reason = reason;
            }

            //Internal seems to allow anything in control base to edit, which i don't think is the best but for now it should work
            internal Exception(DateOnly date, string reason, int numHouses)
            {
                this.date = date;
                this.numHouses = numHouses;
                this.reason = reason;
            }
        }

        public int getMaxDailyHouse()
        {
            return maxDailyHouse;
        }

        public void setMaxDailyHouse(int maxDailyHouse)
        {
            this.maxDailyHouse = maxDailyHouse;
        }

        public DayOfWeek[] getWorkDay()
        {
            return workDay;
        }

        public void setWorkDay(DayOfWeek[] newWorkDay)
        {
            this.workDay = newWorkDay;
        }

        public List<Exception> getExceptions()
        {
            return exceptionDay;
        }

        public void addException(DateOnly date, string reason, int numHouses)
        {
            exceptionDay.Add(new Exception(date, reason, numHouses));
        }

        public bool removeException(DateOnly date)
        {
            Exception found = exceptionDay.Find(e => e.getDate().Equals(date));
            if (found == null)
                return false;
            exceptionDay.Remove(found);
            return true;
        }
    }
}
