﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiCatalogo.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public ILogger<ApiExceptionFilter> _logger { get; }

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }


        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Ocorreu uma exceção não tratada: Status Code 500");
            
            context.Result = 
            new ObjectResult($"Ocorreu um problema ao tratar a solicitação: \n{context.Exception.Message} \nStatus Code: 500")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
