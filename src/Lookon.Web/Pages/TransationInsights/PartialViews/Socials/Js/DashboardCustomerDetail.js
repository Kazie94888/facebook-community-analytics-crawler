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
        centerTemplate(pieChart, container) {
            const content = $(`<svg><circle cx="100" cy="100" fill="#eee" r="${pieChart.getInnerRadius() - 6}"></circle>`
                + '<text text-anchor="middle" style="font-size: 18px;font-family: Open Sans;font-weight: 600" x="100" y="100" fill="#9BABC7">'
                + `<tspan>${l("Gender")}</tspan></text></svg>`);
            container.appendChild(content.get(0));
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
        centerTemplate(pieChart, container) {
            const content = $(`<svg><circle cx="100" cy="100" fill="#eee" r="${pieChart.getInnerRadius() - 6}"></circle>`
                + '<text text-anchor="middle" style="font-size: 18px;font-family: Open Sans;font-weight: 600" x="100" y="95" fill="#9BABC7">'
                + `<tspan>${l("Chart.Relationship.Up")}</tspan>`
                + `<tspan x="100" dy="20px" style="font-size: 18px;font-family: Open Sans;font-weight: 600">${l("Chart.Relationship.Down")}</tspan></text></svg>`);
            container.appendChild(content.get(0));
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
