//namespace Neo.Bpms.Domain.Entities.Attributes.FormAttributes
//{
//    /// <summary>
//    /// Form Identification Attribute
//    /// 
//    /// This attribute defines the form identification and type foe a view model class
//    /// </summary>
//    /// <remarks>
//    /// Initializes a new instance of the <see cref="FormAttr_Id"/> class.
//    /// </remarks>
//    /// <param name="formType">Type of the form.</param>
//    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
//	public class FormAttr_Id(Form.eFormType formType) : Attribute
//	{
//        /// <summary>
//        /// Gets or sets the identifier. 
//        /// if Id not set, the name of view model class will be used.
//        /// </summary>
//        /// <value>
//        /// The identifier.
//        /// </value>
//        public string Id { get; set; }
//		public string Name { get; set; }
//        public Form.eFormType FormType { get; set; } = formType;
//        /// <summary>
//        /// Gets or sets the subject identifier. each FormType for an entity can have multiple subjects.
//        /// </summary>
//        /// <value>
//        /// The subject identifier.
//        /// </value>
//        public string FormSubjectId { get; set; }
//		public string OutputStateId { get; set; }
//		public string SpecificURL { get; set; }

//	}
//}
