﻿//Add event listeners to buttons
function initializeDataHistoryRequests() {
    $(".accordion-button").on("click", function () {
        id = this.id;
        selected = id;

        totalCount = getCountForId(id);
        type = document.getElementById("sensor_type_" + id).value;

        if (type !== "3" && type !== "6" && type !== "7") {
            initializeGraph(id, rawHistoryHourAction, type);
        }
        initializeHistory(id, historyHourAction, type);
    });

    $('[id^="reload_"]').on("click",
        function () {
            id = this.id.substring("reload_".length, this.id.length);
            totalCount = getCountForId(id);
            type = document.getElementById("sensor_type_" + id).value;

            if (type !== "3" && type !== "6" && type !== "7") {
                initializeGraph(id, type, totalCount, rawHistoryAction);
            }
            initializeHistory(id, totalCount, historyAction);
        });

    $('[id^="button_view"]').on("click",
        function () {
            id = this.id.substring("button_view_".length, this.id.length);
            fileType = document.getElementById('fileType_' + id).value;

            viewFile(id, fileType, viewFileAction);
        }
    );

    $('[id^="button_download"]').on("click",
        function () {
            id = this.id.substring("button_download_".length, this.id.length);

            window.location.href = getFileAction + "?Selected=" + id;
        }
    );
}

function InitializePeriodRequests() {
    $('[id^="hour_"]').on("click",
        function() {
            let path = this.id.substring("hour_".length);
            let type = getTypeForSensor(path);
            initializeHistories(path, historyHourAction, rawHistoryHourAction, type);
        }
    );

    $('[id^="day_"]').on("click",
        function () {
            let path = this.id.substring("day_".length);
            let type = getTypeForSensor(path);
            initializeHistories(path, historyDayAction, rawHistoryDayAction, type);
        }
    );

    $('[id^="three_days_"]').on("click",
        function () {
            let path = this.id.substring("three_days_".length);
            let type = getTypeForSensor(path);
            initializeHistories(path, historyThreeDaysAction, rawHistoryThreeDaysAction, type);
        }
    );

    $('[id^="week_"]').on("click",
        function () {
            let path = this.id.substring("week_".length);
            let type = getTypeForSensor(path);
            initializeHistories(path, historyWeekAction, rawHistoryWeekAction, type);
        }
    );

    $('[id^="month_"]').on("click",
        function () {
            let path = this.id.substring("month_".length);
            let type = getTypeForSensor(path);
            initializeHistories(path, historyMonthAction, rawHistoryMonthAction, type);
        }
    );

    $('[id^="all_"]').on("click",
        function () {
            let path = this.id.substring("all_".length);
            let type = getTypeForSensor(path);
            initializeHistories(path, historyAllAction, rawHistoryAllAction, type);
        }
    );
}

function initializeHistories(path, historyAction, rawHistoryAction, type) {
    initializeHistory(path, historyAction, type);
    initializeGraph(path, rawHistoryAction, type);
}

function initializeHistory(path, historyAction, type) {

    $.ajax({
        type: 'POST',
        url: historyAction + "?Path=" + path + "&Type=" + type,
        contentType: 'application/json',
        dataType: 'html',
        cache: false,
        async: true
    }).done(function (data) {
        console.log(data);
        //data = data.replace('{"value":"', ''); //fix sometime
        //data = data.replace('"}', '');

        $(`#values_${path}`).empty();
        $(`#values_${path}`).append(JSON.parse(data).value);
    });
}

function getTypeForSensor(name) {
    let element = document.getElementById("sensor_type_" + name);
    return element.value;
}

function getCountForId(id) {
    let inputCount = $('#inputCount_' + id).val();
    if (inputCount === undefined) {

        inputCount = 10;
    }

    return inputCount;
}

function data(path, totalCount) {
    return { "Path": path, "TotalCount": totalCount };
}

//function initializeHistory(path, totalCount, historyAction) {
//    if (totalCount == undefined)
//        totalCount = 10;

//    $.ajax({
//        type: 'POST',
//        data: JSON.stringify(data(path, totalCount)),
//        url: historyAction,
//        dataType: 'html',
//        contentType: 'application/json',
//        cache: false,
//        async: true
//    }).done(function (data) {
//        data = data.replace('{"value":"', ''); //fix sometime
//        data = data.replace('"}', '');

//        $(`#values_${path}`).empty();
//        $(`#values_${path}`).append(data);
//    });
//}

function initializeGraph(id, type, totalCount, rawHistoryAction) {
    if (totalCount == undefined)
        totalCount = 10;

    $.ajax({
        type: 'POST',
        data: JSON.stringify(data(id, totalCount)),
        url: rawHistoryAction,
        dataType: 'html',
        contentType: 'application/json',
        cache: false,
        async: true
    }).done(function (data) {
        let graphDivId = "graph_" + id;
        displayGraph(data, type, graphDivId, id);
    });
}

function initializeGraph(path, rawHistoryAction, type) {
    $.ajax({
        type: 'POST',
        url: rawHistoryAction + "?Path=" + path + "&Type=" + type,
        contentType: 'application/json',
        dataType: 'html',
        cache: false,
        async: true
    }).done(function (data) {
        let graphDivId = "graph_" + path;
        displayGraph(data, type, graphDivId, path);
    });
}