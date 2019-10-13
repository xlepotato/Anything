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
        $("#loading").show();
        $("#myChart").remove();
        $("#chartContainer").append('<canvas id="myChart"><canvas>');        
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
                $("#loading").hide();
            }
        });
    }
   
    function GenerateGraph(Title, ShortDates, RegressionY, Amounts) {
        var ctx = document.getElementById('myChart').getContext('2d');
        var red = Math.floor((Math.random() * 255) + 0);
        var green = Math.floor((Math.random() * 255) + 0);
        var blue = Math.floor((Math.random() * 255) + 0);
       
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
    $("#trMoneyChanger").html("");
    function Filter() {
        var exchangeFrom = $("#btnExchangeFrom").text().trim();
        var exchangeTo = $("#btnExchangeTo").text().trim();
        var Data = {
            Search: $("#tbSearch").val(),
            ExchangeFrom: exchangeFrom,
            ExchangeTo: exchangeTo
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
                    $("#tbMoneyChanger").append(trMoneyChangerHtml.replace("{{Name}}", this.Name)
                                                    .replace("{{Location}}", this.Location)
                                                    .replace("{{ExchangeFrom}}", exchangeFrom)
                                                    .replace("{{Rate}}", this.Rate.toFixed(4))
                                                    .replace("{{ExchangeTo}}", exchangeTo)
                                                    .replace("{{LastUpdated}}", this.LastUpdated)
                                                    .replace("{{Name}}", this.Name));
                });
            }
        });
    }
    $("#btnViewMoneyChangers").click(function () {
        $('html, .rowContainer').animate({
            scrollTop: ($('#moneyChangerContainer').offset().top)
        }, 500);
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
});
