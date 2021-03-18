﻿using System;
using HSMDataCollector.PublicInterface;

namespace HSMDataCollector.Core
{
    public interface IDataCollector : IDisposable
    {
        /// <summary>
        /// The method sets the sending timer up. No data will be sent without calling this method
        /// </summary>
        void Initialize();
        /// <summary>
        /// This method must be called before stopping the application. It sends all the data left, stops and disposes the timer.
        /// The method also disposes the HttpClient.
        /// </summary>
        void Stop();

        /// <summary>
        /// Creates and initializes sensors, which automatically monitor CPU and RAM usage of the current machine.
        /// Sensors will be placed at Product/System Monitoring node
        /// </summary>
        void InitializeSystemMonitoring();
        /// <summary>
        /// Creates and initializes sensors, which automatically monitor current working process. RAM and CPU usage, and threads amount are monitored.
        /// Sensors will be placed at Product/System Monitoring node
        /// </summary>
        void InitializeProcessMonitoring();
        /// <summary>
        /// Creates and initializes sensors, which automatically monitor the specified process. RAM and CPU usage, and threads amount are monitored.
        /// Sensors will be placed at Product/System Monitoring node
        /// </summary>
        /// <param name="processName"></param>
        void InitializeProcessMonitoring(string processName);
        IBoolSensor CreateBoolSensor(string path);
        IDoubleSensor CreateDoubleSensor(string path);
        IIntSensor CreateIntSensor(string path);
        IStringSensor CreateStringSensor(string path);
        IDefaultValueSensorInt CreateDefaultValueSensorInt(string path, int defaultValue);
        IDefaultValueSensorDouble CreateDefaultValueSensorDouble(string path, double defaultValue);

        #region Bar sensors

        IDoubleBarSensor CreateDoubleBarSensor(string path, int timeout = 300000, int smallPeriod = 15000);
        IDoubleBarSensor Create1HrDoubleBarSensor(string path);
        IDoubleBarSensor Create30MinDoubleBarSensor(string path);
        IDoubleBarSensor Create10MinDoubleBarSensor(string path);
        IDoubleBarSensor Create5MinDoubleBarSensor(string path);
        IDoubleBarSensor Create1MinDoubleBarSensor(string path);
        IIntBarSensor CreateIntBarSensor(string path, int timeout = 300000, int smallPeriod = 15000);
        IIntBarSensor Create1HrIntBarSensor(string path);
        IIntBarSensor Create30MinIntBarSensor(string path);
        IIntBarSensor Create10MinIntBarSensor(string path);
        IIntBarSensor Create5MinIntBarSensor(string path);
        IIntBarSensor Create1MinIntBarSensor(string path);

        #endregion

        int GetSensorCount();

        event EventHandler ValuesQueueOverflow;
    }
}