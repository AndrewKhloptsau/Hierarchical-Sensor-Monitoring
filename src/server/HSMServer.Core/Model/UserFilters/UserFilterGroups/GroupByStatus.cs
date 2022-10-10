﻿namespace HSMServer.Core.Model.UserFilters
{
    public sealed class GroupByStatus : UserFilterGroupBase
    {
        protected override FilterProperty[] Properties => new[] { Ok, Warning, Error, Unknown };

        internal override FilterGroupType Type => FilterGroupType.ByStatus;


        public FilterProperty Ok { get; init; } = new();

        public FilterProperty Warning { get; init; } = new();

        public FilterProperty Error { get; init; } = new();

        public FilterProperty Unknown { get; init; } = new();

        public GroupByStatus() { }


        public bool IsStatusSuitable(SensorStatus status) =>
            status switch
            {
                SensorStatus.Ok => Ok.Value,
                SensorStatus.Warning => Warning.Value,
                SensorStatus.Error => Error.Value,
                SensorStatus.Unknown => Unknown.Value,
                _ => false
            };

        internal override bool IsSensorSuitable(FilteredSensor sensor) => IsStatusSuitable(sensor.Status);
    }
}