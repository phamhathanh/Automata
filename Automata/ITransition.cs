namespace Automata
{
    interface ITransition
    {
        IState CurrentState
        {
            get;
        }

        IState NextState
        {
            get;
        }

        object Symbol
        {
            get;
        }
    }
}
