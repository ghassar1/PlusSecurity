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

var src = 'App',
    srcAssets = 'App/assets',
    srcIndex = src + '/index.html',
    srcTemplate = src + '/template/*.html',
    srcViews = [src + '/views/**/*.html'],
    srcStyle = [
//'!' + srcAssets + '/css/**/*.min.css',
srcAssets + '/css/**/*.css'],
    srcJs = srcAssets + '/js',
    srcScripts = ['!' + srcJs + '/**/*.min.js', srcJs + '/**/*.module.js', srcJs + '/**/*.config.js', srcJs + '/**/*.js'],
    dist = 'App/dist/',
    distAssets = dist + '/assets',
    distIndex = dist + '/index.html',
    distViews = dist + '/views',
    distTemplate = dist + '/template',
    distStyle = distAssets + '/css',
    distScripts = distAssets + '/js';

gulp.task('default', ['build'], function () {
    // place code for your default task here
    //runSequence('clean', 'build');
});

gulp.task('build', ['html', 'style', 'scripts'], function () {
    // place code for your default task here
});

gulp.task('watch', function () {
    livereload.listen();
    gulp.watch(['App/assets/js/**/*.js'], ['scripts']);
    gulp.watch([srcViews], ['views']);
    gulp.watch(['App/assets/js/**/*.js'], ['style']);
});

gulp.task('html', ['clean:html', 'template', 'views'], function (cb) {});

gulp.task('template', ['clean:template'], function (cb) {
    gulp.src(srcTemplate).pipe(gulp.dest(distTemplate));
});

gulp.task('views', ['clean:views'], function (cb) {
    gulp.src(srcViews).pipe(gulp.dest(distViews));
});

gulp.task('style', ['clean:style'], function (cb) {
    gulp.src(srcStyle).pipe(gulp.dest(distStyle));
});

gulp.task('lint', function () {
    return gulp.src(['!App/assets/js/**/*.min.js', 'App/assets/js/**/*.js']).pipe(jshint()).pipe(jshint.reporter('jshint-stylish')); //'default'
});

gulp.task('scripts', ['clean:scripts', 'lint'], function (cb) {

    console.log('Building scripts...');

    pump([gulp.src(srcScripts), sourcemaps.init(), concat('app.js'), uglify(), rename('app.min.js'), sourcemaps.write(''), gulp.dest(distScripts), livereload()], cb);
});

gulp.task('clean', ['clean:scripts', 'clean:views', 'clean:style'], function () {
    return gulp.src(dist, { read: false }).pipe(clean());
});

gulp.task('clean:html', ['clean:template', 'clean:views'], function () {});

gulp.task('clean:template', function () {
    return gulp.src(distTemplate, { read: false }).pipe(clean());
});

gulp.task('clean:views', function () {
    return gulp.src(distViews, { read: false }).pipe(clean());
});

gulp.task('clean:style', function () {
    return gulp.src(distStyle, { read: false }).pipe(clean());
});

gulp.task('clean:scripts', function () {
    return gulp.src(distScripts, { read: false }).pipe(clean());
});

