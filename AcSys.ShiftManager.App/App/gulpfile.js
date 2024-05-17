(function () {
    'use strict';

    var gulp = require('gulp'),
        rename = require('gulp-rename'),
        clean = require('gulp-clean'),
        copy = require('gulp-copy'),
        concat = require('gulp-concat'),
        sourcemaps = require('gulp-sourcemaps'),
        uglify = require('gulp-uglify'),
        pump = require('pump'),
        jshint = require('gulp-jshint'),
        //jshint_stylish = require('jshint-stylish'),
        cssnano = require('gulp-cssnano'),
        livereload = require('gulp-livereload'),
        notify = require('gulp-notify'),
        runSequence = require('run-sequence');

    var solutionRootDir = '../../',
        projectRootDir = '../',
        srcAppDir = '',
        srcNodeModules = srcAppDir + 'node_modules/',
        srcBowerComponents = srcAppDir + 'bower_components/',
        srcApp = srcAppDir + 'src/',
        srcNugetPackages = solutionRootDir + 'packages/',

        srcIndex = projectRootDir + 'index.html',
        srcViews = [srcApp + '**/*.html'],

        srcStyle =
        [
            //'!' + srcAppDir + '/css/**/*.min.css',
            srcAppDir + '**/*.style.css'
        ],

        //srcColorAdmin = srcAppDir + 'ColorAdmin/',
        //srcColorAdminAssets = srcColorAdmin + 'assets/',
        //srcColorAdminCss =
        //[
        //    //'!' + srcColorAdminAssets + '/css/**/*.min.css',
        //    srcColorAdminAssets + 'css/**/*.css'
        //],
        //srcColorAdminPlugins = srcColorAdminAssets + 'plugins/**/*.*',

        srcUtilScripts =
        [
            srcNodeModules + 'underscore/underscore-min.js',
            srcNodeModules + 'ngstorage/ngStorage.min.js',
            srcNodeModules + 'stacktrace-js/dist/stacktrace.min.js',
            srcNodeModules + 'ng-tags-input/build/ng-tags-input.min.js',
            srcNodeModules + 'jquery/dist/jquery.min.js',
            srcNodeModules + 'bootstrap/dist/js/bootstrap.min.js',
            srcNodeModules + 'jquery-ui-dist/jquery-ui.min.js',
            srcNodeModules + 'datatables.net/js/jquery.dataTables.js',
            srcNodeModules + 'angular-datatables/dist/angular-datatables.min.js',
            srcNodeModules + 'ui-select/dist/select.min.js',

            srcNodeModules + 'fullcalendar/dist/fullcalendar.min.js',
            srcNodeModules + 'fullcalendar-scheduler/dist/scheduler.min.js',
            //srcNodeModules + 'angular-ui-calendar/src/calendar.js',
            srcBowerComponents + 'angular-ui-calendar/src/calendar.js',
            
            srcNugetPackages + 'AngularJS.Core.1.6.1/content/Scripts/angular.min.js',
            srcNugetPackages + 'Angular.UI.UI-Router.0.4.2/content/scripts/angular-ui-router.min.js',
            srcNugetPackages + 'AngularJS.Sanitize.1.6.1/content/Scripts/angular-sanitize.min.js',
            srcNugetPackages + 'AngularJS.Animate.1.6.1/content/Scripts/angular-animate.min.js',
            srcNugetPackages + 'Angular.UI.Bootstrap.2.5.0/content/Scripts/angular-ui/ui-bootstrap-tpls.min.js',
            srcNugetPackages + 'Angular.UI.Bootstrap.2.5.0/content/Scripts/angular-ui/ui-bootstrap.min.js',

            srcBowerComponents + 'moment/min/moment.min.js',
            srcBowerComponents + 'moment-timezone/builds/moment-timezone.min.js',
            srcBowerComponents + 'moment-timezone/builds/moment-timezone-with-data.min.js',

            srcBowerComponents + 'bootstrap-daterangepicker/daterangepicker.js',
            srcBowerComponents + 'angular-daterangepicker/js/angular-daterangepicker.js',

            srcNodeModules + 'angular-ui-switch/angular-ui-switch.min.js',
            srcNodeModules + 'bootstrap-menu/dist/BootstrapMenu.min.js',
            srcNodeModules + 'jquery-contextmenu/dist/jquery.contextMenu.min.js',
            srcNodeModules + 'jquery-contextmenu/dist/jquery.ui.position.min.js',

            //srcBowerComponents + 'chartjs/Chart.min.js',
            srcNodeModules + 'chart.js/dist/Chart.min.js',
            srcBowerComponents + 'angles/angles.js',
            srcNodeModules + 'angular-chart.js/dist/angular-chart.min.js',
            
            srcBowerComponents + 'raphael/raphael.min.js',
            srcBowerComponents + 'morris.js/morris.min.js',
            srcBowerComponents + 'ng-morris-js/dist/ng-morris-js.min.js'
        ],

        srcUtilStyle =
        [
            srcNugetPackages + 'Angular.UI.Bootstrap.2.5.0/content/Content/ui-bootstrap-csp.css',

            srcNodeModules + 'bootstrap/dist/css/bootstrap.min.css',
            srcNodeModules + 'jquery-ui-dist/jquery-ui.min.css',

            srcNodeModules + 'ng-tags-input/build/ng-tags-input.min.css',
            srcNodeModules + 'angular-datatables/dist/css/angular-datatables.min.css',

            srcNodeModules + 'ui-select/dist/select.min.css',

            srcNodeModules + 'fullcalendar/dist/fullcalendar.min.css',
            srcNodeModules + 'fullcalendar-scheduler/dist/scheduler.min.css',

            srcBowerComponents + 'bootstrap-daterangepicker/daterangepicker.css',

            srcNodeModules + 'angular-ui-switch/angular-ui-switch.min.css',

            srcNodeModules + 'jquery-contextmenu/dist/jquery.contextMenu.min.css',

            srcBowerComponents + 'morris.js/morris.css'
        ],

        srcFonts =
        [
            srcNodeModules + 'bootstrap/dist/fonts/glyphicons-halflings-regular.woff2',
            srcNodeModules + 'bootstrap/dist/fonts/glyphicons-halflings-regular.woff',
            srcNodeModules + 'bootstrap/dist/fonts/glyphicons-halflings-regular.ttf'
        ],

        srcAppScripts =
        [
            //'!' + srcApp + '/**/*.min.js',
            srcApp + '**/*.module.js',
            srcApp + '**/*.config.js',
            srcApp + '**/*.run.js',
            srcApp + '**/*.js'
        ],

        distRoot = 'dist/',

        //distColorAdmin = distRoot + 'ColorAdmin/',
        //distColorAdminAssets = distColorAdmin + 'assets/',
        //distColorAdminPlugins = distColorAdminAssets + 'plugins/',
        //distColorAdminCss = distColorAdminAssets + 'css/',
        distStyle = distRoot + 'css/',
        distFonts = distRoot + 'fonts/',

        distIndex = distRoot + 'index.html',
        distViews = distRoot + 'views/',
        //targetViews = distRoot + '**/*.html',
        distScripts = distRoot + 'js/';


    gulp.task('default', [], function () {
        // place code for your default task here
        runSequence('clean', 'build');
    });

    gulp.task('build', ['scripts', 'html', 'appStyle', 'utilStyle', 'fonts'], function () {
        // place code for your default task here
        console.log('Building...');
    });

    gulp.task('clean', ['clean:scripts', 'clean:html', 'clean:style', 'clean:fonts'], function () {
        //return gulp.src(distRoot, { read: false }).pipe(clean());
    });

    gulp.task('watch', function () {

        livereload.listen();

        gulp.watch([srcAppScripts], ['scripts']);

        gulp.watch([srcStyle, srcUtilStyle], ['style']);
        
        gulp.watch([srcViews], ['views']);

        gulp.watch([srcIndex], ['index']);
    });


    gulp.task('clean:html', ['clean:index', 'clean:views'], function () { });
    gulp.task('html', ['clean:html'], function () {
        runSequence('index', 'views');
    });

    gulp.task('clean:index', function () { return gulp.src(distIndex, { read: false }).pipe(clean()); });
    gulp.task('index', ['clean:index'], function (cb) {
        return gulp.src(srcIndex)
            .pipe(gulp.dest(distRoot))
            .pipe(livereload());
    });

    gulp.task('clean:views', function () { return gulp.src(distViews, { read: false }).pipe(clean()); });
    gulp.task('views', ['clean:views'], function (cb) {
        return gulp.src(srcViews)
            .pipe(gulp.dest(distViews))
            .pipe(livereload());
    });

    gulp.task('clean:scripts', [/*, 'clean:colorAdmingPlugins'*/], function () {
        return gulp.src(distScripts, { read: false }).pipe(clean());
    });

    gulp.task('scripts', ['clean:scripts'/*'appScripts', 'utilScripts'*//*, 'colorAdminPlugins'*/], function () {
        runSequence('appScripts', 'utilScripts');
    });
    
    gulp.task('appScripts', ['lint'], function (cb) {

        console.log('Building scripts...');

        pump([
            gulp.src(srcAppScripts),
            sourcemaps.init(),
            concat('app.js'),
            uglify(),
            rename('app.min.js'),
            sourcemaps.write(''),
            gulp.dest(distScripts),
            livereload()
        ], cb);
    });

    gulp.task('utilScripts', [], function (cb) {
        return gulp.src(srcUtilScripts)
            .pipe(gulp.dest(distScripts))
            .pipe(livereload());
    });

    //gulp.task('clean:colorAdminPlugins', function () {
    //    return gulp.src(distColorAdminPlugins, { read: false }).pipe(clean());
    //});
    //gulp.task('colorAdminPlugins', ['clean:colorAdminPlugins'], function () {
    //    gulp.src(srcColorAdminPlugins).pipe(gulp.dest(distColorAdminPlugins));
    //});

    gulp.task('clean:style', [], function () { return gulp.src(distStyle, { read: false }).pipe(clean()); });
    gulp.task('style', ['clean:style'/*'appStyle', 'utilStyle', 'fonts'*/], function () {
        runSequence('appStyle', 'utilStyle', 'fonts');
    });

    gulp.task('appStyle', [], function () {
        //gulp.src(srcColorAdminCss).pipe(gulp.dest(distColorAdminCss));
        return gulp.src(srcStyle)
            .pipe(sourcemaps.init())
            .pipe(concat('app.css'))
            .pipe(cssnano())
            .pipe(rename('app.min.css'))
            .pipe(sourcemaps.write(''))
            .pipe(gulp.dest(distStyle));
    });

    gulp.task('utilStyle', [], function (cb) {
        return gulp.src(srcUtilStyle).pipe(gulp.dest(distStyle));
    });

    gulp.task('clean:fonts', [], function () {
        return gulp.src(distFonts, { read: false }).pipe(clean());
    });

    gulp.task('fonts', ['clean:fonts'], function (cb) {
        return gulp.src(srcFonts).pipe(gulp.dest(distFonts));
    });

    gulp.task('lint', function () {
        return gulp.src(srcAppScripts)
            .pipe(jshint())
            .pipe(jshint.reporter('jshint-stylish'));    //'default'
    });

})();
