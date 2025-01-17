﻿using HSMDatabase.AccessManager.DatabaseEntities;

namespace HSMServer.Core.Model
{
    public sealed class IntegerBarSensorModel : BaseSensorModel<IntegerBarValue>, IBarSensor
    {
        protected override IntegerBarValuesStorage Storage { get; } = new IntegerBarValuesStorage();

        public override SensorType Type { get; } = SensorType.IntegerBar;


        BarBaseValue IBarSensor.LocalLastValue => Storage.PartialLastValue;


        public IntegerBarSensorModel(SensorEntity entity) : base(entity) { }
    }
}
