$(function () {
    var $dateRangePicker = $('#DashboardFilterForm .input-daterange');
    $dateRangePicker.datepicker({
        autoclose: true,
        language: abp.localization.currentCulture.cultureName
    });

    // var widget = new abp.WidgetManager({
    //     filterForm: '#HostDashboardWidgetsArea',
    //     filterCallback: function () {
    //         var dateRange = $dateRangePicker.data('datepicker');
    //
    //         return {
    //             startDate: dateRange.dates[0].toISOString(),
    //             endDate: dateRange.dates[1].toISOString()
    //         };
    //     }
    // });
    //
    // $('#DashboardFilterForm').submit(function (e) {
    //     e.preventDefault();
    //     widget.refresh();
    // });
   // widget.init();

    let connectCommunityModal = new abp.ModalManager({
        viewUrl: abp.appPath + "ConnectCommunityModal",
        scriptUrl: "/Pages/connectCommunityModal.js",
        modalClass: "merchantConnectCommunity"
    });

    connectCommunityModal.onResult(function () {
        window.location.reload();
    });

    $("#btnConnectCommunity").click(function (e){
        e.preventDefault();
        connectCommunityModal.open();
    });
});