(function () {
    'use strict';

    angular.module('app.shifts')
        .controller('shiftsListController', listController);

    listController.$inject = ['$scope', '$rootScope', '$state', '$localStorage', 'shiftsService', 'usersService', 'employeeGroupsService', '$log', 'uiCalendarConfig', '$compile', '$timeout'];

    function listController($scope, $rootScope, $state, $localStorage, service, usersService, employeeGroupsService, $log, uiCalendarConfig, $compile, $timeout) {

        var vm = $scope;

        vm.init = init;

        //vm.listUsers = listUsers;
        //vm.getUsers = getUsers;
        vm.getEmployeeGroups = getEmployeeGroups;

        vm.list = list;
        vm.createEvents = createEvents;
        vm.del = del;

        vm.fetch = fetch;

        vm.test = function () {

            console.log(uiCalendarConfig.calendars['shiftsCalendar']);

            uiCalendarConfig.calendars['shiftsCalendar'].fullCalendar('gotoDate', moment().add(12, 'days'));
            console.log('gotoDate: ', moment().add(12, 'days'));
        };

        vm.employeeGroups = [];
        vm.rota = {};
        vm.selectedEmployeeGroupId = null;

        //var startOfWeek = moment().startOf('week').toDate();
        //var endOfWeek = moment().endOf('week').toDate();
        var startOfWeek = moment().startOf('isoweek').format('YYYY-MM-DD'),
            endOfWeek = moment().endOf('isoweek').format('YYYY-MM-DD');

        //var calCurrentStart = startOfWeek,
        //    calCurrentEnd = startOfWeek,
        //    minDateFetched = startOfWeek,
        //    maxDateFetched = endOfWeek;

        vm.query = { includeOpenShifts: true, filterOnGroup: false, groupId: null, startDate: startOfWeek, endDate: endOfWeek, searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'Name', sortType: 1 };
        vm.usersQuery = { searchCriteria: '', pageNo: 1, pageSize: 10, sortColumn: 'Name', sortType: 1 };

        vm.dtOptions = {
            paginationType: 'full_numbers',
            //displayLength: 2,
            paging: false,
            ordering: true,
            info: false,
            searching: false
        };

        /* config object */
        vm.uiConfig = {
            calendar: {

                schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives',
                //schedulerLicenseKey: 'GPL-My-Project-Is-Open-Source',
                //schedulerLicenseKey: '0881607986-fcs-1481549340',

                //height: 1200,
                //contentHeight: 'auto',

                navLinks: true,

                defaultDate: moment(),
                firstDay: 1,
                editable: false,

                eventLimit: true, // allow "more" link when too many events

                //eventColor: '#00acac',
                //eventBorderColor: '',
                //eventTextColor: '',
                //eventBackgroundColor: '#00acac',  // template green
                //eventBackgroundColor: '#5a9d76',
                //eventBackgroundColor: '#6aa4c1',

                allDayDefault: false,

                defaultView: 'timelineWeek',
                //groupByResource: true,
                groupByDateAndResource: true,

                header: {
                    //left: '',
                    //left: 'title',
                    //left: 'timelineWeek timelineWeekHours timelineDay timelineWeek timelineMonth timelineYear month basicWeek basicDay agendaWeek agendaDay listYear listMonth listWeek listDay agendaTwoDay',
                    //left: 'timelineWeek timelineWeekHours timelineDay timelineDay2 timelineDay3 timelineDay4 month basicWeek basicDay agendaWeek agendaDay listYear listMonth listWeek listDay agendaTwoDay',
                    left: 'timelineWeek timelineWeekHours timelineDay timelineDay2 timelineDay3',
                    center: 'title',
                    right: 'today prev,next'
                },

                views: {
                    basic: {
                        // options apply to basicWeek and basicDay views
                    },
                    agenda: {
                        // options apply to agendaWeek and agendaDay views
                        groupByResource: true,
                        groupByDateAndResource: true
                    },
                    week: {
                        // options apply to basicWeek and agendaWeek views
                        groupByResource: true,
                        groupByDateAndResource: true
                    },
                    timelineWeek: {
                        type: 'timelineWeek',
                        buttonText: 'Week',
                        slotDuration: '24:00:00',
                        slotLabelFormat: [
                            //'MMMM YYYY WW', // top level of text
                            'ddd / DD'        // lower level of text
                        ]

                        //editable: false,
                        //startEditable: false,
                        //durationEditable: false
                    },
                    //timelineWeekHours: {
                    //    type: 'timeline',
                    //    buttonText: 'Week in Hours',

                    //    duration: { weeks: 1 },
                    //    slotDuration: '00:15:00',

                    //    //editable: true,
                    //    //startEditable: true,
                    //    //durationEditable: true,

                    //    // views that are more than a day will NOT do this behavior by default
                    //    // so, we need to explicitly enable it
                    //    groupByResource: true

                    //    //// uncomment this line to group by day FIRST with resources underneath
                    //    //groupByDateAndResource: true
                    //},
                    timelineDay: {
                        type: 'timeline',
                        buttonText: '1 Day',
                        slotDuration: '00:15:00'
                        //slotLabelFormat: [
                        //    //'MMMM YYYY WW', // top level of text
                        //    'ddd / DD'        // lower level of text
                        //],

                        //editable: false,
                        //startEditable: false,
                        //durationEditable: false
                    },
                    //day: {
                    //    // options apply to basicDay and agendaDay views
                    //},
                    timelineDay2: {
                        type: 'timeline',
                        // options apply to basicDay and agendaDay views
                        duration: { days: 2 },
                        buttonText: '2 day',

                        slotDuration: '00:15:00',
                        
                        // views that are more than a day will NOT do this behavior by default
                        // so, we need to explicitly enable it
                        groupByResource: true,

                        //// uncomment this line to group by day FIRST with resources underneath
                        groupByDateAndResource: true
                    },
                    timelineDay3: {
                        type: 'timeline',
                        // options apply to basicDay and agendaDay views
                        duration: { days: 3 },
                        buttonText: '3 day',

                        slotDuration: '00:15:00',

                        // views that are more than a day will NOT do this behavior by default
                        // so, we need to explicitly enable it
                        groupByResource: true,

                        //// uncomment this line to group by day FIRST with resources underneath
                        groupByDateAndResource: true
                    },
                    //timelineDay4: {
                    //    type: 'timeline',
                    //    // options apply to basicDay and agendaDay views
                    //    duration: { days: 4 },
                    //    buttonText: '4 day',

                    //    slotDuration: '00:15:00',

                    //    // views that are more than a day will NOT do this behavior by default
                    //    // so, we need to explicitly enable it
                    //    groupByResource: true,

                    //    //// uncomment this line to group by day FIRST with resources underneath
                    //    groupByDateAndResource: true
                    //},
                    month: {
                        groupByResource: false,
                        groupByDateAndResource: false,
                        titleFormat: 'YYYY, MM, DD',
                        slotLabelFormat: [
                            //'MMMM YYYY WW', // top level of text
                            'ddd / DD'        // lower level of text
                        ]
                    },
                    agendaTwoDay: {
                        type: 'agenda',
                        duration: { days: 2 },
                        buttonText: 'Ag 2d',
                        // views that are more than a day will NOT do this behavior by default
                        // so, we need to explicitly enable it
                        groupByResource: true,

                        //// uncomment this line to group by day FIRST with resources underneath
                        groupByDateAndResource: true
                    }
                },

                resources: [],
                events: [],

                navLinkDayClick: navLinkDayClicked,

                viewRender: viewRender,

                dayRender: dayRender,
                dayClick: dayClicked,

                eventRender: eventRender,
                eventAfterRender: eventAfterRender,

                eventMouseover: eventMouseover,
                eventMouseout: eventMouseout,

                eventClick: eventClicked,

                eventDragStart: eventDragStart,
                eventDragStop: eventDragStop,
                eventDrop: eventDropped,

                eventResizeStart: eventResizedStart,
                eventResizeStop: eventResizedStop,
                eventResize: eventResized
            }
        };
        
        //var date = startOfWeek;//new Date();
        //var d = date.getDate();
        //var m = date.getMonth();
        //var y = date.getFullYear();

        ///* event source that pulls from google.com */
        //vm.eventSource = {
        //    url: 'http://www.google.com/calendar/feeds/usa__en%40holiday.calendar.google.com/public/basic',
        //    className: 'gcal-event',           // an option!
        //    currentTimezone: 'America/Chicago' // an option!
        //};

        ///* event source that calls a function on every view switch */
        //vm.eventsF = function (start, end, timezone, callback) {
        //    $log.log('Start', start.format('DD-MM-YY'), 'End', end.format('DD-MM-YY'));
        //    var s = new Date(start).getTime() / 1000;
        //    var e = new Date(end).getTime() / 1000;
        //    var m = new Date(start).getMonth();
        //    //var events = [{ title: 'Feed Me ' + m, start: s + (50000), end: s + (100000), allDay: false, className: ['customFeed'] }];

        //    var startMoment = moment(start);
        //    var endMoment = moment(end).subtract(1, 'days');
        //    var events = [
        //        { title: startMoment.format('DD-MM-YY'), start: startMoment, end: startMoment, allDay: false, className: ['customFeed'] },
        //        { title: endMoment.format('DD-MM-YY'), start: endMoment, end: endMoment, allDay: false, className: ['customFeed'] }
        //    ];
            
        //    callback(events);
        //    //callback(vm.events);
        //};

        /* events array*/
        vm.events = [];

        /* event sources array*/
        //vm.eventSources = [vm.events, vm.eventSource, vm.eventsF];
        //vm.eventSources = [vm.eventsF];
        vm.eventSources = [vm.events];

        //vm.eventSources = {
        //    events: [
        //        {
        //            title: 'Event1',
        //            start: '2016-12-05'
        //        },
        //        {
        //            title: 'Event2',
        //            start: '2016-12-10'
        //        }
        //        // etc...
        //    ],
        //    color: 'yellow',   // an option!
        //    textColor: 'black' // an option!
        //};

        //var paths = [];
        function dayRender(date, cell) {
            cell.addClass('cal-day');
            //console.log(date, cell);
            //console.log(cell.getPath(), cell.attr('data-date'));
            //paths.push(cell.getPath());

            //cell.bind('dblclick', function () {
            //    cell.text('lksjdfla');
            //    cell.attr('bgcolor', 'red');
            //    alert('double click: ' + date.format());
            //});

            //setupDayMenu(cell.getPath());
        }

        /* Render Tooltip */
        function eventRender(event, element, view) {

            element.bind('dblclick', function () {
                //alert('double click: ' + event.title);
                $state.go('app.shifts.view', { id: event.id });
            });

            var attrs = {
                'tooltip': event.title,
                'tooltip-append-to-body': true
            };

            if (event.shift && event.shift.notes) {
                attrs.title = event.shift.title;

                if (event.shift.startTime)
                    attrs.title = attrs.title + '\nDate: ' + moment(event.shift.startTime).format('MMMM DD, YYYY');

                if (event.shift.startTime && event.shift.endTime)
                    attrs.title = attrs.title + '\nDuration: ' + moment(event.shift.startTime).format('hh:mm A') + ' - ' + moment(event.shift.endTime).format('hh:mm A');

                if (event.shift.statusDesc)
                    attrs.title = attrs.title + '\nStatus: ' + event.shift.statusDesc;

                if (event.shift.clockInTime)
                    attrs.title = attrs.title + '\nClock-In Time: ' + moment(event.shift.clockInTime).format('hh:mm A');

                if (event.shift.clockOutTime)
                    attrs.title = attrs.title + ', Clock-Out Time: ' + moment(event.shift.clockOutTime).format('hh:mm A');

                //if (event.shift.notes)
                //    attrs.title = attrs.title + '\n' + event.shift.notes;

            } else {
                attrs.title = event.title;
            }

            element.attr(attrs);
            $compile(element)(vm);

            //var path = element.getPath();
            //console.debug(event, element, view, path);
            //setupEventMenu(path);
        }

        function eventAfterRender(event, element, view) {

            //console.debug(element.parent());
            //var path = element.getPath();
            //console.debug(event, element, view, path);
            //setupEventMenu(path);
        }

        function eventMouseover(event, jsEvent, view) { }

        function eventMouseout(event, jsEvent, view) { }

        function eventDragStart(event, jsEvent, ui, view) { }
        function eventDragStop(event, jsEvent, ui, view) { }

        /* alert on Drop */
        function eventDropped(event, delta, revertFunc, jsEvent, ui, view) {

            $log.log('Event Dropped to make dayDelta ' + delta);
            //console.log('Event: ', event);
            console.log('ResourceId: ', event.resourceId);

            //console.debug(event, delta, revertFunc, jsEvent, ui, view);
            
            //console.debug(delta.days(), delta.hours(), delta.minutes(), delta.seconds(), delta.milliseconds());
            //console.debug(delta._months, delta._days, delta._milliseconds);

            //console.debug(event.resourceId, event.start.format(), event.end.format());

            assignShift(event, revertFunc);
        }

        function assignShift(event, revertFunc) {

            console.debug(event.start.utc().toISOString(), event.end.utc().toISOString());
            if (event.start.isBefore(moment())) {
                revertFunc();
                console.debug(event.start.utc().toISOString(), event.end.utc().toISOString());
                $rootScope.notifications.error('Cannot update event and assign to a back date.');
                return;
            }

            //var dto = { start: event.start.toISOString(), end: event.end.toISOString() };
            var dto = { start: event.start.format('YYYY-MM-DD HH:mm'), end: event.end.format('YYYY-MM-DD HH:mm') };
            console.debug(dto.start, dto.end);
            if (event.resourceId) {
                if (event.resourceId === 'open') {
                    dto.isOpen = true;
                } else {
                    dto.isOpen = false;
                    dto.employeeId = event.resourceId;
                }
            } else {
                dto.isOpen = true;
            }

            service.assign(event.id, dto).then(function (data) {
                $rootScope.notifications.message('Event updated successfully.');
                
                event.shift.startTime = event.start;
                event.shift.endTime = event.end;

                if (dto.isOpen) {
                    event.className = ['shift-event', 'open-shift-event'];
                } else {
                    event.className = ['shift-event', 'allocated-shift-event'];
                }

                vm.calendar.renderEvent(event);
                //vm.calendar.updateEvent(event);
                //vm.calendar.rerenderEvents();
                
                
            }, function (data) {
                revertFunc();
            });
        }

        function eventResizedStart(event, jsEvent, ui, view) { }
        function eventResizedStop(event, jsEvent, ui, view) { }

        /* alert on Resize */
        function eventResized(event, delta, revertFunc, jsEvent, ui, view) {
            $log.log('Event Resized to make dayDelta ' + delta);
            console.debug(event, delta, revertFunc, jsEvent, ui, view);

            console.debug(delta.days(), delta.hours(), delta.minutes(), delta.seconds(), delta.milliseconds());
            console.debug(delta._months, delta._days, delta._milliseconds);

            console.debug(event.start.format(), event.end.format());

            assignShift(event, revertFunc);
        }

        /* alert on eventClick */
        function eventClicked(event, jsEvent, view) {

            $log.log('Event: ' + event.title);
            $log.log('Coordinates: ' + jsEvent.pageX + ',' + jsEvent.pageY);
            $log.log('View: ' + view.name);

            var element = $(this);
            // change the border color just for fun
            element.css('background-color', '#00acac');

            var cords = { x: jsEvent.pageX, y: jsEvent.pageY };
            //$log.log('Coordinates: ', cords);
            //var path = element.getPath();
            //openEventMenu(cords, event, element);
            openCalendarMenu(cords, { event: event });
        }

        var calViewStart = null;
        var calViewEnd = null;

        function viewRender(view, element) {

            //setupEventMenu();

            calViewStart = view.start;
            calViewEnd = view.end;

            console.log('View Changed: ', view.start.format(), view.end.format(), view);

            if (!moment(vm.query.startDate).diff(view.start, 'days')) return;

            console.log('Date Changed: ', view.start.format(), view.end.format());

            selectedDayBgEvent = null;

            //vm.query.startDate = view.start.toDate();
            //vm.query.endDate = view.end.toDate();
            vm.query.startDate = moment.utc(view.start).format('YYYY-MM-DD');
            vm.query.endDate = moment.utc(view.end).format('YYYY-MM-DD');

            //calCurrentStart = view.start;
            //calCurrentEnd = view.end;

            //var fetch = false;
            //if (!minDateFetched || view.start.isBefore(minDateFetched)) {
            //    minDateFetched = view.start;
            //    fetch = true;
            //}

            //if (!maxDateFetched || view.end.isAfter(maxDateFetched)) {
            //    maxDateFetched = view.end;
            //    fetch = true;
            //}

            //console.log('Fetch: ', fetch, view.start.format(), view.end.format());

            //setupBootstrapMenu();
                        
            //if (fetch)
            list();
        }

        function navLinkDayClicked(date, jsEvent) {
            console.log('day', date.format()); // date is a moment

            var cords = { x: jsEvent.pageX, y: jsEvent.pageY };

            var groupId = null;
            if (vm.selectedEmployeeGroupId && vm.selectedEmployeeGroupId !== 'none') {
                groupId = vm.selectedEmployeeGroupId;
            }

            openCalendarMenu(cords, { date: date, resource: groupId });
        }

        function dayClicked(date, jsEvent, view, resource) {

            //$log.log('Current view: ' + view.name);
            //$log.log('Coordinates: ' + jsEvent.pageX + ',' + jsEvent.pageY);
            $log.log('Clicked on Date: ' + date.format());
            if (resource)
                $log.log('Clicked on: ' + resource.id + ', ' + resource.title);

            // change the day's background color just for fun
            //$(this).css('background-color', 'red');
            //$('.selected-day-bg-event').removeClass('selected-day-bg-event');
            //$(this).addClass('selected-day-bg-event');

            createSelectedDayBgEvent(date, resource);

            var cords = { x: jsEvent.pageX, y: jsEvent.pageY };
            //$log.log('Coordinates: ', cords);
            //var path = $(this).getPath();
            //openDayMenu(cords, date, resource, path);
            openCalendarMenu(cords, { date: date, resource: resource });
        }

        var selectedDayBgEventId = '0000';
        var selectedDayBgEvent = null;
        function createSelectedDayBgEvent(date, resource) {

            // if reference to the selected event does not exist
            if (selectedDayBgEvent === null) {
                // find events with id for selected day bg event
                var foundEvents = vm.calendar.getEvents(selectedDayBgEventId);
                // if event is found
                if (foundEvents && foundEvents.length > 0) {
                    // get it's reference.
                    selectedDayBgEvent = foundEvents[0];
                }
            }

            // if selected event does not exist
            if (selectedDayBgEvent === null) {

                // create and render the event
                var bgEvent = {
                    id: selectedDayBgEventId,
                    start: date, //'2016-12-24'
                    end: date, //'2016-12-25'
                    //overlap: false,
                    rendering: 'background',
                    className: 'selected-day-bg-event'
                };

                bgEvent.resourceId = resource ? resource.id : null;
                //vm.events.push(bgEvent);
                vm.calendar.renderEvent(bgEvent);
            } else {
                // update event data and call the calendar's update method to re-render on the new day.
                selectedDayBgEvent.start = date;
                selectedDayBgEvent.end = date;
                selectedDayBgEvent.resourceId = resource ? resource.id : null;

                vm.calendar.updateEvent(selectedDayBgEvent);
            }
        }

        function setupCalendarMenu() {

            // setup context menu
            $.contextMenu({
                selector: '.calendar',
                //trigger: 'left',
                trigger: 'none',
                autoHide: true,
                events: {
                    show: function (options) {

                        //console.debug('Open event menu with selector ' + options.selector + '?');
                        return true;

                        // Add class to the menu
                        //this.addClass('currently-showing-menu');
                    },
                    hide: function (options) {

                        //console.debug('Hide event menu with selector ' + options.selector + '?');
                        return true;
                    }
                },
                build: function ($triggerElement, e) {

                    e.preventDefault();

                    //console.debug($triggerElement, e);

                    // pull a callback from the trigger
                    return $triggerElement.data('callbackForCreatingCalendarMenu')();
                }
            });
        }

        function openCalendarMenu(cords, triggerData) {

            //cords, date, resource, event
            //console.log('Open Calendar: ', triggerData);

            if (!triggerData) return;

            // some asynchronous click handler
            var $this = $('.calendar');

            var createCalendarMenuCallback = null;            
            if (triggerData.event) {
                createCalendarMenuCallback = getCreateEventMenuCallbackFunction(triggerData.event);
            } else if (triggerData.date) {

                if (!vm.canManage) return;

                createCalendarMenuCallback = getCreateDayMenuCallbackFunction(triggerData.date, triggerData.resource);
            }

            // store a callback on the trigger
            $this.data('callbackForCreatingCalendarMenu', createCalendarMenuCallback);

            //var _offset = $this.offset(),
            //    position = { x: _offset.left + 10, y: _offset.top + 10 };

            // open the contextMenu asynchronously
            setTimeout(function () { $this.contextMenu(cords); });
        }

        function getCreateDayMenuCallbackFunction(date, resource) {

            function createDayMenu() {
                return {
                    callback: function (key, options) {
                        console.log(key, options);
                    },
                    items: {
                        "CreateShift": {
                            name: "Create Shift",
                            icon: "fa-plus",
                            visible: function (key, opt) {
                                return vm.canManage;
                            },
                            callback: function (key, options) {
                                console.log(key, options);
                                console.log('Date: ', date, 'Resource: ', resource);
                                $state.go('app.shifts.create', { date: date, employeeId: resource.id });
                            }
                        }
                        //"cut": { name: "Cut", icon: "fa-lock" },
                        //"copy": { name: "Copy", icon: "fa-unlock" }
                    }
                };
            }

            return createDayMenu;
        }

        function getCreateEventMenuCallbackFunction(event, resource) {

            function createEventMenu() {
                return {
                    callback: function (key, opt) {
                        console.log('Event Menu Item Clicked', key, opt);
                    },
                    items: {
                        view: {
                            type: null, //type: null, undefined, text, textarea, checkbox, radio, select, html default: null
                            name: "View",
                            disabled: function (key, opt) {
                                // Disable this item if the menu was triggered on a div
                                //if (opt.$trigger.nodeName === 'div')
                                return false;
                            },
                            visible: function (key, opt) {
                                // Hide this item if the menu was triggered on a div
                                //if (opt.$trigger.nodeName === 'div')
                                return true;
                            },
                            callback: function (key, opt) {
                                //console.log(event);
                                $state.go('app.shifts.view', { id: event.id });
                            }
                        },
                        update: {
                            type: null, //type: null, undefined, text, textarea, checkbox, radio, select, html default: null
                            name: "Update",
                            disabled: function (key, opt) {
                                return event.shift.clockInTime;
                            },
                            visible: function (key, opt) {
                                return event.shift
                                    && vm.canManage
                                    && (event.shift.isOpen === true
                                    || event.shift.statusDesc === 'Due');
                            },
                            callback: function (key, opt) {
                                //console.log(event);
                                $state.go('app.shifts.update', { id: event.id });
                            }
                        },
                        deleteShift: {
                            type: null, //type: null, undefined, text, textarea, checkbox, radio, select, html default: null
                            name: "Delete",
                            disabled: function (key, opt) {
                                return event.shift.clockInTime;
                            },
                            visible: function (key, opt) {
                                return event.shift
                                    && vm.canManage
                                    && event.shift.statusDesc === 'Due';
                            },
                            callback: function (key, opt) {
                                service.del(event.shift.id).then(function (data) {
                                    vm.calendar.removeEvents(event.id);
                                }, function (error) { });
                            }
                        }
                        //select: {
                        //    name: "Select",
                        //    type: 'select',
                        //    options: { 1: 'one', 2: 'two', 3: 'three' },
                        //    selected: "2",
                        //    visible: function (key, opt) {
                        //        //return true;
                        //        if (event.shift) {
                        //            console.log('Is Open: ', event.shift.isOpen);
                        //        }
                        //        return event.shift && event.shift.isOpen == true;
                        //    },
                        //    callback: function (key, opt) {
                        //        console.log('Select Event Menu Item Clicked', key, opt);
                        //        console.log(event);
                        //    }
                        //}
                    }
                };
            }

            return createEventMenu;
        }

        //function setupDayMenu(selector) {

        //    if (!selector) selector = '.cal-day';

        //    // setup context menu
        //    $.contextMenu({
        //        //selector: '.calendar',
        //        selector: selector,
        //        trigger: 'none',
        //        autoHide: true,
        //        events: {
        //            show: function (options) {

        //                console.debug('Open day menu with selector ' + options.selector + '?');
        //                return true;

        //                // Add class to the menu
        //                //this.addClass('currently-showing-menu');

        //                // Show an alert with the selector of the menu
        //                if (confirm('Open day menu with selector ' + options.selector + '?') === true) {
        //                    return true;
        //                } else {
        //                    // Prevent the menu to be shown.
        //                    return false;
        //                }
        //            },
        //            hide: function (options) {
        //                console.debug('Hide day menu with selector ' + options.selector + '?');
        //                return true;

        //                if (confirm('Hide day menu with selector ' + options.selector + '?') === true) {
        //                    return true;
        //                } else {
        //                    // Prevent the menu to be hidden.
        //                    return false;
        //                }
        //            }
        //        },
        //        build: function ($trigger, e) {
        //            e.preventDefault();

        //            // pull a callback from the trigger
        //            return $trigger.data('callbackForCreatingDayMenu')();
        //        }
        //    });
        //}

        //function openDayMenu(cords, date, resource, selector) {

        //    // some asynchronous click handler
        //    //var $this = $('.calendar');
        //    var $this = $(selector);

        //    var createDayMenuCallback = getCreateDayMenuCallbackFunction(date, resource);
        //    // store a callback on the trigger
        //    $this.data('callbackForCreatingDayMenu', createDayMenuCallback);

        //    //var _offset = $this.offset(),
        //    //    position = { x: _offset.left + 10, y: _offset.top + 10 };

        //    // open the contextMenu asynchronously
        //    setTimeout(function () { $this.contextMenu(cords); });
        //    return;

        //    //// setup context menu
        //    //$.contextMenu({
        //    //    //selector: '.shift-event',
        //    //    selector: selector,
        //    //    //selector: '.calendar',
        //    //    //trigger: 'hover',
        //    //    autoHide: true,
        //    //    build: function ($triggerElement, e) {
        //    //        return {
        //    //            callback: function () { },
        //    //            items: {
        //    //                "CreateShift": {
        //    //                    name: "Create Shift",
        //    //                    icon: "fa-plus",
        //    //                    callback: function (key, options) {
        //    //                        console.log(key, options);
        //    //                    }
        //    //                },
        //    //                "cut": { name: "Cut", icon: "fa-lock" },
        //    //                "copy": { name: "Copy", icon: "fa-unlock" }
        //    //            }
        //    //        };
        //    //    }
        //    //});
        //}

        //function setupEventMenu(selector) {

        //    //console.log('Menu for: ', selector, event);
        //    if (!selector) { selector = '.shift-event'; }

        //    // setup context menu
        //    $.contextMenu({
        //        selector: selector, //'.shift-event'
        //        //trigger: 'left',
        //        trigger: 'none',
        //        autoHide: true,
        //        events: {
        //            show: function (options) {

        //                console.debug('Open event menu with selector ' + options.selector + '?');
        //                return true;

        //                // Add class to the menu
        //                //this.addClass('currently-showing-menu');

        //                // Show an alert with the selector of the menu
        //                if (confirm('Open event menu with selector ' + options.selector + '?') === true) {
        //                    return true;
        //                } else {
        //                    // Prevent the menu to be shown.
        //                    return false;
        //                }
        //            },
        //            hide: function (options) {

        //                console.debug('Hide event menu with selector ' + options.selector + '?');
        //                return true;

        //                if (confirm('Hide event menu with selector ' + options.selector + '?') === true) {
        //                    return true;
        //                } else {
        //                    // Prevent the menu to be hidden.
        //                    return false;
        //                }
        //            }
        //        },
        //        build: function ($triggerElement, e) {

        //            console.debug($triggerElement, e);

        //            e.preventDefault();

        //            // pull a callback from the trigger
        //            return $triggerElement.data('callbackForCreatingEventMenu')();
        //        }
        //    });
        //}

        //function openEventMenu(cords, event, element) {

        //    // some asynchronous click handler
        //    //var $this = $('.calendar');
        //    //var $this = $(selector);
        //    var $this = element;

        //    var createEventMenuCallback = getCreateEventMenuCallbackFunction(event);
        //    // store a callback on the trigger
        //    $this.data('callbackForCreatingEventMenu', createEventMenuCallback);

        //    //var _offset = $this.offset(),
        //    //    position = { x: _offset.left + 10, y: _offset.top + 10 };

        //    // open the contextMenu asynchronously
        //    setTimeout(function () { $this.contextMenu(cords); });
        //}

        //function setupBootstrapMenu() {
            
        //    console.log('Setting up cal day menu');
        //    var menu = new BootstrapMenu('.open-shift-event', {
        //        /* $elem is the jQuery element where the menu was opened. The
        //         * returned value is the `row` argument passed to each function. */
        //        fetchElementData: function ($elem) {
        //            //var id = $elem.data('id');
        //            //return tableRows[id];
        //            console.log('Date Right-Clicked: ' + $elem);
        //        },

        //        /* group actions by their id to make use of separators between
        //         * them in the context menu. Actions not added to any group with
        //         * this option will appear in a default group of their own. */
        //        actionsGroups: [['setEditable', 'setUneditable'], ['deleteRow']],

        //        /* you can declare 'actions' as an object instead of an array,
        //         * and its keys will be used as action ids. */
        //        actions: {
        //            createShift: {
        //                name: 'Create Shift',
        //                iconClass: 'fa-plus',
        //                onClick: function (data) {
        //                    console.log('onClick: ' + data);
        //                },
        //                isEnabled: function (data) {
        //                    return true;
        //                }
        //            },
        //            editDescription: {
        //                name: 'Edit description',
        //                iconClass: 'fa-pencil',
        //                onClick: function (row) { /* ... */ },
        //                isEnabled: function (row) {
        //                    return true;//return row.isEditable;
        //                }
        //            },
        //            setEditable: {
        //                name: 'Set editable',
        //                iconClass: 'fa-unlock',
        //                onClick: function (row) { /* ... */ },
        //                isShown: function (row) {
        //                    return true;//return !row.isEditable;
        //                }
        //            },
        //            setUneditable: {
        //                name: 'Set uneditable',
        //                iconClass: 'fa-lock',
        //                onClick: function (row) { /* ... */ },
        //                isShown: function (row) {
        //                    return true;//return row.isEditable;
        //                }
        //            },
        //            deleteRow: {
        //                name: 'Delete row',
        //                iconClass: 'fa-trash-o',
        //                onClick: function (row) { /* ... */ },
        //                isEnabled: function (row) {
        //                    return true;//return row.isEditable && row.isRemovable;
        //                }
        //            }
        //        }
        //    });

        //    var dayMenu = new BootstrapMenu('.cal-day-bg-event', {
        //        actions: [{
        //            name: 'Action',
        //            onClick: function () {
        //                // run when the action is clicked
        //            }
        //        }, {
        //            name: 'Another action',
        //            onClick: function () {
        //                // run when the action is clicked
        //            }
        //        }, {
        //            name: 'A third action',
        //            onClick: function () {
        //                // run when the action is clicked
        //            }
        //        }]
        //    });

        //    //var menu = new BootstrapMenu('#context-menu-button', {
        //    //    actions: [{
        //    //        name: 'Action',
        //    //        onClick: function () {
        //    //            // run when the action is clicked
        //    //        }
        //    //    }, {
        //    //        name: 'Another action',
        //    //        onClick: function () {
        //    //            // run when the action is clicked
        //    //        }
        //    //    }, {
        //    //        name: 'A third action',
        //    //        onClick: function () {
        //    //            // run when the action is clicked
        //    //        }
        //    //    }]
        //    //});
        //}

        function fetch() {

            if (!vm.selectedEmployeeGroupId) {
                vm.query.filterOnGroup = false;
                vm.query.groupId = "";
            } else if (vm.selectedEmployeeGroupId === 'none') {
                vm.query.filterOnGroup = true;
                vm.query.groupId = null;
            } else {
                vm.query.filterOnGroup = true;
                vm.query.groupId = vm.selectedEmployeeGroupId;
            }
            vm.query.includeOpen = true;
            $timeout(function () {
                removeEvents();
                removeResources();

                //console.log('Cal GoTo:', calCurrentStart.format());
                //uiCalendarConfig.calendars['shiftsCalendar'].fullCalendar('gotoDate', calCurrentStart);

                list();
            });
        }

        ///* add custom event*/
        //vm.addEvent = function () {
        //    vm.events.push({
        //        title: 'Open Sesame',
        //        start: new Date(y, m, 28),
        //        end: new Date(y, m, 29),
        //        className: ['openSesame']
        //    });
        //};

        ///* add and removes an event source of choice */
        //vm.addRemoveEventSource = function (sources, source) {
        //    var canAdd = 0;
        //    angular.forEach(sources, function (value, key) {
        //        if (sources[key] === source) {
        //            sources.splice(key, 1);
        //            canAdd = 1;
        //        }
        //    });
        //    if (canAdd === 0) {
        //        sources.push(source);
        //    }
        //};


        /* remove event */
        function removeEvent(index) {
            vm.events.splice(index, 1);
        }

        /* remove events */
        function removeEvents() {

            //vm.uiConfig.calendar.events = [];
            vm.uiConfig.calendar.events.length = 0;

            //vm.events = [];   // reassigning new empty array makes it unbound to the calendar control
            //vm.events.length = 0;
            //vm.events.splice(0, vm.events.length);

            vm.eventSources.length = 0;
            vm.events = [];
            vm.eventSources.push(vm.events);

            //minDateFetched = null;
            //maxDateFetched = null;
        }

        /* remove resources */
        function removeResources() {

            //vm.uiConfig.calendar.resources = [];
            //vm.uiConfig.calendar.resources.length = 0;
            vm.uiConfig.calendar.resources.splice(0, vm.uiConfig.calendar.resources.length);
            vm.calendar.removeResources();
        }

        ///* Change View */
        //function changeView() {
        //    uiCalendarConfig.calendars['shiftsCalendar'].fullCalendar('changeView', '');
        //}

        ///* Change View */
        //function renderCalendar() {
        //    $timeout(function () {
        //        if (uiCalendarConfig.calendars['shiftsCalendar']) {
        //            uiCalendarConfig.calendars['shiftsCalendar'].fullCalendar('render');
        //        }
        //    });
        //}

        ////////////////////////////////////////////////////////////////////////
        
        //vm.calEventsExt = {
        //    color: '#f00',
        //    textColor: 'yellow',
        //    events: [
        //       { type: 'party', title: 'Lunch', start: new Date(y, m, d, 12, 0), end: new Date(y, m, d, 14, 0), allDay: false },
        //       { type: 'party', title: 'Lunch 2', start: new Date(y, m, d, 12, 0), end: new Date(y, m, d, 14, 0), allDay: false },
        //       { type: 'party', title: 'Click for Google', start: new Date(y, m, 28), end: new Date(y, m, 29), url: 'http://google.com/' }
        //    ]
        //};

        //vm.eventSources2 = [vm.calEventsExt, vm.eventsF, vm.events];

        ////////////////////////////////////////////////////////////////////////

        vm.init();

        function init() {

            extendJquery();

            vm.canManage = $rootScope.User.hasAnyRole('RecManager, HRManager');
            
            vm.calendar = new Calendar('#calendar', vm.uiConfig.calendar);
            vm.calendar.init();
                                    
            //console.log('Calendars: ', uiCalendarConfig.calendars.length);
            //uiCalendarConfig.calendars['shiftsCalendar'].fullCalendar('gotoDate', moment().add(2, 'days'));

            //vm.listUsers();

            getEmployeeGroups();

            list();

            //setupBootstrapMenu();
            
            //setupDayMenu();
        }

        function extendJquery() {

            // http://stackoverflow.com/a/5708130/3423802
            jQuery.fn.extend({
                // get unique path of the element to use as jQuery selector
                getPath: function () {
                    var path, node = this;
                    while (node.length) {
                        var realNode = node[0], name = realNode.localName;
                        if (!name) break;
                        name = name.toLowerCase();

                        var parent = node.parent();

                        var sameTagSiblings = parent.children(name);
                        if (sameTagSiblings.length > 1) {
                            var allSiblings = parent.children();
                            var index = allSiblings.index(realNode) + 1;
                            if (index > 1) {
                                name += ':nth-child(' + index + ')';
                            }
                        }

                        path = name + (path ? '>' + path : '');
                        node = parent;
                    }

                    return path;
                }
            });
        }

        function list() {

            //console.log('Query: ', vm.query);
            service.list(vm.query).then(function (data) {

                vm.rota = data;

                createEvents();

                //vm.calendar.setHeight(1200);
                //vm.calendar.setContentHeight(1200);
            }, function (error) { });
        }

        function createResources() {

            //vm.uiConfig.calendar.resources = [];
            if (vm.uiConfig.calendar.resources.length > 0) return;

            if (vm.query.includeOpenShifts) {
                var openResource = { id: 'open', title: 'Open' };
                vm.uiConfig.calendar.resources.push(openResource);
                vm.calendar.addResource(openResource, false);
            }

            if (!vm.rota) return;

            angular.forEach(vm.rota.items, function (rotaItem) {
                var resource = {
                    id: rotaItem.employee.id,
                    title: rotaItem.employee.name,
                    empployee: rotaItem.employee
                };
                vm.uiConfig.calendar.resources.push(resource);
                vm.calendar.addResource(resource, false);
            });
        }
        
        function createEvents() {

            if (!vm.rota) return;

            removeResources();
            removeEvents();
            vm.calendar.removeEvents();

            createResources();
            
            angular.forEach(vm.rota.openShifts, function (shift) {
                console.debug(shift.startTime, shift.endTime);
                console.debug(moment(shift.startTime).toISOString(), moment(shift.endTime).toISOString());
                console.debug(moment(shift.startTime).utc().toISOString(), moment(shift.endTime).utc().toISOString());
                createEvent(shift);
            });

            angular.forEach(vm.rota.items, function (rotaItem) {
                angular.forEach(rotaItem.shifts, function (shift) {
                    console.debug(shift.startTime, shift.endTime);
                    console.debug(moment(shift.startTime).toISOString(), moment(shift.endTime).toISOString());
                    console.debug(moment(shift.startTime).utc().toISOString(), moment(shift.endTime).utc().toISOString());
                    createEvent(shift, rotaItem.employee);
                });
            });

            //vm.calendar.destroy();
            //vm.calendar.init();
            
            vm.calendar.renderEvents(vm.events, true);
            //vm.calendar.renderEvents(vm.uiConfig.calendar.events, true);

            //vm.calendar.addEventSource(vm.events);

            //vm.calendar.render();
            //vm.calendar.rerenderEvents();

            //vm.eventSources = [vm.events];

            //createDayBackgroundEvents();

            //setupBootstrapMenu();

            $.contextMenu('destroy');

            //setupDayMenu();
            //setupEventMenu();
            setupCalendarMenu();
        }

        function createEvent(shift, emp) {

            if (_.findWhere(vm.events, { 'id': shift.id })) return;

            var event = {
                stick: true, // not having this will make the event disappear upon changing view or date.
                id: shift.id,
                title: shift.title,
                start: moment(shift.startTime).toDate(),
                end: moment(shift.endTime).toDate(),
                description: 'my test event',
                allDay: false,
                //url: '',

                //color: '',
                //backgroundColor: '',
                //borderColor: '',
                //textColor: '',
                //className: '',

                //editable: true,
                //startEditable: false,
                //durationEditable: false,

                shift: shift
            };

            if (shift.isOpen) {
                event.resourceId = 'open';
            } else if (emp && emp.id) {
                event.resourceId = emp.id;
            }

            event.className = ['shift-event'];

            if (shift.statusDesc === 'Complete') {
                if (shift.startStatusDesc === 'Late') {
                    event.className.push('late-shift-event');
                } else {
                    event.className.push('complete-shift-event');
                }
            } else if (shift.statusDesc === 'Missed') {
                event.className.push('missed-shift-event');
            } else {

                if ($rootScope.User.hasAnyRole('HRManager')) {
                    event.editable = true;
                    //event.startEditable = false;
                    //event.durationEditable = false;
                }

                if (shift.isOpen) {

                    event.resourceId = 'open';
                    event.className.push('open-shift-event');

                } else if (emp && emp.id) {

                    event.resourceId = emp.id;
                    event.className.push('allocated-shift-event');
                }
            }

            vm.events.push(event);
            //vm.uiConfig.calendar.events.push(event);
            //renderEvent(event, true);
        }

        var dayBgEvent = null;
        function createDayBackgroundEvents() {

            var index = vm.events.indexOf(dayBgEvent);
            if (index > -1) {
                console.log('Removing day bg event.');
                vm.events.splice(index, 1);
            }

            console.log('Adding day bg event.');
            var bgEvent = {
                start: calViewStart,
                end: calViewEnd,
                overlap: false,
                rendering: 'background',
                //color: '#ff9f89',
                className: 'cal-day-bg-event'
            };
            dayBgEvent = bgEvent;
            vm.events.push(bgEvent);
        }

        //function getUsers(criteria) {
        //    vm.usersQuery.searchCriteria = criteria;
        //    vm.listUsers();
        //}

        //function listUsers() {
        //    usersService.list(vm.usersQuery).then(function (data) {
        //        vm.users = data.items;
        //        console.log('Users', vm.users);
        //    }, function (error) { });
        //}

        function getEmployeeGroups() {

            vm.selectedEmployeeGroupId = null;
            if ($rootScope.User.hasAnyRole('Employee')) return;
            
            employeeGroupsService.list().then(function (data) {
                vm.employeeGroups = data.items;
            }, function (error) { });
        }

        function del(id) {
            service.del(id).then(function (data) { }, function (error) { });
        }

        function pageSizeChanged() {
            vm.query.pageNo = 1;
            list();
        }

        function pageChanged() {
            list();
        }
    }

    function Calendar(selector, config) {

        var me = this;
        me.init = initCalendar;
        me.destroy = destroy;

        me.setHeight = setHeight;
        me.setContentHeight = setContentHeight;

        me.addResource = addResource;

        me.addEventSource = addEventSource;

        me.render = render;
        me.renderEvent = renderEvent;
        me.renderEvents = renderEvents;
        me.rerenderEvents = rerenderEvents;
        
        me.getEvent = getEvent;
        me.getEvents = getEvents;
        me.updateEvent = updateEvent;
        me.removeEvents = removeEvents;

        me.getResources = getResources;
        me.removeResources = removeResources;

        init();

        function init() {
            me.calendar = $(selector);//'#calendar'
            if (!me.calendar) throw new Error('Calendar selector is not valid.');

            me.config = config;
            if (!me.config) throw new Error('Calendar config is not valid.');
        }

        function initCalendar() {
            me.calendar.fullCalendar(me.config);
        }

        function destroy() {
            me.calendar.fullCalendar('destroy');
        }

        function addResource(resource, scroll) {
            me.calendar.fullCalendar('addResource', resource, scroll);
        }

        function addEventSource(events) {
            me.calendar.fullCalendar('addEventSource', events);
        }

        function renderEvent(event, stick) {
            if (stick === undefined || stick === null) stick = true;
            me.calendar.fullCalendar('renderEvent', event, stick);
        }

        function renderEvents(events, stick) {
            if (stick === undefined || stick === null) stick = true;
            me.calendar.fullCalendar('renderEvents', events, stick);
        }

        function rerenderEvents() {
            me.calendar.fullCalendar('rerenderEvents');
        }

        function updateEvent(event) {
            me.calendar.fullCalendar('updateEvent', event);
        }

        function render() {
            me.calendar.fullCalendar('render');
        }

        function removeEvents(idOrFilter) {
            me.calendar.fullCalendar('removeEvents', idOrFilter);
        }

        function getEvent(idOrFilter) {
            var events = me.calendar.fullCalendar('clientEvents', idOrFilter);
            return events.length === 0 ? null : events[0];
        }

        function getEvents(idOrFilter) {
            return me.calendar.fullCalendar('clientEvents', idOrFilter);
        }

        function setHeight(height) {
            me.calendar.fullCalendar('option', 'height', height);
        }

        function setContentHeight(height) {
            me.calendar.fullCalendar('option', 'contentHeight', height);
        }

        function getResources() {
            return me.calendar.fullCalendar('getResources');
        }

        function removeResources() {

            var resources = me.getResources();

            angular.forEach(resources, function (resource) {
                me.calendar.fullCalendar('removeResource', resource);
            });
        }
    }
})();
