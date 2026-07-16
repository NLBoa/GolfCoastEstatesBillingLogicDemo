using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Text;

namespace GolfCoastEstatesBillingLogicDemo
{
    //An abstract class that holds the similarities of Client and Employee
    public abstract class Person
    {
        //Maybe better toSetup in Billing
        // public void setId;
        // public int getId;
        public string name { get; set; }
    }

    //Defining Employee Basics

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

    //Defining Client Information
    public class Client : Person
    {
        private List<House> houses;

        private LicenseType license { get; set; }

        public Client(LicenseType license, string name)
        {
            houses = new List<House>();
            this.license = license;
            this.name = name;
        }

        public int getNumHouses()
        {
            return houses.Count;
        }

        //Defining the basics of House
        private class House
        {
            private List<string> notes;

            public House(string address, string comments)
            {
                this.address = address;
                notes = new List<string>();
                notes.Add(comments);
            }

            public string address { get; set; }

            public void addHouseNotes(string comments)
            {
                notes.Add(comments);
            }

            public string getHouseNotes()
            {
                return string.Join(", ", notes);
            }
        }

        public string getPosition()
        {
            return license.ToString();
        }

        public void addHouse(string address, string notes)
        {
            House temp = new House(address, notes);
            houses.Add(temp);
        }

        //Helper method to search through houses
        private House findHouse(string address)
        {
            House house = houses.Find(e => e.address.Equals(address));
            if (house == null)
                return null;
            return house;
        }

        public void removeHouse(string address)
        {
            House temp = findHouse(address);

            houses.Remove(temp);
        }

        public string getHouseList()
        {
            StringBuilder houseList = new StringBuilder();
            foreach (House h in houses)
            {
                houseList.Append("Address: " + h.address + "\n");
            }

            return houseList.ToString();
        }
    }
}
