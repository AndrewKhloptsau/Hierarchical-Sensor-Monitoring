# HSM Server

## New sensor type [**Version**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Sensor-types) has been added:
* New endpoint **/version** has been added
* New table for sensor type **Version** has been added. Version format *1.2.3.4*

## New type of Access keys has been added - **Master key**
* Master key has access to ALL products on the server
* Admin ONLY can create Master key

## Ability to integrate with [**Grafana**](https://grafana.com/) has been added:
* New endopoints for [**JsonDatasource**](https://grafana.com/grafana/plugins/simpod-json-datasource/) have been added: */grafana/JsonDatasource*, */grafana/JsonDatasource/metrics*, */grafana/JsonDatasource/metric-payload-options*, */grafana/JsonDatasource/query*
* Grafana connection guide is [here]()
* List of available datasources and sensor types is [here](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Integration-with-Grafana#json-datasource)

## Tree
* **Enable/Disable Grafana** item has been added in Context menu
* Grafana icon is shown for sensors with Grafana enabled setting

## Sensor info
* New property **Enable for** has been added

## Access keys
* Select product input has been added in Edit modal
* New link for creating access keys has been added on Access Keys tab (Only for admins)
* Unique access key name validation has been added in Edit modal
* **Unselect all** button has been added in Edit modal
* Key's authors that have been removed have been marked as *Removed*

# HSM Datacollector

### Structure and optimizations
* Async requests and handlers for HttpClient have been added
* Base structure for **Simple sensor** (a sensor that sends data on user request, not on a timer) has been added
* Collector statuses have been added. Now collector has 4 statuses: **Starting**, **Running**, **Stopping**, **Stopped**. [More info](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/DataCollector-statuses)

### Default sensors
* **CollectorAlive** sensor has been renamed to [**CollectorHeartbeat**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Unix-sensors-collection#addcollectorheartbeat). Sensor name has been renamed from **Service alive** to **Service heartbeat**.
* New default sensor [**Product info**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Windows-sensors-collection#addproductversion) has been added. Now it contains Product Version with Version start/stop time.
* New defaut sensor [**Collector status**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Unix-sensors-collection#addcollectorstatus) has been added. It describes current collector state and contains error message (if exsists).

### Unix sensors
* [**Free RAM memory MB**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Unix-sensors-collection#addfreerammemory) has been added.
* [**Total CPU**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Unix-sensors-collection#addtotalcpu) has been added.
* [**AddSystemMonitoringSensors**](https://github.com/SoftFx/Hierarchical-Sensor-Monitoring/wiki/Unix-sensors-collection#addsystemmonitoringsensors) facade for **Free RAM memory MB** and **Total CPU** has been added.

### New methods
* New method **SendFileAsync** has been added

### Other
* Collector version has been increased to 3.1.0