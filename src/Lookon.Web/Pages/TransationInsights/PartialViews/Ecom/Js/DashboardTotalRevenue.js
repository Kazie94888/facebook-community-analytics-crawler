var abp = abp || {};
$(function (){
    var l = abp.localization.getResource("LookOn");
    $('#barChartContainer').dxChart({
        //palette: 'Material',
        palette: palette_barchart,
        dataSource: revenueByDates,
        commonSeriesSettings: {
            argumentField: 'DateTimeLabel',
            type: 'bar',
            hoverMode: 'allArgumentPoints',
            selectionMode: 'allArgumentPoints',
            barPadding: 0.7,
            label: {
                visible: false,
                format: {
                    type: 'fixedPoint',
                    precision: 0
                },
            },
        },
        series: [
            { valueField: 'Revenue',
                name: l('Price'),
                cornerRadius: 10,
            }
        ],
        //title: 'Total Revenue',
        legend: {
            visible: false,
            // verticalAlignment: 'bottom',
            // horizontalAlignment: 'center',
        },
        onPointClick(e) {
            e.target.select();
        },
    });
});