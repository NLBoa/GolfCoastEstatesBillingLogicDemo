using System;
using System.Collections.Generic;
using Xunit;

namespace GolfCoastEstatesBillingLogicDemo.Tests
{
    // Manager and Scheduler are singletons, so each test creates its own employees/clients on
    // far-future, non-overlapping dates and cleans them up in a finally block. xUnit runs the
    // methods within a single test class sequentially, so there's no cross-test race on the
    // shared singleton state.
    public class SchedulerTests
    {
        // ---- Client tests ----

        // Adding a house should raise getNumHouses by one; removing it should bring the count back down.
        [Fact]
        public void ClientAddAndRemoveHouse_UpdatesHouseCount()
        {
            Client client = Manager.Instance.createClient(LicenseType.PRO, "Test_HouseCount");
            try
            {
                Assert.Equal(0, client.getNumHouses());

                client.addHouse("123 Fairway Dr", "Initial note");
                Assert.Equal(1, client.getNumHouses());

                client.removeHouse("123 Fairway Dr");
                Assert.Equal(0, client.getNumHouses());
            }
            finally
            {
                Manager.Instance.removeClient("Test_HouseCount");
            }
        }

        // getPosition() should just be the client's license enum rendered as a string.
        [Fact]
        public void ClientGetPosition_ReturnsLicenseType()
        {
            Client client = Manager.Instance.createClient(LicenseType.ENTERPRISE, "Test_Position");
            try
            {
                Assert.Equal("ENTERPRISE", client.getPosition());
            }
            finally
            {
                Manager.Instance.removeClient("Test_Position");
            }
        }

        // ---- Employee tests ----

