var abp = abp || {};
$(function (){
    var l = abp.localization.getResource("LookOn");
        
    $("#btnUpdateInfo").click(function (e){
        e.preventDefault();
        $("#formUpdateInfo").ajaxSubmit({
            success: function() {
                abp.message.success(l("UpdateUserSuccess"));
                window.location.reload();
            },
            error: function(e) {
                abp.message.error(e.responseJSON.error.message);
            }
        });
    });
});