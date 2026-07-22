using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Text;

namespace EastCoastEstatesScheduling
{
    public class Client : Person
    {
        private List<House> houses;

        private LicenseType license { get; set; }

        public DateOnly? ContractStart { get; private set; }
        public DateOnly? ContractEnd { get; private set; }
        public int ContractHouseCount { get; private set; }

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

        public void setContract(DateOnly start, DateOnly end, int numberHouses)
        {
            ContractStart = start;
            ContractEnd = end;
            ContractHouseCount = numberHouses;
        }

        public void clearContract()
        {
            ContractStart = null;
            ContractEnd = null;
            ContractHouseCount = 0;
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
