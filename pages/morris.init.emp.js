
/**
 * Theme: Minton Admin Template
 * Author: Coderthemes
 * Morris Chart
 */

!function ($) {
    "use strict";

    var MorrisCharts = function () { };

    

    //creates Stacked chart
        MorrisCharts.prototype.createStackedChart = function (element, data, xkey, ykeys, labels, lineColors) {
            Morris.Bar({
                element: element,
                data: data,
                xkey: xkey,
                ykeys: ykeys,
                stacked: true,
                labels: labels,
                hideHover: 'auto',
                resize: true, //defaulted to true
                gridLineColor: '#eeeeee',
                barColors: lineColors
            });
        },

        MorrisCharts.prototype.init = function () {

            //creating Stacked chart
            var $stckedData = [
                { y: 'Sick', a: 5, b: 7 },
                { y: 'Casual', a: 2, b: 8 },
                { y: 'Annual', a: 4, b: 10 }
            ];
            this.createStackedChart('morris-bar-stacked', $stckedData, 'y', ['a', 'b'], ['Availed Leaves', 'Allocated Leaves'], ["#3bafda", "#ededed"]);

        },
    //init
        $.MorrisCharts = new MorrisCharts, $.MorrisCharts.Constructor = MorrisCharts
}(window.jQuery),

//initializing 
    function ($) {
        "use strict";
        $.MorrisCharts.init();
    }(window.jQuery);