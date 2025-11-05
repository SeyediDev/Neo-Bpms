//using System.Collections.Generic;
//using Neo.Bpms.Domain.Model.BPMN.Core.Infrastructure;
//using Neo.Bpms.Domain.Model.BPMN.Extensions;
//using Neo.Bpms.Domain.Model.UI;
//
//namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.Menu
//{
//    public class PossibleMenuItem
//    {
//        public PossibleMenuItem()
//        {            
//        }
//
//        public PossibleMenuItem(Dashboard item)
//        {
//            id = $"d-{item.Id}";
//            name = item.Name;
//            namespaceId = item.NamespaceId;
//            entityId = item.EntityId;
//            parameters = new[] {new MenuParamViewModel(eMenuItemParameter.DashboardId, item.Id)};
//        }
//        public PossibleMenuItem(Model.UI.Form item)
//        {
//            id = $"f-{item.Id}";
//            name = item.Name;
//            namespaceId = item.NamespaceId;
//            entityId = item.EntityId;
//            parameters = new[] {new MenuParamViewModel(eMenuItemParameter.FormId, item.Id)};
//            // todo subject :(              
//        }
//        public PossibleMenuItem(Model.UI.Report item)
//        {
//            id = $"r-{item.Id}";
//            name = item.Name;
//            namespaceId = item.NamespaceId;
//            entityId = item.EntityId;
//            parameters = new[] {new MenuParamViewModel(eMenuItemParameter.ReportId, item.Id)};
//        }
//        public PossibleMenuItem(BusinessProcess item)
//        {
//            id = $"c-{item.Id}";
//            name = item.Name;
//            parameters = new[] {new MenuParamViewModel(eMenuItemParameter.ProcessId, item.Id)};
//        }
//        public string id { get; set; }
//        public string name { get; set; }
//        public string namespaceId { get; set; }
//        public string entityId { get; set; }
//        public IEnumerable<MenuParamViewModel> parameters { get; set; }
//    }
//}