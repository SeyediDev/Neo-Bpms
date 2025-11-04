/*namespace Neo.Bpms.Domain.Model.BPMN.Test
{
	public class ProcessTest
	{
		public long Id { get; set; }
		public string Name { get; set; }
	}

	public class ProcessTestStep
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public ProcessTestStepTypeId TypeId { get; set; }
		public string? Description { get; set; }
		public string Formula { get; set; }
		public string Question { get; set; }
		public string TestNamespaceId { get; set; }
		public string TestEntityId { get; set; }
		public string TestFormId { get; set; }
		/*
		AddField(tstprocprmTestSteps, tstproc_tststpprmDataModel, "Data model", "مدل عملیات داده", varObjectInstanceId,"", "", "");
		SetFieldReferState(DBObjectObjectsId, objInfo);

		AddField(tstprocprmTestSteps, tstproc_tststpprmDataAction, "Data action", "کد عملیات داده", varObjectInstanceId4,"", "", "");

		SetParameter_LinkedNode(DBObjectActionsId, tstproc_tststpprmDataModel, 0);

		AddField(tstprocprmTestSteps, tstproc_tststpprmInputType, "Input Type", "نوع سند ورودی تست", varStringListItem, 2,2, 0, "", "", "");

		SetFieldStringList(strlstTestInputType);

		AddField(tstprocprmTestSteps, tstproc_tststpprmInputPKV, "Input PKV", "کد سند ورودی", varLong, 4, 4, 0, "", "","");

		AddField(tstprocprmTestSteps, tstproc_tststpprmOutputPKV, "Output PKV", "کد سند خروجی", varLong, 4, 4, 0, "", "","");

		AddField(tstprocprmTestSteps, tstproc_tststpprmInputNodeId, "Input Node Id", "پوشه سند ورودی", varObjectInstanceId4, "", "", "");
		SetParameter_LinkedNode(DBObjectNodesId, tstproc_tststpprmDataModel, 0);

		AddField(tstprocprmTestSteps, tstproc_tststpprmOutputNodeId, "Output Node Id", "پوشه سند خروجی", varObjectInstanceId4, "", "", "");

		SetParameter_LinkedNode(DBObjectNodesId, tstproc_tststpprmDataModel, 0);

		AddField(tstprocprmTestSteps, tstproc_tststpprmTestStepResult, "Test step result", "نتیجه تست", varBOOL, 1, 1, 0,"", "", "");

		AddField(tstprocprmTestSteps, tstproc_tststpprmInputPKVTetstId, "Input PKV tetst Id","کد مرحله تست تعیین کننده سند ورودی", varULong, 4, 4, 0, "", "", "");

		AddField(tstprocprmTestSteps, tstproc_tststpprmWait, "Wait", "مکث", varBOOL, 1, 1, 0, "", "", "");

		AddField(tstprocprmTestSteps, tstproc_tststpprmContinueOnFail, "Continue on fail", "ادامه در صورت عدم موفقیت",varBOOL, 1, 1, 0, "", "", "");

		AddField(tstprocprmTestSteps, tstproc_tststpprmErrorText, "Error text", "متن اشکال", varString, 0, 256, 0, "", "","");

		AddField(tstprocprmTestSteps, tstproc_tststpprmRetestTask, "Retest Task", "کارجبرانی", varString, -1, 256, 0, "","", "");

		AddField(tstprocprmTestSteps, tstproc_tststpprm120, "120", "انجام تست", varBOOL, "", "", "");
		AddFieldCase(tstproccaseControllerTest);
		AddField(tstprocprmTestSteps, tstproc_tststpprm121, "121", "موفقیت تست", varBOOL, "", "", "");
		AddFieldCase(tstproccaseControllerTest);

		AddField(tstprocprmTestSteps, tstproc_tststpprm122, "122", "شرح انجام تست", varString, -1, 1024, prmactMultiline,"", "", "");

		AddFieldCase(tstproccaseControllerTest);

		AddField(tstprocprmTestSteps, tstproc_tststpprmParameterValues, "Parameter Values", "مقادیر پارامتر ها", varTable,-1, 0, 0, "", "", "");

		{
			SetInternalParameterPrefix("_pvalu");
			AddField(tstproc_tststpprmParameterValues, tstproc_tststp_pvaluprmParameterId, "Parameter Id", "کد پارامتر",varULong, 4, 4, "", "", "");
			AddField(tstproc_tststpprmParameterValues, tstproc_tststp_pvaluprmParameterValue, "Parameter Value","مقدار پارامتر", varString, -1, 41, 0, "", "", "");
			AddField(tstproc_tststpprmParameterValues, tstproc_tststp_pvaluprmParameterType, "Parameter Type","نوع پارامتر", varStringListItem, 2, 2, 0, "", "", "");
			SetFieldStringList(strlstTestParameterType);
			AddField(tstproc_tststpprmParameterValues, tstproc_tststp_pvaluprmFormula, "Formula", "فرمول", varString, -1,81, 0, "", "", "");
		}
		* /
	}

	public class ProcessTestStepParameter
	{
		public long Id { get; set; }
		public string Name { get; set; }
	}

	public enum ProcessTestStepTypeId
	{
		Description,
		UserTask,
		Form,
		Assert,
		ManualTest,
		Report
	}
}*/