var abp = abp || {};
(function (){
    var l = abp.localization.getResource("HaravanManagement");
    var sampleService = window.lookOn.haravanManagement.controllers.samples.sample;

    //Pie chart
    var pieChartDataSource = new DevExpress.data.DataSource({
        store: new DevExpress.data.CustomStore({
            loadMode: "raw",
            load: function () {
                const deferred = $.Deferred();

                sampleService.getPieChartData().then(function(result){
                    deferred.resolve(result);
                });
                return deferred.promise();
            }
        }),
        paginate: false
    });

    $('#pieChartContainer').dxPieChart({
        size: {
            width: 500,
        },
        palette: 'bright',
        dataSource: pieChartDataSource,
        series: [
            {
                argumentField: 'label',
                valueField: 'value',
                label: {
                    visible: true,
                    connector: {
                        visible: true,
                        width: 1,
                    },
                },
            },
        ],
        title: l('SamplePieChart'),
        export: {
            enabled: true,
        },
        onPointClick(e) {
            const point = e.target;

            toggleVisibility(point);
        },
        onLegendClick(e) {
            const arg = e.target;

            toggleVisibility(this.getAllSeries()[0].getPointsByArg(arg)[0]);
        },
    });

    function toggleVisibility(item) {
        if (item.isVisible()) {
            item.hide();
        } else {
            item.show();
        }
    }

    //End Pie chart

    //Line chart
    var lineChartDataSource = new DevExpress.data.DataSource({
        store: new DevExpress.data.CustomStore({
            loadMode: "raw",
            load: function () {
                const deferred = $.Deferred();

                sampleService.getLineChartData().then(function(result){
                    deferred.resolve(result);
                });
                return deferred.promise();
            }
        }),
        paginate: false
    });

    $('#lineChartContainer').dxChart({
        palette: 'Violet',
        dataSource: lineChartDataSource,
        commonSeriesSettings: {
            argumentField: 'label',
            type: 'line',
        },
        margin: {
            bottom: 20,
        },
        argumentAxis: {
            valueMarginsEnabled: false,
            discreteAxisDivisionMode: 'crossLabels',
            grid: {
                visible: true,
            },
        },
        series: [
            { valueField: 'line1', name: l('Line1') },
            { valueField: 'line2', name: l('Line2')}
        ],
        legend: {
            verticalAlignment: 'bottom',
            horizontalAlignment: 'center',
            itemTextPosition: 'bottom',
        },
        title: {
            text: l('Sample Line chart'),
            subtitle: {
                text: l('Subtext: Subtitle Line chart'),
            },
        },
        export: {
            enabled: true,
        },
        tooltip: {
            enabled: true,
        },
    });

    $('#barChartContainer').dxChart({
        //palette: 'Material',
        palette: ['#F97179' , '#DC48B3'],
        dataSource: lineChartDataSource,
        commonSeriesSettings: {
            argumentField: 'label',
            type: 'bar',
            hoverMode: 'allArgumentPoints',
            selectionMode: 'allArgumentPoints',
            label: {
                visible: false,
                format: {
                    type: 'fixedPoint',
                    precision: 0,
                },
            },
        },
        series: [
            { valueField: 'line1', name: l('Line1') },
            { valueField: 'line2', name: l('Line2')}
        ],
        //title: 'Total Revenue',
        legend: {
            verticalAlignment: 'bottom',
            horizontalAlignment: 'center',
        },
        export: {
            enabled: true,
        },
        onPointClick(e) {
            e.target.select();
        },
    });
    
    ///Map Viet nam
    $('#map-container').dxVectorMap({
        title: {
            text: 'Map of Vietnam'
        },
        maxZoomFactor: 200,
        zoomFactor: 20,
        center: [107.44291701900002, 16.699500137000101],
        export: {
            enabled: false,
        },
        layers: [{
            dataSource: pangaeaBorders,
            hoverEnabled: false,
            name: 'pangaea',
            color: '#bb7862',
        }, {
            dataSource: pangaeaContinents,
            customize(elements) {
                $.each(elements, (_, element) => {
                    element.applySettings({
                        color: element.attribute('color'),
                    });
                });
            },
            label: {
                enabled: true,
                dataField: 'name',
            },
            name: 'continents',
        }],
    });
})(jQuery);