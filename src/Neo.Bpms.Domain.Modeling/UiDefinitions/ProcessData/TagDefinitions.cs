using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;

namespace Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessData;

public class TagDefinitions : EntityDefinition
{
    #region form definitions
    protected override void Forms()
    {
        DefineForm("Tag_15_Form");
        DefineForm("Tag_16_Index_Form");
        DefineForm("Tag_16_Form");
        DefineForm("Tag_17_Index_Form");
        DefineForm("Tag_17_Form");
        DefineForm("Tag_18_Form");
        DefineForm("Tag_19_Index_Form");
        DefineForm("Tag_19_Form");
        DefineForm("Tag_20_Index_Form");
        DefineForm("Tag_20_Form");

    }
    public class Tag_15_Form : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Tag_15_Form", "ثبت برچسب", Form.eFormType.Create);
            SetOutputStateId(12);
            return frm;
        }

        protected override void ViewModel()
        {



            AddField("Name");
            AddProperty(eControlPropertyId.Required, true);
            AddField("Description", eControlTypeId.MultilineTextInput);
            AddProperty(eControlPropertyId.Required, true);
            AddField("F17", eControlTypeId.File);
            AddField("F18");
            AddField("ValidateTime");

        }
        protected override void UIRules()
        {

        }
    }
    public class Tag_16_Index_Form : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Tag_16_Index_Form", "ویرایش برچسب", Form.eFormType.Index);
            return frm;
        }

        protected override void ViewModel()
        {



            AddColumn("Name");
            AddColumn("ValidateTime");




        }
    }
    public class Tag_16_Form : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Tag_16_Form", "ویرایش برچسب", Form.eFormType.Edit);
            return frm;
        }

        protected override void ViewModel()
        {



            AddField("Name");
            AddProperty(eControlPropertyId.Required, true);
            AddField("Description", eControlTypeId.MultilineTextInput);
            AddProperty(eControlPropertyId.Required, true);
            AddField("F17", eControlTypeId.File);
            AddField("F18");
            AddField("ValidateTime");

        }
    }
    public class Tag_17_Index_Form : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Tag_17_Index_Form", "حذف برچسب", Form.eFormType.Index);
            return frm;
        }

        protected override void ViewModel()
        {



            AddColumn("Name");
            AddColumn("ValidateTime");




        }
    }
    public class Tag_17_Form : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Tag_17_Form", "حذف برچسب", Form.eFormType.Edit);
            SetOutputStateId(13);
            return frm;
        }

        protected override void ViewModel()
        {



            AddField("Name");
            AddField("Description", eControlTypeId.MultilineTextInput);
            AddField("F17", eControlTypeId.File);
            AddField("F18");
            AddField("ValidateTime");

        }
    }
    //public class Tag_18_Form : IFormDefinition
    //{
    //	protected override Form IdentifyReport()
    //	{
    //		var frm = DefineForm("Tag_18_Form", "ثبت اطلاعات کارتابل", Form.eFormType.Create);
    //		setOutputStateId("");
    //		return frm;
    //	}
    //	protected override void ViewModel()
    //	{
    //		


    //		
    //	}
    //	protected override void UIRules()
    //	{
    //		
    //	}
    //}
    //public class Tag_19_Index_Form : IFormDefinition
    //{
    //	protected override Form IdentifyReport()
    //	{
    //		var frm = DefineForm("Tag_19_Index_Form", "ویرایش اطلاعات کارتابل", Form.eFormType.Index);
    //		return frm;
    //	}
    //	protected override void ViewModel()
    //	{
    //		


    //		AddColumn("Name");
    //		AddColumn("ValidateTime");

    //		

    //		
    //	}
    //	protected override void UIRules()
    //	{
    //		
    //	}
    //}
    //public class Tag_19_Form : IFormDefinition
    //{
    //	protected override Form IdentifyReport()
    //	{
    //		var frm = DefineForm("Tag_19_Form", "ویرایش اطلاعات کارتابل", Form.eFormType.Edit);
    //		return frm;
    //	}
    //	protected override void ViewModel()
    //	{
    //		


    //		
    //	}
    //	protected override void UIRules()
    //	{
    //		
    //	}
    //}
    //public class Tag_20_Index_Form : IFormDefinition
    //{
    //	protected override Form IdentifyReport()
    //	{
    //		var frm = DefineForm("Tag_20_Index_Form", "حذف اطلاعات کارتابل", Form.eFormType.Index);
    //		return frm;
    //	}
    //	protected override void ViewModel()
    //	{
    //		


    //		AddColumn("Name");
    //		AddColumn("ValidateTime");

    //		

    //		
    //	}
    //	protected override void UIRules()
    //	{
    //		
    //	}
    //}
    //public class Tag_20_Form : IFormDefinition
    //{
    //	protected override Form IdentifyReport()
    //	{
    //		var frm = DefineForm("Tag_20_Form", "حذف اطلاعات کارتابل", Form.eFormType.Edit);
    //		setOutputStateId("");
    //		return frm;
    //	}
    //	protected override void ViewModel()
    //	{
    //		


    //		
    //	}
    //	protected override void UIRules()
    //	{
    //		
    //	}
    //}
    #endregion form definitions
}
