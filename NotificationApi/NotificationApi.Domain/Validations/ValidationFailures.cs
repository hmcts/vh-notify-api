﻿using System.Collections.Generic;

namespace NotificationApi.Domain.Validations
{
    public class ValidationFailures : List<ValidationFailure>
    {
        public void AddFailure(string name, string message)
        {
            Add(new ValidationFailure(name, message));
        }
    }
}
