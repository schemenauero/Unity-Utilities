using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine<T> {
    public T CurrentState { get; set;  }
    private IDictionary<T, T[]> transitions = new Dictionary<T, T[]>(); // from: [to, to, to]

    public delegate bool CanTransitionCallback();
    public delegate void OnTransitionCallback(T from, T to);
    private IDictionary<T, List<CanTransitionCallback>> canTransitionTo = new Dictionary<T, List<CanTransitionCallback>>(); // callbacks when transitioning to a state
    private IDictionary<T, List<CanTransitionCallback>> canTransitionFrom = new Dictionary<T, List<CanTransitionCallback>>(); // callbacks when transitioning from a state
    private IDictionary<T, List<OnTransitionCallback>> onTransitionTo = new Dictionary<T, List<OnTransitionCallback>>(); // callbacks after transitioning to a state

    public StateMachine(T defaultState) {
        CurrentState = defaultState;
    }

    public void SetTransitions(T from, T[] tos) {
        transitions[from] = tos;
    }

    public bool CanTransition(T from, T to) {
        bool transitionIsDefined = transitions[from].Contains(to);

        bool transitionFromPassed = true;
        bool transitionToPassed = true;

        if (canTransitionTo.ContainsKey(to)) {
            foreach (CanTransitionCallback transitionCallback in canTransitionTo[to]) {
                if (!transitionCallback()) transitionToPassed = false;
            }
        }

        if (canTransitionFrom.ContainsKey(from)) {
            foreach (CanTransitionCallback transitionCallback in canTransitionFrom[from]) {
                if (!transitionCallback()) transitionFromPassed = false;
            }
        }

        bool notCurrentState = !to.Equals(CurrentState);

        return transitionIsDefined && transitionToPassed && transitionFromPassed && notCurrentState;
    }

    public bool CanTransitionFromCurrent(T to) {
        return CanTransition(CurrentState, to);
    }

    public void BeforeTransitionTo(T to, CanTransitionCallback callback) {
        if (!canTransitionTo.ContainsKey(to)) canTransitionTo[to] = new List<CanTransitionCallback>();
        canTransitionTo[to].Add(callback);
    }

    public void BeforeTransitionFrom(T from, CanTransitionCallback callback) {
        if (!canTransitionFrom.ContainsKey(from)) canTransitionFrom[from] = new List<CanTransitionCallback>();
        canTransitionFrom[from].Add(callback);
    }

    public void OnTransitionTo(T to, OnTransitionCallback callback) {
        if (!onTransitionTo.ContainsKey(to)) onTransitionTo[to] = new List<OnTransitionCallback>();
        onTransitionTo[to].Add(callback);
    }

    public bool Transition(T to) {
        bool canTransition = CanTransitionFromCurrent(to);
        if (canTransition) {
            Debug.Log("TRANSITION: from: " + CurrentState + " to: " + to);
            T oldCurrentState = CurrentState;
            CurrentState = to;

            if (onTransitionTo.ContainsKey(to)) {
                foreach (OnTransitionCallback transitionCallback in onTransitionTo[to]) {
                    transitionCallback(oldCurrentState, to);
                }
            }

        }
        return canTransition;
    }

}
