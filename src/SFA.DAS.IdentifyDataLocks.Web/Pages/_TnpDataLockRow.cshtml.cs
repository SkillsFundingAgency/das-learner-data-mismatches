﻿using SFA.DAS.IdentifyDataLocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class TnpDataLockRowModel
    {
        public string RowClass { get; }
        public string Heading { get; }

        public IEnumerable<AmountFromDate> IlrValues { get; set; } =
            Enumerable.Empty<AmountFromDate>();

        public TnpDataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, List<AmountFromDate>> valueExtractor)
        {
            RowClass = heading.ToLower().Replace(" ", "");
            Heading = heading;
            if (period.Ilr != null)
            {
                IlrValues = valueExtractor(period.Ilr);
            }
        }
    }
}