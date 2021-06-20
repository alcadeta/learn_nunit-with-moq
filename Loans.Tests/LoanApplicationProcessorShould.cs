using Loans.Domain.Applications;
using NUnit.Framework;
using Moq;

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

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object,
                                                   mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.False);
        }

        delegate void ValidateCallback(string applicantName,
                                       int applicantAge,
                                       string applicantAddress,
                                       ref IdentityVerificationStatus status);

        [Test]
        public void Accept()
        {
            var product = new LoanProduct(99, "Loan", 5.25m);
            var amount = new LoanAmount("USD", 200_000);
            var application = new LoanApplication(42,
                                                  product,
                                                  amount,
                                                  "Sarah",
                                                  25,
                                                  "133 Pluralsight Drive, Draper, Utah",
                                                  65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();

            mockIdentityVerifier
                // Whatever is passed for the `ref` parameter...
                .Setup(x => x.Validate("Sarah",
                                       25,
                                       "133 Pluralsight Drive, Draper, Utah",
                                       ref It.Ref<IdentityVerificationStatus>.IsAny))
                // ...assign a new IdentityVerificationStatus of `true` to it.
                .Callback(new ValidateCallback((string applicantName,
                                                int applicantAge,
                                                string applicantAddress,
                                                ref IdentityVerificationStatus status) =>
                                                   status = new IdentityVerificationStatus(true)));

            #region block1
            // // When we don't care what argument is passed in to the mock method, we can use It.IsAny<T>
            // mockIdentityVerifier.Setup(x => x.Validate(It.IsAny<string>(),
            //                                            It.IsAny<int>(),
            //                                            It.IsAny<string>()))
            //                     .Returns(true);
            #endregion

            #region block2
            // mockIdentityVerifier.Setup(x => x.Validate("Sarah",
            //                                            25,
            //                                            "133 Pluralsight Drive, Draper, Utah"))
            //                     .Returns(true);
            #endregion

            #region block3
            // When using an "out" parameter, .Returns method doesn't work on the mock object. Instead,
            // whatever value we give to the "out" parameter is the return value.
            // var isValidOutValue = true;
            // mockIdentityVerifier.Setup(x => x.Validate("Sarah",
            //                                            25,
            //                                            "133 Pluralsight Drive, Draper, Utah",
            //                                            out isValidOutValue));
            #endregion

            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object,
                                                   mockCreditScorer.Object);

            sut.Process(application);

            Assert.That(application.GetIsAccepted(), Is.True);
        }
    }
}