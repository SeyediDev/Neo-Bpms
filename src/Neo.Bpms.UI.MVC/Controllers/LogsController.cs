// using System;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Neo.Bpms.UI.MVC.Controllers
// {
//     public class LogsController : NeoController
// 	{
// 	    public ActionResult Index()
// 	    {
// 	        var user = MVCUserManager.GetUser(User);
// 	        if (user == null)
// 	            return RedirectToAction("Login", "Account");
// //	        if (!AccessServices.CheckControllerActionAccess(user, "MicroservicesMonitor", "Index"))
// 	        if (!user.IsAdmin)
// 	            throw new UnauthorizedAccessException();
// 	        ViewBag.user = user;
// 	        SetPagePackId("/Logs/Index");
//
//             return View();
// 	    }
//
// 	    public string Data(string fileName)
// 	    {
// 	        var user = MVCUserManager.GetUser(User);	        
// 	        //	        if (!AccessServices.CheckControllerActionAccess(user, "MicroservicesMonitor", "Index"))
// 	        if (!user?.IsAdmin ?? false)
// 	            throw new UnauthorizedAccessException();
// 	        if (string.IsNullOrEmpty(fileName))
// 	            fileName = "file.txt";
// 	        else fileName = "log/" + fileName;
// 	        
//
//             var result = System.IO.File.ReadAllText(HttpContext.Server.MapPath($"~/{fileName}"));
//             return result;
// 	    }	    
//     }
// }
