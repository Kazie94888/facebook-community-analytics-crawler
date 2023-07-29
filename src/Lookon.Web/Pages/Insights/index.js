$(function () {
    var l = abp.localization.getResource("LookOn");
    
    ChangeTimeFrameLabel(7);
    function loadSegmentInsightsBody(data) {
        abp.ui.block({
            elm: '#pageWrap',
            busy: true
        });
        $.ajax({
            type: "GET",
            url: "/insight-data" + data,
            success: function (viewHTML) {
                $("#insightData").html(viewHTML);
                abp.ui.unblock();
            },
            error: function (errorData) {
            }
        });
    }
    
    function ChangeTimeFrameLabel(days){
        let date = new Date();
        let from = new Date(new Date().setUTCDate(date.getUTCDate() - days));
        let fromDay = from.getUTCDate();
        let fromMonth = from.getUTCMonth() + 1;
        let day = date.getUTCDate();
        let month = date.getUTCMonth() + 1;
        
        if (fromDay < 10) fromDay = '0' + fromDay;
        if (fromMonth < 10) fromMonth = '0' + fromMonth;
        
        if (day < 10) day = '0' + day;
        if (month < 10) month = '0' + month;
        document.getElementById("TimeFrameLabel").innerHTML = `${fromDay}/${fromMonth} - ${day}/${month}`;
    }

    function resetTimeFrame(){
        document.getElementById("TimeFrameContainer").style.display = '';
        ChangeTimeFrameLabel(7);
        $("#ButtonWeekly").removeClass("btn-outline-dark").addClass("btn-dark");
        $("#ButtonMonthly").removeClass("btn-dark").addClass("btn-outline-dark");
    }
    
    $("#ButtonWeekly").click(function (){
        ChangeTimeFrameLabel(7);
        var value = $("input[name=insightTypeFilter]:checked").val();
        $("#ButtonWeekly").removeClass("btn-outline-dark").addClass("btn-dark");
        $("#ButtonMonthly").removeClass("btn-dark").addClass("btn-outline-dark");
        let data = [{name: "dataType", value: value}, {name: "timeFrameTypeString", value: "7days"}]
        let queryString = abp.utils.buildQueryString(data);
        loadSegmentInsightsBody(queryString);
    })
    
    $("#ButtonMonthly").click(function (){
        ChangeTimeFrameLabel(30);
        var value = $("input[name=insightTypeFilter]:checked").val();
        $("#ButtonMonthly").removeClass("btn-outline-dark").addClass("btn-dark");
        $("#ButtonWeekly").removeClass("btn-dark").addClass("btn-outline-dark");
        let data = [{name: "dataType", value: value}, {name: "timeFrameTypeString", value: "30days"}]
        let queryString = abp.utils.buildQueryString(data);
        loadSegmentInsightsBody(queryString);
    })
    
    $("#venn-filter input[name=insightTypeFilter]").change(function (){
        SelectInsightType();
    })
    
    function SelectEcom() {
        var div = d3.select("#venn")
        div.selectAll("path").each(function (d, i) {
            if (d.id === 1) {
                this.style.fill = '#D4DFFD';
                this.style.strokeOpacity = 1;
                venn.sortAreas(div, d, true);
            }
            if (d.id === 2) {
                this.style.fill = '#F6EAF0';
                this.style.strokeOpacity = 0;
            }
            if (d.id === 3) {
                this.style.fill = '#F3E6B8';
                this.style.strokeOpacity = 0;
            }
        })
        resetTimeFrame();
    }
    
    function SelectEcomOnly() {
        var div = d3.select("#venn")
        div.selectAll("path").each(function (d, i) {
            this.style.strokeOpacity = 1;
            if (d.id === 1) {
                this.style.fill = '#D4DFFD';
            }
            
            if(d.id === 3) {
                this.style.fill = '#F3E6B8';
                this.style.stroke = '#7197F9';
                this.style.strokeWidth = 6;
            }
            
            if (d.id === 2) {
                this.style.fill = '#F6EAF0';
                venn.sortAreas(div, d, true);
                this.style.strokeOpacity = 0;
            }
        })
        resetTimeFrame();
    }

    function SelectSocial() {
        var div = d3.select("#venn")
        document.getElementById("TimeFrameContainer").style.display = 'none';
        div.selectAll("path").each(function (d, i) {
            if (d.id === 2) {
                this.style.fill = '#F3DBE7';
                this.style.strokeOpacity = 1;
                venn.sortAreas(div, d, true);
            }
            if (d.id === 1) {
                this.style.fill = '#EAEFFE';
                this.style.strokeOpacity = 0;
            }

            if (d.id === 3) {
                this.style.fill = '#F3E6B8';
                this.style.strokeOpacity = 0;
            }
        })
    }

    function SelectSocialOnly() {
        var div = d3.select("#venn")
        document.getElementById("TimeFrameContainer").style.display = 'none';
        div.selectAll("path").each(function (d, i) {
            this.style.strokeOpacity = 1;
            if (d.id === 2) {
                this.style.fill = '#F3DBE7';
            }
            if(d.id === 3) {
                this.style.fill = '#F3E6B8';
                this.style.stroke = '#DE8AB1';
                this.style.strokeWidth = 6;
            }

            if (d.id === 1) {
                this.style.fill = '#EAEFFE';
                venn.sortAreas(div, d, true);
                this.style.strokeOpacity = 0;
            }
        })
    }

    function SelectIntersect() {
        var div = d3.select("#venn")
        div.selectAll("path").each(function (d, i) {
            if (d.id === 3) {
                this.style.fill = '#FFFCF2';
                this.style.strokeOpacity = 1;
                venn.sortAreas(div, d, true);
                this.style.stroke = 'rgb(240 192 121)';
                this.style.strokeWidth = 3;
            }
            if (d.id === 1) {
                this.style.strokeOpacity = 0;
                this.style.fill = '#EAEFFE';
            }

            if (d.id === 2) {
                this.style.strokeOpacity = 0;
                this.style.fill = '#F6EAF0';
            }
        })
        resetTimeFrame();
    }
    
    function SelectInsightType(){
        var value = $("input[name=insightTypeFilter]:checked").val();
        $("#venn-filter span[name=insightTypeText]").each(function (){
            if(this.dataset.val === value){
                this.classList.remove("font-weight-500");
                this.classList.add("font-weight-700");
            }else{
                this.classList.remove("font-weight-700");
                this.classList.add("font-weight-500");
            }
        })
        let data = [{name: "dataType", value: value}]
        let queryString = abp.utils.buildQueryString(data);
        loadSegmentInsightsBody(queryString);
        switch(value){
            case '1':
                SelectEcom();
                break;
            case '2':
                SelectEcomOnly();
                break;
            case '3':
                SelectSocial();
                break;
            case '4':
                SelectSocialOnly();
                break;
            case '5':
                SelectIntersect();
                break;
            default:
                break;
        }
    }
    SelectInsightType();
})

