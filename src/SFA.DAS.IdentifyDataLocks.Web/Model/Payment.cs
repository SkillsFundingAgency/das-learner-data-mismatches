﻿namespace SFA.DAS.IdentifyDataLocks.Web.Model
{
    public class Payment : ICommitmentItem
    {
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public string TransactionType { get; set; }

        public string Type => "Payment";

        public byte DeliveryPeriod { get; set; }

        public string PriceEpisodeIdentifier { get; internal set; }
        public Payments.Model.Core.CollectionPeriod CollectionPeriod { get; internal set; }
    }
}