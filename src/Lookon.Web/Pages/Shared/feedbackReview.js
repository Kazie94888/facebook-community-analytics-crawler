var abp = abp || {};
$(function () {
    var l = abp.localization.getResource("LookOn");
    var feedbackService = window.lookOn.controllers.feedbacks.feedback;
    // let feedbackReviewModal = new abp.ModalManager({
    //     viewUrl: abp.appPath + "Shared/FeedbackReview",
    //     modalClass: "FeedbackModel"
    // });
    //
    // feedbackReviewModal.onResult(function () {
    //     window.location.reload();
    // });
    //
    // window.onload = function () {
    //     feedbackReviewModal.open();
    // }
    var currentUserId = abp.currentUser.id;


    let review = [];
    $(".btn-feedback-review").click(function (e) {
        const index = review.indexOf(this.value);
        if (index > -1) { // only splice array when item is found
            review.splice(index, 1); // 2nd parameter means remove one item only
            this.style.color = "#11142d";
            this.style.borderColor = "#11142d";
            this.style.backgroundColor = "#fff";
        } else {
            this.style.color = "#fff";
            this.style.backgroundColor = "#212529";
            this.style.borderColor = "#212529";
            review.push(this.value);

        }
    });
    $("#feedbackReviewButton").click(function (e) {
        if (review.length === 0) {
            review = ['0'];
        }
        var userFeedbackDto = {
            userId: currentUserId,
            score: feedbackScore,
            categories: review,
            page: "6",
            content: $("#Feedback_Content").val()
        };

        feedbackService.createUserFeedback(userFeedbackDto, {
            type: 'POST'
        }).then(function (result) {
            abp.message.success(l("Feedback.Review.Success"));
            // setTimeout(function () {
            //     window.location.reload();
            // }, 3000);
        });
    });
});
