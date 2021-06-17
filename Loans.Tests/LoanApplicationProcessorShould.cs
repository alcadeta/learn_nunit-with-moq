using Loans.Domain.Applications;
using NUnit.Framework;

namespace Loans.Tests
{
    public class LoanApplicationProcessorShould
    {
        [Test]
        public void DeclineLoanSalary()
        {
            var product = new LoanProduct(99, "Loan", 5.25m);
            var amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  64_999);

            // Throws exception because of null checks in the LAP
            var sut = new LoanApplicationProcessor(null, null);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);
        }
    }
}