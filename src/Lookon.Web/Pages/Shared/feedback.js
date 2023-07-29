var abp = abp || {};
$(function (){
    $(".feedback-rating-btn").click(function(){
        $(".feedback-rating-btn").each(function(){
            this.setAttribute('disabled','disabled');
        })
        var score = this.value;
        let feedbackModal = new abp.ModalManager({
            viewUrl: abp.appPath + "Shared/FeedbackReview?score=" + score,
            script:"Shared/feedbackReview.js",
            modalClass: "FeedbackReviewModel"
        });
        feedbackModal.onResult(function () {
            // window.location.reload();
        });
        feedbackModal.open();
    });
})