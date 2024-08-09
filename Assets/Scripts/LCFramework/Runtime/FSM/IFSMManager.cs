namespace Native.FSM
{
    public interface IFSMManager
    {
        /// <summary>
        /// 创建FSM
        /// </summary>
        IFSM CreateFSM<T>(T Owner, string fsmName, params IFSMState[] fsmStates);

        /// <summary>
        /// 获取FSM
        /// </summary>
        IFSM GetFSM(string fsmName);

        /// <summary>
        /// 删除FSM
        /// </summary>
        /// <param name="fsmName"></param>
        void DeleteFSM(string fsmName);
    }
}
