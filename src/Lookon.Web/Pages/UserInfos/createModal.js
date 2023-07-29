var abp = abp || {};

abp.modals.userInfoCreate = function () {
    var initModal = function (publicApi, args) {
        
        var lastNpIdId = '';
        var lastNpDisplayNameId = '';

        var _lookupModal = new abp.ModalManager({
            viewUrl: abp.appPath + "Shared/LookupModal",
            scriptUrl: "/Pages/Shared/lookupModal.js",
            modalClass: "navigationPropertyLookup"
        });

        $('.lookupCleanButton').on('click', '', function () {
            $(this).parent().parent().find('input').val('');
        });

        _lookupModal.onClose(function () {
            var modal = $(_lookupModal.getModal());
            $('#' + lastNpIdId).val(modal.find('#CurrentLookupId').val());
            $('#' + lastNpDisplayNameId).val(modal.find('#CurrentLookupDisplayName').val());
        });
        
        $('#AppUserLookupOpenButton').on('click', '', function () {
            lastNpDisplayNameId = 'AppUser_UserName';
            lastNpIdId = 'AppUser_Id';
            _lookupModal.open({
                currentId: $('#AppUser_Id').val(),
                currentDisplayName: $('#AppUser_UserName').val(),
                serviceMethod: function() {
                    return window.lookOn.controllers.userInfos.userInfo.getAppUserLookup;
                    
                }
            });
        });
        
    };

    return {
        initModal: initModal
    };
};
