namespace Neo.Bpms.Domain.Entities.Cmmn.Data;

	public interface IDataBase<T>
	{
		//		RecordSet GetDataSet(string domain);
		IEnumerable<T> GetDataSetAsEnumerable(string domain);
		//		IQueryable<T> GetDataSetAsQueriable(string domain); 
		//		List<T> GetRange(string domain, int index, int count);
		object GetFieldValue(T record, string fieldName);
		bool SetFieldValue(T record, string fieldName, object value);
		bool ChangeState(T record, string fieldName, EFieldState newState);
	}