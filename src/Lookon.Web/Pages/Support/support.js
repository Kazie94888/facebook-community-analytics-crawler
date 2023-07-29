var abp = abp || {};
$(function () {
    let feedbackReviewModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Shared/Feedback",
        script: "Shared/feedback.js",
        modalClass: "FeedbackModel"
    });
    feedbackReviewModal.onResult(function () {
        
    });
    //
    // window.onload = function () {
    //     feedbackReviewModal.open();
    // }
    $(".btn-feedback").click(function(){
        feedbackReviewModal.open();
    })
})