function loadRevenueBarChart(){
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
}

function loadVennChart() {
    var wrapperChartElement = $("#venn-wrapper");
    var withChartWrapper = wrapperChartElement.width();
    var heightChartWrapper = wrapperChartElement.height();
    if (withChartWrapper !== undefined || heightChartWrapper !== undefined) {
        var chart = venn.VennDiagram()
            .width(withChartWrapper)
            .height(heightChartWrapper);

        var sets = [
            {"sets": [0], "label": "E-commerce", "size": ecomUserCount, "id": 1},
            {"sets": [1], "label": "Social", "size": socialUserCount, "id": 2},
            {"sets": [0, 1], "size": intersectUserCount, "id": 3},
        ];

        var div = d3.select("#venn")
        div.datum(sets).call(chart);

        var tooltip = d3.select("body").append("div")
            .attr("class", "venn-tooltip");

        div.selectAll("path")
            .style("fill-opacity", 1)
            .style("stroke-opacity", 0)
            .style("stroke-width", 3)
            .style("stroke-dasharray", 5)
            .attr("class", "rainbow")

        div.selectAll("path").each(function (d, i) {
            if (d.id === 1) {
                this.style.fill = '#EAEFFE';
                this.style.stroke = '#7197F9';
            } else if (d.id === 2) {
                this.style.fill = '#F6EAF0';
                this.style.stroke = '#DE8AB1';
            } else {
                this.style.fill = '#FFFCF2';
                this.style.stroke = '#DB0000';
            }
        })

        div.selectAll("text").each(function (d, i) {
            this.style.fill = '#463535';
            $(this).css('font-size', 20);
            $(this).css('font-weight', 600);
        })

        div.selectAll("tspan").each(function (d, i) {
            if(this.textContent === "E-commerce"){
                this.setAttribute('x', "115");
            }
            if(this.textContent === "Social"){
                this.setAttribute('x', "320");
            }
        })

        // div.selectAll("g")
        //     .on("mouseover", function (d, i) {
        //         // Display a tooltip with the current size
        //         tooltip.transition().duration(400).style("opacity", .9);
        //         tooltip.text(d.size + " users");
        //     })
        //
        //     .on("mousemove", function () {
        //         tooltip.style("left", (d3.event.pageX) + "px")
        //             .style("top", (d3.event.pageY - 28) + "px");
        //     })
        //
        //     .on("mouseout", function (d, i) {
        //         tooltip.transition().duration(400).style("opacity", 0);
        //     });
    }
}
function loadSocialPieChart() {
    $('#maleChart').dxPieChart({
        type: 'doughnut',
        innerRadius: 0.75,
        palette: palette_piechart,
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
            customizeItems: function (items) {
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
        palette: palette_piechart,
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
            customizeItems: function (items) {
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
}


