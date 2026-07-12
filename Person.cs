using System.Collections.Generic;
using System.Text;

namespace GolfCoastEstatesBillingLogicDemo
{

//An abstract class that holds the similarities of Client and Employee
public abstract class Person
{

    //Maybe better toSetup in Billing
    // public void setId;
    // public int getId;
    public string name {get; set;}
}

//Defining Employee Basics

public class Employee : Person
{
    string[] workDay;
    //I can see workDay been an array with a max amount of 7 days. Then another tab for exception day's which are dates that the employee doesn't work.
    List<string> exceptionDay;
    public Employee(string name, string[] workDay)
    {
        this.name = name;
        this.workDay = workDay;
        exceptionDay = new List<string>();
    }
}

//Defining Client Information
public class Client : Person
{

    private List<House> houses;

    private LicenseType license {get; set;}
    public Client(LicenseType license, string name)
    {
        houses = new List<House>();
        this.license = license;
        this.name = name;
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

        public string address {get; set;}

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
        foreach (House house in houses)
        {
            if (house.address.Equals(address))
            {
                return house;
            }
        }

        //Temp exception for now
        throw new Exception("NO HOUSE");
    }

    public void removeHouse(string address)
    {
        House temp = findHouse(address);

        houses.Remove(temp);

    }
    public string getHouseList()
    {
        StringBuilder houseList = new StringBuilder();
        foreach(House h in houses)
        {
            houseList.Append("Address: " + h.address + "\n");
        }

        return houseList.ToString();
    }
}

}