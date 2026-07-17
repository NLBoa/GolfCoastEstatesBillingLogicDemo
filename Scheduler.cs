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
            return housesBooked.GetValueOrDefault(date, 0);
        }

        // getTotalCapacity(date) minus getBookedHouses(date) - what's left to give out.
        private int getAvailableSpace(DateOnly date)
        {
            int count = getTotalCapacity(date) - getBookedHouses(date);

            return count;
        }

        // Filter candidateDates down to the ones where getAvailableSpace(date) can fit
        // client.getHouseCount() more houses.
        public List<DateOnly> getAvailableDays(
            Client client,
            int numberHouses,
            IEnumerable<DateOnly> candidateDates
        )
        {
            if (numberHouses > client.getNumHouses() || numberHouses <= 0)
            {
                //Incorrect house amount
                throw new Exception();
            }
            List<DateOnly> openDays = new List<DateOnly>();
            foreach (DateOnly d in candidateDates)
            {
                if (getAvailableSpace(d) >= numberHouses)
                {
                    openDays.Add(d);
                }
            }

            return openDays;
        }

        private IEnumerable<DateOnly> getRecurrenceDates(DateOnly start, DateOnly end)
        {
            //LicenseType could determine intervalDays, but it makes sense to write this logic elsewhere
            int intervalDays = 7;

            for (DateOnly d = start; d <= end; d = d.AddDays(intervalDays))
            {
                yield return d;
            }
        }

        // Re-check getAvailableSpace(date) >= client.getHouseCount() (don't trust a stale
        // getAvailableDays result), then if it still fits: bump housesBooked[date] and
        // record the client in bookings[date]. Return false instead of throwing if it no longer fits.

        //houseList should also not be an integer but a way for the Client to select the specific house
        public bool scheduleClient(Client client, DateOnly start, DateOnly end, int numberHouses)
        {
            //TODO: Once demo works license should be incorporated here alongside priority next month booking
            List<DateOnly> datesToSchedule = getRecurrenceDates(start, end).ToList();

            foreach (DateOnly d in datesToSchedule)
            {
                if (getAvailableSpace(d) < numberHouses)
                {
                    return false;
                }
            }

            foreach (DateOnly d in datesToSchedule)
            {
                housesBooked[d] = housesBooked.GetValueOrDefault(d, 0) + numberHouses;

                if (!bookings.TryGetValue(d, out List<Client> clientsForDate))
                {
                    clientsForDate = new List<Client>();
                    bookings[d] = clientsForDate;
                }
                clientsForDate.Add(client);
            }

            return true;
        }
    }
}
