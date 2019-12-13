using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BW.StateMachine
{
    /// <summary>
    /// 각 스테이트는 이 인터페이스를 상속받습니다.
    /// 각 스테이트 핸들러 클래스 이름과 enum 형식의 스테이트 이름은 동일해야합니다.
    /// </summary>
    /// <typeparam name="T">스테이트(enum)</typeparam>
    public interface IState<T> where T : struct
    {
        void Enter();
        void Exit();
    }

    /// <summary>
    /// 각 스테이트는 이 베이스 클래스를 상속받습니다.
    /// 각 스테이트 핸들러 클래스 이름과 enum 형식의 스테이트 이름은 동일해야합니다.
    /// Machine 프로퍼티를 통해 StateMachine 에 접근할수 있습니다.
    /// </summary>
    /// <typeparam name="T">스테이트(enum)</typeparam>
    public class StateBase<T> : IState<T> where T : struct
    {
        public StateMachine<T> Machine { get; private set; } = null;
        public virtual void Enter(){}
        public virtual void Exit(){}
    }

    /// <summary>
    /// 스테이트 머신 클래스
    /// </summary>
    /// <typeparam name="T">스테이트(enum)</typeparam>
    public class StateMachine<T> where T : struct
    {
        protected List<IState<T>> _stateList = new List<IState<T>>();
        protected IState<T> GetCurrentState => _stateList.FirstOrDefault(w => w.GetType().Name.Equals(CurrentState.ToString()));
        public T LastState { get; private set; }
        public T CurrentState { get; private set; }

        /// <summary>
        /// 스테이트머신을 생성합니다.
        /// </summary>
        /// <param name="initState">초기화 스테이트</param>
        public StateMachine(T initState)
        {            
            Assembly assembly = typeof(T).Assembly;
            _stateList.AddRange(assembly.GetImplementedObjectsByInterface<IState<T>>());
            foreach (var item in _stateList)
            {
                var pi = item.GetType()?.BaseType?.GetProperties()?.FirstOrDefault(p => p.Name == "Machine");
                pi?.SetValue(item, this);
            }
            LastState = initState;
            CurrentState = initState;
            GetCurrentState?.Enter();
        }

        /// <summary>
        /// 현재 스테이트를 변경합니다.
        /// </summary>
        /// <param name="newState">새로운 스테이트</param>
        public void ChangeState(T newState)
        {
            if(!newState.Equals(CurrentState))
            {
                GetCurrentState?.Exit();
                LastState = CurrentState;
                CurrentState = newState;
                GetCurrentState?.Enter();
            }
        }
    }
}
