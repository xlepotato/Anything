$(document).ready(function () {
    //Home Page(Exchange)////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    GetCurrencyTo();
    $("#drawBorder1").click(function () {
        $(this).toggleClass('selected');

        if ($(this).hasClass('selected')) {
            $('#ddlExchangeFrom').show();
        } else {
            $('#ddlExchangeFrom').hide();
        }
    });
    $("#drawBorder2").click(function () {
        $(this).toggleClass('selected');

        if ($(this).hasClass('selected')) {
            $('#ddlExchangeTo').show();
        } else {
            $('#ddlExchangeTo').hide();
        }
    });
    $(".exchangeDropdown").click(function () {
        $(this).find(".dropdown-menu").hide();
    });
    $("#tbExchangeFrom").on('input', function () {
        GetCurrencyTo();
    });
    $("#tbExchangeTo").on('input', function () {
        GetCurrencyFrom();
    });

    $("#ddlExchangeFrom li a").click(function () {
        console.log($(this).html());
        var imgHtml = $("#btnExchangeFrom img")[1].outerHTML;
        $("#btnExchangeFrom").html("");
        $("#btnExchangeFrom").append($(this).html());
        $("#btnExchangeFrom img").removeClass("scrollbarDispFlag").addClass("dispFlag");
        $("#btnExchangeFrom").append(imgHtml);
        
        GetCurrencyTo();
        GetGraph();
    });
    $("#ddlExchangeTo li a").click(function () {
        var imgHtml = $("#btnExchangeTo img")[1].outerHTML;
        $("#btnExchangeTo").html("");
        $("#btnExchangeTo").append($(this).html());
        $("#btnExchangeTo img").removeClass("scrollbarDispFlag").addClass("dispFlag");
        $("#btnExchangeTo").append(imgHtml);

        GetCurrencyFrom();
        GetGraph();
    });
    function GetCurrencyTo() {
        var Data = {
            ExchangeAmount: $("#tbExchangeFrom").val(),
            ExchangeFrom: $("#btnExchangeFrom").text().trim(),
            ExchangeTo: $("#btnExchangeTo").text().trim()
        }
        $.ajax({
            url: window.location.href + "/Home/GetCurrency",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#tbExchangeTo").val(result.Amount.toFixed(2));
                $("#lblResult").text(result.Rate);
            }
        });
    }
    function GetCurrencyFrom() {
        var Data = {
            ExchangeAmount: $("#tbExchangeTo").val(),
            ExchangeFrom: $("#btnExchangeTo").text().trim(),
            ExchangeTo: $("#btnExchangeFrom").text().trim()
        }
        $.ajax({
            url: window.location.href + "/Home/GetCurrency",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#tbExchangeFrom").val(result.Amount.toFixed(2));
                $("#lblResult").text(result.Rate);
            }
        });
    }
    //Home Page(Graph)////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    GetGraph();
    function GetGraph() {
        $(".loadingContainer").show();
        $("#myChart").remove();
        $(".chartContainer").hide();
        $(".chartContainer").append('<canvas id="myChart"><canvas>');        
        var Data = {
            ExchangeFrom: $("#btnExchangeFrom").text().trim(),
            ExchangeTo: $("#btnExchangeTo").text().trim() 
        }
        $.ajax({
            url: window.location.href + "/Home/GetGraph", 
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                GenerateGraph(result.Title, result.ShortDate, result.RegressionY, result.Amount);
                $(".loadingContainer").hide();
                $(".chartContainer").show();
            }
        });
    }
   
    function GenerateGraph(Title, ShortDates, RegressionY, Amounts) {
        var ctx = document.getElementById('myChart').getContext('2d');
        var red = 235;
        var green = 156;
        var blue = 21;
       
        var chart = new Chart(ctx, {
            // The type of chart we want to create
            type: 'line',
            // The data for our dataset
            data: {
                labels: ShortDates,
                datasets: [{
                    label: 'Prediction',
                    
                    borderColor: 'rgb(' + red + ', ' + green + ', ' + blue + ')',
                    fill: false,
                    data: RegressionY,

                    // Changes this dataset to become a line

                },{
                    label: Title,
                    //backgroundColor: 'rgb(' + red + ', ' + green + ', ' + blue + ')',
                    borderColor: 'rgb(' + red + ', ' + green + ', ' + blue + ')',
                   
                    data: Amounts
                }]
            },

            // Configuration options go here
            options: {
                legend: {
                    labels: {
                        // This more specific font property overrides the global property
                        defaultFontColor: '#FFF'
                    }
                }
            }
        });
      
    }
    //Home Page(Money Changer)////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    var trMoneyChangerHtml = $("#trMoneyChanger").parent().html();
    var sortBy = "Best";
    var isFavourite = false;
    $("#trMoneyChanger").html("");
    function Filter() {
        var exchangeFrom = $("#btnExchangeFrom").text().trim();
        var exchangeTo = $("#btnExchangeTo").text().trim();
        var Data = {
            Search: $("#tbSearch").val(),
            ExchangeFrom: exchangeFrom,
            ExchangeTo: exchangeTo,
            SortBy: sortBy,
            IsFavourite : isFavourite
        }
        $.ajax({
            url: window.location.href + "/Home/Filter",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                $("#tbMoneyChanger").html("");
                if (result.MoneyChangers> 0)
                {
                    $("#moneyChangerContainer").show();
                    $("#btnViewMoneyChangers").show();
                }
                else
                {
                    $("#moneyChangerContainer").hide();
                    $("#btnViewMoneyChangers").hide();
                }
                $.each(result.ExchangeRates, function () {
                    if (this.HasFavourite)
                    {
                        trMoneyChangerHtml = trMoneyChangerHtml.replace('/Assets/Images/yellowStar_white.png','/Assets/Images/yellowStar.png');
                    }
                    else
                    {
                        trMoneyChangerHtml = trMoneyChangerHtml.replace('/Assets/Images/yellowStar.png', '/Assets/Images/yellowStar_white.png');
                    }
                    $("#tbMoneyChanger").append(trMoneyChangerHtml.replace("{{Name}}", this.Name)
                                                    .replace("{{Location}}", this.Location)
                                                    .replace("{{ExchangeFrom}}", exchangeFrom)
                                                    .replace("{{Rate}}", this.Rate.toFixed(4))
                                                    .replace("{{ExchangeTo}}", exchangeTo)
                                                    .replace("{{LastUpdated}}", this.LastUpdated)
                                                    .replace("{{Name}}", this.Name));
                });
                $(".btnFav").click(function () {
                    var src = ($(this).find("img").attr('src'));
                    if (src == '/Assets/Images/yellowStar_white.png')
                    {
                        src = '/Assets/Images/yellowStar.png';
                    }
                    else
                    {
                        src = '/Assets/Images/yellowStar_white.png';
                    }                   
                    $(this).find("img").attr('src', src);
                    SetFavourite($(this).parent().next().children().children().text().trim());
                });
            }
        });
    }
    function SetFavourite(moneyChangerName)
    {
        var Data = {
            MoneyChangerName: moneyChangerName
        }
        $.ajax({
            url: window.location.href + "Home/SetFavourite",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                console.log(result);
            }
        });
    }
    $("#btnStyleFav").click(function () {
        if ($("#btnStyleFav").hasClass("btnStyleFav"))
        {
            $("#btnStyleFav").removeClass("btnStyleFav").addClass("btnStyleBest");
            isFavourite = true;
            Filter();
        }
        else
        {
            $("#btnStyleFav").removeClass("btnStyleBest").addClass("btnStyleFav");
            isFavourite = false;
            Filter();
        }
    });
    $(".starExchange").click(function () {
        $("#btnStyleFav").removeClass("btnStyleFav").addClass("btnStyleBest");
        isFavourite = true;
        Filter();
        $('html, .rowContainer').animate({
            scrollTop: ($('#moneyChangerContainer').offset().top)
        }, 500);
    });
    $("#btnViewMoneyChangers").click(function () {
        $("#btnStyleFav").removeClass("btnStyleBest").addClass("btnStyleFav");
        isFavourite = false;
        Filter();
        $('html, .rowContainer').animate({
            scrollTop: ($('#moneyChangerContainer').offset().top)
        }, 500);
    });
    $("#btnStyleLowest").click(function () {
        sortBy = "Lowest";
        $("#btnStyleLowest").removeClass("btnStyleLowest").addClass("btnStyleBest");
        $("#btnStyleBest").removeClass("btnStyleBest").addClass("btnStyleLowest");
        Filter();
    });
    $("#btnStyleBest").click(function () {
        sortBy = "Best";
        $("#btnStyleBest").removeClass("btnStyleLowest").addClass("btnStyleBest");
        $("#btnStyleLowest").removeClass("btnStyleBest").addClass("btnStyleLowest");
        Filter();
    });
    $("#tbSearch").on('input', function () {
        Filter();
    });
    $("#ddlExchangeFrom li a").click(function () {
        Filter();
    });
    $("#ddlExchangeTo li a").click(function () {
        Filter();
    });
    Filter();

    //Home Page (Drop-down clear)//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    $(document).on("click", function (event) {
        var $trigger = $(".dropdown");
        if ($trigger !== event.target && !$trigger.has(event.target).length) {
            $(".dropdown-menu").slideUp("fast");
        }
    });

    //Login Page//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    if (window.location.pathname === '/login' || window.location.pathname === '/Login' || window.location.pathname === '/Login/Index') { //hide scrollbar
        $(".rowContainer").css({ 'overflow': 'hidden' });
        document.getElementById("headerBtnDiv").remove();
    }

    $("#btnLogin").click(function () {
        Login();
    });
    $("#username").on('input', function () {
        $(".lblFailMsg").text("");
    });
    $("#password").on('input', function () {
        $(".lblFailMsg").text("");
    });
    function Login() {
        var Data = {
            Username: $("#username").val(),
            Password: $("#password").val(),
        }
        $.ajax({
            url: window.location.href + "/login",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result == "Success") {
                    window.location.href = "/";
                }
                else {
                    $(".lblFailMsg").text(result);
                }
            }
        });
    }
    //Register Page//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    
    
    if (window.location.pathname === '/signup' || window.location.pathname === '/SignUp' || window.location.pathname === '/SignUp/Index') { //hide scrollbar
        $(".rowContainer").css({ 'overflow': 'hidden' });
        document.getElementById("headerBtnDiv").remove();

    }
    function ValidateUsername() {
        var reg = /[a-zA-Z0-9]{6,}$/;
        if (reg.test($("#username").val())) {
            return true;
        }
        else
        {
            $("#usernameError").text("Please enter at least 6 alpha numeric characters.");
            $("#usernameError").show();
            return false;
        }
    }
    function ValidatePassword() {
        var reg = /[\w]{8,}$/;
        if (reg.test($("#password").val())) {
            return true;
        }
        else {
            $("#passwordError").text("Please enter at least 8 characters.");
            $("#passwordError").show();
            return false;
        }
    }
    function ValidateConfirmPassword() {
      
        if ($("#password").val() == $("#confirmpassword").val()) {
            return true;
        }
        else {
            $("#confirmPWError").text("Password does not match!");
            $("#confirmPWError").show();
            return false;
        }
    }
    function ValidateMobileNumber() {
        var reg = /^[0-9]{8}$/;
        if (reg.test($("#mobilenumber").val())) {
            return true;
        }
        else {
            $("#mobileNoError").text("Please enter your 8 digit singapore number.");
            $("#mobileNoError").show();
            return false;
        }
    }

    $("#btnRegister").click(function () {
        ValidateUsername();
        ValidatePassword();
        ValidateConfirmPassword();
        ValidateMobileNumber();
        if (ValidateUsername() && ValidatePassword() && ValidateConfirmPassword() && ValidateMobileNumber()) {
            Register();
        }
    });
    $("#username").on('input', function () {
        $("#usernameError").text("");
    });
    $("#password").on('input', function () {
        $("#passwordError").text("");
        $("#confirmPWError").text("");
    });
    $("#confirmPassword").on('input', function () {
        $("#confirmPWError").text("");
    });
    $("#mobilenumber").on('input', function () {
        $("#mobileNoError").text("");
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
                if (result == "Success") {
                    $(".lblSuccessMsg").text("Account Has Been Successfully Created.");
                }
                else if (result == "Mobile")
                {
                    $("#mobileNoError").text("Mobile number already exists");
                }
                else if (result == "Username")
                {
                    $("#usernameError").text("Username already exists");
                }
            }
        });
    }
    //Details//////////////////////////////////////////////////////////////////////////////////////////////////////////////
    $('.table-curved tr:last').addClass("lastRow");
    $(".table-curved tr:even").addClass("evenRow");
    $(".table-curved tr:odd").addClass("oddRow");
    $('.table-curved tr:first').removeClass("evenRow");

    $(".btnFav").click(function () {
        var src = ($(this).find("img").attr('src'));
        if (src == '/Assets/Images/yellowStar_white.png') {
            src = '/Assets/Images/yellowStar.png';
        }
        else {
            src = '/Assets/Images/yellowStar_white.png';
        }
        $(this).find("img").attr('src', src);
        SetFavouriteDetails($(".btnFav").parent().next().text().trim());
    });
    function SetFavouriteDetails(moneyChangerName) {
        var Data = {
            MoneyChangerName: moneyChangerName
        }
        $.ajax({
            url: "/MoneyChangerDetails/SetFavourite",
            data: Data,
            typr: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                console.log(result);
            }
        });
    }
    //GetWebscrape();
});
//Home Read Webscrape Date
function GetWebscrape() {
    $.ajax({
        url: "/Assets/Data/Data.json",
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $.each(result, function () {
                var MoneyChangers = {
                    Name: this.moneychanger_name,
                    Address: this.address,
                    Img: this.img,
                    OpeningHours: this.operating_hours,
                    Tel_No: this.tel_No.trim()
                };
                var moneychanger_name = this.moneychanger_name;
                SaveWebscrapeMoneyChanger(MoneyChangers);
                $.each(this.currency_table, function () {
                    var ExchangeRates = {
                        moneychanger_name: moneychanger_name,
                        currency_code: this.currency_code,
                        exchange_rate_buy: this.exchange_rate_buy,
                        exchange_rate_sell: this.exchange_rate_sell,
                        last_update_buy: this.last_update_buy,
                        last_update_sell: this.last_update_sell
                    };
                    SaveWebscrapeExchangeRates(ExchangeRates);
                });

            });
        }
    });
}
function SaveWebscrapeMoneyChanger(MoneyChangers) {
    $.ajax({
        url: "/Home/UpdateMoneyChanger",
        data: MoneyChangers,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
          
        }
    });
}
function SaveWebscrapeExchangeRates(ExchangeRates) {
    $.ajax({
        url: "/Home/UpdateExchangeRates",
        data: ExchangeRates,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {

        }
    });
}

