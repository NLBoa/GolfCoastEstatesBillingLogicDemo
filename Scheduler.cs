using System.Runtime.CompilerServices;

namespace GolfCoastEstatesBillingLogicDemo
{
    public sealed class Scheduler
    {
        private static readonly Lazy<Scheduler> instance = new Lazy<Scheduler>(() =>
            new Scheduler()
        );
        public static Scheduler Instance => instance.Value;

        private Dictionary<DateOnly, int> housesBooked;
        private Dictionary<DateOnly, List<Client>> bookings;

        private Scheduler()
        {
            housesBooked = new Dictionary<DateOnly, int>();
            bookings = new Dictionary<DateOnly, List<Client>>();
        }

        // Sum every employee's capacity for this date (0 if they don't work that weekday,
        private int getTotalCapacity(DateOnly date)
        {
            int tCap = 0;
            foreach (Employee e in Manager.Instance.getEmployees())
            {
                tCap += e.getCapacityForDate(date);
            }

            return tCap;
        }

        // How many houses are already committed against this date so far.
        private int getBookedHouses(DateOnly date)
        {
            int count = housesBooked[date];

            return count;
        }

        // getTotalCapacity(date) minus getBookedHouses(date) - what's left to give out.
        private int getAvailableSpace(DateOnly date)
        {
            int count = getTotalCapacity(date) - getBookedHouses(date);

            return count;
        }

        // Filter candidateDates down to the ones where getAvailableSpace(date) can fit
        // client.getHouseCount() more houses.
        public List<DateOnly> getAvailableDays(Client client, IEnumerable<DateOnly> candidateDates)
        {
            throw new NotImplementedException();
        }

        // Re-check getAvailableSpace(date) >= client.getHouseCount() (don't trust a stale
        // getAvailableDays result), then if it still fits: bump housesBooked[date] and
        // record the client in bookings[date]. Return false instead of throwing if it no longer fits.
        public bool scheduleClient(Client client, DateOnly date)
        {
            throw new NotImplementedException();
        }
    }
}
