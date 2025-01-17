﻿using HSMServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSMServer.Core.MonitoringHistoryProcessor.Processor
{
    internal abstract class HistoryProcessorBase : IHistoryProcessor
    {
        public abstract string GetCsvHistory(List<BaseValue> originalData);

        public List<BaseValue> ProcessingAndCompression(List<BaseValue> values, int compressedValuesCount)
        {
            values = values.OrderBy(v => v.Time).ThenBy(v => v.ReceivingTime).ToList();

            if (values.Count < compressedValuesCount)
                return values;

            var interval = CountInterval(values, compressedValuesCount);
            if (interval == TimeSpan.Zero)
                return values;

            return Compress(values, interval);
        }

        protected virtual List<BaseValue> Compress(List<BaseValue> history, TimeSpan compressionInterval) => history;

        private static TimeSpan CountInterval(List<BaseValue> values, int compressedValuesCount)
        {
            var fullTime = values.Last().Time - values.First().Time;
            var fullMilliseconds = fullTime.TotalMilliseconds;
            var intervalMilliseconds = fullMilliseconds / compressedValuesCount;

            return TimeSpan.FromMilliseconds(intervalMilliseconds);
        }
    }
}
