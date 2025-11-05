using Microsoft.AspNetCore.Html;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms.UIRules;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

public interface IFormLogicHelper
{
    void AddFormLogic(CommonFormStructure structure, List<UIRule> controllers, string tableName);
    HtmlString HtmlLogicEvent(CommonFormStructure structure, string scope, string fieldName);
    string ConvertToClientCode(string formula);
    eOperationType GetClientOperationType(UIRuleTask operation);
    ServerOperationResult GetEventServerOperations(string namespaceId, string entityId, string formId,
        string source, string eventName, IdentityUser user, List<QField> qs, List<LCField> lCs, string culture);
    string GetLogicEvent(CommonFormStructure structure, string scope, string fieldName);
    string GetLogicEvent(FormLogicDefinition logicDefinition, string scope, string fieldName);
    HtmlString HtmlLogicController(CommonFormStructure structure, string eventType);
    object ServerEval(string formula, LocalParameters el);
    object ServerEval(IdentityUser user, string namespaceId, string entityId,
        string formula, IList<QField> qs, IList<LCField> lCs);
}