        // An employee should report their normal daily capacity on a day they're scheduled to work.
        [Fact]
        public void EmployeeCapacity_MatchesWorkday()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_WorkdayEmployee",
                new[] { DayOfWeek.Monday }
            );
            try
            {
                DateOnly monday = NextDateOnWeekday(DayOfWeek.Monday, 1000);
                Assert.Equal(employee.getMaxDailyHouse(), employee.getCapacityForDate(monday));
            }
            finally
            {
                Manager.Instance.removeEmployee("Test_WorkdayEmployee");
            }
        }

        // An employee should have zero capacity on a day they're not scheduled to work.
        [Fact]
        public void EmployeeCapacity_NonWorkday_IsZero()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_NonWorkdayEmployee",
                new[] { DayOfWeek.Monday }
            );
            try
            {
                DateOnly tuesday = NextDateOnWeekday(DayOfWeek.Tuesday, 1001);
                Assert.Equal(0, employee.getCapacityForDate(tuesday));
            }
            finally
            {
                Manager.Instance.removeEmployee("Test_NonWorkdayEmployee");
            }
        }

        // An exception day (time off / reduced capacity) should override the normal workday capacity.
        [Fact]
        public void EmployeeCapacity_ExceptionOverridesWorkday()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_ExceptionEmployee",
                new[] { DayOfWeek.Monday }
            );
            try
            {
                DateOnly monday = NextDateOnWeekday(DayOfWeek.Monday, 1002);
                employee.addException(monday, "Half day", 2);

                Assert.Equal(2, employee.getCapacityForDate(monday));
            }
            finally
            {
                Manager.Instance.removeEmployee("Test_ExceptionEmployee");
            }
        }

        // ---- Scheduler.getAvailableDays tests ----

        // numberHouses outside the client's owned house range (0, negative, or more than they own) should throw.
        [Fact]
        public void GetAvailableDays_ThrowsOnInvalidHouseCount()
        {
            Client client = Manager.Instance.createClient(LicenseType.PRO, "Test_InvalidHouseCount");
            try
            {
                client.addHouse("1 Test Ln", "note");

                Assert.Throws<Exception>(
                    () => Scheduler.Instance.getAvailableDays(client, 0, new List<DateOnly>())
                );
                Assert.Throws<Exception>(
                    () => Scheduler.Instance.getAvailableDays(client, 5, new List<DateOnly>())
                );
            }
            finally
            {
                Manager.Instance.removeClient("Test_InvalidHouseCount");
            }
        }

        // Given a set of candidate dates, only the ones with enough remaining capacity should come back.
        [Fact]
        public void GetAvailableDays_FiltersOutFullDays()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_FilterEmployee",
                new[] { DayOfWeek.Wednesday }
            );
            Client client = Manager.Instance.createClient(LicenseType.PRO, "Test_FilterClient");
            try
            {
                client.addHouse("1 Test Ln", "note");
                DateOnly openWednesday = NextDateOnWeekday(DayOfWeek.Wednesday, 1003);
                DateOnly closedThursday = openWednesday.AddDays(1); // employee doesn't work Thursdays

                List<DateOnly> candidates = new List<DateOnly> { openWednesday, closedThursday };
                List<DateOnly> available = Scheduler.Instance.getAvailableDays(client, 1, candidates);

                Assert.Contains(openWednesday, available);
                Assert.DoesNotContain(closedThursday, available);
            }
            finally
            {
                Manager.Instance.removeClient("Test_FilterClient");
                Manager.Instance.removeEmployee("Test_FilterEmployee");
            }
        }

        // ---- Scheduler.scheduleClient tests ----

        // scheduleClient should book the client onto every date in the weekly recurrence, not just the first one.
        [Fact]
        public void ScheduleClient_BooksEveryDateInRecurrence()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_RecurEmployee",
                new[] { DayOfWeek.Friday }
            );
            employee.setMaxDailyHouse(1); // exactly enough for one booking, so a booked day has 0 space left
            Client client = Manager.Instance.createClient(LicenseType.PRO, "Test_RecurClient");
            try
            {
                client.addHouse("1 Test Ln", "note");
                DateOnly start = NextDateOnWeekday(DayOfWeek.Friday, 1004);
                DateOnly end = start.AddDays(14); // three Fridays: start, +7, +14

                bool scheduled = Scheduler.Instance.scheduleClient(client, start, end, 1);
                Assert.True(scheduled);

                List<DateOnly> stillOpen = Scheduler.Instance.getAvailableDays(
                    client,
                    1,
                    new List<DateOnly> { start, start.AddDays(7), end }
                );
                Assert.Empty(stillOpen);
            }
            finally
            {
                Manager.Instance.removeClient("Test_RecurClient");
                Manager.Instance.removeEmployee("Test_RecurEmployee");
            }
        }

        // If any date in the recurrence can't fit the booking, nothing should be booked at all (all-or-nothing).
        [Fact]
        public void ScheduleClient_FailsAtomically_WhenOneDateIsFull()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_AtomicEmployee",
                new[] { DayOfWeek.Saturday }
            );
            employee.setMaxDailyHouse(1);
            Client firstClient = Manager.Instance.createClient(LicenseType.PRO, "Test_AtomicClient1");
            Client secondClient = Manager.Instance.createClient(LicenseType.PRO, "Test_AtomicClient2");
            try
            {
                firstClient.addHouse("1 Test Ln", "note");
                secondClient.addHouse("2 Test Ln", "note");

                DateOnly firstSaturday = NextDateOnWeekday(DayOfWeek.Saturday, 1005);
                DateOnly secondSaturday = firstSaturday.AddDays(7);

                // Fill up the second Saturday completely first.
                bool fillScheduled = Scheduler.Instance.scheduleClient(
                    firstClient,
                    secondSaturday,
                    secondSaturday,
                    1
                );
                Assert.True(fillScheduled);

                // Now try to book a recurring series spanning both Saturdays - the second one has no room left.
                bool scheduled = Scheduler.Instance.scheduleClient(
                    secondClient,
                    firstSaturday,
                    secondSaturday,
                    1
                );
                Assert.False(scheduled);

                List<DateOnly> stillOpen = Scheduler.Instance.getAvailableDays(
                    secondClient,
                    1,
                    new List<DateOnly> { firstSaturday }
                );
                Assert.Contains(firstSaturday, stillOpen);
            }
            finally
            {
                Manager.Instance.removeClient("Test_AtomicClient1");
                Manager.Instance.removeClient("Test_AtomicClient2");
                Manager.Instance.removeEmployee("Test_AtomicEmployee");
            }
        }

        // Booking houses on a date should reduce how much space getAvailableDays reports as left for later requests.
        [Fact]
        public void ScheduleClient_ReducesAvailableSpaceForFutureBookings()
        {
            Employee employee = Manager.Instance.createEmployee(
                "Test_SpaceEmployee",
                new[] { DayOfWeek.Sunday }
            );
            employee.setMaxDailyHouse(2);
            Client client = Manager.Instance.createClient(LicenseType.PRO, "Test_SpaceClient");
            try
            {
                client.addHouse("1 Test Ln", "note");
                client.addHouse("2 Test Ln", "note");
                DateOnly sunday = NextDateOnWeekday(DayOfWeek.Sunday, 1006);

                bool scheduled = Scheduler.Instance.scheduleClient(client, sunday, sunday, 1);
                Assert.True(scheduled);

                List<DateOnly> stillFitsOne = Scheduler.Instance.getAvailableDays(
                    client,
                    1,
                    new List<DateOnly> { sunday }
                );
                Assert.Contains(sunday, stillFitsOne);

                List<DateOnly> noLongerFitsTwo = Scheduler.Instance.getAvailableDays(
                    client,
                    2,
                    new List<DateOnly> { sunday }
                );
                Assert.DoesNotContain(sunday, noLongerFitsTwo);
            }
            finally
            {
                Manager.Instance.removeClient("Test_SpaceClient");
                Manager.Instance.removeEmployee("Test_SpaceEmployee");
            }
        }

        // Finds a date on the given weekday, offset weekOffset weeks into the future, so each test works
        // against its own slice of the calendar and can't collide with another test's bookings.
        private static DateOnly NextDateOnWeekday(DayOfWeek weekday, int weekOffset)
        {
            DateOnly candidate = DateOnly.FromDateTime(DateTime.Today).AddDays(weekOffset * 7);
            while (candidate.DayOfWeek != weekday)
            {
                candidate = candidate.AddDays(1);
            }
            return candidate;
        }
    }
}
