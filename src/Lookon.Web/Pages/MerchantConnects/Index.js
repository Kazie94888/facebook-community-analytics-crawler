var connectCommunityModal;
$(function () {
    var l = abp.localization.getResource("LookOn");
    var merchantService = window.lookOn.controllers.merchants.merchant;
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

    connectCommunityModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantConnects/ConnectCommunityModal",
        scriptUrl: "/Pages/MerchantConnects/connectCommunityModal.js",
        modalClass: "merchantConnectCommunity"
    });

    connectCommunityModal.onResult(function (res) {
        abp.message.success(l('ConnectCommunityPage.Message')).then(function(){
            window.location.reload();
        });

    });

    let maxCommunitySocialModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantConnects/MaxSocialPageModal",
        modalClass: "maxSocialPageModal"
    });

    $("#btnConnectCommunity").click(function (e) {
        e.preventDefault();
        if (isMaxSocialPage) {
            maxCommunitySocialModal.open();
        } else {
            connectCommunityModal.open();
        }
    });

    maxCommunitySocialModal.onOpen(function () {
        $("#btnCancelUpgradeSubscription").click(function (e) {
            e.preventDefault();
            maxCommunitySocialModal.close();
        });

        $("#btnUpgradeSubscription").click(function (e) {
            e.preventDefault();
            window.open("https://lookon.vn", '_blank');
            maxCommunitySocialModal.close();
        });
    });


    // $("#storeChart").click(function (e){
    //     e.preventDefault();
    //     window.open("/transaction-insights/timeframe=weekly", '_blank');
    // }); 
    //
    $("#storeSiteUrl").click(function (e) {
        e.preventDefault();
        window.open("https://apps.haravan.com/", '_blank');
    });


    $("#btnSaveCommunityModal").click(function (e) {
        e.preventDefault();
        $("#formSaveCommunityModal").ajaxSubmit({
            success: function () {
                abp.message.success(l("SaveCommunityModal"));
                window.location.reload();
            },
            error: function (e) {
                abp.message.error(e.responseJSON.error.message);
            }
        });
    });

    $(".btn-delete-community").click(function (e) {
        debugger;
        let communityName = this.attributes.value.value;
        let myObj = { merchantId: merchantId, communityName: communityName};
        abp.message.confirm()
            .then(function (confirmed) {
                if (confirmed) {
                    debugger;
                    merchantService.deleteCommunity(myObj)
                        .then(function () {
                            abp.message.success(l("Merchant.Community.DeleteSuccess")).then(function(){
                                window.location.reload();
                            });
                        });
                }
            });
    });
    //
    // $("#communityChart").click(function (e){
    //     e.preventDefault();
    //     window.open("/social-overview", '_blank');
    // });
    //
    // $(".view-page-details").click(function (e){
    //     e.preventDefault();
    //     var pageId = e.currentTarget.getAttribute("data-pageid");
    //     window.open("https://www.facebook.com/" + pageId, '_blank');
    // });

});