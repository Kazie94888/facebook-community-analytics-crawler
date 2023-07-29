var abp = abp || {};
$(function (){
    var l = abp.localization.getResource("LookOn");
    $('.top-product-name').on('click', '', function (e) {
        var title = e.currentTarget.getAttribute("data-bs-original-title");
        if (title) {
            if ('clipboard' in navigator) {
                navigator.clipboard.writeText(title);
            } else {
                document.execCommand('copy', true, title);
            }

            abp.notify.success(
                l("CopyToClipboardSuccess")
            );
        }        
    });
});