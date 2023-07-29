$(function (){
    var togglePassword = $("#togglePassword");
    var password = $("#Password");
    var toggleConfirmPassword = $("#toggleConfirmPassword");
    var confirmPassword = $("#ConfirmPassword");

    togglePassword.on("click", function () {
        // toggle the type attribute
        const type = password.attr("type") === "password" ? "text" : "password";
        password.attr("type", type);

        // toggle the icon
        this.classList.toggle("bi-eye");
    });

    toggleConfirmPassword.on("click", function () {
        // toggle the type attribute
        const type = confirmPassword.attr("type") === "password" ? "text" : "password";
        confirmPassword.attr("type", type);

        // toggle the icon
        this.classList.toggle("bi-eye");
    });
});