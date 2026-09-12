"""Maintainer command: refresh reviewed recipe evidence after reviewing source changes."""
import hashlib
import json
from pathlib import Path
import subprocess

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1]
BASE = 'src/Neo.Bpms.Infrastructure/'
RECIPES = {
    'new-process': {
        'steps': [
            'Process / فرآیند: inspect ProjectBpmn.Load and ProjectProcess.LoadBpmn before changing import/version behavior.',
            'ProjectBpmn reads Bpmn/bpmn.xml and merges root elements by identifier; keep identifiers and versions deliberate.',
            'Do not call Load/Save as validation: they touch project metadata and the filesystem. Use BPMN structural checks then an isolated runtime fixture.'
        ],
        'sources': [(BASE + 'Features/MetaLoader/MetaProcess/ProjectBpmn.cs', 'public void Load'),
                    (BASE + 'Features/MetaLoader/MetaProcess/ProjectProcess.cs', 'LoadBpmn')]
    },
    'dynamic-form': {
        'steps': [
            'Form / فرم: inspect FormStructures and FormStructRoutines for the actual field model and rendering path.',
            'Keep FormItemPropertiesViewModel identifiers, parent-control links, labels and properties aligned with existing metadata.',
            'Preserve EditTypeId and eCreateType save/navigation behavior; exercise the existing form render and validation flow with synthetic data.'
        ],
        'sources': [(BASE + 'Features/Cmmn/Forms/FormStructures/FormStructures.cs', 'public class FormItemPropertiesViewModel'),
                    (BASE + 'DependencyInjection.cs', 'services.AddScoped<FormStructRoutines>')]
    },
    'human-task': {
        'steps': [
            'Task / کارتابل: WorkItemManager.GetWorkItems receives user, culture and filter, then counts and pages the query.',
            'Preserve user/owner restrictions, paging, totals and QueryUtility resource release; a displayed row is not authorization to complete a task.',
            'Trace the existing completion handler and state checks before changing transitions; test two different users and an already-completed task.'
        ],
        'sources': [(BASE + 'Features/Bpms/Processes/Managers/WorkItemManager.cs', 'public List<WorkItemViewModel> GetWorkItems'),
                    ('src/Neo.Bpms.UI.MVC/Views/Process/MyWorkItems.cshtml', 'Html.BeginForm')]
    },
    'integration': {
        'steps': [
            'Integration / اتصال: ISendFormCommand.Send accepts an ElasticObject and command Type and returns a boolean/message tuple.',
            'In this baseline SendFormCommand does not dispatch MediatR: dispatch code is commented and the implementation returns false.',
            'Do not present it as a working command integration. Implement and test the requested dispatch separately before using it in a real workflow.',
            'AddNeoBpmsInfrastructure registers ISendFormCommand scoped, while IBpmsEngine and several metadata loaders are singleton; inspect lifetimes before adding dependencies.'
        ],
        'sources': [('src/Neo.Bpms.Application/Features/ISendFormCommand.cs', 'public interface ISendFormCommand'),
                    (BASE + 'Features/SendFormCommand.cs', 'return (false,'),
                    (BASE + 'DependencyInjection.cs', 'services.AddScoped<ISendFormCommand')]
    }
}


def main():
    result = {'baseline': subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT, text=True).strip(), 'recipes': {}}
    for topic, recipe in RECIPES.items():
        entries = []
        for path, anchor in recipe['sources']:
            text = (ROOT / path).read_text(encoding='utf-8-sig').replace('\r\n', '\n')
            offset = text.index(anchor)
            entries.append({'path': path, 'line': text.count('\n', 0, offset) + 1,
                            'sha256': hashlib.sha256(text.encode()).hexdigest()})
        result['recipes'][topic] = {'steps': recipe['steps'], 'evidence': entries}
    (HERE / 'knowledge.json').write_text(json.dumps(result, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
    print('Updated four reviewed recipes. Review the diff before committing.')


if __name__ == '__main__':
    main()
