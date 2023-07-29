var abp = abp || {};
$(function (){
    var l = abp.localization.getResource("LookOn");
    var merchantService = window.lookOn.controllers.merchants.merchant;
    var merchantUserService = window.lookOn.controllers.merchantUsers.merchantUser;

    var merchantId = $("#Merchant_Id").val();
    var dataTable = $("#MerchantUsersTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: false,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        ordering: false,
        ajax: abp.libs.datatables.createAjax(merchantUserService.getsByMerchant, {merchantId: merchantId}),
        columnDefs: [
            { data: "appUser.surname" },
            { data: "appUser.name" },
            { data: "appUser.userName" },
            { data: "appUser.phoneNumber" },
            { data: "appUser.email" },
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Delete"),
                                //visible: abp.auth.isGranted('LookOn.MerchantUsers.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    merchantUserService.delete(data.record.merchantUser.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            }
        ]
    }));
    
    $("#btnAddUser").click(function (){
         var   userNameOrEmail = $("#AddUserNameOrEmailInput").val()
        merchantUserService.addUser(merchantId,userNameOrEmail).then(function(result){
            abp.message.success(l("Successfully"));
        });
        
        dataTable.ajax.reload();
    });
    
    $("#btnUpdateMerchant").click(function (e){
        e.preventDefault();
        $("#forUpdateMerchant").ajaxSubmit({
            success: function() {
                abp.message.success(l("Successfully"));
            },
            error: function(e) {
                abp.message.error(e.responseJSON.error.message);
            }
        });
    });
    
    $("#btnUpdateMerchantConfiguration").click(function (e){
        var ecomRetentionThresholdInMonth = $("#updateEcomRetentionThresholdInMonth").val()
        var orderTotalKPI = $("#updateOrderTotalKPI").val()
        merchantService.updateMetricConfigures(merchantId, {"Ecom_RetentionThresholdInMonth":ecomRetentionThresholdInMonth, "OrderTotalKPI":orderTotalKPI}).then(function(result){
            abp.message.success(l("Successfully"));
            $("#updateEcomRetentionThresholdInMonthSuccessMessage").removeAttr('style');
        });
    })
});