using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Core.StateHandler
{
    public class AppState
    {
        private static AppState _instance = null;
        private static IllegalState IllegalState = new IllegalState();
        public static State Current { get; private set; }
        private State _previous { get; set; }
        public int IntValue { get => (int)Current; }

        public AppState()
        {
            Current = State.StartUp;
            _instance = this;
        }

        public static void Set(State state, Action action = null, State fallback = State.NonFallback)
        {
            Set(state);
            action?.Invoke();
            if (fallback != State.NonFallback)
                Set(fallback);

        }

        public static void Set(State state, Action action = null, Action fallback = null)
        {
            Set(state);
            action?.Invoke();
            fallback?.Invoke();
        }

        public static void Set(State state)
        {
            if (state == Current) return;
            if (IllegalState[state]) throw new IllegalStateException($"{Current} --> {state}");
            if (_instance == null)
                throw new NullReferenceException("Can not set state before app state has been initialised.");
            _instance._previous = Current;
            Current = state;
            _instance.ShowStateChange();

        }

        public static void Set(State state, Action action)
        {
            Set(state);
            action.Invoke();
        }
        public static async Task SetAsync(State state, Func<Task> action)
        {
            Set(state);
            await action();
        }
        public static async Task SetAsync(State state, Func<Task> action, State fallback)
        {
            Set(state);
            await action();
            Set(fallback);
        }
        public static async Task SetAsync(State state, Func<Task> action, Action callback)
        {
            Set(state);
            await action();
            callback.Invoke();
        }


        public static void Reset()
        {
            Current = _instance._previous;
        }

        public static State GetPreviousState() => _instance._previous;

        private void ShowStateChange()
        {
            Debug.WriteLine(ToString(), "STATE CHANGE");
        }

        public override string ToString() => $"CURRENT: {Current.ToString().PadRight(20, ' ')} VALUE: {IntValue}";
    }

    class IllegalState
    {
        State _current { get => AppState.Current; }

        public bool this[State state] {
            get {
                return state switch
                {
                    /* StartUp States */
                    State.StartUp when ((int)state >> 15) == 1 => false,
                    State.Init when _current == State.StartUp => false,
                    State.DBInit when _current == State.StartUp => false,
                    State.DBVersionCheck when _current == State.DBInit => false,
                    State.Loading when _current == State.Init => false,
                    
                    State.NoteCreating when _current == State.SearchNavigation => false,
                    
                    /* Navigation States */
                    State.Navigation when _current == State.Ready => false,
                    State.RecentNavigation when _current == State.Navigation => false,
                    State.RecentNavigation when _current == State.Ready => false,
                    State.RecentNavigation when _current == State.NotSaved => false,
                    State.ListNavigation when _current == State.Ready => false,
                    State.SearchNavigation when _current == State.Ready => false,
                    State.Navigation when _current == State.NotSaved => false,
                    State.ListNavigation when _current == State.NotSaved => false,
                    State.SearchNavigation when _current == State.NotSaved => false,
                    State.NavigationCancelled when _current == State.Ready => false,
                    State.ProtocolImportNavigation when _current == State.Ready => false,
                    State.ProtocolImportNavigation when _current == State.NotSaved => false,
                    State.ProtocolNavigating when _current == State.Ready => false,
                    State.ProtocolNavigating when _current == State.NotSaved => false,

                    /* Acceptable Error states */
                    State.LoadError when _current == State.SearchNavigation => false,
                    State.WorkerCanceled when _current == State.NotSaved => false,

                    /* Ready States */
                    // Ensure you can return from a navigation state to ready state
                    State.Ready when ((int)_current >> 14) == 1 => false,
                    // Ensure you can return from a Startup state to ready state
                    State.Ready when ((int)_current >> 15) == 1 => false,
                    // Ensure you can return from a Save state to ready state
                    State.Ready when ((int)_current >> 12) == 1 => false,
                    State.Ready when _current == State.Loading => false,
                    State.Ready when _current == State.SaveCompleted => false,

                    /* Save States */
                    State.Saving when _current == State.Ready => false,
                    State.Saving when _current == State.NotSaved => false,
                    State.SaveCompleted when _current == State.Saving => false,
                    State.SaveError when _current == State.Saving => false,
                    State.NotSaved when _current == State.Saving => false,
                    State.NotSaved when _current == State.Ready => false,
                    State.NotSaved when _current == State.WorkerCanceled => false,

                    _ => true,
                };
            }
        }
    }

    [Flags]
    public enum State
    {
        /*
        
        This enum handles the state of Relanota. 
        The state will be stored in binary representation, with bits signifying what action 
        is being taken.

        The 1111 1111 1111 1111 value represents an error state.

          StartUp Bit
          | Save bit
          | |
          | | 
          | |
          v v
       0b_0000_0000_0000_0000
           ^
           |
           |
           |
           |
           Navigation bit

        If the StartUp, Navigation, Save, and XYZ bit are all set, the state is an error state
         
         */

        // Default States
        StartUp =                   0b_1000_0000_0000_0000, // 32768
        Init =                      0b_1000_0000_0000_0010, // 32770
        StartUpFinished =           0b_1000_0000_0000_0011, // 32771
        Ready =                     0b_0000_0000_0000_0001, // 1
        Loading =                   0b_0000_0000_0010_0000, // 32
        NoteCreating =              0b_0001_1000_0000_0001, // 6145
        NonFallback =               0b_1110_1111_1111_1111, // 32767

        // StartUp States
        DBInit =                    0b_1000_1000_0000_0001, // 34817
        DBVersionCheck =            0b_1000_1000_0000_1001, // 34825
        DBMigrate =                 0b_1000_1000_0000_1000, // 34824

        // Error States
        Error =                     0b_1111_1111_1111_1111, // 65535
        SaveError =                 0b_1111_0000_0001_0000,
        LoadError =                 0b_1111_0000_0010_0000,
        NoteCreatingFailed =        0b_1111_1000_0000_1100,

        // Navigation States
        Navigation =                0b_0100_0000_0000_0000,
        RecentNavigation =          0b_0100_0000_1100_0001,
        ListNavigation =            0b_0100_0000_0000_0010,
        SearchNavigation =          0b_0100_0000_0100_0100,
        ProtocolNavigating =        0b_0100_0000_1000_1000,
        ProtocolImportNavigation =  0b_0100_0000_1111_0000,
        NavigationCancelled =       0b_0110_0000_0000_0000,

        // Worker States
        WorkerCanceled =            0b_0000_0001_0000_0000,

        // Save State
        Saving =                    0b_0010_0000_0000_0010,
        SaveCompleted =             0b_0010_0000_0000_1000,
        NotSaved =                  0b_0010_0000_0011_0000,
    }
}