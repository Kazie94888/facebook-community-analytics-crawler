var abp = abp || {};
var l = abp.localization.getResource("LookOn");
var userInfoService = window.lookOn.controllers.userInfos.userInfo;

$('#UpdateNotification').click(function(){
    userInfoService.updateNotificationStatus(true);
    window.location.reload();
});