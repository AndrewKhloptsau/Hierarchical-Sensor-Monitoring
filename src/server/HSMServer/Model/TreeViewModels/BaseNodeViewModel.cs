﻿using HSMServer.Extensions;
using System;

namespace HSMServer.Model.TreeViewModel
{
    public abstract class BaseNodeViewModel
    {
        public TimeIntervalViewModel ExpectedUpdateInterval { get; protected set; }

        public TimeIntervalViewModel SensorRestorePolicy { get; protected set; }


        public Guid Id { get; protected set; }

        public string Name { get; protected set; }

        public string Description { get; protected set; }


        public SensorStatus Status { get; protected set; }

        public DateTime UpdateTime { get; protected set; }


        public string Title => Name?.Replace('\\', ' ') ?? string.Empty; //TODO remove after rename bad products

        public string Tooltip => $"{Name}{Environment.NewLine}{(UpdateTime != DateTime.MinValue ? UpdateTime.ToDefaultFormat() : "no data")}";
    }
}
