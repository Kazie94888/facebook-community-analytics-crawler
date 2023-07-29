var abp = abp || {};
$(function () {
    var l = abp.localization.getResource("LookOn");


    let communityIntegrationModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Shared/CommunityIntegrationNotificationModal",
        modalClass: "communityIntegrationNotification"
    });

    communityIntegrationModal.onResult(function () {
        window.location.reload();
    });

    $('#communityIntegrationButton').click(function () {
        window.location.href = '/merchant-connects';
    });
    window.onload = function () {
        communityIntegrationModal.open();
    }
});
