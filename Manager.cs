using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GolfCoastEstatesBillingLogicDemo
{
    //Class that manages list of employers / clients and scheduling
    public sealed class Manager
    {
        //I don't understand this part, but it's what's used for Singularity Pattern (will look into)
        private static readonly Lazy<Manager> instance = new Lazy<Manager>(() => new Manager());

        public static Manager Instance => instance.Value;

        private List<Employee> employers;
        private List<Client> clients;

        private Manager()
        {
            employers = new List<Employee>();
            clients = new List<Client>();
        }

        public IReadOnlyList<Employee> getEmployees()
        {
            return employers.AsReadOnly();
        }

        public IReadOnlyList<Client> getClients()
        {
            return clients.AsReadOnly();
        }

        public int getNumberOfEmployees()
        {
            return employers.Count;
        }

        public int getNumberOfClients()
        {
            return clients.Count;
        }

        public Employee getEmployee(string name)
        {
            Employee foundEmployee = employers.Find(e => e.name.Equals(name));
            return foundEmployee;
        }

        public Client GetClient(string name)
        {
            Client foundClient = clients.Find(e => e.name.Equals(name));
            return foundClient;
        }

        public Employee createEmployee(string name, DayOfWeek[] workday)
        {
            Employee employee = new Employee(name, workday);
            employers.Add(employee);
            return employee;
        }

        public Client createClient(LicenseType license, string name)
        {
            Client client = new Client(license, name);
            clients.Add(client);
            return client;
        }

        public Employee removeEmployee(string name)
        {
            Employee temp = null;

            foreach (Employee e in employers)
            {
                if (e.name.Equals(name))
                {
                    temp = e;
                    employers.Remove(e);
                    return temp;
                }
            }

            return temp;
        }

        public Client removeClient(string name)
        {
            Client temp = null;
            foreach (Client e in clients)
            {
                if (e.name.Equals(name))
                {
                    temp = e;
                    clients.Remove(e);
                    return temp;
                }
            }

            return temp;
        }
    }
}
