var abp = abp || {};
(function (){
    let subscriptionWarningModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSubscriptions/CheckSubscription",
        //scriptUrl: "/Pages/Terms/createModal.js",
        modalClass: "merchantSubscriptionWarning"
    });

    subscriptionWarningModal.open();

    // subscriptionWarningModal.onResult(function () {
    //     window.location.reload();
    // });
})(jQuery);