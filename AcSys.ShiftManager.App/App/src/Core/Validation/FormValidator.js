(function () {
    'use strict';

    angular.module('app.core')
        .run(setup);

    setup.$inject = ['$rootScope'];
    function setup($rootScope) {
        var formValidator = new FormValidator();
        $rootScope.FormValidator = formValidator;
    }

    function FormValidator() {

        var me = this;
        me.errorElementIdentifierClass = 'app-field-validation-error';
        me.validate = _validate;
        me.clear = _clearFormErrors;

        function _validate(form) {

            if (form.$valid) {
                console.log('Form is valid.');
            }

            if (!form.$valid) {

                //console.log('Form is invalid.', form.$error);

                var formElement = _getFormElement(form);

                //formElement.tooltip('destroy');

                angular.forEach(form.$error, function (field) {
                    angular.forEach(field, function (errorField) {

                        errorField.$setTouched();

                        // get handle to the input field
                        var targetContainer = 'form[name="' + form.$name + '"] [name="' + errorField.$name + '"]';

                        // determine the validator type for current field i.e. required, email, url, number, minLength, maxLength, alphabets etc.
                        var validationType = determineValidatorTypeForField(errorField);

                        var targetMessage = prepareTargetMessageForField(errorField);

                        //showErrorTooltipOverField(targetContainer, targetMessage);

                        //$rootScope.notifications.message(targetMessage);

                        showErrorNextToField(targetContainer, validationType, errorField, targetMessage);
                    });
                });
            }

            return form.$valid;
        }

        function showErrorNextToField(targetContainer, validationType, errorField, targetMessage) {

            $(targetContainer).addClass('validation_error');

            // add error messages next to fields.
            var errorMessageElementId = 'validation_' + validationType + '_' + errorField.$name;
            var errorMessageElement = $('p#' + errorMessageElementId);
            if (errorMessageElement.length) {
                // Error element already exists. Replace the message text
                errorMessageElement.text(targetMessage);
            } else {
                // Error element does not exist. Create the element and set the message text
                $(targetContainer)
                    //.before($('<p></p>').attr('id', errorMessageElementId)
                    .after($('<p></p>').attr('id', errorMessageElementId)
                    .addClass('text-danger')
                    .addClass(me.errorElementIdentifierClass)
                    .text(targetMessage));
            }
        }

        function _clearFormErrors(form) {

            var formElement = _getFormElement(form);
            formElement.find('.' + me.errorElementIdentifierClass).remove();

            var inputFields = formElement.find('input').removeClass('validation_error');
            var textareaFields = formElement.find('textarea').removeClass('validation_error');
            var selectFields = formElement.find('select').removeClass('validation_error');
        }

        function _getFormElement(form) {

            var formElement = $('form[name="' + form.$name + '"] *');
            return formElement;
        }

        function determineValidatorTypeForField(errorField) {
            var validationType = errorField.$error.required ? 'required' : '';
            validationType = errorField.$error.email ? 'email' : validationType;
            validationType = errorField.$error.url ? 'url' : validationType;
            validationType = errorField.$error.number ? 'number' : validationType;
            validationType = errorField.$name === 'alphabets' ? 'alphabets' : validationType;
            validationType = errorField.$error.minlength ? 'minlength' : validationType;
            validationType = errorField.$error.maxlength ? 'maxlength' : validationType;
            return validationType;
        }

        function prepareTargetMessageForField(errorField) {

            //console.debug(errorField);

            var targetMessage = errorField.$error.required ? 'This is required' : '';
            targetMessage = errorField.$error.email ? 'Invalid email' : targetMessage;
            targetMessage = errorField.$error.url ? 'Invalid website url' : targetMessage;
            targetMessage = errorField.$error.number ? 'Only number is allowed' : targetMessage;
            targetMessage = errorField.$name === 'alphabets' ? 'Only alphabets is allowed' : targetMessage;
            targetMessage = errorField.$error.minlength ? 'You must provide at least 20 characters for this field' : targetMessage;
            targetMessage = errorField.$error.maxlength ? 'You must not exceed the maximum of 200 characters for this field' : targetMessage;
            targetMessage = errorField.$error.minTags ? 'This is required' : targetMessage;
            //targetMessage = errorField.$error.minTags ? errorField.$name + ' is required' : targetMessage;
            targetMessage = targetMessage || 'This must be entered correctly';
            return targetMessage;
        }

        function showErrorTooltipOverField(targetContainer, targetMessage) {
            // show tooltip on the control
            $(targetContainer).first().tooltip({
                placement: 'top',
                trigger: 'normal',
                title: targetMessage,
                container: 'body',
                animation: false
            });
            $(targetContainer).first().tooltip('show');
        }
    }
})();
