﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Documents.Serialization;
using Google.Protobuf.WellKnownTypes;
using HSMClientWPFControls.ConnectorInterface;
using HSMClientWPFControls.Model.SensorDialog;
using HSMClientWPFControls.Objects;
using HSMClientWPFControls.ViewModel;
using HSMSensorDataObjects.TypedDataObject;
using LiveCharts.Defaults;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HSMClient.Dialog
{
    class ClientBarSensorModel : ClientDialogTimerModel, IBarSensorModel
    {
        private Collection<BoxPlotItem> _items;
        private Collection<OhlcPoint> _points;
        private Collection<string> _labels;
        public ClientBarSensorModel(ISensorHistoryConnector connector, MonitoringSensorViewModel sensor) : base(connector, sensor)
        {
            Items = new Collection<BoxPlotItem>();
            Points = new Collection<OhlcPoint>();
            Labels = new Collection<string>();
            Count = 10;
            Title = _path;
        }
        public string Title { get; set; }
        public int Count { get; set; }

        public Collection<BoxPlotItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public Collection<OhlcPoint> Points
        {
            get => _points;
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        public Collection<string> Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }
        //protected override void OnTimerTick()
        //{
        //    List<SensorHistoryItem> list = _connector.GetSensorHistory(_product, _path, _name, Count);
        //    if (list.Count < 1)
        //        return;

        //    list.Reverse();
        //    var type = list[0].Type;
        //    Collection<OhlcPoint> points = new Collection<OhlcPoint>();
        //    Collection<string> labels = new Collection<string>();
        //    switch (type)
        //    {
        //        case SensorTypes.BarIntSensor:
        //        {
        //            var serializedList = list.Select(l => JsonSerializer.Deserialize<IntBarSensorData>(l.SensorValue))
        //                .ToList();
        //            List<IntBarSensorData> finalList = new List<IntBarSensorData>();
        //            for (int i = 0; i < serializedList.Count; ++i)
        //            {
        //                if (i < serializedList.Count - 1)
        //                {
        //                    if (serializedList[i].StartTime != serializedList[i + 1].StartTime)
        //                    {
        //                        finalList.Add(serializedList[i]);
        //                        continue;
        //                    }

        //                    if (serializedList[i].EndTime >= serializedList[i + 1].EndTime)
        //                    {
        //                        finalList.Add(serializedList[i]);
        //                        ++i;
        //                    }
        //                    else
        //                    {
        //                        finalList.Add(serializedList[i + 1]);
        //                        i += 2;
        //                    }

        //                    break;
        //                }
        //            }

        //            foreach (var item in finalList)
        //            {
        //                int firstQuant = item.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.25) < double.Epsilon).Value;
        //                int thirdQuant = item.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.75) < double.Epsilon).Value; 
        //                points.Add(new OhlcPoint(thirdQuant, item.Max, thirdQuant, item.Min));
        //                labels.Add($"{item.StartTime:T}");
        //            }
        //            break;
        //        }
        //    }

        //    Points = points;
        //    Labels = labels;
        //}
        protected override void OnTimerTick()
        {
            List<SensorHistoryItem> list = _connector.GetSensorHistory(_product, _path, _name, Count);
            if (list.Count < 1)
                return;

            var boxes = new Collection<BoxPlotItem>();

            list.Reverse();
            var type = list[0].Type;
            switch (type)
            {
                case SensorTypes.BarIntSensor:
                    {
                        var serializedList = list.Select(l => JsonSerializer.Deserialize<IntBarSensorData>(l.SensorValue)).ToList();
                        List<IntBarSensorData> finalList = new List<IntBarSensorData>();
                        for (int i = 0; i < serializedList.Count; ++i)
                        {
                            if (i < serializedList.Count - 1)
                            {
                                if (serializedList[i].StartTime != serializedList[i + 1].StartTime)
                                {
                                    finalList.Add(serializedList[i]);
                                    continue;
                                }

                                if (serializedList[i].EndTime >= serializedList[i + 1].EndTime)
                                {
                                    finalList.Add(serializedList[i]);
                                    ++i;
                                }
                                else
                                {
                                    finalList.Add(serializedList[i + 1]);
                                    i += 2;
                                }
                            }
                        }
                        foreach (var boxValue in finalList)
                        {
                            int firstQuant = boxValue.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.25) < double.Epsilon).Value;
                            int thirdQuant = boxValue.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.75) < double.Epsilon).Value;
                            int median = boxValue.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.5) < double.Epsilon).Value;
                            double lowerWhisker = firstQuant - (1.5 * (thirdQuant - firstQuant));
                            double upperWhisker = thirdQuant + (1.5 * (thirdQuant - firstQuant));

                            //DateTime time = new DateTime(1970, 1, 1).AddMilliseconds(boxValue.StartTime.ToUniversalTime().ToTimestamp().Seconds);
                            double x = DateTimeAxis.ToDouble(boxValue.StartTime);
                            BoxPlotItem plotItem = new BoxPlotItem(x, lowerWhisker, firstQuant, median,
                                thirdQuant, upperWhisker);
                            plotItem.Mean = boxValue.Mean;

                            boxes.Add(plotItem);
                        }

                        break;
                    }
                case SensorTypes.BarDoubleSensor:
                    {
                        foreach (var box in list)
                        {
                            var boxValue = JsonSerializer.Deserialize<IntBarSensorData>(box.SensorValue);

                            double firstQuant = boxValue.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.25) < double.Epsilon).Value;
                            double thirdQuant = boxValue.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.75) < double.Epsilon).Value;
                            double median = boxValue.Percentiles.FirstOrDefault(p => Math.Abs(p.Percentile - 0.5) < double.Epsilon).Value;
                            double lowerWhisker = firstQuant - (int)(1.5 * (thirdQuant - firstQuant));
                            double upperWhisker = thirdQuant - (int)(1.5 * (thirdQuant - firstQuant));

                            BoxPlotItem plotItem = new BoxPlotItem(DateTimeAxis.ToDouble(box.Time), lowerWhisker, firstQuant, median,
                                thirdQuant, upperWhisker);

                            boxes.Add(plotItem);
                        }

                        break;
                    }
            }

            Items = boxes;

        }
    }
}
