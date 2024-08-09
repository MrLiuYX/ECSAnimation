namespace Native.Procedure
{
    public interface IProcedureManager
    {
        void StartProcedure<T>() where T : ProcedureBase;
        void StartProcedure<T>(T t) where T : ProcedureBase;
        ProcedureBase GetCurrentProcedure();
        ProcedureBase[] GetAllProcedure();
        void SetProcedureBase(params ProcedureBase[] procedureBases);
    }
}
