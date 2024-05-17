(function () {
    'use strict';

    angular.module('app.dashboard')
        .controller('dashboardController', dashboardController);

    /* -------------------------------
       CONTROLLER - Dashboard
    ------------------------------- */

    dashboardController.$inject = ['$scope', '$rootScope', '$state', '$timeout', 'dashboardService'];

    function dashboardController($scope, $rootScope, $state, $timeout, service) {

        var blue = '#348fe2', blueLight = '#5da5e8', blueDark = '#1993E4', aqua = '#49b6d6', aquaLight = '#6dc5de', aquaDark = '#3a92ab', green = '#00acac', greenLight = '#33bdbd', greenDark = '#008a8a', orange = '#f59c1a', orangeLight = '#f7b048', orangeDark = '#c47d15', dark = '#2d353c', grey = '#b6c2c9', purple = '#727cb6', purpleLight = '#8e96c5', purpleDark = '#5b6392', red = '#ff5b57';

        var vm = $scope;

        vm.init = init;
        vm.get = get;

        //vm.morrisBarChart = {
        //    data: [
        //      { year: '2015 Q1', sales: 3, net: 2, profit: 1 },
        //      { year: '2015 Q2', sales: 2, net: 0.9, profit: 0.45 },
        //      { year: '2015 Q3', sales: 1, net: 0.4, profit: 0.2 },
        //      { year: '2015 Q4', sales: 2, net: 1, profit: 0.5 }
        //    ],
        //    options: {
        //        xkey: 'year',
        //        ykeys: ['sales', 'net', 'profit'],
        //        labels: ['Sales', 'Net', 'Profit'],
        //        barColors: ['#777777', '#e74c3c', 'rgb(11, 98, 164)']
        //    }
        //};

        vm.morrisDonutChart = {
            options: {
                formatter: function (y) { return y + "%"; },
                resize: true,
                colors: [green, red, grey, dark, orange]
            }
            //completedThisWeek: [],
            //lateThisWeek: [],
            //completedLastWeek: [],
            //lateLastWeek: []
        };

        vm.barChart = {
            labels: [],
            series: [],
            //colors : [ '#803690', '#00ADF9', '#DCDCDC', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360'],
            colors: [dark, green, red, blue, grey, orange],
            data: [],
            option: {}
        };

        vm.init();

        function init() {

            //if (!$rootScope.User.hasAnyRole('SuperAdmin, Admin')) {
            //    if ($rootScope.User.hasAnyRole('RecManager'))
            //        return $state.go('app.employees.list');
            //    else if ($rootScope.User.hasAnyRole('HRManager, Employee'))
            //        return $state.go('app.shifts.list');
            //}

            setupChartJsOptions();
            setupBarChart();

            setupMorrisData();

            vm.get();
        }

        function setupMorrisData() {

            vm.morrisBarChart = {
                data: [
                  { year: '2015 Q1', sales: 3, net: 2, profit: 1 },
                  { year: '2015 Q2', sales: 2, net: 0.9, profit: 0.45 },
                  { year: '2015 Q3', sales: 1, net: 0.4, profit: 0.2 },
                  { year: '2015 Q4', sales: 2, net: 1, profit: 0.5 }
                ],
                options: {
                    xkey: 'year',
                    ykeys: ['sales', 'net', 'profit'],
                    labels: ['Sales', 'Net', 'Profit'],
                    barColors: ['#777777', '#e74c3c', 'rgb(11, 98, 164)']
                }
            };

            //vm.morrisDonutChart = {
            //    data: [
            //        { label: 'Jam', value: 25 },
            //        { label: 'Frosted', value: 40 },
            //        { label: 'Custard', value: 25 },
            //        { label: 'Sugar', value: 10 }
            //    ],
            //    options: {
            //        formatter: function (y) { return y + "%"; },
            //        resize: true
            //        //colors: [dark, orange, red, grey]
            //    }
            //};
        }

        function get() {
            service.getDashboardData().then(function (data) {

                vm.model = data;

                console.debug(vm.model);

                setBarChartData();

                morris();

            }, function (error) {
                console.log(error);
            });
        }

        function setBarChartData() {
            
            vm.barChart.data = [[], [], []];
            vm.barChart.series = ['Attended', 'OnTime', 'Late'];

            angular.forEach(vm.model.performanceChartItems, function (item) {
                vm.barChart.labels.push(item.label);
                vm.barChart.data[0].push(item.stats.completed);
                vm.barChart.data[1].push(item.stats.onTime);
                vm.barChart.data[2].push(item.stats.late);
            });
        }

        function morris() {

            //if (vm.model.completedThisMonth && vm.model.completedThisMonth.length > 0) {
            //    vm.morrisDonutChart.completedThisMonth = vm.model.completedThisMonth;
            //}

            //if (vm.model.lateThisMonth && vm.model.lateThisMonth.length > 0) {
            //    vm.morrisDonutChart.lateThisMonth = vm.model.lateThisMonth;
            //}

            if (vm.model.currentMonthStats && vm.model.currentMonthStats.total > 0) {

                var completedThisMonth = [
                    { label: "Attended", value: vm.model.currentMonthStats.completedPercentage },
                    { label: "Missed", value: vm.model.currentMonthStats.missedPercentage },
                    { label: "Due", value: vm.model.currentMonthStats.duePercentage }
                ];
                vm.morrisDonutChart.completedThisMonth = completedThisMonth;

                var lateThisMonth = [
                    { label: "OnTime", value: vm.model.currentMonthStats.onTimePercentage },
                    { label: "Late", value: vm.model.currentMonthStats.latePercentage }
                ];
                vm.morrisDonutChart.lateThisMonth = lateThisMonth;
            }

            //if (vm.model.completedLastMonth && vm.model.completedLastMonth.length > 0) {
            //    vm.morrisDonutChart.completedLastMonth = vm.model.completedLastMonth;
            //}

            //if (vm.model.lateLastMonth && vm.model.lateLastMonth.length > 0) {
            //    vm.morrisDonutChart.lateLastMonth = vm.model.lateLastMonth;
            //}

            if (vm.model.lastMonthStats && vm.model.lastMonthStats.total > 0) {

                var completedLastMonth = [
                    { label: "Attended", value: vm.model.lastMonthStats.completedPercentage || 0 },
                    { label: "Missed", value: vm.model.lastMonthStats.missedPercentage || 0 },
                    { label: "Due", value: vm.model.lastMonthStats.duePercentage || 0 }
                ];
                vm.morrisDonutChart.completedLastMonth = completedLastMonth;

                var lateLastMonth = [
                    { label: "OnTime", value: vm.model.lastMonthStats.onTimePercentage || 0 },
                    { label: "Late", value: vm.model.lastMonthStats.latePercentage || 0 }
                ];
                vm.morrisDonutChart.lateLastMonth = lateLastMonth;
            }

            /* Morris Area Chart
            ------------------------- */
            //setDonutGraph('morris-donut-chart', [
            //        { label: 'Jam', value: 25 },
            //        { label: 'Frosted', value: 40 },
            //        { label: 'Custard', value: 25 },
            //        { label: 'Sugar', value: 10 }
            //]);

            //setDonutGraph('morris-donut-chart1', [
            //        { label: 'Jam', value: 25 },
            //        { label: 'Frosted', value: 40 },
            //        { label: 'Custard', value: 25 },
            //        { label: 'Sugar', value: 10 }
            //]);

            //return;

            //$timeout(function () {
            //    var newData = [
            //        { label: 'Jam', value: 25 },
            //        { label: 'Frosted', value: 30 },
            //        { label: 'Custard', value: 25 },
            //        { label: 'Sugar', value: 10 },
            //        { label: 'Pudding', value: 10 }
            //    ];

            //    //setDonutGraph('morris-donut-chart1', newData);
            //    vm.morrisDonutChart.completedThisWeek = newData;

            //    console.debug('done');

            //}, 5000);
        }

        var donutGraphs = {};
        function setDonutGraph(elementId, data) {

            var graph = donutGraphs[elementId];
            if (graph) {
                graph.setData(data);
            } else {
                graph = Morris.Donut({
                    element: elementId,
                    data: data,
                    formatter: function (y) { return y + "%"; },
                    resize: true,
                    colors: [dark, orange, red, grey]
                });
                donutGraphs[elementId] = graph;
                if (data && data.length > 0) {
                    graph.setData(data);
                }
            }
        }

        function setGraphData(graph, data) {
            graph.setData(data);
        }

        function setupBarChart() {

            // white
            var white = 'rgba(255,255,255,1.0)';
            var fillBlack = 'rgba(45, 53, 60, 0.6)';
            var fillBlackLight = 'rgba(45, 53, 60, 0.2)';
            var strokeBlack = 'rgba(45, 53, 60, 0.8)';
            var highlightFillBlack = 'rgba(45, 53, 60, 0.8)';
            var highlightStrokeBlack = 'rgba(45, 53, 60, 1)';

            // blue
            var fillBlue = 'rgba(52, 143, 226, 0.6)';
            var fillBlueLight = 'rgba(52, 143, 226, 0.2)';
            var strokeBlue = 'rgba(52, 143, 226, 0.8)';
            var highlightFillBlue = 'rgba(52, 143, 226, 0.8)';
            var highlightStrokeBlue = 'rgba(52, 143, 226, 1)';

            // grey
            var fillGrey = 'rgba(182, 194, 201, 0.6)';
            var fillGreyLight = 'rgba(182, 194, 201, 0.2)';
            var strokeGrey = 'rgba(182, 194, 201, 0.8)';
            var highlightFillGrey = 'rgba(182, 194, 201, 0.8)';
            var highlightStrokeGrey = 'rgba(182, 194, 201, 1)';

            // green
            var fillGreen = 'rgba(0, 172, 172, 0.6)';
            var fillGreenLight = 'rgba(0, 172, 172, 0.2)';
            var strokeGreen = 'rgba(0, 172, 172, 0.8)';
            var highlightFillGreen = 'rgba(0, 172, 172, 0.8)';
            var highlightStrokeGreen = 'rgba(0, 172, 172, 1)';

            // purple
            var fillPurple = 'rgba(114, 124, 182, 0.6)';
            var fillPurpleLight = 'rgba(114, 124, 182, 0.2)';
            var strokePurple = 'rgba(114, 124, 182, 0.8)';
            var highlightFillPurple = 'rgba(114, 124, 182, 0.8)';
            var highlightStrokePurple = 'rgba(114, 124, 182, 1)';


            ///* ChartJS Bar Chart
            //------------------------- */
            //var randomScalingFactor = function () {
            //    return Math.round(Math.random() * 100);
            //};

            //var barChartData = {
            //    //labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            //    labels: [],
            //    datasets: [{
            //        label: "Attendance",
            //        fillColor: blueLight,
            //        strokeColor: strokeBlack,
            //        highlightFill: blueDark,
            //        highlightStroke: highlightStrokeBlack,
            //        //data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
            //        data: []
            //    }, {
            //        label: "Late",
            //        fillColor: orangeLight,
            //        strokeColor: strokeBlack,
            //        highlightFill: orangeDark,
            //        highlightStroke: highlightStrokeBlack,
            //        //data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
            //        data: []
            //    }]
            //};

            //vm.barChart.data = barChartData;
        }

        function setupChartJsOptions() {

            /* ChartJS Chart Options
            ------------------------- */
            var chartOptions = {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        fontColor: 'rgb(255, 99, 132)'
                    }
                }
                //animation: true,
                //animationSteps: 60,
                //animationEasing: 'easeOutQuart',
                //showScale: true,
                //scaleOverride: false,
                //scaleSteps: null,
                //scaleStepWidth: null,
                //scaleStartValue: null,
                //scaleLineColor: 'rgba(0,0,0,.1)',
                //scaleLineWidth: 1,
                //scaleShowLabels: true,
                //scaleLabel: '<%=value%>',
                //scaleIntegersOnly: true,
                //scaleBeginAtZero: false,
                //scaleFontFamily: '"Open Sans", "Helvetica Neue", "Helvetica", "Arial", sans-serif',
                //scaleFontSize: 12,
                //scaleFontStyle: 'normal',
                //scaleFontColor: '#707478',
                //responsive: true,
                //maintainAspectRatio: false,
                //showTooltips: true,
                //customTooltips: false,
                //tooltipEvents: ['mousemove', 'touchstart', 'touchmove'],
                //tooltipFillColor: 'rgba(0,0,0,0.8)',
                //tooltipFontFamily: '"Open Sans", "Helvetica Neue", "Helvetica", "Arial", sans-serif',
                //tooltipFontSize: 12,
                //tooltipFontStyle: 'normal',
                //tooltipFontColor: '#ccc',
                //tooltipTitleFontFamily: '"Open Sans", "Helvetica Neue", "Helvetica", "Arial", sans-serif',
                //tooltipTitleFontSize: 12,
                //tooltipTitleFontStyle: 'bold',
                //tooltipTitleFontColor: '#fff',
                //tooltipYPadding: 10,
                //tooltipXPadding: 10,
                //tooltipCaretSize: 8,
                //tooltipCornerRadius: 3,
                //tooltipXOffset: 10,
                //tooltipTemplate: '<%if (label){%><%=label%>: <%}%><%= value %>',
                //multiTooltipTemplate: '<%= value %>',
                //onAnimationProgress: function () { },
                //onAnimationComplete: function () { }
            };

            vm.barChart.options = chartOptions;
        }

        //$rootScope.breadcrumbs.add({ text: 'Home', href: $state.href('app.dashboard.home') });
        //$rootScope.breadcrumbs.add([
        //    { text: 'Home', href: $state.href('app.dashboard.home') },
        //    { text: 'Notifications', href: $state.href('app.notifications.list') },
        //    { text: 'List', href: '' }
        //]);

        angular.element(document).ready(function () {

            //setupBarChart();
            //setupChartJsOptions();

            /* Line Chart
            ------------------------- */
            var green = '#0D888B';
            var greenLight = '#00ACAC';
            var blue = '#3273B1';
            var blueLight = '#348FE2';
            var blackTransparent = 'rgba(0,0,0,0.6)';
            var whiteTransparent = 'rgba(255,255,255,0.4)';
            var month = [];
            month[0] = "January";
            month[1] = "February";
            month[2] = "March";
            month[3] = "April";
            month[4] = "May";
            month[5] = "Jun";
            month[6] = "July";
            month[7] = "August";
            month[8] = "September";
            month[9] = "October";
            month[10] = "November";
            month[11] = "December";

            //Morris.Line({
            //    element: 'visitors-line-chart',
            //    data: [
            //        { x: '2014-02-01', y: 60, z: 30 },
            //        { x: '2014-03-01', y: 70, z: 40 },
            //        { x: '2014-04-01', y: 40, z: 10 },
            //        { x: '2014-05-01', y: 100, z: 70 },
            //        { x: '2014-06-01', y: 40, z: 10 },
            //        { x: '2014-07-01', y: 80, z: 50 },
            //        { x: '2014-08-01', y: 70, z: 40 }
            //    ],
            //    xkey: 'x',
            //    ykeys: ['y', 'z'],
            //    xLabelFormat: function (x) {
            //        x = month[x.getMonth()];
            //        return x ? x.toString() : '';
            //    },
            //    labels: ['Page Views', 'Unique Visitors'],
            //    lineColors: [green, blue],
            //    pointFillColors: [greenLight, blueLight],
            //    lineWidth: '2px',
            //    pointStrokeColors: [blackTransparent, blackTransparent],
            //    resize: true,
            //    gridTextFamily: 'Open Sans',
            //    gridTextColor: whiteTransparent,
            //    gridTextWeight: 'normal',
            //    gridTextSize: '11px',
            //    gridLineColor: 'rgba(0,0,0,0.5)',
            //    hideHover: 'auto'
            //});

            ///* Donut Chart
            //------------------------- */
            //green = '#00acac';
            //blue = '#348fe2';
            //Morris.Donut({
            //    element: 'visitors-donut-chart',
            //    data: [
            //        { label: "New Visitors", value: 900 },
            //        { label: "Return Visitors", value: 1200 }
            //    ],
            //    colors: [green, blue],
            //    labelFamily: 'Open Sans',
            //    labelColor: 'rgba(255,255,255,0.4)',
            //    labelTextSize: '12px',
            //    backgroundColor: '#242a30'
            //});


            /* Vector Map
            ------------------------- */
            var map = new jvm.WorldMap({
                map: 'world_merc_en',
                scaleColors: ['#e74c3c', '#0071a4'],
                container: $('#visitors-map'),
                normalizeFunction: 'linear',
                hoverOpacity: 0.5,
                hoverColor: false,
                markerStyle: {
                    initial: {
                        fill: '#4cabc7',
                        stroke: 'transparent',
                        r: 3
                    }
                },
                regions: [{ attribute: 'fill' }],
                regionStyle: {
                    initial: {
                        fill: 'rgb(97,109,125)',
                        "fill-opacity": 1,
                        stroke: 'none',
                        "stroke-width": 0.4,
                        "stroke-opacity": 1
                    },
                    hover: { "fill-opacity": 0.8 },
                    selected: { fill: 'yellow' }
                },
                series: {
                    regions: [{
                        values: {
                            IN: '#00acac',
                            US: '#00acac',
                            KR: '#00acac'
                        }
                    }]
                },
                focusOn: { x: 0.5, y: 0.5, scale: 2 },
                backgroundColor: '#2d353c'
            });


            ///* Calendar
            //------------------------- */
            //var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            //var dayNames = ["S", "M", "T", "W", "T", "F", "S"];
            //var now = new Date();
            //month = now.getMonth() + 1;
            //var year = now.getFullYear();
            //var events = [[
            //    '2/' + month + '/' + year,
            //    'Popover Title',
            //    '#',
            //    '#00acac',
            //    'Some contents here'
            //], [
            //    '5/' + month + '/' + year,
            //    'Tooltip with link',
            //    'http://www.seantheme.com/color-admin-v1.3',
            //    '#2d353c'
            //], [
            //    '18/' + month + '/' + year,
            //    'Popover with HTML Content',
            //    '#',
            //    '#2d353c',
            //    'Some contents here <div class="text-right"><a href="http://www.google.com">view more >>></a></div>'
            //], [
            //    '28/' + month + '/' + year,
            //    'Color Admin V1.3 Launched',
            //    'http://www.seantheme.com/color-admin-v1.3',
            //    '#2d353c'
            //]];
            //var calendarTarget = $('#schedule-calendar');
            //$(calendarTarget).calendar({
            //    months: monthNames,
            //    days: dayNames,
            //    events: events,
            //    popover_options: {
            //        placement: 'top',
            //        html: true
            //    }
            //});
            //$(calendarTarget).find('td.event').each(function () {
            //    var backgroundColor = $(this).css('background-color');
            //    $(this).removeAttr('style');
            //    $(this).find('a').css('background-color', backgroundColor);
            //});
            //$(calendarTarget).find('.icon-arrow-left, .icon-arrow-right').parent().on('click', function () {
            //    $(calendarTarget).find('td.event').each(function () {
            //        var backgroundColor = $(this).css('background-color');
            //        $(this).removeAttr('style');
            //        $(this).find('a').css('background-color', backgroundColor);
            //    });
            //});


            /* Gritter Notification
            ------------------------- */
            // https://github.com/jboesch/Gritter/wiki

            //var gritter_notification_id;

            //setTimeout(function () {

            //    gritter_notification_id = $.gritter.add({
            //        title: 'Welcome back, Admin!',
            //        text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed tempus lacus ut lectus rutrum placerat.',
            //        image: '/App/ColorAdmin/assets/img/user-14.jpg',

            //        sticky: false,
            //        time: 6000, // hang on the screen for...
            //        fade_in_speed: 'medium', // how fast notifications fade in (string or int)
            //        fade_out_speed: 2000, // how fast the notices fade out

            //        class_name: 'my-sticky-class'

            //    });
            //}, 1000);

            //$.gritter.add({
            //    // (string | mandatory) the heading of the notification
            //    title: 'This is a regular notice!',
            //    // (string | mandatory) the text inside the notification
            //    text: 'This will fade out after a certain amount of time.',
            //    // (string | optional) the image to display on the left
            //    image: 'http://a0.twimg.com/profile_images/59268975/jquery_avatar_bigger.png',
            //    // (bool | optional) if you want it to fade out on its own or just sit there
            //    sticky: false,
            //    // (int | optional) the time you want it to be alive for before fading out (milliseconds)
            //    time: 8000,
            //    // (string | optional) the class name you want to apply directly to the notification for custom styling
            //    class_name: 'my-class',
            //    // (function | optional) function called before it opens
            //    before_open: function () {
            //        console.debug('I am a sticky called before it opens');
            //    },
            //    // (function | optional) function called after it opens
            //    after_open: function (e) {
            //        console.debug('I am a sticky called after it opens: \nI am passed the jQuery object for the created Gritter element...\n' + e);
            //    },
            //    // (function | optional) function called before it closes
            //    before_close: function (e, manual_close) {
            //        // the manual_close param determined if they closed it by clicking the 'x'
            //        console.debug('I am a sticky called before it closes: I am passed the jQuery object for the Gritter element... \n' + e);
            //    },
            //    // (function | optional) function called after it closes
            //    after_close: function () {
            //        console.debug('I am a sticky called after it closes');
            //    }
            //});

            //// remove gritter notification
            //setTimeout(function () {

            //    //$.gritter.removeAll();

            //    //$.gritter.removeAll({
            //    //    before_close: function (e) {
            //    //        alert('I am called before all notifications are closed.  I am passed the jQuery object containing all  of Gritter notifications.\n' + e);
            //    //    },
            //    //    after_close: function () {
            //    //        alert('I am called after everything has been closed.');
            //    //    }
            //    //});

            //    //$.gritter.remove(gritter_notification_id, {
            //    //    fade: true, // optional
            //    //    speed: 'fast' // optional
            //    //});
            //}, 7000);
        });
    }
})();
