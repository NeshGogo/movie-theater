using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueSupplier = bindingContext.ValueProvider.GetValue(propertyName);

            if (valueSupplier == ValueProviderResult.None) return Task.CompletedTask;

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueSupplier.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch 
            {
                bindingContext.ModelState.TryAddModelError(propertyName, $"Invalid value to type {typeof(T)}");
            }

            return Task.CompletedTask;
        }
    }
}
