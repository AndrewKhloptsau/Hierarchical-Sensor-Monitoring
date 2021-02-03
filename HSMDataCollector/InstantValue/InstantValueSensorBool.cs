﻿using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HSMSensorDataObjects;

namespace HSMDataCollector.InstantValue
{
    class InstantValueSensorBool : InstantValueTypedSensorBase<bool>
    {
        public InstantValueSensorBool(string path, string productKey, string address) : base(path, productKey, $"{address}/bool")
        {
        }

        public override void AddValue(object value)
        {
            bool boolValue = (bool) value;
            lock (_syncRoot)
            {
                Value = boolValue;
            }

            BoolSensorValue data = GetDataObject();
            Task.Run(() => SendData(data));
        }

        private BoolSensorValue GetDataObject()
        {
            BoolSensorValue result = new BoolSensorValue();
            lock (_syncRoot)
            {
                result.BoolValue = Value;
            }

            result.Path = Path;
            result.Key = ProductKey;
            result.Time = DateTime.Now;
            return result;
        }

        protected override byte[] GetBytesData(object data)
        {
            try
            {
                BoolSensorValue typedData = (BoolSensorValue)data;
                string convertedString = JsonSerializer.Serialize(typedData);
                return Encoding.UTF8.GetBytes(convertedString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new byte[1];
            }
            
        }
    }
}