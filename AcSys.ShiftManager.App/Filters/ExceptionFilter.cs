using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using AcSys.ShiftManager.Service.Common;

namespace AcSys.ShiftManager.App.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            await base.OnExceptionAsync(actionExecutedContext, cancellationToken);

            //Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(context.Exception));

            //TODO: All uncaught exceptions must be caught and sent to client as fancy messages. Probably only the inner most exceptions should be sent to client?
            //Exception ex = context.Exception;
            //while (ex != null)
            //{
            //    ex = ex.InnerException;
            //}

            HttpResponseMessage response = null;

            if (actionExecutedContext.Exception is NotImplementedException)
            {
                //response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "This section is under construction.");
            }
            else if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, actionExecutedContext.Exception.Message);
            }
            else if (actionExecutedContext.Exception is ForbiddenException)
            {
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, actionExecutedContext.Exception.Message);
            }
            else if (actionExecutedContext.Exception is ApplicationException)
            {
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionExecutedContext.Exception.Message);
            }
            else if (actionExecutedContext.Exception is System.ComponentModel.DataAnnotations.ValidationException)
            {
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionExecutedContext.Exception.Message);
            }
            //else if (context.Exception is FluentValidation.ValidationException)
            //{
            //    response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, context.Exception.Message);
            //}
            else if (actionExecutedContext.Exception is DataException)
            {
                response = HandleDbException(actionExecutedContext, response);
            }
            else
            {
                //var error = new HttpError("Oops some internal Exception. Please contact your administrator") { { "ErrorCode", 500 } };
                var error = new HttpError(actionExecutedContext.Exception.Message) { { "ErrorCode", 500 } };
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }

            if (response != null)
                actionExecutedContext.Response = response;
        }

        static HttpResponseMessage HandleDbException(HttpActionExecutedContext actionExecutedContext, HttpResponseMessage response)
        {
            if (actionExecutedContext.Exception is DbEntityValidationException)
            {
                DbEntityValidationException dbevex = actionExecutedContext.Exception as DbEntityValidationException;

                //StringBuilder messages = new StringBuilder();
                string message = string.Empty;

                if (dbevex.EntityValidationErrors.Count() > 0)
                {
                    foreach (var error in dbevex.EntityValidationErrors)
                    {
                        var verror = error.ValidationErrors.FirstOrDefault();
                        if (verror != null)
                        {
                            //message += String.Format("{0}: {1}{2}", verror.PropertyName, verror.ErrorMessage, Environment.NewLine);
                            message += String.Format("{1}: {0}", verror.ErrorMessage, (error.Entry.Entity.GetType().Name));
                        }
                        //foreach (var verror in error.ValidationErrors)
                        //{
                        //    messages.AppendLine(String.Format("{0}: {1}{2}", verror.PropertyName, verror.ErrorMessage, Environment.NewLine));
                        //}
                    }
                }
                else
                {
                    //message = dbevex.Message;
                    //messages.Append(dbevex.Message);
                }
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            if (actionExecutedContext.Exception is DbUpdateException)
            {
                DbUpdateException dbuex = actionExecutedContext.Exception as DbUpdateException;
                response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, dbuex.Message);
            }

            return response;
        }

        //public override void OnException(HttpActionExecutedContext context)
        //{
        //    base.OnException(context);

        //    //Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(context.Exception));
        //}
    }
}
