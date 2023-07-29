$(function (){
    var togglePassword = $("#togglePassword");
    var password = $("#LoginInput_Password");
    togglePassword.on("click", function () {
        // toggle the type attribute
        const type = password.attr("type") === "password" ? "text" : "password";
        password.attr("type", type);

        // toggle the icon
        this.classList.toggle("bi-eye");
    });
});