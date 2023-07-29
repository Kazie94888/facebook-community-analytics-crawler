$(document).ready(function () {
    let provinceToggleModal = $('#provinceToggleModal').dxSwitch({
        value: false,
        // switchedOnText: '',
        // switchedOffText: '',
        width: "40px",
        onValueChanged(data) {
            provinceToggleSwitch(data);
        },
    }).dxSwitch('instance');
    function provinceToggleSwitch (data) {
        if(data.value){
            $('.city-group-modal').each(function () {
                $(this).prop('checked',true);
            });
        }else{
            $('.city-group-modal').each(function () {
                $(this).prop('checked',false);
            });
        }
    }
    
    loadSegmentInsightsBody('');
    let genders = [];
    let relationships = [];
    let ages = [];
    let cities = [];
    $("#filterSearch").click(function () {
        genders = [];
        relationships = [];
        ages = [];
        cities = [];
        let hasCard = '';
        let hasHouse = '';
        $('.gender-group').each(function () {
            if ($(this).is(":checked")) {
                genders.push($(this).data('value'));
            }
        });
        $('.relationship-group').each(function () {
            if ($(this).is(":checked")) {
                relationships.push($(this).data('value'));
            }
        });
        $('.age-group').each(function () {
            if ($(this).is(":checked")) {
                ages.push($(this).data('value'));
            }
        });
        $('.city-group').each(function () {
            if ($(this).is(":checked")) {
                cities.push($(this).data('value'));
            }
        });
        $('.city-group-modal').each(function () {
            if ($(this).is(":checked")) {
                cities.push($(this).data('value'));
            }
        });
        $('.car-group').each(function () {
            if ($(this).is(":checked")) {
                hasCard = $(this).data('value');
            }
        });
        $('.house-group').each(function () {
            if ($(this).is(":checked")) {
                hasHouse = $(this).data('value');
            }
        });
        let carOwner;
        if(hasCard === 0){
            carOwner = true;
        }
        if(hasCard === 1){
            carOwner = false;
        }

        let houseOwner;
        if(hasHouse === 0){
            houseOwner = true;
        }
        if(hasHouse === 1){
            houseOwner = false;
        }
        let uniqueCities = cities.filter(onlyUnique);
        // let filter = function () {
        //     return {
        //         genderTypes: genders,
        //         relationshipStatus: relationships,
        //         ageSegmentEnums: ages,
        //         cities: uniqueCities,
        //         carOwner: null,
        //     };
        // };
        // let getMetricsInput = function () {
        //     return {
        //         merchantId: null,
        //         socialCommunityIds: null,
        //         filter: {
        //             genderTypes: genders,
        //             relationshipStatus: relationships,
        //             ageSegmentEnums: ages,
        //             cities: uniqueCities,
        //             carOwner: false,
        //         }
        //     };
        // };
        let data = [];
        if(uniqueCities === null || uniqueCities.length === 0){
            data = [{name:"genderTypes",value:genders},{name:"relationshipStatus",value:relationships},{name:"ageSegmentEnums",value:ages},{name:"carOwner",value:carOwner},{name:"houseOwner",value:houseOwner}];
            
        }else{
            data = [{name:"genderTypes",value:genders},{name:"relationshipStatus",value:relationships},{name:"ageSegmentEnums",value:ages},{name:"cities",value:uniqueCities},{name:"carOwner",value:carOwner},{name:"houseOwner",value:houseOwner}];
        }

        let queryString = abp.utils.buildQueryString(data);
        loadSegmentInsightsBody(queryString);
    });
    $("#filterClear").click(function () {
        $('.gender-group').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
        $('.relationship-group').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
        $('.age-group').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
        $('.city-group').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
        $('.city-group-modal').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
        $('.car-group').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
        $('.house-group').each(function () {
            if ($(this).is(":checked")) {
                $(this).prop('checked',false);
            }
        });
    });
    $("#openProvinceModal").click(function () {
        provinceToggleModal.option('onValueChanged', '');
        const cityGroupFilter = [];
        $('.city-group').each(function () {
            if ($(this).is(":checked")) {
                cityGroupFilter.push($(this).data('value'));
            }
        });
        $('.city-group-modal').each(function () {
            if(cities.includes($(this).data('value'))){
                $(this).prop('checked',true);
            }else{
                $(this).prop('checked',false);
            }
            if(cityGroupFilter.includes($(this).data('value'))){
                $(this).prop('checked',true);
            }
            if($(this).is(":checked") !== true){
                provinceToggleModal.option('value', false);
            }
        });
        provinceToggleModal.option('onValueChanged', provinceToggleSwitch);
    });
    $("#provinceModalSave").click(function () {
        cities = [];
        $('.city-group-modal').each(function () {
            if ($(this).is(":checked")) {
                cities.push($(this).data('value'));
            }
        });
        $('.city-group').each(function () {
            if(cities.includes($(this).data('value'))){
                $(this).prop('checked',true);
            }else{
                $(this).prop('checked',false);
            }
        });
    });
});

function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

function loadSegmentInsightsBody(data){
    $.ajax({
        type: "GET",
        url: "/segment-insight-data" + data,
        success: function (viewHTML) {
            $("#segmentInsightBody").html(viewHTML);
            loadChart();
        },
        error: function (errorData) { onError(errorData); }
    });
}

function loadChart(){
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
            text: l("Relationship"),
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
}