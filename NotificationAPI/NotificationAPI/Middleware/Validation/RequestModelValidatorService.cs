﻿using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace NotificationAPI.Middleware.Validation
{
    public class RequestModelValidatorService : IRequestModelValidatorService
    {
        private readonly IValidatorFactory _validatorFactory;

        public RequestModelValidatorService(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public IList<ValidationFailure> Validate(Type requestModel, object modelValue)
        {
            var validator = _validatorFactory.GetValidator(requestModel);
            if (validator == null)
            {
                var failure = new ValidationFailure(modelValue.GetType().ToString(), "Validator not found for request");
                return new List<ValidationFailure>{failure};
            }
            var result = validator.Validate(modelValue);
            return result.Errors;
        }
    }
}