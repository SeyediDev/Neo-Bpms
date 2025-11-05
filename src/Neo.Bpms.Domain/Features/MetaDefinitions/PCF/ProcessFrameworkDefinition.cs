//note: The implementation of the model, modeling interface and supporting engine is pending for future requirements.
//using Neo.Bpms.Domain.Model.BPMN.Processes;
//using Neo.Bpms.Domain.Model.ProcessClassification;
//
//namespace Neo.Bpms.Domain.Features.Definitions.Entities
//{
//	public abstract class IProcessFrameworkDefinition: IBaseModellingDefinition
//	{
//		protected ProcessClassificationFramework DefineFramework(string id, string name, string enName, string source, string destination)
//		{
//			framework = new ProcessClassificationFramework(id, name, source, destination);
//			return framework;
//		}
//		public ProcessClassificationFramework framework;
//		public ProcessClassificationFramework DefineAll()
//		{
//			var framework = IdentifyReport();
//			if (framework == null) return null;
//			if (!DefineStructure()) return null;
//			return framework;
//		}
//		protected ProcessCategory currentProcessCategory;
//		protected ProcessCategory addProcessCategory(ProcessCategory pc)
//		{
//			if (framework == null) return null;
//			framework.addProcessCategory(pc);
//			currentProcessCategory = pc;
//			currentBaseElement = pc;
//			return pc;
//		}
//		protected ProcessGroup currentProcessGroup;
//		protected ProcessGroup addProcessGroup(ProcessGroup pg)
//		{
//			if (currentProcessCategory == null) return null;
//			currentProcessCategory.addProcessGroup(pg);
//			currentProcessGroup = pg;
//			currentBaseElement = pg;
//			return pg;
//		}
//		protected BusinessProcess currentBusinessProcess;
//		protected BusinessProcess addBusinessProcess(BusinessProcess bprocess)
//		{
//			if (currentProcessGroup == null) return null;
//			currentProcessGroup.addBusinessProcess(bprocess);
//			currentBusinessProcess = bprocess;
//			currentBaseElement = bprocess;
//			return bprocess;
//		}
//		protected BusinessTask currentBusinessTask;
//		protected BusinessTask addBusinessTask(BusinessTask task)
//		{
//			if (currentBusinessProcess == null) return null;
//			currentBusinessProcess.addBusinessTask(task);
//			currentBusinessTask = task;
//			currentBaseElement = task;
//			return task;
//		}
//		protected KPI currentKPI;
//		protected KPI addKPI(KPI kpi)
//		{
//			if (currentBusinessProcess == null) return null;
//			currentBusinessProcess.addKPI(kpi);
//			currentKPI = kpi;
//			currentBaseElement = kpi;
//			return kpi;
//		}
//		protected ProcessRole currentProcessRole;
//		protected ProcessRole addProcessRole(ProcessRole processRole)
//		{
//			if (currentBusinessProcess == null) return null;
//			currentBusinessProcess.addProcessRole(processRole);
//			currentProcessRole = processRole;
//			currentBaseElement = processRole;
//			return processRole;
//		}
//		protected Process currentProcess;
//		protected Process addProcess(Process process)
//		{
//			if (currentBusinessProcess == null) return null;
//			currentBusinessProcess.addProcessDefinition(process);
//			currentProcess = process;
//			currentBaseElement = process;
//			return process;
//		}
//
//		public abstract ProcessClassificationFramework IdentifyReport();
//		public abstract bool DefineStructure();
//	}
//}
