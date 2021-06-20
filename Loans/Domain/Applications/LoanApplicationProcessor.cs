using System;

namespace Loans.Domain.Applications
{
    public class LoanApplicationProcessor
    {
        private const decimal _MinimumSalary = 65_000;
        private const int _MinimumAge = 18;
        private const int _MinimumCreditScore = 300;

        private readonly IIdentityVerifier _identityVerifier;
        private readonly ICreditScorer _creditScorer;

        public LoanApplicationProcessor(IIdentityVerifier identityVerifier,
                                        ICreditScorer creditScorer)
        {
            _identityVerifier = identityVerifier
                                ?? throw new ArgumentNullException(nameof(identityVerifier));

            _creditScorer = creditScorer
                            ?? throw new ArgumentNullException(nameof(creditScorer));
        }

        public void Process(LoanApplication application)
        {
            if (application.GetApplicantSalary() < _MinimumSalary)
            {
                application.Decline();
                return;
            }

            if (application.GetApplicantAge() < _MinimumAge)
            {
                application.Decline();
                return;
            }

            _identityVerifier.Initialize();

            var isValidIdentity = _identityVerifier.Validate(application.GetApplicantName(),
                                                             application.GetApplicantAge(),
                                                             application.GetApplicantAddress());

            if (!isValidIdentity)
            {
                application.Decline();
                return;
            }

            _creditScorer.CalculateScore(application.GetApplicantName(),
                                         application.GetApplicantAddress());

            if (_creditScorer.Score < _MinimumCreditScore)
            {
                application.Decline();
                return;
            }

            application.Accept();
        }
    }
}
