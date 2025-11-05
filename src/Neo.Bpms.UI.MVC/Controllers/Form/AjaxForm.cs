// using System.Web.Mvc;
// using Neo.Bpms.Engine.Dynamic;
// using Neo.Bpms.Engine.UI.Attribute;
//
// 
// #pragma warning disable 1573
//
// namespace Neo.Bpms.UI.MVC.Controllers
// {
//     public partial class FormController
//     {
//         [HttpPost]
//         public JsonResult Data(string NamespaceId,
//             string EntityId, string FormId,
//             [ModelBinder(typeof(DynamicActionBinder))]
//             ElasticObject record, string ids)
//         {
//             var form = GetFormAndThings(NamespaceId, EntityId, FormId, out var user, out _);
//             
//             SubmitForm(form, ids, record, user, out var errors, null, null, false, );
//             
//             if (errors != null)
//             {
//
//                 // todo ModelState and JsonResult?
//             }
//
//             return null; // todo navigation info or entirely client side
//         }
//     }
// }
