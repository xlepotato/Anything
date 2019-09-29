$(document).ready(function () {
    //Home Page(Exchange)////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    $("#tbExchangeFrom").on('input', function () {
        GetCurrencyTo();
    });
    $("#tbExchangeTo").on('input', function () {
        GetCurrencyFrom();
    });
    $("#selExchangeFrom").change(function () {
        GetCurrencyTo();
    });
    $("#selExchangeTo").change(function () {
        GetCurrencyFrom();
    });
    function GetCurrencyTo() {
        var Data = {
            ExchangeAmount: $("#tbExchangeFrom").val(),
            ExchangeFrom: $("#selExchangeFrom").val(),
            ExchangeTo: $("#selExchangeTo").val()
        }
        $.ajax({
            url: window.location.href + "/Home/GetCurrency",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#tbExchangeTo").val(result.Amount);
                $("#lblResult").text(result.Rate);
            }
        });
    }
    function GetCurrencyFrom() {
        var Data = {
            ExchangeAmount: $("#tbExchangeTo").val(),
            ExchangeFrom: $("#selExchangeTo").val(),
            ExchangeTo: $("#selExchangeFrom").val()
        }
        $.ajax({
            url: window.location.href + "/Home/GetCurrency",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#tbExchangeFrom").val(result.Amount);
                $("#lblResult").text(result.Rate);
            }
        });
    }
    //Home Page(Money Changer)////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //Login Page
    $("#btnLogin").click(function () {
        Login();
    });
    function Login() {
        console.log(window.location.href);
        var Data = {
            Username: $("#username").val(),
            Password: $("#password").val(),
        }
        $.ajax({
            url: window.location.href+ "/login",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#lblLoginMessage").html("");
                $("#lblLoginMessage").append(result);
            }
        });
    }
    //Register Page
    function validateUsername() {
        var name = document.getElementById("username").value;

        if (name.length == 0) {
            producePrompt("This field is Required.", "usernamePrompt", "red");
            return false;
        }
    }

    function producePrompt(message, promptLocation, color) {
        document.getElementById(promptLocation).innerHTML = message;
        document.getElementById(promptLocation).style.color = color;
    }

    $("#btnRegister").click(function () {2
        Register();
    });
    function Register() {
        var Data = {
            Username: $("#username").val(),
            Password: $("#password").val(),
            MobileNumber: $("#mobilenumber").val()
        }
        $.ajax({
            url: window.location.href + "/register",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#lblSuccess").html("");
                $("#lblSuccess").append(result);
            }
        });
    }
});