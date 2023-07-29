var abp = abp || {};

abp.modals.merchantCreate = function () {
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
            lastNpDisplayNameId = 'AppUser_Email';
            lastNpIdId = 'AppUser_Id';
            _lookupModal.open({
                currentId: $('#AppUser_Id').val(),
                currentDisplayName: $('#AppUser_Email').val(),
                serviceMethod: function() {
                    return window.lookOn.controllers.merchants.merchant.getAppUserLookup;
                    
                }
            });
        });
        
    };

    return {
        initModal: initModal
    };
};
