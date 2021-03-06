﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Core.ViewModels;

namespace SimplCommerce.Module.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IWidgetInstanceService _widgetInstanceService;

        public HomeController(ILoggerFactory factory, IWidgetInstanceService widgetInstanceService)
        {
            _logger = factory.CreateLogger("Unhandled Error");
            _widgetInstanceService = widgetInstanceService;
        }

        public IActionResult TestError()
        {
            throw new Exception("Test behavior in case of error");
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel();

            model.WidgetInstances = _widgetInstanceService.GetPublished().Select(x => new WidgetInstanceViewModel
            {
                Id = x.Id,
                Name = x.Name,
                ViewComponentName = x.Widget.ViewComponentName,
                WidgetId = x.WidgetId,
                WidgetZoneId = x.WidgetZoneId,
                Data = x.Data,
                HtmlData = x.HtmlData
            }).ToList();

            return View(model);
        }

        [HttpGet("/Home/ErrorWithCode/{statusCode}")]
        public IActionResult ErrorWithCode(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("404");
            }

            return View("Error");
        }

        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            if (error != null)
            {
                _logger.LogError(error.Message, error);
            }

            return View("Error");
        }
    }
}