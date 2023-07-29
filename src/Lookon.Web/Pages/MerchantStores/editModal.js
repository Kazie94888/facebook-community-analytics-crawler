var abp = abp || {};

abp.modals.merchantStoreEdit = function () {
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
        
        $('#MerchantLookupOpenButton').on('click', '', function () {
            lastNpDisplayNameId = 'Merchant_Name';
            lastNpIdId = 'Merchant_Id';
            _lookupModal.open({
                currentId: $('#Merchant_Id').val(),
                currentDisplayName: $('#Merchant_Name').val(),
                serviceMethod: function() {
                    return window.lookOn.controllers.merchantStores.merchantStore.getMerchantLookup;
                    
                }
            });
        });
        
    };

    return {
        initModal: initModal
    };
};
