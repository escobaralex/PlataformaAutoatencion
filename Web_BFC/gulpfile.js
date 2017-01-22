/*"use strict";

var
    gulp = require('gulp'),
    bower = require('gulp-bower'),
    del = require('del'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    less = require('gulp-less'),
    concatCss = require('gulp-concat-css'),
    minifyCss = require('gulp-minify-css'),
    rename = require('gulp-rename');

// Borra los archivos de Distribucion
gulp.task('clean', function () {
    return del(['assets/dist/*.*']);
});

// Borra los archivos CSS
gulp.task('clean-css', ['clean'], function () {
    return del(['assets/css/*.css']);
});

// Compila Less a CSS
gulp.task('build-css', ['clean-css'], function () {
    return gulp.src(['assets/less/*.less'])
    .pipe(less())
    //.pipe(minifyCSS())
    .pipe(gulp.dest('assets/css/'));
});

// Concatena CSS
gulp.task('concat-css', ['build-css'], function () {
    return gulp.src('assets/css/*.css')
      .pipe(concatCss("app.css"))
      .pipe(gulp.dest('assets/css/'));
});

// minify CSS
gulp.task('min-css', ['concat-css'], function () {
    return gulp.src('assets/css/app.css')
    .pipe(minifyCss({ compatibility: 'ie8' }))
    .pipe(rename({ suffix: '.min' }))
    .pipe(gulp.dest('assets/dist/'));
});

// Concatenar y minify JS
gulp.task('min-js', ['clean'], function () {
    gulp.src(['assets/js/*.js'])
    .pipe(concat('app.min.js'))
    .pipe(uglify())
    .pipe(gulp.dest('assets/dist/'));
});

// Tarea por Defecto que desencadena las demas
gulp.task('default', ['min-css', 'min-js']);
*/