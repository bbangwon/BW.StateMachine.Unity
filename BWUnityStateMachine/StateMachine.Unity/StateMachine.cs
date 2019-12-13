namespace BW.StateMachine.Unity
{
    interface IUpdatable
    {
        void Update();
    }

    /// <summary>
    /// 각 스테이트는 이 베이스 클래스를 상속받습니다.
    /// 각 스테이트 핸들러 클래스 이름과 enum 형식의 스테이트 이름은 동일해야합니다.
    /// Machine 프로퍼티를 통해 StateMachine 에 접근할수 있습니다.
    /// </summary>
    /// <typeparam name="T">스테이트(enum)</typeparam>
    public class StateBase<T> : StateMachine.StateBase<T>, IState<T>, IUpdatable where T : struct
    {
        public new StateMachine<T> Machine { get; private set; } = null;
        public virtual void Update(){}
    }

    /// <summary>
    /// 스테이트 머신 클래스(유니티)
    /// </summary>
    /// <typeparam name="T">스테이트(enum)</typeparam>
    public class StateMachine<T> : StateMachine.StateMachine<T> where T : struct
    {
        public StateMachine(T initState) : base(initState) => StateMachineRunner.Instance.AddUpdateAction(() => (GetCurrentState as IUpdatable)?.Update());
    }
}
