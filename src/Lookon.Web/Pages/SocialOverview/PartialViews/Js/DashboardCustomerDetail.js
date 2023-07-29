var abp = abp || {};
$(function () {
    var l = abp.localization.getResource("LookOn");
    
    // build Gender Chart
    $('#maleChart').dxPieChart({
        type: 'doughnut',
        innerRadius: 0.75,
        palette:  palette_piechart,
        dataSource: genderTypesData,
        size: {
            height: 200,
            width: '100%'
        },
        title: {
            text: l("Gender"),
            font: {
                color: '#9BABC7',
                family: 'Open Sans',
                size: '15px',
                weight: 600
            }
        },
        tooltip: {
            enabled: true,
            customizeTooltip(arg) {
                return {
                    text: `${arg.valueText} - ${(arg.percent * 100).toFixed(2)}%`,
                };
            },
        },
        legend: {
            horizontalAlignment: 'right',
            verticalAlignment: 'top',
            margin: 0,
            customizeItems: function(items) {
                return items.forEach(item => {
                    item.text = l(item.text);
                    return item;
                })
            }
        },
        export: {
            enabled: false,
        },
        series: [{
            argumentField: 'Type',
            valueField: 'Count',
            label: {
                visible: false,
                connector: {
                    visible: true,
                },
            },
        }],
    });
    
    // build Device Chart
    $('#relationshipChart').dxPieChart({
        type: 'doughnut',
        innerRadius: 0.75,
        palette:  palette_piechart,
        dataSource: relationShipTypesData,
        title: {
            text: l("Page2.DashboardCustomerCompare.RelationShip"),
            font: {
                color: '#9BABC7',
                family: 'Open Sans',
                size: '15px',
                weight: 600
            }
        },
        size: {
            height: 200,
            width: '100%'
        },
        tooltip: {
            enabled: true,
            customizeTooltip(arg) {
                return {
                    text: `${arg.valueText} - ${(arg.percent * 100).toFixed(2)}%`,
                };
            },
        },
        legend: {
            horizontalAlignment: 'right',
            verticalAlignment: 'top',
            margin: 0,
            customizeItems: function(items) {
                return items.forEach(item => {
                    item.text = l(item.text);
                    return item;
                })
            }
        },
        export: {
            enabled: false,
        },
        series: [{
            argumentField: 'Name',
            valueField: 'Count',
            label: {
                visible: false,
                connector: {
                    visible: true,
                },
            },
        }],
    });
});
