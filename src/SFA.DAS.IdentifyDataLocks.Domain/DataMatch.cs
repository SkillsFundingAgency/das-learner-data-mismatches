using System;
using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class AmountFromDate
    {
        public DateTime Start { get; set; }
        public decimal Amount { get; set; }
    }

    public class DataMatch
    {
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public short? Standard { get; set; }
        public short? Framework { get; set; }
        public short? Program { get; set; }
        public short? Pathway { get; set; }
        public decimal Cost { get; set; }
        public DateTime? PriceStart { get; set; }
        public short AimSequenceNumber { get; set; }
        public DateTime? PausedOn { get; set; }
        public DateTime? ResumedOn { get; set; }
        public DateTime? StoppedOn { get; set; }
        public DateTime? PlannedCompletion { get; set; }
        public ApprenticeshipStatus CompletionStatus { get; set; }
        public List<(DateTime, decimal)> Tnp1 { get; set; }
        public List<(DateTime, decimal)> Tnp2 { get; set; }
        public List<(DateTime, decimal)> Tnp3 { get; set; }
        public List<(DateTime, decimal)> Tnp4 { get; set; }
        public DateTime IlrSubmissionDate { get; set; }
    }
}