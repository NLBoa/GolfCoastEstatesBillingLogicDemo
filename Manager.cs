using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Text;

namespace GolfCoastEstatesBillingLogicDemo
{
    //Class that manages list of employers / clients and scheduling
    public sealed class Manager
    {
        //I don't understand this part, but it's what's used for Singularity Pattern (will look into)
        private static readonly Lazy<Manager> instance = new Lazy<Manager>(() => new Manager());

        public static Manager Instance => instance.Value;

        List<Employee> employers;
        List<Client> clients;

        public Manager()
        {
            employers = new List<Employee>();
            clients = new List<Client>();
        }
    }
}
